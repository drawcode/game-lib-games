using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelProducts : GameUIPanelBase {
    
    public static GameUIPanelProducts Instance;
    public GameObject listItemItemPrefab;
    public string currentProductType;
    public string productCodeUse = "";
    public string productTypeUse = "";
    public string productCharacterUse = "";
      
    public static bool isInst {
        get {
            if (Instance != null) {
                return true;
            }
            return false;
        }
    }
    
    public virtual void Awake() {
        
    }
    
    public override void Start() {
        Init();
    }
    
    public override void Init() {
        base.Init();   
    }

    public override void OnEnable() {

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.AddListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);
    }

    public override void OnDisable() {

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);
    }

    public override void OnUIControllerPanelAnimateIn(string classNameTo) {
        if (className == classNameTo) {
            AnimateIn();
        }
    }

    public override void OnUIControllerPanelAnimateOut(string classNameTo) {
        if (className == classNameTo) {
            AnimateOut();
        }
    }

    public override void OnUIControllerPanelAnimateType(string classNameTo, string code) {
        if (className == classNameTo) {
            //
        }
    }

    public override void OnButtonClickEventHandler(string buttonName) {
    
    }

    /*
 public static void ChangeList(BaseGameUIPanelStoreListType listType) {
     if(isInst) {
         Instance.changeList(listType);
     }
 }
 
 public void changeList(BaseGameUIPanelStoreListType listType) {
     panelListType = listType;
     loadData();
     AnimateInList();
 }
 */

    public static void LoadData() {
        if (GameUIPanelProducts.Instance != null) {
            GameUIPanelProducts.Instance.loadData();
        }
    }
    
    public virtual void loadData() {
        loadData(currentProductType);
    }

    public static void LoadData(string productType) {
        if (GameUIPanelProducts.Instance != null) {
            GameUIPanelProducts.Instance.loadData(productType);
        }
    }
    
    public virtual void loadData(string productType) {

        //if(currentProductType == productType
        //   && !string.IsNullOrEmpty(currentProductType)) {
        //    return;
        //}

        StartCoroutine(loadDataCo(productType));
    }

    //bool loading = false;
    string lastProductType = "";
        
    IEnumerator loadDataCo(string productType) {

        //if (loading) {
        //    yield break;
        //}
         
        //loading = true;
                
        LogUtil.Log("LoadDataCo");

        currentProductType = productType;

        if (listGridRoot != null) {
            //listGridRoot.DestroyChildren();
            ClearList();
                
            yield return new WaitForSeconds(1f);
                
            yield return new WaitForEndOfFrame();
                
            loadDataProducts(productType);
                
            yield return new WaitForEndOfFrame();
            listGridRoot.GetComponent<UIGrid>().Reposition();
            yield return new WaitForEndOfFrame();               
        }

        listGridRoot.GetComponent<UIGrid>().Reposition();  
            
        // Reposition scroll for items with less products
        // but keep in place in case user is buying many of one thing 
        // i.e. upgrades etc.

        if(lastProductType != currentProductType) {
            lastProductType = currentProductType;

            RepositionListScroll(0);
        }

        //loading = false;
    }

    public virtual void loadDataRPGUpgrades() {
        loadData(GameProductType.rpgUpgrade);
    }
    
    public virtual void loadDataPowerups() {        
        loadData(GameProductType.powerup);
    }
    
    public virtual void loadDataProducts(string type) {
            
        loadDataProductsItems(type);
    }

    public virtual void loadDataProductsItems(string type) {

        LogUtil.Log("Load loadDataProducts:type:" + type);
                
        List<GameProduct> products = null;

        if (!string.IsNullOrEmpty(type)) {
            products = GameProducts.Instance.GetListByType(type);
        }
        else {
            products = GameProducts.Instance.GetAll();
        }

        LogUtil.Log("Load products: products.Count: " + products.Count);
        
        int i = 0;
        
        foreach (GameProduct product in products) {
            
            GameObject item = NGUITools.AddChild(listGridRoot, listItemItemPrefab);
            item.name = "WeaponItem" + i;

            GameProductInfo info = product.GetDefaultProductInfoByLocale();

            UIUtil.UpdateLabelObject(item.transform, "LabelName", info.display_name);
            UIUtil.UpdateLabelObject(item.transform, "LabelDescription", info.description);
            UIUtil.UpdateLabelObject(item.transform, "LabelCost", info.cost);

            Transform inventoryItem = item.transform.FindChild("Container/Inventory");
            if (inventoryItem != null) {

                double currentValue = 0;

                if (product.type == GameProductType.rpgUpgrade) {
                    currentValue = GameProfileRPGs.Current.GetUpgrades();
                    UIUtil.UpdateLabelObject(inventoryItem, "LabelCurrentValue", currentValue.ToString("N0"));
                }
                else {
                    inventoryItem.gameObject.Hide();
                }
            }
            
            Transform iconTransform = item.transform.FindChild("Container/Icon");
            if (iconTransform != null) {
                GameObject iconObject = iconTransform.gameObject;   
                UISprite iconSprite = iconObject.GetComponent<UISprite>();          
    
                if (iconSprite != null) {
                    iconSprite.alpha = 1f;
                    
                    // TODO change out image...
                }
            }
            
            // Update button action
            
            
            Transform buttonObject = item.transform.FindChild("Container/Button/ButtonAction");
            if (buttonObject != null) {
                UIImageButton button = buttonObject.gameObject.GetComponent<UIImageButton>();
                if (button != null) {
                    
                    // TODO change to get from character skin
                    string productType = product.type;
                    string productCode = product.code;
                    string productCharacter = GameProfileCharacters.Current.GetCurrentCharacterProfileCode();
                    
                    //productCode = productCode.Replace(productType + "-", "");
                                        
                    button.name = BaseUIButtonNames.buttonGameActionItemBuyUse + 
                        "$" + productType + "$" + productCode + "$" + productCharacter;
                }
            }
            
            i++;
        }
    }
    
    public virtual void ClearList() {
        if (listGridRoot != null) {
            listGridRoot.DestroyChildren();
        }
    }
    
    public override void HandleShow() {
        base.HandleShow();
        
        buttonDisplayState = UIPanelButtonsDisplayState.ProductsSections;
        characterDisplayState = UIPanelCharacterDisplayState.Character;
        backgroundDisplayState = UIPanelBackgroundDisplayState.PanelBacker;
        adDisplayState = UIPanelAdDisplayState.BannerBottom;
    }
        
    public override void AnimateIn() {
        
        base.AnimateIn();
        
        LoadData(currentProductType);
    }

    public override void AnimateOut() {
        
        base.AnimateOut();
        
        ClearList();
    }
    
    
}
