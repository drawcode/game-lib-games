#define ANDROID_AMAZONN
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public enum ProductPurchaseType {
    LOCAL,
    SERVER,
    EXTERNAL_GOOGLE_PLAY,
    EXTERNAL_APPLE_ITUNES,
    EXTERNAL_AMAZON,
    EXTERNAL_SAMSUNG
}

public class ProductPurchase : MonoBehaviour { 

    public static string PRODUCT_GAME_PURCHASE = "all";
    public static string statusPurchaseSuccessful = "status_purchase_successful";
    public static string statusPurchaseFailed = "status_purchase_failed";

    public static ProductPurchase Instance;

    public GameObject productSystem;
    public GameObject productManagerObject;
    public GameObject productEventListenerObject;

    public bool paymentSystemAdded = false;
    public bool enableProductUnlocks = false;    
    public bool purchaseProcessCompleted = false;
    
#if UNITY_IPHONE
    public StoreKitManager productManager;
    public StoreKitEventListener productEventListener;
#elif UNITY_ANDROID
#if ANDROID_AMAZON
    public AmazonIAPManager productManager;
    public AmazonIAPEventListener productEventListener;
    
#else
    public GoogleIABManager productManager;
    public GoogleIABEventListener productEventListener;
    
#endif
#elif UNITY_WEBPLAYER
#else
    // Web/PC
    public StoreKitManager productManager;
    public StoreKitEventListener productEventListener;
#endif

    // Only one ProductPurchase can exist. We use a singleton pattern to enforce this.
    private static ProductPurchase _instance = null;
    
    public static ProductPurchase instance {
        get {
            if (!_instance) {
                
                // check if an ObjectPoolManager is already available in the scene graph
                _instance = FindObjectOfType(typeof(ProductPurchase)) as ProductPurchase;
                
                // nope, create a new one
                if (!_instance) {
                    var obj = new GameObject("_ProductPurchase");
                    _instance = obj.AddComponent<ProductPurchase>();
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
        
        #if UNITY_EDITOR
        #elif UNITY_IPHONE
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
    }

    public virtual void OnDisable() {        
        
        #if UNITY_EDITOR
        #elif UNITY_IPHONE
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
    }

    public static void Init() {
        instance.init();
    }
    
    public virtual void init() {    

        if(!paymentSystemAdded) {
          
#if UNITY_IPHONE
            productManager = gameObject.AddComponent<StoreKitManager>();             
            productEventListener = gameObject.AddComponent<StoreKitEventListener>();     

            LogUtil.Log("ProductPurchase::InitPaymentSystem StoreKit/iOS added...");
#elif UNITY_ANDROID
            // TODO isAmazonDevice()
#if ANDROID_AMAZON
            productManager = gameObject.AddComponent<AmazonIAPManager>();                
            productEventListener = gameObject.AddComponent<AmazonIAPEventListener>();

            LogUtil.LogProduct("ProductPurchase::InitPaymentSystem Amazon IAP/Android added...");               
#else
            productManager = gameObject.AddComponent<GoogleIABManager>();                
            productEventListener = gameObject.AddComponent<GoogleIABEventListener>();

            LogUtil.LogProduct("ProductPurchase::InitPaymentSystem Google Play IAB/Android added...");  

            GoogleIAB.init("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAllfTLDqNeLzmIczqRP4Mramyc0Rd/RUlg+fBRO7VGRPMorv2UKWaxUqbKEYy10ldNu43anV3MHGliSX7wyED1v5GCt8syAeDT59wZZzY7aqMB3CBPmqfFm52ONY7BKaew/uWqjjn1w5Kq4BySLXyBTfrwlnqVsnMnW12lUGPpzgdBODe00JOk+DDjcZcunGB6xXxNA2wPO1pB8VSawVwfiztFd0l0ow0YPBu0JmhNvGwXfG2p0NcrTn0jNvoFXlHPqVb+t0DBETtUd/IckMbk4YZoT+7D0yy3LwwDZiPWmzTD8ODVE9U6zaB4NpXnaohYNPlbLyq0uDShX2dGGBVpwIDAQAB");
#endif  
            
#elif UNITY_WEBPLAYER               
            LogUtil.LogProduct("ProductPurchase::InitPaymentSystem none added...");
            
#else   
            // Web/PC - storekit stub for now...
            productManager = productSystem.AddComponent<StoreKitManager>();             
            productEventListener = productSystem.AddComponent<StoreKitEventListener>(); 
            
            LogUtil.LogProduct("ProductPurchase::InitPaymentSystem default added...");
#endif      
            if (productManagerObject != null)
                DontDestroyOnLoad(productManagerObject);    
            
            if (productEventListenerObject != null)
                DontDestroyOnLoad(productEventListenerObject);  
            
            paymentSystemAdded = true;
            
            GetProducts();
        }

    }
    
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
            string productId = product.GetCurrentProductInfoByLocale().productId;
            if (!string.IsNullOrEmpty(productId)) {
                productIdentifiers += productId;
            }
        }
        RequestProductList(productIdentifiers);
        LogUtil.LogProduct("GetProducts: " + productIdentifiers);
    }
    
    public void RequestProductList(string productIdentifiers) {
#if UNITY_IPHONE
        StoreKitBinding.requestProductData( productIdentifiers.Split(',') );
#elif UNITY_ANDROID
#if ANDROID_AMAZON
        
        /*
        productIdentifiers = "";
        foreach(GameProduct product in GameProducts.Instance.GetAll()) {
            if(productIdentifiers.Length > 0) {
                productIdentifiers += ",";
            }
            productIdentifiers += GamePacks.currentGameBundle + "." + product.code.Replace("-", "_");
        }
        */
        
        AmazonIAP.initiateItemDataRequest( productIdentifiers.Split(',') );
#else
        //IABAndroid.purchaseProduct( "kk", 1 );
#endif
#else       
        // Web/PC
#endif
        
    }

    public static void PurchaseProduct(string code, int quantity) {
        if (Instance != null) {
            Instance.purchaseProduct(code, quantity);
        }
    }
    
    public void purchaseProduct(string code, int quantity) {

#if UNITY_IPHONE && !UNITY_EDITOR
        StoreKitBinding.purchaseProduct(code, quantity);
#elif UNITY_ANDROID && !UNITY_EDITOR

#if ANDROID_AMAZON
        //AmazonIAP.initiatePurchaseRequest(GamePacks.currentGameBundle + "." + code);
        AmazonIAP.initiatePurchaseRequest(code);
#else
        GoogleIAB.purchaseProduct(code);
#endif
#else   
        // Web/PC
        purchaseProcessCompleted = true;
        GameStoreController.HandleCurrencyPurchase(code, quantity);
#endif  
    }        
    
    // THIRD PARTY EVENTS
    
    public void SetContentAccessPermissions(string code) {
        
    } 
        
    #if UNITY_IPHONE 
    
    public void purchaseSuccessfulEvent(StoreKitTransaction transaction) {
        LogUtil.LogProduct( "SCREEN purchased product: " + transaction.productIdentifier + ", quantity: " + transaction.quantity );
        //transaction.base64EncodedTransactionReceipt
        SetContentAccessPermissions(transaction.productIdentifier);
        Contents.SetContentAccessTransaction(transaction.productIdentifier, transaction.productIdentifier,
                                             transaction.base64EncodedTransactionReceipt, transaction.quantity, true);
        purchaseProcessCompleted = true;
        
        GameStorePurchaseRecord data = 
            GameStorePurchaseRecord.Create(
                true, transaction.ToJson(), 
                transaction.base64EncodedTransactionReceipt, 
                "Purchase Complete:" + GameStoreController.itemPurchasing.product.display_name, 
                GameStoreController.itemPurchasing.product.description);
        
        GameStoreController.BroadcastThirdPartyPurchaseSuccess(data);
    }
    
    public void purchaseCancelledEvent(string error) {
        purchaseProcessCompleted = true;
        LogUtil.LogProduct( "purchase cancelled with error: " + error );
        
        
        GameStoreController.itemPurchasing = null;
    }
    
    public void purchaseFailedEvent(string error) {
        purchaseProcessCompleted = true;
        LogUtil.LogProduct( "purchase failed with error: " + error );
        
        GameStorePurchaseRecord data = 
            GameStorePurchaseRecord.Create(
                false, null, 
                error, 
                "Purchase Failed For:" + GameStoreController.itemPurchasing.product.display_name, 
                GameStoreController.itemPurchasing.product.description);
        
        GameStoreController.BroadcastThirdPartyPurchaseFailed(data);
    }
    
    public void productPurchaseAwaitingConfirmationEvent(StoreKitTransaction transaction) {
        Debug.Log("productPurchaseAwaitingConfirmationEvent:" + " transaction.productIdentifier:" + transaction.productIdentifier);
    }
    
    public void paymentQueueUpdatedDownloadsEvent(List<StoreKitDownload> downloads) {
        Debug.Log("paymentQueueUpdatedDownloadsEvent");
        
    }
    
    public void productListReceivedEvent(List<StoreKitProduct> products) {
        Debug.Log("paymentQueueUpdatedDownloadsEvent");
        
    }
    
    public void productListRequestFailedEvent(string error) {
        Debug.Log("productListRequestFailedEvent:" + " error:" + error);
        
    }
    
    public void restoreTransactionsFailedEvent(string error) {
        Debug.Log("restoreTransactionsFailedEvent:" + " error:" + error);
        
    }
    
    public void restoreTransactionsFinishedEvent() {
        Debug.Log("restoreTransactionsFinishedEvent");
        
    }
    
    public void transactionUpdatedEvent(StoreKitTransaction transaction) {
        Debug.Log("transactionUpdatedEvent:" + " transaction.productIdentifier:" + transaction.productIdentifier);        
    }
    
    #endif
    
    
    #if UNITY_ANDROID
    #if ANDROID_AMAZON
    
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
    #else
    void billingSupportedEvent(bool success) {
        LogUtil.LogProduct( "billingSupportedEvent: " + success );
        //IABAndroid.restoreTransactions();
    }
    
    void purchaseSucceededEvent(string productId) {
        LogUtil.LogProduct( "purchaseSucceededEvent product: " + productId );
        SetContentAccessPermissions(productId);
        purchaseProcessCompleted = true;
        HandleSuccess();
    }       
    
    void purchaseCancelledEvent(string productId) {
        LogUtil.LogProduct( "purchaseCancelledEvent product: " + productId );
    }
    
    void purchaseRefundedEvent(string productId) {
        LogUtil.LogProduct( "purchaseRefundedEvent product: " + productId );
    }
    
    void purchaseSignatureVerifiedEvent(string signedData, string signature) {
        LogUtil.LogProduct( "purchaseSignatureVerifiedEvent signedData: " + signedData + " signature:" + signature);
    }
    
    void purchaseFailedEvent(string productId) {
        LogUtil.LogProduct( "purchaseFailedEvent product: " + productId );
        HandleError();
    }
    
    void transactionsRestoredEvent() {
        LogUtil.LogProduct( "transactionsRestored");
    }
    
    void transactionRestoreFailedEvent(string error) {
        LogUtil.LogProduct( "transactionRestoreFailedEvent product: " + error );
    }
    #endif
    #endif
    
}

