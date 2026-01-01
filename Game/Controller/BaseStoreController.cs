#define ANDROID_AMAZONN
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Events;
using Engine.Game.App;
using Engine.Game.App.BaseApp;
using Engine.Game.Data;
using UnityEngine;

public class GameStoreMessages {

    public static string purchaseStarted = "store-purchase-started";
    public static string purchaseSuccess = "store-purchase-success";
    public static string purchaseFailed = "store-purchase-failed";
    public static string purchaseThirdPartyStarted = "store-third-party-purchase-started";
    public static string purchaseThirdPartySuccess = "store-third-party-purchase-success";
    public static string purchaseThirdPartyFailed = "store-third-party-purchase-failed";
    public static string purchaseThirdPartyCancelled = "store-third-party-purchase-cancelled";
    //
    public static string purchaseAccessSuccess = "store-purchase-access-success";

}

public class GameStorePurchaseRecord : DataObjectItem {
    public bool successful = false;
    public object data;
    public string dataType;
    public string receipt = "";
    public DateTime datePurchased;
    public string messageTitle = "";
    public string messageDescription = "";
    public string productId = "";
    public double quantity = 1;

    public GameStorePurchaseRecord() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        successful = false;
        data = null;
        dataType = "object";
        receipt = "";
        datePurchased = DateTime.Now;
        messageTitle = "";
        messageDescription = "";
        productId = "";
        quantity = 1;
    }

    public static GameStorePurchaseRecord Create(
        bool success,
        object data,
        string dataType,
        string receipt,
        string title,
        string message,
        string productId,
        double quantity) {

        GameStorePurchaseRecord record = new GameStorePurchaseRecord();
        record.successful = success;
        record.data = data;
        record.dataType = dataType;
        record.receipt = receipt;
        record.datePurchased = DateTime.Now;
        record.messageTitle = title;
        record.messageDescription = message;
        record.productId = productId;
        record.quantity = quantity;

        return record;
    }
}

public class GameStorePurchaseDataItem {

    public string gameStorePurchaseType = ProductNetworkType.typeLocal;
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
        dataItem.gameStorePurchaseType = ProductNetworkType.typeLocal;
        dataItem.product = GameProducts.Instance.GetById(productCode);
        dataItem.quantity = quantity;
        data.Add(dataItem);
        return data;
    }

}

public class BaseStoreController : GameObjectBehavior {

    public Dictionary<string, GameStorePurchaseDataItem> itemsPurchasing = new Dictionary<string, GameStorePurchaseDataItem>();

    public virtual void Awake() {

    }

    public virtual void Start() {

    }

    public virtual void Init() {

    }

    public virtual void OnEnable() {

        Messenger<ProductNetworkRecord>.AddListener(
            ProductNetworkMessages.productPurchaseSuccess, onProductPurchaseSuccess);

        Messenger<ProductNetworkRecord>.AddListener(
            ProductNetworkMessages.productPurchaseFailed, onProductPurchaseFailed);

        Messenger<ProductNetworkRecord>.AddListener(
            ProductNetworkMessages.productPurchaseCancelled, onProductPurchaseCancelled);

        Messenger<GameStorePurchaseData>.AddListener(
            GameStoreMessages.purchaseStarted, onStorePurchaseStarted);

        Messenger<GameStorePurchaseRecord>.AddListener(
            GameStoreMessages.purchaseSuccess, onStorePurchaseSuccess);

        Messenger<GameStorePurchaseRecord>.AddListener(
            GameStoreMessages.purchaseFailed, onStorePurchaseFailed);

        Messenger<GameStorePurchaseData>.AddListener(
            GameStoreMessages.purchaseThirdPartyStarted, onStoreThirdPartyPurchaseStarted);

        Messenger<GameStorePurchaseRecord>.AddListener(
            GameStoreMessages.purchaseThirdPartySuccess, onStoreThirdPartyPurchaseSuccess);

        Messenger<GameStorePurchaseRecord>.AddListener(
            GameStoreMessages.purchaseThirdPartyFailed, onStoreThirdPartyPurchaseFailed);
    }

    public virtual void OnDisable() {

        Messenger<ProductNetworkRecord>.RemoveListener(
            ProductNetworkMessages.productPurchaseSuccess, onProductPurchaseSuccess);

        Messenger<ProductNetworkRecord>.RemoveListener(
            ProductNetworkMessages.productPurchaseFailed, onProductPurchaseFailed);

        Messenger<ProductNetworkRecord>.RemoveListener(
            ProductNetworkMessages.productPurchaseCancelled, onProductPurchaseCancelled);

        Messenger<GameStorePurchaseData>.RemoveListener(
            GameStoreMessages.purchaseStarted, onStorePurchaseStarted);

        Messenger<GameStorePurchaseRecord>.RemoveListener(
            GameStoreMessages.purchaseSuccess, onStorePurchaseSuccess);

        Messenger<GameStorePurchaseRecord>.RemoveListener(
            GameStoreMessages.purchaseFailed, onStorePurchaseFailed);

        Messenger<GameStorePurchaseData>.RemoveListener(
            GameStoreMessages.purchaseThirdPartyStarted, onStoreThirdPartyPurchaseStarted);

        Messenger<GameStorePurchaseRecord>.RemoveListener(
            GameStoreMessages.purchaseThirdPartySuccess, onStoreThirdPartyPurchaseSuccess);

        Messenger<GameStorePurchaseRecord>.RemoveListener(
            GameStoreMessages.purchaseThirdPartyFailed, onStoreThirdPartyPurchaseFailed);
    }

    // QUEUE/PROCESSING

    public void SetItemPurchasing(string key, GameStorePurchaseDataItem item) {

        LogUtil.Log("SET SetItemPurchasing:" + " key:" + key + " item.product.code:" + item.product.code);

        LogUtil.Log("BEFORE SetItemPurchasing:" + " itemsPurchasing:" + itemsPurchasing.ToJson());

        itemsPurchasing.Set<string, GameStorePurchaseDataItem>(key, item);

        LogUtil.Log("AFTER SetItemPurchasing:" + " itemsPurchasing:" + itemsPurchasing.ToJson());
    }

    public GameStorePurchaseDataItem GetItemPurchasing(string key) {

        LogUtil.Log("GET GetItemPurchasing:" + " key:" + key);

        LogUtil.Log("BEFORE GetItemPurchasing:" + " itemsPurchasing:" + itemsPurchasing.ToJson());

        GameStorePurchaseDataItem itemPurchasing = itemsPurchasing.Get<GameStorePurchaseDataItem>(key);

        LogUtil.Log("AFTER GetItemPurchasing:" + " itemsPurchasing:" + itemsPurchasing.ToJson() + " itemPurchasing:" + itemPurchasing.ToJson());

        return itemPurchasing;
    }

    public void RemoveItemPurchasing(string key) {

        LogUtil.Log("REMOVING RemoveItemPurchasing:" + " key:" + key);

        LogUtil.Log("BEFORE RemoveItemPurchasing:" + " itemsPurchasing:" + itemsPurchasing.ToJson());

        itemsPurchasing.Remove(key);

        LogUtil.Log("AFTER RemoveItemPurchasing:" + " itemsPurchasing:" + itemsPurchasing.ToJson());
    }

    // PRODUCT PURCHASE EVENTS

    public void onProductPurchaseSuccess(ProductNetworkRecord record) {

        LogUtil.Log("onProductPurchaseSuccess:" + " record:" + record.ToJson());

        if (record == null) {
            LogUtil.Log("record not found");
            return;
        }

        GameProduct product = GameProducts.Instance.GetProductByPlaformProductCode(record.productId);

        if (product == null) {
            LogUtil.Log("Product not found:" + record.productId);
            return;
        }

        GameStorePurchaseDataItem itemPurchasing = GetItemPurchasing(product.code);

        if (itemPurchasing == null) {
            LogUtil.Log("itemPurchasing not found:" + product.code);

            LogUtil.Log("itemsPurchasing:" + itemsPurchasing.ToJson());
        }

        if (itemPurchasing != null) {

            GameStorePurchaseRecord data =
                GameStorePurchaseRecord.Create(
                    true,
                    record.data,
                    record.dataType,
                    record.receipt,
                    "Purchase Complete:" + itemPurchasing.product.GetCurrentProductInfoByLocale().display_name,
                    itemPurchasing.product.GetCurrentProductInfoByLocale().description,
                    record.productId,
                    record.quantity);

            GameStoreController.BroadcastThirdPartyPurchaseSuccess(data);
        }
    }

    public void onProductPurchaseFailed(ProductNetworkRecord record) {

        LogUtil.Log("onProductPurchaseSuccess:" + " record:" + record.ToJson());

        if (record == null) {
            LogUtil.Log("record not found");
            return;
        }

        GameProduct product = GameProducts.Instance.GetProductByPlaformProductCode(record.productId);

        if (product == null) {
            LogUtil.Log("Product not found:" + record.productId);
            return;
        }

        GameStorePurchaseDataItem itemPurchasing = GetItemPurchasing(product.code);

        if (itemPurchasing == null) {
            LogUtil.Log("itemPurchasing not found:" + product.code);

            LogUtil.Log("itemsPurchasing:" + itemsPurchasing.ToJson());
        }

        if (itemPurchasing != null) {

            GameStorePurchaseRecord data =
                GameStorePurchaseRecord.Create(
                    false,
                    record.data,
                    record.dataType,
                    record.receipt,
                    "Purchase FAILED:" + itemPurchasing.product.GetCurrentProductInfoByLocale().display_name,
                    itemPurchasing.product.GetCurrentProductInfoByLocale().description,
                    record.productId,
                    record.quantity);

            GameStoreController.BroadcastThirdPartyPurchaseFailed(data);
        }
    }

    public void onProductPurchaseCancelled(ProductNetworkRecord record) {

        LogUtil.Log("onProductPurchaseSuccess:" + " record:" + record.ToJson());

        if (record == null) {
            LogUtil.Log("record not found");
            return;
        }

        GameProduct product = GameProducts.Instance.GetProductByPlaformProductCode(record.productId);

        if (product == null) {
            LogUtil.Log("Product not found:" + record.productId);
            return;
        }

        GameStorePurchaseDataItem itemPurchasing = GetItemPurchasing(product.code);

        if (itemPurchasing == null) {
            LogUtil.Log("itemPurchasing not found:" + product.code);

            LogUtil.Log("itemsPurchasing:" + itemsPurchasing.ToJson());
        }

        if (itemPurchasing != null) {

            GameStorePurchaseRecord data =
                GameStorePurchaseRecord.Create(
                    false,
                    record.data,
                    record.dataType,
                    record.receipt,
                    "Purchase CANCELLED:" + itemPurchasing.product.GetCurrentProductInfoByLocale().display_name,
                    itemPurchasing.product.GetCurrentProductInfoByLocale().description,
                    product.code,
                    record.quantity);

            GameStoreController.BroadcastThirdPartyPurchaseCancelled(data);
        }
    }

    // STORE EVENTS

    public virtual void onStorePurchaseStarted(GameStorePurchaseData data) {

    }

    public virtual void onStorePurchaseSuccess(GameStorePurchaseRecord data) {

        if (data == null)
            return;

        GameStorePurchaseDataItem itemPurchasing = GetItemPurchasing(data.productId);

        if (itemPurchasing != null) {

            GameProduct product = itemPurchasing.product;

            GameProductInfo gameProductInfo = product.GetCurrentProductInfoByLocale();

            decimal cost = 0;
            decimal.TryParse(gameProductInfo.cost, out cost);

            int quantity = 1;
            quantity = Convert.ToInt32(itemPurchasing.quantity);

            Dictionary<string, object> dataDict =
                data.ToDataObject<Dictionary<string, object>>();

            ResetPurchase(itemPurchasing.product.code);

            AnalyticsNetworks.LogEventStorePurchase(
                product.code,
                quantity,
                cost,
                gameProductInfo.currency,
                dataDict);

        }

        Messenger<string, string>.Broadcast(GameNotificationMessages.gameQueueInfo, data.messageTitle, data.messageDescription);

        //UINotificationDisplay.QueueInfo(data.messageTitle, data.messageDescription);
    }

    public virtual void onStorePurchaseFailed(GameStorePurchaseRecord data) {

        if (data == null)
            return;

        GameStorePurchaseDataItem itemPurchasing = GetItemPurchasing(data.productId);

        if (itemPurchasing != null) {
            ResetPurchase(itemPurchasing.product.code);
        }

        Messenger<string, string>.Broadcast(GameNotificationMessages.gameQueueError, data.messageTitle, data.messageDescription);
        //UINotificationDisplay.QueueError(data.messageTitle, data.messageDescription);
    }

    // THIRD PARTY

    public virtual void onStoreThirdPartyPurchaseStarted(GameStorePurchaseData data) {

    }

    public virtual void onStoreThirdPartyPurchaseSuccess(GameStorePurchaseRecord data) {

        LogUtil.Log("onStoreThirdPartyPurchaseSuccess");

        if (data != null) {

            LogUtil.Log("onStoreThirdPartyPurchaseSuccess: data.messageTitle:" + data.messageTitle);

            Messenger<string, string>.Broadcast(GameNotificationMessages.gameQueueInfo, data.messageTitle, data.messageDescription);
            //UINotificationDisplay.QueueInfo(data.messageTitle, data.messageDescription);
        }

        GameProduct product = GameProducts.Instance.GetProductByPlaformProductCode(data.productId);

        if (product == null) {
            return;
        }

        GameStorePurchaseDataItem itemPurchasing = GetItemPurchasing(product.code);

        if (itemPurchasing != null) {
            LogUtil.Log("onStoreThirdPartyPurchaseSuccess: itemPurchasing.product:" + itemPurchasing.product.code);

            if (product.type == GameProductType.currency) {
                GameStoreController.HandleCurrencyPurchase(itemPurchasing.product, itemPurchasing.quantity);
            }
            else if (product.type == GameProductType.access) {
                GameStoreController.HandleAccessPurchase(itemPurchasing.product, itemPurchasing.quantity);
            }

            // ANALYTICS

            GameProductInfo gameProductInfo = product.GetCurrentProductInfoByLocale();

            decimal cost = 0;
            decimal.TryParse(gameProductInfo.cost, out cost);

            int quantity = 1;
            quantity = Convert.ToInt32(itemPurchasing.quantity);

            Dictionary<string, object> dataDict = data.ToDataObject<Dictionary<string, object>>(false);

            // TODO LOCALIZE CURRENCY 

            string currency = gameProductInfo.currency;

            if (currency.IsNotNullOrEmpty()) {
                if (currency == "$") {
                    currency = "USD";
                }
            }

            ResetPurchase(itemPurchasing.product.code);

            AnalyticsNetworks.LogEventStoreThirdPartyPurchase(
                product.code,
                quantity,
                product.GetPlatformProductCode(),
                cost,
                currency,
                data.receipt,
                null,
                dataDict);
        }
    }

    public virtual void onStoreThirdPartyPurchaseFailed(GameStorePurchaseRecord data) {

        if (data != null) {
            LogUtil.Log("onStoreThirdPartyPurchaseFailed: data.messageTitle:" + data.messageTitle);

            Messenger<string, string>.Broadcast(GameNotificationMessages.gameQueueInfo, data.messageTitle, data.messageDescription);
            //UINotificationDisplay.QueueInfo(data.messageTitle, data.messageDescription);
        }

        GameProduct product = GameProducts.Instance.GetProductByPlaformProductCode(data.productId);

        if (product == null) {
            return;
        }

        GameStorePurchaseDataItem itemPurchasing = GetItemPurchasing(product.code);

        if (itemPurchasing != null) {
            LogUtil.Log("onStoreThirdPartyPurchaseFailed: itemPurchasing.product:" + itemPurchasing.product.code);
            ResetPurchase(itemPurchasing.product.code);
        }
    }

    public bool IsPurchasing(string key) {
        return GetItemPurchasing(key) != null;
    }

    public void ResetPurchase(string key) {
        RemoveItemPurchasing(key);
    }

    public virtual void purchase(GameStorePurchaseData data) {

        foreach (GameStorePurchaseDataItem item in data.items) {

            if (item.product != null) {

                if (IsPurchasing(item.product.code)) {
                    return;
                }

                SetItemPurchasing(item.product.code, item);

                if (item.product.type == GameProductType.currency
                    || item.product.type == GameProductType.access) {

                    // do third party process and event

                    GameStoreController.PurchaseThirdParty(item.product, item.quantity);
                }
                else {
                    // do local or server process and event

                    if (checkIfCanPurchase(item.product)) { // has the money

                        GameStoreController.HandlePurchase(item.product, item.quantity);
                    }
                    else {

                        GameStoreController.BroadcastPurchaseFailed(
                            GameStorePurchaseRecord.Create(
                                false,
                                data,
                                data.GetType().ToString(),
                                "",
                                "Purchase Unsuccessful",
                                "Not enough coins to purchase. Earn more coins by playing or training.",
                                item.product.code,
                                item.quantity));

                    }
                }
            }

            // TODO handle multiple events, for now only purchase one at a time...
            break;
        }
    }
    public virtual void purchasesRestore() {
        ProductNetworks.RestoreTransactions();
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

        double productCost = double.Parse(product.GetDefaultProductInfoByLocale().cost);

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
        ProductNetworks.PurchaseProduct(
            gameProduct.GetPlatformProductCode(), (int)quantity);
    }

    public virtual void handlePurchase(GameProduct gameProduct, double quantity) {

        // HANDLE ACCOUNTING

        double currentCurrency = GameProfileRPGs.Current.GetCurrency();
        double productCost = double.Parse(gameProduct.GetDefaultProductInfoByLocale().cost);

        // TODO quantity...

        if (currentCurrency > productCost) {
            // can buy

            GameProfileRPGs.Current.SubtractCurrency(productCost);

            // HANDLE INVENTORY
            GameStoreController.HandleInventory(gameProduct, quantity);
        }
    }

    public virtual void handleInventory(GameProduct gameProduct, double quantity) {

        //string message = "Enjoy your new purchase.";

        bool doPurchase = false;

        if (gameProduct.type == GameProductType.rpgUpgrade) {
            // Add upgrades

            double val = gameProduct.GetDefaultProductInfoByLocale().quantity;
            GameProfileRPGs.Current.AddUpgrades(val);

            doPurchase = true;

            //message = "Advance your character with your upgrades and get to top of the game";

        }
        else if (gameProduct.type == GameProductType.powerup) {
            // Add upgrades

            if (gameProduct.code.Contains("rpg-recharge-full")) {
                GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergyAndHealth(1f, 1f);
                //message = "Recharging your health + energy...";
                doPurchase = true;
            }
            else if (gameProduct.code.Contains("rpg-recharge-health")) {
                GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(1f);
                //message = "Recharging your health...";
                doPurchase = true;
            }
            else if (gameProduct.code.Contains("rpg-recharge-energy")) {
                GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(1f);
                //message = "Recharging your health...";
                doPurchase = true;
            }

        }
        else if (gameProduct.type == GameProductType.currency) {
            // Add skraight cash moneh
            GameStoreController.HandleCurrencyPurchase(gameProduct, quantity);
            doPurchase = true;

        }
        else if (gameProduct.type == GameProductType.access) {
            // Add skraight cash moneh
            GameStoreController.HandleAccessPurchase(gameProduct, quantity);
            doPurchase = true;

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
            doPurchase = true;

        }
        else {

            // trigger purchased and rewards from the product items
            handleGameProductItems(gameProduct);

            doPurchase = true;
        }

        if (doPurchase) {
            GameStoreController.BroadcastPurchaseSuccess(
                GameStorePurchaseRecord.Create(
                    true,
                    gameProduct,
                    gameProduct.GetType().ToString(),
                    "",
                    "Purchase Successful:" +
                    gameProduct.GetCurrentProductInfoByLocale().display_name,
                    gameProduct.GetCurrentProductInfoByLocale().description,
                    gameProduct.code,
                    quantity));
        }
    }

    public virtual void handleGameProductItems(GameProduct gameProduct) {

        if (gameProduct.data == null) {
            return;
        }

        if (gameProduct.data.items == null) {
            return;
        }

        foreach (GameDataObject item in gameProduct.data.items) {

            if (item.type == GameProductType.character) {

                GameProfileCharacters.Current.AddCharacter(item.code);
            }
            else if (item.type == GameProductType.currency) {

                // Add skraight cash moneh
                GameProfileRPGs.Current.AddCurrency(item.valDouble);
            }
            else if (item.type == GameProductType.access) {

                // Add permission
                GameProfiles.Current.SetAccessPermission(item.valString);
            }
            else if (item.type == GameProductType.item) {

                // Add skraight cash moneh
                //GameProfileRPGs.Current.AddCurrency(item.valDouble);
                //GameProfileCharacters.Current.

            }
            else if (item.type == GameProductType.powerup) {
                // Add upgrades

                if (gameProduct.code.Contains("rpg-recharge-full")) {

                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergyAndHealth(1f, 1f);
                    //message = "Recharging your health + energy...";
                    //doPurchase = true;
                }
                else if (gameProduct.code.Contains("rpg-recharge-health")) {

                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(1f);
                    //message = "Recharging your health...";
                    //doPurchase = true;
                }
                else if (gameProduct.code.Contains("rpg-recharge-energy")) {

                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(1f);
                    //message = "Recharging your health...";
                    //doPurchase = true;
                }

            }
            else if (item.type == GameProductType.rpgUpgrade) {

                GameProfileRPGs.Current.AddUpgrades(item.valDouble);
            }
        }
    }

    public virtual void handleCurrencyPurchase(GameProduct gameProduct, double quantity) {

        LogUtil.Log("GameStoreController:handleCurrencyPurchase:productId:" + gameProduct.code);

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

        ResetPurchase(gameProduct.code);
    }

    public virtual void handleAccessPurchase(GameProduct gameProduct, double quantity) {

        string productCode = gameProduct.code;

        LogUtil.Log("GameStoreController:handleAccessPurchase:productId:" + productCode);

        GameProfiles.Current.SetAccessPermission(productCode);

        GameState.SaveProfile();

        Messenger<string>.Broadcast(
            GameStoreMessages.purchaseAccessSuccess, productCode);

        ResetPurchase(productCode);
    }
}