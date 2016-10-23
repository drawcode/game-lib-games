// TODO wire in UNITY native product purchas networks

#if UNITY_IPHONE
//#define PURCHASE_USE_APPLE_ITUNES
#endif

#if UNITY_ANDROID
//#define PURCHASE_USE_GOOGLE_PLAY
//#define PURCHASE_USE_AMAZON
//#define PURCHASE_USE_SAMSUNG
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

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

}

