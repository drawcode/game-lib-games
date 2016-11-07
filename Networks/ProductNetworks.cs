// TODO wire in UNITY native product purchas networks
//#define PURCHASE_USE_UNITY
#if UNITY_IPHONE
//#define PURCHASE_USE_UNITY_APPLE_ITUNES
//#define PURCHASE_USE_APPLE_ITUNES
#endif

#if UNITY_ANDROID
//#define PURCHASE_USE_UNITY_GOOGLE_PLAY
//#define PURCHASE_USE_GOOGLE_PLAY
//#define PURCHASE_USE_AMAZON
//#define PURCHASE_USE_SAMSUNG
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
#if PURCHASE_USE_UNITY
using UnityEngine.Purchasing;
#endif

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class ProductNetworkType {
    public static string typeLocal = "local";
    public static string typeServer = "server";
    public static string typeThirdParty = "third-party";
}

public class ProductNetworkStoreType {
    public static string typeGameverses = "gameverses";
    public static string typeGoogle = "external-google";
    public static string typeApple = "external-apple";
    public static string typeAmazon = "external-amazon";
    public static string typeSamsung = "external-samsung";
}

public class ProductNetworkMessages {
    public static string productPurchaseSuccess = "product-network-success";
    public static string productPurchaseFailed = "product-network-failed";
    public static string productPurchaseCancelled = "product-network-cancelled";
    public static string productPurchaseProgress = "product-network-progress";
    public static string productPurchaseProgressStarted = "product-network-progress-started";
    public static string productPurchaseProgressCompleted = "product-network-progress-completed";
}

public class ProductNetworkRecord : DataObjectItem {
    public bool success = false;
    public object data;
    public string receipt = "";
    public DateTime datePurchased;
    public string code = "";
    public string displayName = "";
    public string description = "";
    public string productPurchaseType = ProductNetworkType.typeLocal;
    public string productStoreType = ProductNetworkStoreType.typeApple;
    public string productId = "";
    public int quantity = 1;

    public ProductNetworkRecord() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        success = false;
        data = null;
        receipt = "";
        datePurchased = DateTime.Now;
        code = "";
        displayName = "";
        description = "";
        productId = "";
        quantity = 1;
        productPurchaseType = ProductNetworkType.typeLocal;
        productStoreType = ProductNetworkStoreType.typeApple;
    }

    public static ProductNetworkRecord Create(
        string code,
        string displayName,
        string description,
        bool success,
        object data,
        string receipt,
        string productId,
        int productQuantity,
        string productPurchaseType,
        string productStoreType) {

        ProductNetworkRecord record = new ProductNetworkRecord();
        record.success = success;
        record.data = data;
        record.receipt = receipt;
        record.datePurchased = DateTime.Now;
        record.code = code;
        record.displayName = displayName;
        record.description = description;
        record.productId = productId;
        record.quantity = productQuantity;
        record.productPurchaseType = productPurchaseType;
        record.productStoreType = productStoreType;

        return record;
    }
}

public class ProductNetworks : GameObjectBehavior {

    public static string PRODUCT_GAME_PURCHASE = "all";
    public static string statusPurchaseSuccessful = "status_purchase_successful";
    public static string statusPurchaseFailed = "status_purchase_failed";
    public GameObject productSystem;
    public GameObject productManagerObject;
    public GameObject productEventListenerObject;
    public bool paymentSystemAdded = false;
    public bool enableProductUnlocks = false;
    public bool purchaseProcessCompleted = false;

#if PURCHASE_USE_APPLE_ITUNES
    public StoreKitManager productManager;
    public StoreKitEventListener productEventListener;
#elif PURCHASE_USE_AMAZON
    public AmazonIAPManager productManager;
    public AmazonIAPEventListener productEventListener;

#elif PURCHASE_USE_GOOGLE_PLAY
    public GoogleIABManager productManager;
    public GoogleIABEventListener productEventListener;

#elif UNITY_WEBPLAYER
    // Web/PC
    //public StoreKitManager productManager;
    //public StoreKitEventListener productEventListener;
#else
#endif

    // Only one ProductNetworks can exist. We use a singleton pattern to enforce this.
    private static ProductNetworks _instance = null;

    public static ProductNetworks instance {
        get {
            if (!_instance) {

                // check if an ObjectPoolManager is already available in the scene graph
                _instance = FindObjectOfType(typeof(ProductNetworks)) as ProductNetworks;

                // nope, create a new one
                if (!_instance) {
                    var obj = new GameObject("_ProductNetworks");
                    _instance = obj.AddComponent<ProductNetworks>();
                }
            }

            return _instance;
        }
    }

    public virtual void Awake() {

    }

    public virtual void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public virtual void OnEnable() {

        Messenger<ProductNetworkRecord>.AddListener(
            ProductNetworkMessages.productPurchaseSuccess, onProductNetworksSuccess);

        Messenger<ProductNetworkRecord>.AddListener(
            ProductNetworkMessages.productPurchaseFailed, onProductNetworksFailed);

        Messenger<ProductNetworkRecord>.AddListener(
            ProductNetworkMessages.productPurchaseCancelled, onProductNetworksCancelled);

#if PURCHASE_USE_APPLE_ITUNES
        StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessfulEvent;
        StoreKitManager.purchaseCancelledEvent += purchaseCancelledEvent;
        StoreKitManager.purchaseFailedEvent += purchaseFailedEvent;
        StoreKitManager.productPurchaseAwaitingConfirmationEvent += productPurchaseAwaitingConfirmationEvent;
        StoreKitManager.paymentQueueUpdatedDownloadsEvent += paymentQueueUpdatedDownloadsEvent;
        StoreKitManager.productListReceivedEvent += productListReceivedEvent;
        StoreKitManager.productListRequestFailedEvent += productListRequestFailedEvent;
        StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailedEvent;
        StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinishedEvent;
        StoreKitManager.transactionUpdatedEvent += transactionUpdatedEvent;
#endif
       
#if PURCHASE_USE_GOOGLE_PLAY
        GoogleIABManager.billingSupportedEvent += billingSupportedEvent;
        GoogleIABManager.billingNotSupportedEvent += billingNotSupportedEvent;
        GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
        GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
        GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
        GoogleIABManager.purchaseSucceededEvent += purchaseSucceededEvent;
        GoogleIABManager.purchaseFailedEvent += purchaseFailedEvent;
        GoogleIABManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
        GoogleIABManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
#endif
    }

    public virtual void OnDisable() {

        Messenger<ProductNetworkRecord>.RemoveListener(
            ProductNetworkMessages.productPurchaseSuccess, onProductNetworksSuccess);

        Messenger<ProductNetworkRecord>.RemoveListener(
            ProductNetworkMessages.productPurchaseFailed, onProductNetworksFailed);

        Messenger<ProductNetworkRecord>.RemoveListener(
            ProductNetworkMessages.productPurchaseCancelled, onProductNetworksCancelled);

#if PURCHASE_USE_APPLE_ITUNES
        StoreKitManager.purchaseSuccessfulEvent -= purchaseSuccessfulEvent;
        StoreKitManager.purchaseCancelledEvent -= purchaseCancelledEvent;
        StoreKitManager.purchaseFailedEvent -= purchaseFailedEvent;
        StoreKitManager.productPurchaseAwaitingConfirmationEvent -= productPurchaseAwaitingConfirmationEvent;
        StoreKitManager.paymentQueueUpdatedDownloadsEvent -= paymentQueueUpdatedDownloadsEvent;
        StoreKitManager.productListReceivedEvent -= productListReceivedEvent;
        StoreKitManager.productListRequestFailedEvent -= productListRequestFailedEvent;
        StoreKitManager.restoreTransactionsFailedEvent -= restoreTransactionsFailedEvent;
        StoreKitManager.restoreTransactionsFinishedEvent -= restoreTransactionsFinishedEvent;
        StoreKitManager.transactionUpdatedEvent -= transactionUpdatedEvent;
#endif
        
#if PURCHASE_USE_GOOGLE_PLAY
        GoogleIABManager.billingSupportedEvent -= billingSupportedEvent;
        GoogleIABManager.billingNotSupportedEvent -= billingNotSupportedEvent;
        GoogleIABManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
        GoogleIABManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
        GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
        GoogleIABManager.purchaseSucceededEvent -= purchaseSucceededEvent;
        GoogleIABManager.purchaseFailedEvent -= purchaseFailedEvent;
        GoogleIABManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
        GoogleIABManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
#endif
    }

    public void onProductNetworksSuccess(ProductNetworkRecord record) {
        //LogUtil.Log("onProductNetworksSuccess:" + " record:" + record.ToJson());
    }

    public void onProductNetworksFailed(ProductNetworkRecord record) {
        //LogUtil.Log("onProductNetworksFailed:" + " record:" + record.ToJson());
    }

    public void onProductNetworksCancelled(ProductNetworkRecord record) {
        //LogUtil.Log("onProductNetworksCancelled:" + " record:" + record.ToJson());
    }

    // MESSAGES

    // success

    public static void BroadcastProductNetworksSuccess(ProductNetworkRecord record) {
        instance.broadcastProductNetworksSuccess(record);
    }

    public void broadcastProductNetworksSuccess(ProductNetworkRecord record) {
        Messenger<ProductNetworkRecord>.Broadcast(
            ProductNetworkMessages.productPurchaseSuccess, record);
    }

    // failed

    public static void BroadcastProductNetworksFailed(ProductNetworkRecord record) {
        instance.broadcastProductNetworksFailed(record);
    }

    public void broadcastProductNetworksFailed(ProductNetworkRecord record) {
        Messenger<ProductNetworkRecord>.Broadcast(
            ProductNetworkMessages.productPurchaseFailed, record);
    }

    // cancelled

    public static void BroadcastProductNetworksCancelled(ProductNetworkRecord record) {
        instance.broadcastProductNetworksCancelled(record);
    }

    public void broadcastProductNetworksCancelled(ProductNetworkRecord record) {
        Messenger<ProductNetworkRecord>.Broadcast(
            ProductNetworkMessages.productPurchaseCancelled, record);
    }

    // INIT

    public static void Init() {
        instance.init();
    }

    public virtual void init() {

        if (!paymentSystemAdded) {

#if PURCHASE_USE_APPLE_ITUNES
            productManager = gameObject.AddComponent<StoreKitManager>();
            productEventListener = gameObject.AddComponent<StoreKitEventListener>();

            LogUtil.Log("ProductNetworks::InitPaymentSystem StoreKit/iOS added...");
            
            GetProducts();
#elif PURCHASE_USE_AMAZON
            productManager = gameObject.AddComponent<AmazonIAPManager>();
            productEventListener = gameObject.AddComponent<AmazonIAPEventListener>();

            LogUtil.LogProduct("ProductNetworks::InitPaymentSystem Amazon IAP/Android added...");
#elif PURCHASE_USE_GOOGLE_PLAY
            productManager = gameObject.AddComponent<GoogleIABManager>();
            productEventListener = gameObject.AddComponent<GoogleIABEventListener>();

            LogUtil.LogProduct("ProductNetworks::InitPaymentSystem Google Play IAB/Android added...");

            GoogleIAB.init(AppConfigs.productGoogleKey);
#elif UNITY_WEBPLAYER
            LogUtil.LogProduct("ProductNetworks::InitPaymentSystem none added...");
#else
            LogUtil.LogProduct("ProductNetworks::InitPaymentSystem none added...");
#endif

            if (productManagerObject != null)
                DontDestroyOnLoad(productManagerObject);

            if (productEventListenerObject != null)
                DontDestroyOnLoad(productEventListenerObject);

            paymentSystemAdded = true;
        }

    }

    // HELPERS

    public bool EnableProductUnlocks {
        get {
            return enableProductUnlocks;
        }
        set {
            enableProductUnlocks = value;
        }
    }

    public void GetProducts() {
        string productIdentifiers = "";
        foreach (GameProduct product in GameProducts.Instance.GetAll()) {
            if (productIdentifiers.Length > 0) {
                productIdentifiers += ",";
            }
            string code = product.GetPlatformProductCode();
            if (!string.IsNullOrEmpty(code)) {
                productIdentifiers += code;
            }
        }
        RequestProductList(productIdentifiers);
        LogUtil.LogProduct("GetProducts: " + productIdentifiers);
    }

    public void RequestProductList(string productIdentifiers) {

        string[] skus = productIdentifiers.Split(',');

#if PURCHASE_USE_APPLE_ITUNES
        StoreKitBinding.requestProductData(skus);
#elif PURCHASE_USE_AMAZON
        AmazonIAP.initiateItemDataRequest(skus);
#elif PURCHASE_USE_GOOGLE_PLAY
        GoogleIAB.queryInventory(skus);
#else
        // Web/PC
        Debug.Log("skus" + skus.ToJson());
#endif

    }

    public static void RestoreTransactions() {
        if(instance != null) {
            instance.restoreTransactions();
        }
    }
    
    public void restoreTransactions() {
#if PURCHASE_USE_APPLE_ITUNES
        //return false;
#elif PURCHASE_USE_AMAZON
        //return false;
#elif PURCHASE_USE_GOOGLE_PLAY
        //return GoogleIABManager.
#else
        //return false;
#endif
    }

    public static bool IsSubscriptionSupported() {
        if(instance != null) {
            return instance.isSubscriptionSupported();
        }
        return false;
    }

    public bool isSubscriptionSupported() {
#if PURCHASE_USE_APPLE_ITUNES
        return false;
#elif PURCHASE_USE_AMAZON
        return false;
#elif PURCHASE_USE_GOOGLE_PLAY
        return GoogleIAB.areSubscriptionsSupported();
#else
        return false;
#endif
        
    }

    public static void PurchaseProduct(string code, int quantity) {
        if (instance != null) {
            instance.purchaseProduct(code, quantity);
        }
    }

    public void purchaseProduct(string code, int quantity) {

#if PURCHASE_USE_APPLE_ITUNES
        StoreKitBinding.purchaseProduct(code, quantity);
#elif PURCHASE_USE_AMAZON
        //AmazonIAP.initiatePurchaseRequest(GamePacks.currentGameBundle + "." + code);
        AmazonIAP.initiatePurchaseRequest(code);
#elif PURCHASE_USE_GOOGLE_PLAY
        GoogleIAB.purchaseProduct(code);
#else
        // Web/PC
        purchaseProcessCompleted = true;
        
        ProductNetworkRecord data = 
            ProductNetworkRecord.Create(
                code, 
                "Product Purchase", 
                "Product purchased:" + code,
                true,
                "TODO",
                "TODO",
                code,
                1,
                ProductNetworkType.typeThirdParty, 
                ProductNetworkStoreType.typeAmazon);
        
        BroadcastProductNetworksSuccess(data);
#endif
    }

    public void SetContentAccessPermissions(string code) {
        SystemPrefUtil.SetLocalSettingString(code, code);
    }

    // THIRD PARTY EVENTS

#if PURCHASE_USE_APPLE_ITUNES

    public void purchaseSuccessfulEvent(StoreKitTransaction transaction) {
        LogUtil.LogProduct( "SCREEN purchased product: " + transaction.productIdentifier + ", quantity: " + transaction.quantity );
        //transaction.base64EncodedTransactionReceipt
        SetContentAccessPermissions(transaction.productIdentifier);
        Contents.SetContentAccessTransaction(transaction.productIdentifier, transaction.productIdentifier,
                                             transaction.base64EncodedTransactionReceipt, transaction.quantity, true);
        purchaseProcessCompleted = true;

        ProductNetworksRecord data = 
            ProductNetworksRecord.Create(
                transaction.productIdentifier, 
                "Product Purchase", 
                "Product purchased:" + transaction.productIdentifier,
                true,
                transaction.ToJson(),
                transaction.base64EncodedTransactionReceipt,
                transaction.productIdentifier,
                transaction.quantity,
                ProductNetworksType.typeThirdParty, 
                ProductStoreType.typeApple);

        BroadcastProductNetworksSuccess(data);
    }

    public void purchaseCancelledEvent(string error) {
        purchaseProcessCompleted = true;
        LogUtil.LogProduct( "purchase cancelled with error: " + error );

        ProductNetworksRecord data = 
            ProductNetworksRecord.Create(
                "cancel", 
                "Product Purchase Cancelled", 
                error,
                false,
                error,
                "",
                "",
                1,
                ProductNetworksType.typeThirdParty, 
                ProductStoreType.typeApple);
        
        BroadcastProductNetworksCancelled(data);
    }

    public void purchaseFailedEvent(string error) {
        purchaseProcessCompleted = true;
        LogUtil.LogProduct( "purchase failed with error: " + error );

        ProductNetworksRecord data = 
            ProductNetworksRecord.Create(
                "error", 
                "Product Purchase FAILED", 
                error,
                false,
                error,
                "",
                "",
                1,
                ProductNetworksType.typeThirdParty, 
                ProductStoreType.typeApple);
        
        BroadcastProductNetworksFailed(data);
    }

    public void productPurchaseAwaitingConfirmationEvent(StoreKitTransaction transaction) {
        LogUtil.Log("productPurchaseAwaitingConfirmationEvent:" + " transaction.productIdentifier:" + transaction.productIdentifier);
    }

    public void paymentQueueUpdatedDownloadsEvent(List<StoreKitDownload> downloads) {
        LogUtil.Log("paymentQueueUpdatedDownloadsEvent");

    }

    public void productListReceivedEvent(List<StoreKitProduct> products) {
        LogUtil.Log("paymentQueueUpdatedDownloadsEvent");

    }

    public void productListRequestFailedEvent(string error) {
        LogUtil.Log("productListRequestFailedEvent:" + " error:" + error);

    }

    public void restoreTransactionsFailedEvent(string error) {
        LogUtil.Log("restoreTransactionsFailedEvent:" + " error:" + error);

    }

    public void restoreTransactionsFinishedEvent() {
        LogUtil.Log("restoreTransactionsFinishedEvent");

    }

    public void transactionUpdatedEvent(StoreKitTransaction transaction) {
        LogUtil.Log("transactionUpdatedEvent:" + " transaction.productIdentifier:" + transaction.productIdentifier);
    }

#endif

#if PURCHASE_USE_AMAZON

    void itemDataRequestFailedEvent() {
        LogUtil.LogProduct( "ANDROID_AMAZON itemDataRequestFailedEvent:");
    }

    void itemDataRequestFinishedEvent(List<string> unavailableSkus, List<AmazonItem> products) {
        LogUtil.LogProduct( "ANDROID_AMAZON itemDataRequestFinishedEvent:");

        foreach(string sku in unavailableSkus) {
            LogUtil.LogProduct( "ANDROID_AMAZON AmazonItem: unavailableSkus:" + sku);
        }

        LogUtil.LogProduct( "ANDROID_AMAZON itemDataRequestFinishedEvent: products.Count:" + products.Count);
        foreach(AmazonItem item in products) {
            LogUtil.LogProduct( "ANDROID_AMAZON AmazonItem: sku:" + item.sku + "\r\n title:" + item.title + "\r\n description:" + item.description + "\r\n price:" + item.price);
        }
    }

    void onGetUserIdResponseEvent(string userId) {
        LogUtil.LogProduct( "ANDROID_AMAZON onGetUserIdResponseEvent: userId:" + userId);
    }

    void onSdkAvailableEvent(bool debug) {
        LogUtil.LogProduct( "ANDROID_AMAZON onSdkAvailableEvent: debug:" + debug);
    }

    void purchaseFailedEvent() {
        LogUtil.LogProduct( "ANDROID_AMAZON purchaseFailedEvent");
        HandleError();
    }

    void purchaseSuccessfulEvent(AmazonReceipt transaction) {
        LogUtil.LogProduct( "ANDROID_AMAZON purchaseSuccessfulEvent: transaction.sku:" + transaction.sku + "\r\n token:" + transaction.token + "\r\n type:" + transaction.type);
        SetContentAccessPermissions(transaction.sku);
        Contents.Instance.SetContentAccessTransaction(transaction.sku, transaction.sku,
                                                      transaction.token, 1, true);
        purchaseProcessCompleted = true;
        HandleSuccess();
    }

    void purchaseUpdatesRequestFailedEvent() {
        LogUtil.LogProduct( "ANDROID_AMAZON purchaseUpdatesRequestFailedEvent:");
    }

    void purchaseUpdatesRequestSuccessfulEvent(List<string> unavailableSkus, List<AmazonReceipt> transactions) {
        LogUtil.LogProduct( "ANDROID_AMAZON purchaseUpdatesRequestSuccessfulEvent:");
        foreach(string sku in unavailableSkus) {
            LogUtil.LogProduct( "ANDROID_AMAZON AmazonReceipt: unavailableSkus:" + sku);
        }

        LogUtil.LogProduct( "ANDROID_AMAZON purchaseUpdatesRequestSuccessfulEvent: transactions.Count:" + transactions.Count);
        foreach(AmazonReceipt transaction in transactions) {
            LogUtil.LogProduct( "ANDROID_AMAZON AmazonReceipt: sku:" + transaction.sku  + "\r\n token:" + transaction.token + "\r\n type:" + transaction.type);

            string productId = transaction.sku.Replace(GamePacks.currentGameBundle + ".", "");
            Contents.Instance.SetGlobalContentAccess(productId);
            Contents.Instance.SetContentAccessTransaction(productId, transaction.sku,
                                                          transaction.token, 1, true);
        }
    }
#endif

#if PURCHASE_USE_GOOGLE_PLAY

    // Fired after init is called when billing is supported on the device
    public void billingSupportedEvent() {        
        Debug.Log("GoogleIABManager:billingNotSupportedEvent");
        GetProducts();
        //IABAndroid.restoreTransactions();
    }
    
    // Fired after init is called when billing is not supported on the device
    public void billingNotSupportedEvent(string error) {        
        Debug.Log("GoogleIABManager:billingNotSupportedEvent: " + error);
        //IABAndroid.restoreTransactions();
    }
    
    // Fired when the inventory and purchase history query has returned
    public void queryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus) {
        //LogUtil.Log( string.Format( "queryInventorySucceededEvent. total purchases: {0}, total skus: {1}", purchases.Count, skus.Count ) );        
        Debug.Log("GoogleIABManager:queryInventorySucceededEvent");

        Prime31.Utils.logObject( purchases );
        Prime31.Utils.logObject( skus );

        foreach(GoogleSkuInfo sku in skus) {       
            Debug.Log(
                "GoogleIABManager:queryInventorySucceededEvent: " +
                " sku.productId:" + sku.productId +
                " sku.title:" + sku.title
            );
        }
        
        foreach(GooglePurchase purchase in purchases) {       
            Debug.Log(
                "GoogleIABManager:queryInventorySucceededEvent: " +
                " purchase.productId:" + purchase.productId +
                " purchase.orderId:" + purchase.orderId
                );
        }

        //IABAndroid.restoreTransactions();
    }
    
    // Fired when the inventory and purchase history query fails
    public void queryInventoryFailedEvent(string error) {
        Debug.Log("GoogleIABManager:queryInventoryFailedEvent: " + error);
    }
    
    // Fired when a purchase completes allowing you to verify the signature on an external server if you would like
    public void purchaseCompleteAwaitingVerificationEvent(string purchaseData, string signature) {
        Debug.Log("GoogleIABManager:purchaseCompleteAwaitingVerificationEvent: " + " purchaseData: " + purchaseData + ", signature: " + signature );
        
    }
    
    // Fired when a purchase succeeds
    public void purchaseSucceededEvent(GooglePurchase purchase) {        
        Debug.Log( "GoogleIABManager:purchaseSucceededEvent product: " + purchase.productId );
        SetContentAccessPermissions(purchase.productId);
        GoogleIAB.consumeProduct(purchase.productId);
        purchaseProcessCompleted = true;
        //HandleSuccess();
    }
    
    // Fired when a purchase fails
    public void purchaseFailedEvent(string error, int val) {
        Debug.Log("GoogleIABManager:purchaseFailedEvent: " + error + " val:" + val);
    }
    
    // Fired when a call to consume a product succeeds
    public void consumePurchaseSucceededEvent(GooglePurchase purchase) {       
        Debug.Log( "GoogleIABManager:purchaseSucceededEvent product: " + purchase.productId );
        SetContentAccessPermissions(purchase.productId);
        purchaseProcessCompleted = true;
    }
    
    // Fired when a call to consume a product fails
    public void consumePurchaseFailedEvent(string error) {
        Debug.Log("GoogleIABManager:consumePurchaseFailedEvent: " + error);
    }

#endif

#if PURCHASE_USE_UNITY
    // UNITY INAPP PURCHASING

    // Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    public class Purchaser : MonoBehaviour, IStoreListener {
        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        // Product identifiers for all products capable of being purchased: 
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values 
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
        // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.
        public static string kProductIDConsumable = "consumable";
        public static string kProductIDNonConsumable = "nonconsumable";
        public static string kProductIDSubscription = "subscription";

        // Apple App Store-specific product identifier for the subscription product.
        private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

        // Google Play Store-specific product identifier subscription product.
        private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

        void Start() {
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null) {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
        }

        public void InitializePurchasing() {
            // If we have already connected to Purchasing ...
            if (IsInitialized()) {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add a product to sell / restore by way of its identifier, associating the general identifier
            // with its store-specific identifiers.
            builder.AddProduct(kProductIDConsumable, ProductType.Consumable);
            // Continue adding the non-consumable product.
            builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
            // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
            // if the Product ID was configured differently between Apple and Google stores. Also note that
            // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
            // must only be referenced here. 
            builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
                { kProductNameAppleSubscription, AppleAppStore.Name },
                { kProductNameGooglePlaySubscription, GooglePlay.Name },
            });

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);
        }


        private bool IsInitialized() {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }


        public void BuyConsumable() {
            // Buy the consumable product using its general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            BuyProductID(kProductIDConsumable);
        }


        public void BuyNonConsumable() {
            // Buy the non-consumable product using its general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            BuyProductID(kProductIDNonConsumable);
        }


        public void BuySubscription() {
            // Buy the subscription product using its the general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            // Notice how we use the general product identifier in spite of this ID being mapped to
            // custom store-specific identifiers above.
            BuyProductID(kProductIDSubscription);
        }


        void BuyProductID(string productId) {
            // If Purchasing has been initialized ...
            if (IsInitialized()) {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase) {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }


        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases() {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized()) {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer) {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) => {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }


        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
        }


        public void OnInitializeFailed(InitializationFailureReason error) {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
            // A consumable product has been purchased by this user.
            if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable, StringComparison.Ordinal)) {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
                //ScoreManager.score += 100;
            }
            // Or ... a non-consumable product has been purchased by this user.
            else if (String.Equals(args.purchasedProduct.definition.id, kProductIDNonConsumable, StringComparison.Ordinal)) {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
            }
            // Or ... a subscription product has been purchased by this user.
            else if (String.Equals(args.purchasedProduct.definition.id, kProductIDSubscription, StringComparison.Ordinal)) {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // TODO: The subscription item has been successfully purchased, grant this to the player.
            }
            // Or ... an unknown product has been purchased by this user. Fill in additional products here....
            else {
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            }

            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 
            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }
    }
#endif

}

