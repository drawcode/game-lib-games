using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;

public class UICustomizeCharacterItemMessages {
    public static string rpgItemCodeChanged = "rpg-item-code-changed";
    public static string rpgUpgradesChanged = "rpg-upgrades-changed";
}

//Messenger<string, double>.AddListener(UIRPGItemMessages.rpgItemCodeUp, OnRPGItemCodeUp);
//Messenger<string, double>.RemoveListener(UIRPGItemMessages.rpgItemCodeUp, OnRPGItemCodeDown);
//Messenger<string, double>.Broadcast(UIRPGItemMessages.rpgItemCodeUp, rpgCode, 1);

public class UICustomizeCharacter : UIAppPanelBaseList {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UILabel labelUpgradesAvailable;
    public UIImageButton buttonResetRPG;
    public UIImageButton buttonSaveRPG;
    public UIImageButton buttonBuyUpgrades;
#else
    public Text labelUpgradesAvailable;
    public Button buttonResetRPG;
    public Button buttonSaveRPG;
    public Button buttonBuyUpgrades;
#endif

    GameProfileRPGItem profileGameDataItemRPG;
    string currentCharacterCode = "default";
    public static UICustomizeCharacter Instance;
    public GameObject listItemPrefab;
    public double upgradesAvailable = 0;
    public double modifierDisplay = 10;

    public override void Awake() {
        base.Awake();

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

    public override void OnEnable() {
        base.OnEnable();
        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, string, double>.AddListener(UICustomizeCharacterRPGItemMessages.rpgUpgradesChanged, OnRPGUpgradesHandler);

    }

    public override void OnDisable() {
        base.OnDisable();
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

    public override void OnButtonClickEventHandler(string buttonName) {

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

            if(item.rpgCode.ToLower() == GameDataItemRPGAttributes.speed) {
                profileGameDataItemRPG.SetSpeed(item.currentValue);
            }
            else if(item.rpgCode.ToLower() == GameDataItemRPGAttributes.energy) {
                profileGameDataItemRPG.SetEnergy(item.currentValue);
            }
            else if(item.rpgCode.ToLower() == GameDataItemRPGAttributes.health) {
                profileGameDataItemRPG.SetHealth(item.currentValue);
            }
            else if(item.rpgCode.ToLower() == GameDataItemRPGAttributes.attack) {
                profileGameDataItemRPG.SetAttack(item.currentValue);
            }
        }

        LogUtil.Log("profileGameDataItemRPG:attack:" + profileGameDataItemRPG.GetAttack());

        GameProfileCharacters.Current.SetCharacterRPG(currentCharacterCode, profileGameDataItemRPG);
        GameProfileRPGs.Current.SetUpgrades(upgradesAvailable);

        GameState.SaveProfile();

        loadData();
    }

    public void ResetRPG() {
        loadData();
    }

    public void BuyUpgrades() {
#if ENABLE_FEATURE_PRODUCTS
        GameUIController.ShowProducts(GameProductType.rpgUpgrade);
#endif
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

        LogUtil.Log("LoadDataCo");

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

        LogUtil.Log("Load RPGs:");

        int i = 0;

        SetUpgradesAvailable(GameProfileRPGs.Current.GetUpgrades());

        profileGameDataItemRPG = GameProfileCharacters.Current.GetCharacterRPG(currentCharacterCode);

        LogUtil.Log("profileGameDataItemRPG:attack2:" + profileGameDataItemRPG.GetAttack());

        List<string> rpgItems = new List<string>();

        rpgItems.Add(GameDataItemRPGAttributes.speed);
        rpgItems.Add(GameDataItemRPGAttributes.health);
        rpgItems.Add(GameDataItemRPGAttributes.energy);
        rpgItems.Add(GameDataItemRPGAttributes.attack);
        //rpgItems.Add(GameDataItemRPGAttributes.defense);           

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
            LogUtil.Log("Adding upgrades:");
            GameProfileRPGs.Current.AddUpgrades(5);
            loadData();
        }
    }
}