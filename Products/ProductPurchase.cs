#define ANDROID_AMAZONN

using System;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;


public class ProductMeta {
	public string price;
	public string currencySymbol;
	public string title;
	public string description;
	public string locale;
	public string productId;
	
	public ProductMeta() {
		Reset();
	}
	
	public void Reset() {
		price = ".99";
		currencySymbol = "$";
		title = "";
		description = "";
		locale = "en";
	}
	
	public string productPrice {
		get {
			return currencySymbol + price;
		}
	}
}

public class ProductPurchase : MonoBehaviour
{	
	public static string PRODUCT_GAME_PURCHASE = "all";
	
	public static string statusPurchaseSuccessful = "status_purchase_successful";
	public static string statusPurchaseFailed = "status_purchase_failed";
	
	public static ProductPurchase Instance;
	
	public GameObject productSystem;
	public GameObject productManagerObject;
	public GameObject productEventListenerObject;	
	public bool paymentSystemAdded = false;
	
	public bool enableProductUnlocks = false;
	
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
#else
	// Web/PC
	public StoreKitManager productManager;
	public StoreKitEventListener productEventListener;
#endif
	
	public bool EnableProductUnlocks {
		get {
			return enableProductUnlocks;
		}
		set {
			enableProductUnlocks = value;
		}
	}
	
	
	void Awake() {        
		if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
		
        Instance = this;
	}
	
	void Start() {
		InitPaymentSystem();
		DontDestroyOnLoad(gameObject);
	}
	
	void InitPaymentSystem() {
		// If iOS add the Storekit plugin
		// else do nothing for now
		
		if(!paymentSystemAdded) {
			LogUtil.LogProduct("ProductPurchase::InitPaymentSystem ");
			
			productSystem = new GameObject("ProductSystem");
			DontDestroyOnLoad(productSystem);
			
#if UNITY_IPHONE							
			productManager = productSystem.AddComponent<StoreKitManager>();				
			productEventListener = productSystem.AddComponent<StoreKitEventListener>();		
			
			LogUtil.Log("ProductPurchase::InitPaymentSystem StoreKit/iOS added...");
#elif UNITY_ANDROID
			// TODO isAmazonDevice()
#if ANDROID_AMAZON
			productManager = productSystem.AddComponent<AmazonIAPManager>();				
			productEventListener = productSystem.AddComponent<AmazonIAPEventListener>();
		
			LogUtil.LogProduct("ProductPurchase::InitPaymentSystem Amazon IAP/Android added...");				
#else
			productManager = productSystem.AddComponent<GoogleIABManager>();				
			productEventListener = productSystem.AddComponent<GoogleIABEventListener>();
		
			LogUtil.LogProduct("ProductPurchase::InitPaymentSystem Google Play IAB/Android added...");	
			
			GoogleIAB.init("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAllfTLDqNeLzmIczqRP4Mramyc0Rd/RUlg+fBRO7VGRPMorv2UKWaxUqbKEYy10ldNu43anV3MHGliSX7wyED1v5GCt8syAeDT59wZZzY7aqMB3CBPmqfFm52ONY7BKaew/uWqjjn1w5Kq4BySLXyBTfrwlnqVsnMnW12lUGPpzgdBODe00JOk+DDjcZcunGB6xXxNA2wPO1pB8VSawVwfiztFd0l0ow0YPBu0JmhNvGwXfG2p0NcrTn0jNvoFXlHPqVb+t0DBETtUd/IckMbk4YZoT+7D0yy3LwwDZiPWmzTD8ODVE9U6zaB4NpXnaohYNPlbLyq0uDShX2dGGBVpwIDAQAB");
#endif		
#else	
			// Web/PC - storekit stub for now...
			productManager = productSystem.AddComponent<StoreKitManager>();				
			productEventListener = productSystem.AddComponent<StoreKitEventListener>();	
			
			LogUtil.LogProduct("ProductPurchase::InitPaymentSystem default added...");
#endif		
			if(productManagerObject != null)
				DontDestroyOnLoad(productManagerObject);	
			
			if(productEventListenerObject != null)
				DontDestroyOnLoad(productEventListenerObject);	
			
			paymentSystemAdded = true;
			
			GetProducts();
		}
		
	}
	
	public void GetProducts() {
		string productIdentifiers = "";
		foreach(GameProduct product in GameProducts.Instance.GetAll()) {
			if(productIdentifiers.Length > 0) {
				productIdentifiers += ",";
			}
			if(!string.IsNullOrEmpty(product.code)) {
				productIdentifiers += product.code.Replace("-", "_");
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
	
	public void PurchaseGame() { 
		PurchaseProduct(PRODUCT_GAME_PURCHASE, 1);
		LogUtil.LogProduct("ProductPurchase::PurchaseGame ...");
	}
	
	public void PurchaseProduct(string code, int quantity) {
		code = code.Replace("-", "_");
#if UNITY_IPHONE
		StoreKitBinding.purchaseProduct(code, quantity);
#elif UNITY_ANDROID
#if ANDROID_AMAZON
		//AmazonIAP.initiatePurchaseRequest(GamePacks.currentGameBundle + "." + code);
		AmazonIAP.initiatePurchaseRequest(code);
#else
		GoogleIAB.purchaseProduct(code);
#endif
#else	
		// Web/PC
#endif	
	}
	
}

