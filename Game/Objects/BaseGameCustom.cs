using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Events;
using Engine.Game.App.BaseApp;
using Engine.Game.App;

public class GameCustomActorTypes {
    public static string displayType = "display"; // used for customizer, defualt to profile, then allow changes.
    public static string heroType = "hero"; // used for customizer, defualt to profile, then allow changes.
    public static string enemyType = "enemy"; // use profile
    public static string sidekickType = "sidekick"; // call out a preset set
}

public class GameCustomTypes {
    public static string customType = "custom"; // used for customizer, defualt to profile, then allow changes.
    public static string defaultType = "default"; // use profile
    public static string explicitType = "explicit"; // call out a preset set
    public static string teamType = "team"; // call out a preset set
}

[Serializable]
public class GameCustomCharacterData {

    public string type = GameCustomTypes.defaultType;
    public string actorType = GameCustomActorTypes.heroType;
    public string teamCode = "default";
    public string presetType = "character";
    public string presetColorCodeDefault = ProfileConfigs.defaultGameCharacterColorPreset;
    public string presetColorCode = ProfileConfigs.defaultGameCharacterColorPreset;
    public string presetTextureCodeDefault = ProfileConfigs.defaultGameCharacterTexturePreset;
    public string presetTextureCode = ProfileConfigs.defaultGameCharacterTexturePreset;
    public string characterCode = ProfileConfigs.defaultGameCharacterCode;
    public string characterDisplayName = ProfileConfigs.defaultGameCharacterDisplayName;
    public string characterDisplayCode = ProfileConfigs.defaultGameCharacterDisplayCode;

    public bool isCustomType {
        get {
            return type == GameCustomTypes.customType;
        }
    }

    public bool isDefaultType {
        get {
            return type == GameCustomTypes.defaultType;
        }
    }

    public bool isExplicitType {
        get {
            return type == GameCustomTypes.explicitType;
        }
    }

    public bool isTeamType {
        get {
            return type == GameCustomTypes.teamType;
        }
    }

    public bool isActorTypeHero {
        get {
            return actorType == GameCustomActorTypes.heroType;
        }
    }

    public bool isActorTypeEnemy {
        get {
            return actorType == GameCustomActorTypes.enemyType;
        }
    }

    public bool isActorTypeSidekick {
        get {
            return actorType == GameCustomActorTypes.sidekickType;
        }
    }

    public void SetActorEnemy() {
        SetActorType(GameCustomActorTypes.enemyType);
    }

    public void SetActorHero() {
        SetActorType(GameCustomActorTypes.heroType);
    }

    public void SetActorSidekick() {
        SetActorType(GameCustomActorTypes.sidekickType);
    }

    public void SetActorType(string actorTypeTo) {
        actorType = actorTypeTo;
    }
}

public class GameCustomCharacterDataCurrent {

    public string lastCustomColorCode = "--";
    public string lastCustomTextureCode = "--";
    public string lastCustomDisplayName = "";
    public string lastCustomDisplayCode = "";
}

/*
[Serializable]
public class GameCharacterDataSet { 
    //public GameCustomCharacterObjectData characterObjectData;

    [HideInInspector]
    public GameCustomCharacterDataCurrent characterDataCurrent;

    public GameCustomCharacterData characterData;

    public GameCharacterDataSet() {
        Reset();
    }

    public void Reset() {

        //characterObjectData = 
        //    new GameCustomCharacterObjectData();

        characterData = 
            new GameCustomCharacterData();    

        characterDataCurrent = new GameCustomCharacterDataCurrent();
    }
}
*/

[Serializable]
public class GameCustomCharacterObjectData {

    public GameCustomPlayer gameCustomPlayer;

    public GameCustomCharacterObjectData() {

    }

    public void Fill(GameObject go) {
        gameCustomPlayer = go.Get<GameCustomPlayer>();
    }

    public bool hasPlayer {
        get {
            return gameCustomPlayer != null
                && gameCustomPlayer.isActorTypeHero;
        }
    }
}

public class BaseGameCustom : GameObjectBehavior {

    public GameCustomCharacterDataCurrent customCharacterDataCurrent;
    public GameCustomCharacterData customCharacterData;
    [HideInInspector]

    public GameCustomPlayerContainer
        gameCustomPlayerContainer;
    public bool freezeRotation = false;
    public bool resetPositionRotationModel = true;
    float lastCustomUpdate = 0;

    public virtual void Start() {

        Init();
    }

    public virtual void Init() {

        if (customCharacterDataCurrent == null) {
            customCharacterDataCurrent = new GameCustomCharacterDataCurrent();
        }

        if (customCharacterData == null) {
            customCharacterData = new GameCustomCharacterData();

            if (customCharacterData.presetColorCode == GameCustomTypes.customType) {
                customCharacterData.type = GameCustomTypes.customType;
            }
            else if (customCharacterData.presetColorCode == GameCustomTypes.defaultType) {
                customCharacterData.type = GameCustomTypes.defaultType;
            }
            else {
                customCharacterData.type = GameCustomTypes.explicitType;
            }

            Load(customCharacterData);
        }
    }

    public virtual void OnEnable() {
        Messenger.AddListener(
            GameCustomMessages.customColorsChanged,
            BaseOnCustomizationColorsChangedHandler);
    }

    public virtual void OnDisable() {
        Messenger.RemoveListener(
            GameCustomMessages.customColorsChanged,
            BaseOnCustomizationColorsChangedHandler);
    }

    public void SetActorEnemy() {
        SetActorType(GameCustomActorTypes.enemyType);
    }

    public void SetActorHero() {
        SetActorType(GameCustomActorTypes.heroType);
    }

    public void SetActorSidekick() {
        SetActorType(GameCustomActorTypes.sidekickType);
    }

    public bool isTypeDefault {
        get {

            CheckData();

            return customCharacterData.isDefaultType;
        }
    }

    public bool isTypeTeam {
        get {

            CheckData();

            return customCharacterData.isTeamType;
        }
    }

    public bool isTypeCustom {
        get {

            CheckData();

            return customCharacterData.isCustomType;
        }
    }

    public bool isTypeExplicit {
        get {

            CheckData();

            return customCharacterData.isExplicitType;
        }
    }

    public bool isActorTypeHero {
        get {

            CheckData();

            return customCharacterData.isActorTypeHero;
        }
    }

    public bool isActorTypeSidekick {
        get {

            CheckData();

            return customCharacterData.isActorTypeSidekick;
        }
    }

    public bool isActorTypeEnemy {
        get {

            CheckData();

            return customCharacterData.isActorTypeEnemy;
        }
    }

    public void SetActorType(string actorTypeTo) {

        CheckData();

        customCharacterData.SetActorType(actorTypeTo);
    }

    /*
    public virtual void Load(string typeTo) {
        Load(typeTo, customActorType, typeTo, typeTo);
    }

    public virtual void Load(string typeTo, string actorType, string presetColorCodeTo, string presetTextureCodeTo) {

        GameCustomCharacterData customCharacterDataTo = new GameCustomCharacterData();
        customCharacterDataTo.type = typeTo;
        characterData.actorType = actorType;
        customCharacterDataTo.presetColorCode = presetColorCodeTo;
        customCharacterDataTo.presetTextureCode = presetTextureCodeTo;

        Load(customCharacterDataTo);
    }
    */

    public virtual void Load(GameCustomCharacterData customCharacterDataTo) {
        Change(customCharacterDataTo);
    }

    public virtual void Change(GameCustomCharacterData customCharacterDataTo) {

        customCharacterData = customCharacterDataTo;
        //customCharacterDataCurrent = customCharacterDataTo; 

        CheckData();

        if (gameCustomPlayerContainer != null) {
            gameCustomPlayerContainer.customCharacterData = customCharacterData;
        }

        //LogUtil.Log("GameCustomBase:Change:characterData:" + characterData.teamCode);

        if (customCharacterData != null) {
            //customCharacterData.presetColorCode = customCharacterData.presetColorCode;
            //customCharacterData.presetTextureCode = customCharacterData.presetTextureCode;

            //LogUtil.Log("GameCustomBase:Change:customColorCode:" + customColorCode);
            //LogUtil.Log("GameCustomBase:Change:customTextureCode:" + customTextureCode);

            if (!string.IsNullOrEmpty(customCharacterData.teamCode)
                && customCharacterData.teamCode != "default") {

                //LogUtil.Log("Loading TEAM Custom Type:characterData.teamCode:" + characterData.teamCode);

                GameTeam team = GameTeams.Instance.GetById(customCharacterData.teamCode);

                if (team != null) {

                    if (team.data != null) {

                        customCharacterData.teamCode = team.code;
                        customCharacterData.type = GameCustomTypes.teamType;

                        //LogUtil.Log("Loading TEAM EXISTS Type:teamCode:" + teamCode);                        

                        GameDataTexturePreset itemTexture = team.data.GetTexturePreset();

                        if (itemTexture != null) {
                            customCharacterData.presetTextureCode = itemTexture.code;
                            customCharacterDataCurrent.lastCustomColorCode = "--";
                        }

                        GameDataColorPreset itemColor = team.data.GetColorPreset();

                        if (itemColor != null) {
                            customCharacterData.presetColorCode = itemColor.code;
                            customCharacterDataCurrent.lastCustomTextureCode = "--";
                        }
                    }

                    GameCustomController.UpdateCharacterDisplay(
                        gameObject,
                        team.display_name,
                        UnityEngine.Random.Range(1, 99).ToString());
                }
            }
        }

        UpdatePlayer();
    }

    public virtual void UpdatePlayer() {

        CheckData();

        /*
        LogUtil.Log("UpdatePlayer"  
                  + " type:" + characterData.type
                  + " presetType:" + characterData.presetType
                  + " presetColorCode:" + characterData.presetColorCode
                  + " presetTextureCode:" + characterData.presetTextureCode
                  + " isCustomType:" + characterData.isCustomType
                  + " isDefaultType:" + characterData.isDefaultType
                  + " isExplicitType:" + characterData.isExplicitType);
                  */

        if (customCharacterData.isCustomType
            || customCharacterData.isTeamType) {
            return;
        }
        else if (customCharacterData.isDefaultType) {
            SetCustom();
        }
    }

    void BaseOnCustomizationColorsChangedHandler() {
        UpdatePlayer();

        if (customCharacterData.isDefaultType) {
            //Debug.Log("BaseOnCustomizationColorsChangedHandler");
            //Debug.Log("UpdatePlayer"  
            //    + " type:" + customCharacterData.type
            //    + " presetType:" + customCharacterData.presetType
            //    + " presetColorCode:" + customCharacterData.presetColorCode
            //    + " presetTextureCode:" + customCharacterData.presetTextureCode
            //    + " isCustomType:" + customCharacterData.isCustomType
            //    + " isDefaultType:" + customCharacterData.isDefaultType
            //    + " isExplicitType:" + customCharacterData.isExplicitType);
        }
    }

    public void SetCustom() {

        //Debug.Log("SetCustom");

        CheckData();

        SetCustomTextures();

        SetCustomColors();

        SetCustomDisplayValues();
    }

    public void SetCustomColors() {

        CheckData();

        if (customCharacterDataCurrent.lastCustomColorCode
            == customCharacterData.presetColorCode) {
            //return;
        }

        //LogUtil.Log("SetCustomColors"  
        //          + " type:" + characterData.type
        //          + " presetType:" + characterData.presetType
        //          + " presetColorCode:" + characterData.presetColorCode
        //          + " presetTextureCode:" + characterData.presetTextureCode);


        if (customCharacterData.isCustomType
            || customCharacterData.isTeamType
            || customCharacterData.isExplicitType) {
            return;
        }
        else if (customCharacterData.isDefaultType) {

            if (customCharacterData.actorType == GameCustomActorTypes.heroType) {

                GameProfileCustomItem customItem = GameProfileCharacters.currentCustom;

                //LogUtil.Log("SetCustomColors"  
                //         + " customItem:" + customItem.ToJson());

                if (customItem != null) {

                    if (!customItem.HasData()) {

                        GameCustomController.UpdateColorPresetObject(
                            gameObject,
                            AppColorPresets.Instance.GetByCode(
                                customCharacterData.presetColorCodeDefault));
                    }
                    else {

                        //customItem = GameCustomController.FillDefaultCustomColors(customItem, type);

                        GameCustomController.UpdateColorPresetObject(
                            customItem,
                            gameObject,
                            customCharacterData.presetType);
                    }
                }
                else {

                    GameCustomController.UpdateColorPresetObject(
                        gameObject,
                        AppColorPresets.Instance.GetByCode(
                            customCharacterData.presetColorCodeDefault));
                }//GameCustomController.BroadcastCustomColorsChanged
            }
            else {

                GameCustomController.UpdateColorPresetObject(
                    gameObject,
                    AppColorPresets.Instance.GetByCode(
                        customCharacterData.presetColorCodeDefault));

            }//GameCustomController.BroadcastCustomColorsChanged
        }

        customCharacterDataCurrent.lastCustomColorCode =
            customCharacterData.presetColorCode;
    }

    public void SetCustomTextures() {


        CheckData();

        if (customCharacterDataCurrent.lastCustomTextureCode
            == customCharacterData.presetTextureCode) {

            //return;
        }

        if (customCharacterData.isCustomType
            || customCharacterData.isTeamType
            || customCharacterData.isExplicitType) {
            return;
        }
        else if (customCharacterData.isDefaultType) {

            if (customCharacterData.actorType == GameCustomActorTypes.heroType) {

                GameProfileCustomItem customItem = GameProfileCharacters.currentCustom;

                if (customItem != null) {

                    GameCustomController.UpdateTexturePresetObject(
                        customItem,
                        gameObject,
                        customCharacterData.presetType);
                }
                else {

                    GameCustomController.UpdateTexturePresetObject(
                        gameObject,
                        AppContentAssetTexturePresets.Instance.GetByCode(
                        customCharacterData.presetTextureCodeDefault));
                }//GameCustomController.BroadcastCustomColorsChanged
            }
            else {
                GameCustomController.UpdateTexturePresetObject(
                    gameObject,
                    AppContentAssetTexturePresets.Instance.GetByCode(
                    customCharacterData.presetTextureCodeDefault));
            }
        }

        customCharacterDataCurrent.lastCustomTextureCode =
            customCharacterData.presetTextureCode;

    }

    public void SetCustomDisplayValues() {

        CheckData();

        //if (customCharacterData.isCustomType 
        //    || customCharacterData.isTeamType 
        //    || customCharacterData.isExplicitType) {
        //    return;
        //}
        //else if (customCharacterData.isDefaultType) {

        //if (customCharacterData.actorType == GameCustomActorTypes.heroType) {
        HandleCustomPlayerDisplayValues();
        //}
        //}
    }

    public void HandleCustomPlayer() {

        CheckData();

        HandleCustomPlayerTexture();
        HandleCustomPlayerColor();
        HandleCustomPlayerDisplayValues();
    }

    public void HandleCustomPlayerTexture() {

        CheckData();

        if (customCharacterData.isCustomType
            || customCharacterData.isDefaultType) {
            //return;
        }
        else if (customCharacterDataCurrent.lastCustomTextureCode
            != customCharacterData.presetTextureCode) {

            //if(AppColorPresets.Instance.CheckByCode(customTextureCode)) {

            //LogUtil.Log("HandleCustomPlayerColor:changing:" + 
            //          " lastCustomColorCode:" + lastCustomTextureCode + 
            //          " characterData.presetColorCode:" + characterData.presetTextureCode);

            AppContentAssetTexturePreset preset =
                AppContentAssetTexturePresets.Instance.GetByCode(
                    customCharacterData.presetTextureCode);

            if (preset != null) {
                // load from current code
                GameCustomController.UpdateTexturePresetObject(
                    gameObject, preset);
            }

            customCharacterDataCurrent.lastCustomTextureCode =
                customCharacterData.presetTextureCode;
            //}
        }
    }

    public void CheckData() {
        if (customCharacterData == null || customCharacterDataCurrent == null) {
            Init();
        }
    }

    public void HandleCustomPlayerColor() {

        CheckData();

        if (customCharacterData.isCustomType
            || customCharacterData.isDefaultType) {
            //return;
        }
        else if (customCharacterDataCurrent.lastCustomColorCode
            != customCharacterData.presetColorCode) {

            if (AppColorPresets.Instance.CheckByCode(customCharacterData.presetColorCode)) {

                //LogUtil.Log("HandleCustomPlayerColor:changing:" + 
                //          " lastCustomColorCode:" + lastCustomColorCode + 
                //         " characterData.presetColorCode:" + characterData.presetColorCode);

                // load from current code
                AppColorPreset preset = AppColorPresets.Instance.GetByCode(
                    customCharacterData.presetColorCode);

                GameCustomController.UpdateColorPresetObject(
                    gameObject, preset);

                customCharacterDataCurrent.lastCustomColorCode =
                    customCharacterData.presetColorCode;

            }
        }
    }

    public void HandleCustomPlayerDisplayValues() {

        CheckData();

        if (customCharacterData.isCustomType
            || customCharacterData.isDefaultType) {

            if (customCharacterDataCurrent.lastCustomDisplayCode
                != customCharacterData.characterDisplayCode
                || customCharacterDataCurrent.lastCustomDisplayName
                != customCharacterData.characterDisplayName) {

                GameCustomController.UpdateCharacterDisplay(
                    gameObject,
                    customCharacterData.characterDisplayName,
                    customCharacterData.characterDisplayCode);

                customCharacterDataCurrent.lastCustomDisplayCode = customCharacterData.characterDisplayCode;
                customCharacterDataCurrent.lastCustomDisplayName = customCharacterData.characterDisplayName;
            }
        }
    }

    public virtual void Update() {

        if (resetPositionRotationModel) {

            gameObject.transform.localPosition =
                Vector3.Lerp(gameObject.transform.localPosition,
                             Vector3.zero, Time.deltaTime);

            gameObject.transform.localRotation =
                Quaternion.Lerp(gameObject.transform.localRotation,
                                Quaternion.identity, Time.deltaTime);
        }

        if (lastCustomUpdate + 1 < Time.time) {
            lastCustomUpdate = Time.time;

            HandleCustomPlayer();

            if (freezeRotation) {
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localRotation = Quaternion.identity;
            }
        }

    }
}