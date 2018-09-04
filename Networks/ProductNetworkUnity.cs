//#define PURCHASE_USE_UNITY

using System;
using System.Collections.Generic;
using Engine.Events;
using UnityEngine;

#if PURCHASE_USE_UNITY
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
#endif

public class ProductNetworkUnityMessages {
    public static string productNetworkUnitySuccess = "product-network-unity-success";
    public static string productNetworkUnityFailed = "product-network-unity-failed";
    public static string productNetworkUnityCancelled = "product-network-unity-cancelled";
}

#if PURCHASE_USE_UNITY
// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class ProductNetworkUnity : MonoBehaviour, IStoreListener {

    private static IStoreController productController;
    private static IExtensionProvider productExtensionProvider;

    // Product identifiers for all products capable of being purchased:
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.

    public static string typeConsumable = "consumable";
    public static string typeNonConsumable = "nonconsumable";
    public static string typeSubscription = "subscription";

    // Apple App Store-specific product identifier for the subscription product.
    //private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

    // Google Play Store-specific product identifier subscription product.
    //private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

    void Start() {
        // If we haven't set up the Unity Purchasing reference
        if(productController == null) {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing() {
        // If we have already connected to Purchasing ...
        if(IsInitialized()) {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        List<GameProduct> products = GameProducts.Instance.GetAll();

        foreach(GameProduct product in products) {

            // check type then add based on type

            //string purchaseType = typeNonConsumable;

            if(product.data != null) {
                if(product.data.meta != null && product.data.meta.Count > 0) {

                    foreach(GameDataObject dataItem in product.data.meta) {

                        if(dataItem.data_type == typeConsumable) {
                            builder.AddProduct(product.GetPlatformProductCode(), ProductType.Consumable);
                        }
                        else if(dataItem.data_type == typeNonConsumable) {
                            builder.AddProduct(product.GetPlatformProductCode(), ProductType.NonConsumable);
                        }
                        else if(dataItem.data_type == typeSubscription) {
                            builder.AddProduct(product.GetPlatformProductCode(), ProductType.Subscription, new IDs() {
                                { product.GetPlatformProductCode(PlatformKeys.ios), AppleAppStore.Name },
                                { product.GetPlatformProductCode(PlatformKeys.android), GooglePlay.Name },
                            });
                        }
                    }
                }
            }
        }

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized() {
        // Only say we are initialized if both the Purchasing references are set.
        return productController != null && productExtensionProvider != null;
    }


    public void PurchaseProduct(string productId) {

        if(IsInitialized()) {

            Product product = productController.products.WithID(productId);

            if(product != null && product.availableToPurchase) {

                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));

                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.

                productController.InitiatePurchase(product);
            }
            else {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google.
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.

    public bool RestorePurchases() {

        if(!IsInitialized()) {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return false;
        }

        if(Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer) {

            Debug.Log("RestorePurchases started ...");

            var apple = productExtensionProvider.GetExtension<IAppleExtensions>();

            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.

            apple.RestoreTransactions((result) => {

                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.

                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");

            });

            return true;
        }
        else {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);

            return false;
        }
    }

    //
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        productController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        productExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error) {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {

        Messenger<PurchaseEventArgs>.Broadcast(
            ProductNetworkUnityMessages.productNetworkUnitySuccess, args);

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {

        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
            product.definition.storeSpecificId, failureReason));

        Messenger<Product, PurchaseFailureReason>.Broadcast(
            ProductNetworkUnityMessages.productNetworkUnityFailed, product, failureReason);
    }
}

#endif