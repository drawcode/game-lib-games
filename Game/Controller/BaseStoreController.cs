#define ANDROID_AMAZONN
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Events;

using UnityEngine;

public class GameStoreMessages {

    public static string purchaseStarted = "store-purchase-started";
    public static string purchaseSuccess = "store-purchase-success";
    public static string purchaseFailed = "store-purchase-failed";
    public static string purchaseThirdPartyStarted = "store-third-party-purchase-started";
    public static string purchaseThirdPartySuccess = "store-third-party-purchase-success";
    public static string purchaseThirdPartyFailed = "store-third-party-purchase-failed";
    public static string purchaseThirdPartyCancelled = "store-third-party-purchase-cancelled";

}

public class GameStorePurchaseRecord : DataObjectItem {
    public bool successful = false;
    public object data;
    public string receipt = "";
    public DateTime datePurchased;
    public string messageTitle = "";
    public string messageDescription = "";

    public GameStorePurchaseRecord() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        successful = false;
        data = null;
        receipt = "";
        datePurchased = DateTime.Now;
        messageTitle = "";
        messageDescription = "";
    }

    public static GameStorePurchaseRecord Create(
        bool success,
        object data,
        string receipt,
        string title,
        string message) {

        GameStorePurchaseRecord record = new GameStorePurchaseRecord();
        record.successful = success;
        record.data = data;
        record.receipt = receipt;
        record.datePurchased = DateTime.Now;
        record.messageTitle = title;
        record.messageDescription = message;

        return record;
    }
}

public class GameStorePurchaseDataItem {

    public string gameStorePurchaseType = ProductPurchaseType.typeLocal;
    public GameProduct product;
    public double quantity;
}

public class GameStorePurchaseData {

    public List<GameStorePurchaseDataItem> items;

    public GameStorePurchaseData() {
        Reset();
    }

    public void Reset() {
        items = new List<GameStorePurchaseDataItem>();
    }

    public void Add(GameStorePurchaseDataItem dataItem) {
        if (!items.Contains(dataItem)) {
            items.Add(dataItem);
        }
    }

    public static GameStorePurchaseData PurchaseData(string productCode, double quantity) {
        GameStorePurchaseData data = new GameStorePurchaseData();
        GameStorePurchaseDataItem dataItem = new GameStorePurchaseDataItem();
        dataItem.gameStorePurchaseType = ProductPurchaseType.typeLocal;
        dataItem.product = GameProducts.Instance.GetById(productCode);
        dataItem.quantity = quantity;
        data.Add(dataItem);
        return data;
    }

}

public class BaseStoreController : MonoBehaviour {
    
    public static GameStorePurchaseDataItem itemPurchasing;

    public virtual void Awake() {

    }

    public virtual void Start() {

    }

    public virtual void Init() {

    }

    public virtual void OnEnable() {

        Messenger<ProductPurchaseRecord>.AddListener(
            ProductPurchaseMessages.productPurchaseSuccess, onProductPurchaseSuccess);
        
        Messenger<ProductPurchaseRecord>.AddListener(
            ProductPurchaseMessages.productPurchaseFailed, onProductPurchaseFailed);
        
        Messenger<ProductPurchaseRecord>.AddListener(
            ProductPurchaseMessages.productPurchaseCancelled, onProductPurchaseCancelled);

        Messenger<GameStorePurchaseData>.AddListener(GameStoreMessages.purchaseStarted, onStorePurchaseStarted);
        Messenger<GameStorePurchaseRecord>.AddListener(GameStoreMessages.purchaseSuccess, onStorePurchaseSuccess);
        Messenger<GameStorePurchaseRecord>.AddListener(GameStoreMessages.purchaseFailed, onStorePurchaseFailed);
        
        Messenger<GameStorePurchaseData>.AddListener(GameStoreMessages.purchaseThirdPartyStarted, onStoreThirdPartyPurchaseStarted);
        Messenger<GameStorePurchaseRecord>.AddListener(GameStoreMessages.purchaseThirdPartySuccess, onStoreThirdPartyPurchaseSuccess);
        Messenger<GameStorePurchaseRecord>.AddListener(GameStoreMessages.purchaseThirdPartyFailed, onStoreThirdPartyPurchaseFailed);
    }
    
    public virtual void OnDisable() {

        Messenger<ProductPurchaseRecord>.RemoveListener(
            ProductPurchaseMessages.productPurchaseSuccess, onProductPurchaseSuccess);
        
        Messenger<ProductPurchaseRecord>.RemoveListener(
            ProductPurchaseMessages.productPurchaseFailed, onProductPurchaseFailed);
        
        Messenger<ProductPurchaseRecord>.RemoveListener(
            ProductPurchaseMessages.productPurchaseCancelled, onProductPurchaseCancelled);
        
        Messenger<GameStorePurchaseData>.RemoveListener(GameStoreMessages.purchaseStarted, onStorePurchaseStarted);
        Messenger<GameStorePurchaseRecord>.RemoveListener(GameStoreMessages.purchaseSuccess, onStorePurchaseSuccess);
        Messenger<GameStorePurchaseRecord>.RemoveListener(GameStoreMessages.purchaseFailed, onStorePurchaseFailed);
        
        Messenger<GameStorePurchaseData>.RemoveListener(GameStoreMessages.purchaseThirdPartyStarted, onStoreThirdPartyPurchaseStarted);
        Messenger<GameStorePurchaseRecord>.RemoveListener(GameStoreMessages.purchaseThirdPartySuccess, onStoreThirdPartyPurchaseSuccess);
        Messenger<GameStorePurchaseRecord>.RemoveListener(GameStoreMessages.purchaseThirdPartyFailed, onStoreThirdPartyPurchaseFailed);
    }
    
    
    public void onProductPurchaseSuccess(ProductPurchaseRecord record) {
        
        GameStorePurchaseRecord data =
            GameStorePurchaseRecord.Create(
                true, record.data,
                record.receipt,
                "Purchase Complete:" + itemPurchasing.product.GetCurrentProductInfoByLocale().title,
                itemPurchasing.product.GetCurrentProductInfoByLocale().description);
        
        GameStoreController.BroadcastThirdPartyPurchaseSuccess(data);
    }
    
    public void onProductPurchaseFailed(ProductPurchaseRecord record) {

        GameStorePurchaseRecord data =
            GameStorePurchaseRecord.Create(
                false, record.data,
                record.receipt,
                "Purchase FAILED:" + itemPurchasing.product.GetCurrentProductInfoByLocale().title,
                itemPurchasing.product.GetCurrentProductInfoByLocale().description);
        
        GameStoreController.BroadcastThirdPartyPurchaseFailed(data);
    }
    
    public void onProductPurchaseCancelled(ProductPurchaseRecord record) {
        
        GameStorePurchaseRecord data =
            GameStorePurchaseRecord.Create(
                false, record.data,
                record.receipt,
                "Purchase CANCELLED:" + itemPurchasing.product.GetCurrentProductInfoByLocale().title,
                itemPurchasing.product.GetCurrentProductInfoByLocale().description);
        
        GameStoreController.BroadcastThirdPartyPurchaseCancelled(data);
    }

    public virtual void onStorePurchaseStarted(GameStorePurchaseData data) {

    }

    public virtual void onStorePurchaseSuccess(GameStorePurchaseRecord data) {   
        ResetPurchase();     
        UINotificationDisplay.Instance.QueueInfo(data.messageTitle, data.messageDescription);  
    }

    public virtual void onStorePurchaseFailed(GameStorePurchaseRecord data) {    
        ResetPurchase();    
        UINotificationDisplay.Instance.QueueError(data.messageTitle, data.messageDescription);
    }

    public virtual void onStoreThirdPartyPurchaseStarted(GameStorePurchaseData data) {

    }

    public virtual void onStoreThirdPartyPurchaseSuccess(GameStorePurchaseRecord data) {
        
        Debug.Log("onStoreThirdPartyPurchaseSuccess");

        if (data != null) {
            
            Debug.Log("onStoreThirdPartyPurchaseSuccess: data.messageTitle:" + data.messageTitle);
            UINotificationDisplay.Instance.QueueInfo(data.messageTitle, data.messageDescription);      
        }

        if (itemPurchasing != null) {
            Debug.Log("onStoreThirdPartyPurchaseSuccess: itemPurchasing.product:" + itemPurchasing.product.code);
            GameStoreController.HandleCurrencyPurchase(itemPurchasing.product, itemPurchasing.quantity); 
            itemPurchasing = null;
        }
    }

    public virtual void onStoreThirdPartyPurchaseFailed(GameStorePurchaseRecord data) {
        
        if (data != null) {
            Debug.Log("onStoreThirdPartyPurchaseFailed: data.messageTitle:" + data.messageTitle);
            UINotificationDisplay.Instance.QueueInfo(data.messageTitle, data.messageDescription);      
        }
        
        if (itemPurchasing != null) {
            Debug.Log("onStoreThirdPartyPurchaseFailed: itemPurchasing.product:" + itemPurchasing.product.code);
            itemPurchasing = null;
        }
    }

    public bool IsPurchasing() {
        return itemPurchasing != null;
    }

    public void ResetPurchase() {
        itemPurchasing = null;
    }

    public virtual void purchase(GameStorePurchaseData data) {

        if(IsPurchasing()) {
            //return;
        }

        foreach (GameStorePurchaseDataItem item in data.items) {

            if (item.product != null) {
    
                if (item.product.type == GameProductType.currency) {
                    // do third party process and event    
                    
                    itemPurchasing = item;

                    GameStoreController.PurchaseThirdParty(item.product, item.quantity); 
                }
                else {
                    // do local or server process and event

                    if (checkIfCanPurchase(item.product)) { // has the money
    
                        GameStoreController.HandlePurchase(item.product, item.quantity);
                    }
                    else {

                        GameStoreController.BroadcastPurchaseFailed(
                            GameStorePurchaseRecord.Create(false,
                                data, "",
                                "Purchase Unsuccessful",
                                "Not enough coins to purchase. Earn more coins by playing or training."));
    
                    }
                }
            }

            // TODO handle multiple events, for now only purchase one at a time...
            break;
        }
    }

    public virtual void purchase(string productId, double quantity) {
        GameStoreController.Purchase(
            GameStorePurchaseData.PurchaseData(productId, quantity));
    }

    public virtual void broadcastPurchaseStarted(GameStorePurchaseData data) {
        Messenger<GameStorePurchaseData>.Broadcast(GameStoreMessages.purchaseStarted, data);
    }

    public virtual void broadcastPurchaseSuccess(GameStorePurchaseRecord data) {
        Messenger<GameStorePurchaseRecord>.Broadcast(GameStoreMessages.purchaseSuccess, data);
    }

    public virtual void broadcastPurchaseFailed(GameStorePurchaseRecord data) {
        Messenger<GameStorePurchaseRecord>.Broadcast(GameStoreMessages.purchaseFailed, data);
    }

    public virtual void broadcastThirdPartyPurchaseStarted(GameStorePurchaseData data) {
        Messenger<GameStorePurchaseData>.Broadcast(GameStoreMessages.purchaseThirdPartyStarted, data);
    }

    public virtual void broadcastThirdPartyPurchaseSuccess(GameStorePurchaseRecord data) {
        Messenger<GameStorePurchaseRecord>.Broadcast(GameStoreMessages.purchaseThirdPartySuccess, data);
    }

    public virtual void broadcastThirdPartyPurchaseFailed(GameStorePurchaseRecord data) {
        Messenger<GameStorePurchaseRecord>.Broadcast(GameStoreMessages.purchaseThirdPartyFailed, data);
    }
    
    public virtual void broadcastThirdPartyPurchaseCancelled(GameStorePurchaseRecord data) {
        Messenger<GameStorePurchaseRecord>.Broadcast(GameStoreMessages.purchaseThirdPartyCancelled, data);
    }


    public virtual bool checkIfCanPurchase(GameProduct product) {
        double currentCurrency = GameProfileRPGs.Current.GetCurrency();

        double productCost = double.Parse(product.GetDefaultProductInfoByLocale().price);

        if (currentCurrency > productCost) {
            return true;
        }

        return false;
    }

    public virtual void purchaseLocal(GameProduct gameProduct, double quantity) {

        // server
        // event
    }

    public virtual void purchaseServer(GameProduct gameProduct, double quantity) {

    }

    public virtual void purchaseThirdParty(GameProduct gameProduct, double quantity) {
        ProductPurchase.PurchaseProduct(gameProduct.GetCurrentProductInfoByLocale().productId, (int)quantity);
    }

    public virtual void handlePurchase(GameProduct gameProduct, double quantity) {

        // HANDLE ACCOUNTING

        double currentCurrency = GameProfileRPGs.Current.GetCurrency();
        double productCost = double.Parse(gameProduct.GetDefaultProductInfoByLocale().price);

        // TODO quantity...

        if (currentCurrency > productCost) {
            // can buy

            GameProfileRPGs.Current.SubtractCurrency(productCost);

            // HANDLE INVENTORY
            GameStoreController.HandleInventory(gameProduct, quantity);
        }
    }

    public virtual void handleInventory(GameProduct gameProduct, double quantity) {

        string message = "Enjoy your new purchase.";

        if (gameProduct.type == GameProductType.rpgUpgrade) {
            // Add upgrades

            double val = gameProduct.GetDefaultProductInfoByLocale().quantity;
            GameProfileRPGs.Current.AddUpgrades(val);

            message = "Advance your character with your upgrades and get to top of the game";

        }
        else if (gameProduct.type == GameProductType.powerup) {
            // Add upgrades

            if (gameProduct.code.Contains("rpg-recharge-full")) {
                GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergyAndHealth(1f, 1f);
                message = "Recharging your health + energy...";
            }
            else if (gameProduct.code.Contains("rpg-recharge-health")) {
                GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(1f);
                message = "Recharging your health...";
            }
            else if (gameProduct.code.Contains("rpg-recharge-energy")) {
                GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(1f);
                message = "Recharging your health...";
            }

        }
        else if (gameProduct.type == GameProductType.currency) {
            // Add skraight cash moneh
            GameStoreController.HandleCurrencyPurchase(gameProduct, quantity);

        }
        else if (gameProduct.type == GameProductType.characterSkin) {
            // TODO lookup skin and apply

            /*
            GameCharacterSkin skin = GameCharacterSkins.Instance.GetById(productCodeUse);
            GameCharacterSkinItemRPG rpg = skin.GetGameCharacterSkinByData(productCharacterUse, weaponType);
            if(rpg != null) {
                GameProfileCharacters.Current.SetCurrentCharacterCostumeCode(rpg.prefab);
            }
            */
        }

        GameStoreController.BroadcastPurchaseSuccess(
            GameStorePurchaseRecord.Create(true,
                gameProduct, "",
                "Purchase Successful",
                message));

    }

    public virtual void handleCurrencyPurchase(GameProduct gameProduct, double quantity) {
                
        Debug.Log("GameStoreController:handleCurrencyPurchase:productId:" + gameProduct.code);

        if (gameProduct.code == "currency-tier-1") {
            GameProfileRPGs.Current.AddCurrency(1000);
        }
        else if (gameProduct.code == "currency-tier-2") {
            GameProfileRPGs.Current.AddCurrency(3500);
        }
        else if (gameProduct.code == "currency-tier-3") {
            GameProfileRPGs.Current.AddCurrency(15000);
        }
        else if (gameProduct.code == "currency-tier-5") {
            GameProfileRPGs.Current.AddCurrency(50000);
        }
        else if (gameProduct.code == "currency-tier-10") {
            GameProfileRPGs.Current.AddCurrency(100000);
        }
        else if (gameProduct.code == "currency-tier-20") {
            GameProfileRPGs.Current.AddCurrency(250000);
        }
        else if (gameProduct.code == "currency-tier-50") {
            GameProfileRPGs.Current.AddCurrency(1000000);
        }

        itemPurchasing = null;
    }       
}