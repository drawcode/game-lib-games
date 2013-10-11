using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UICustomizeCharacterRPGItemMessages {
    public static string rpgItemCodeChanged = "rpg-item-code-changed";
    public static string rpgUpgradesChanged = "rpg-upgrades-changed";
}

//Messenger<string, double>.AddListener(UIRPGItemMessages.rpgItemCodeUp, OnRPGItemCodeUp);
//Messenger<string, double>.RemoveListener(UIRPGItemMessages.rpgItemCodeUp, OnRPGItemCodeDown);
//Messenger<string, double>.Broadcast(UIRPGItemMessages.rpgItemCodeUp, rpgCode, 1);

public class UICustomizeCharacterRPG: UIAppPanelBaseList {

    public UILabel labelUpgradesAvailable;
    public UIImageButton buttonResetRPG;
    public UIImageButton buttonSaveRPG;
    public UIImageButton buttonBuyUpgrades;
    GameProfileRPGItem profileGameItemRPG;
    string currentCharacterCode = "default";
    public static UICustomizeCharacterRPG Instance;
    public GameObject listItemPrefab;
    public double upgradesAvailable = 0;
    public double modifierDisplay = 10;

    public void Awake() {

        if(Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }

        Instance = this;
    }
     
    public static bool isInst {
        get {
            if(Instance != null) {
                return true;
            }
            return false;
        }
    }

    public override void Start() {
        Init();
    }
 
    public override void Init() {
        base.Init();
     
        loadData();
    }
 
    void OnEnable() {
        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, string, double>.AddListener(UICustomizeCharacterRPGItemMessages.rpgUpgradesChanged, OnRPGUpgradesHandler);

    }
     
    void OnDisable() {
        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, string, double>.RemoveListener(UICustomizeCharacterRPGItemMessages.rpgUpgradesChanged, OnRPGUpgradesHandler);
    }

    void OnRPGUpgradesHandler(string rpgCodeFrom, string characterCodeFrom, double valFrom) {

        double val = valFrom * modifierDisplay;

        upgradesAvailable -= val;

        if(upgradesAvailable < 0) {
            upgradesAvailable = 0;
        }

        SetUpgradesAvailable(upgradesAvailable);
    }

    void OnButtonClickEventHandler(string buttonName) {

        if(UIUtil.IsButtonClicked(buttonSaveRPG, buttonName)) {
            SaveRPG();
        }
        //else if(UIUtil.IsButtonClicked(buttonResetRPG, buttonObj.name)) {
        //    ResetRPG();
        //}
        else if(UIUtil.IsButtonClicked(buttonBuyUpgrades, buttonName)) {
            BuyUpgrades();
        }

    }

    public void SaveRPG() {
        foreach(UICustomizeCharacterRPGItem item
            in listGridRoot.GetComponentsInChildren<UICustomizeCharacterRPGItem>(true)) {

            if(item.rpgCode.ToLower() == GameItemRPGAttributes.speed) {
                profileGameItemRPG.SetSpeed(item.currentValue);
            }
            else if(item.rpgCode.ToLower() == GameItemRPGAttributes.energy) {
                profileGameItemRPG.SetEnergy(item.currentValue);
            }
            else if(item.rpgCode.ToLower() == GameItemRPGAttributes.health) {
                profileGameItemRPG.SetHealth(item.currentValue);
            }
            else if(item.rpgCode.ToLower() == GameItemRPGAttributes.attack) {
                profileGameItemRPG.SetAttack(item.currentValue);
            }
        }

        Debug.Log("profileGameItemRPG:attack:" + profileGameItemRPG.GetAttack());

        GameProfileCharacters.Current.SetCharacterRPG(currentCharacterCode, profileGameItemRPG);
        GameProfileRPGs.Current.SetUpgrades(upgradesAvailable);

        GameState.SaveProfile();

        loadData();
    }

    public void ResetRPG() {
        loadData();
    }

    public void BuyUpgrades() {

    }

    public void HandleRPGItemChanged(string rpgCodeFrom, string characterCodeFrom, double valFrom) {

    }

    public void SetUpgradesAvailable(double upgradesAvailableTo) {
        if(labelUpgradesAvailable != null) {
            upgradesAvailable = upgradesAvailableTo;
            labelUpgradesAvailable.text = upgradesAvailableTo.ToString("N0");
        }
    }
 
    public static void LoadData() {
        if(Instance != null) {
            Instance.loadData();
        }
    }

    public void loadData() {
        StartCoroutine(loadDataCo());
    }
 
    IEnumerator loadDataCo() {       
     
        Debug.Log("LoadDataCo");
     
        if(listGridRoot != null) {
            listGridRoot.DestroyChildren();
         
            yield return new WaitForEndOfFrame();
                 
            loadDataRPG();
         
            yield return new WaitForEndOfFrame();
            listGridRoot.GetComponent<UIGrid>().Reposition();
            yield return new WaitForEndOfFrame();                
        }
    }
     
    public void loadDataRPG() {
     
        Debug.Log("Load RPGs:");

        int i = 0;
     
        SetUpgradesAvailable(GameProfileRPGs.Current.GetUpgrades());
     
        profileGameItemRPG = GameProfileCharacters.Current.GetCharacterRPG(currentCharacterCode);

        Debug.Log("profileGameItemRPG:attack2:" + profileGameItemRPG.GetAttack());

        List<string> rpgItems = new List<string>();
     
        rpgItems.Add(GameItemRPGAttributes.speed);
        rpgItems.Add(GameItemRPGAttributes.health);      
        rpgItems.Add(GameItemRPGAttributes.energy);      
        rpgItems.Add(GameItemRPGAttributes.attack);          
        //rpgItems.Add(GameItemRPGAttributes.defense);           
     
        foreach(string rpgItem in rpgItems) {
         
            GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
            item.name = "AItem" + i;
         
            UICustomizeCharacterRPGItem rpg = item.transform.GetComponent<UICustomizeCharacterRPGItem>();
         
            if(rpg != null) {
                rpg.Load(rpgItem);
            }                
                         
            i++;
        }
    }

    void Update() {

        if(GameConfigs.isGameRunning) {
            return;
        }

        if(!isVisible) {
            return;
        }

        if(Input.GetKeyDown("u")) {
            GameProfileRPGs.Current.AddUpgrades(5);
            loadData();
        }
    }
}
