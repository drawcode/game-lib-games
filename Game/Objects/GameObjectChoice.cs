using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class GameObjectChoiceMessages {
    public static string gameChoiceDataResponse = "game-choice-data-response";
    public static string gameChoiceObjectDataLoad = "game-choice-object-data-load";
}

public class GameObjectChoiceData {
    public string choiceCode = "";
    public string choiceType = "";
    public string choiceItemCode = "";
    public string choiceItemDisplay = "";
    public string choiceItemAssetCode = "";
    public bool choiceItemIsCorrect = true;
}

public class GameObjectChoice : BaseGameObjectLevel {

    public AppContentChoice appContentChoice;
    public AppContentChoiceItem appContentChoiceItem;
    public GameObjectChoiceData choiceData;
    public GameObject containerLabel;
    public UILabel labelResponse;
    public UILabel labelResponseShadow;
    public GameObject containerEffects;
    public GameObject containerEffectsAlwaysOn;
    public GameObject containerEffectsCorrect;
    public GameObject containerEffectsIncorrect;
    public GameObject containerAsset;
    public Color startColor = Color.red;
    public bool isUI = false;
    public bool hasBroadcasted = false;
    public bool hasLoadedChoice = false;
    public string uuid = "";

    public void OnEnable() {
        Messenger.AddListener(
            GameObjectChoiceMessages.gameChoiceObjectDataLoad,
            OnGameChoiceObjectDataLoadHandler);
    }

    public void OnDisable() {
        Messenger.RemoveListener(
            GameObjectChoiceMessages.gameChoiceObjectDataLoad,
            OnGameChoiceObjectDataLoadHandler);
    }

    public override void Start() {
        base.Start();

        uuid = System.Guid.NewGuid().ToString();

        LoadData();
    }

    public void OnGameChoiceObjectDataLoadHandler() {
        if (!hasLoadedChoice) {
            // recieve the message and load if it is this object
        }
    }

    public override void LoadData() {
        base.LoadData();

        //LoadChoice("question-1", "correct", true, code, "false","barrel-1");

        StopCorrect();
        StopIncorrect();

        SetChoiceParticleSystemColors();

        if (isUI) {
            gameObject.SetLayerRecursively("UIDialog");
        }
    }

    public void PlayCorrect() {
        if (containerEffectsCorrect != null) {
            containerEffectsCorrect.PlayParticleSystem(true);
        }
        BroadcastChoiceDelayed(2f);
    }

    public void StopCorrect() {
        if (containerEffectsCorrect != null) {
            containerEffectsCorrect.StopParticleSystem(true);
        }
    }

    public void PlayIncorrect() {
        if (containerEffectsIncorrect != null) {
            containerEffectsIncorrect.PlayParticleSystem(true);
        }
        BroadcastChoiceDelayed(2f);
    }

    public void StopIncorrect() {
        if (containerEffectsIncorrect != null) {
            containerEffectsIncorrect.StopParticleSystem(true);
        }
    }

    public void SetChoiceParticleSystemColors() {
        SetChoiceParticleSystemColorsAlwaysOn(startColor);
        SetChoiceParticleSystemColorsCorrect(startColor, false);
        SetChoiceParticleSystemColorsIncorrect(startColor, false);
    }

    public void SetChoiceParticleSystemColors(Color colorTo) {
        SetChoiceParticleSystemColorsAlwaysOn(colorTo);
        SetChoiceParticleSystemColorsCorrect(colorTo, false);
        SetChoiceParticleSystemColorsIncorrect(startColor, false);
    }

    public void SetChoiceParticleSystemColorsAlwaysOn(Color colorTo) {
        SetChoiceParticleSystemColors(containerEffectsAlwaysOn, colorTo, true);
    }

    public void SetChoiceParticleSystemColorsCorrect(Color colorTo, bool playNow) {
        SetChoiceParticleSystemColors(containerEffectsCorrect, colorTo, playNow);
    }

    public void SetChoiceParticleSystemColorsIncorrect(Color colorTo, bool playNow) {
        SetChoiceParticleSystemColors(containerEffectsIncorrect, colorTo, playNow);
    }

    public void SetChoiceParticleSystemColors(GameObject go, Color colorTo, bool playNow) {
        startColor = colorTo;
        if (go != null) {
            go.SetParticleSystemStartColor(colorTo, true);

            if (playNow) {
                go.StopParticleSystem(true);
                go.PlayParticleSystem(true);
            }

            //LogUtil.Log("SetChoiceParticleSystemColors:go:" + go.name + " colorTo:" + colorTo);
        }
    }

    public void LoadAsset(string assetCode) {

        if (containerAsset != null && containerAsset.transform.childCount == 0) {

            containerAsset.DestroyChildren();

            GameObject assetItem = GameObjectHelper.LoadFromResources(
                Path.Combine(
                ContentPaths.appCacheVersionSharedPrefabLevelAssets,
                assetCode));

            if (assetItem != null) {
                if (isUI) {
                    assetItem.SetLayerRecursively("UIOverlay");
                }
                else {
                    assetItem.SetLayerRecursively("Default");
                }
                assetItem.transform.parent = containerAsset.transform;
                //assetItem.transform.position = containerAsset.transform.position;
                //assetItem.transform.rotation = containerAsset.transform.rotation;
                assetItem.transform.localPosition = Vector3.zero;
                assetItem.transform.localRotation = Quaternion.identity;
                assetItem.transform.localRotation = Quaternion.identity;
                assetItem.transform.localScale = Vector3.one;

                foreach (Rigidbody rigidbody in assetItem.GetComponentsInChildren<Rigidbody>(true)) {

                    GameObject go = rigidbody.gameObject;

                    go.AddComponent<GameObjectChoiceAsset>();
                    go.ResetPosition(true);
                    go.ResetRotation(true);
                }

                //assetItem.ResetPosition();
                //assetItem.ResetRotation();
            }
        }
    }

    public void LoadChoiceItem(AppContentChoice choice, AppContentChoiceItem choiceItem, Color colorTo) {

        LogUtil.Log("LoadChoiceItem:" + choice.code);

        SetChoiceParticleSystemColors(colorTo);
        LoadChoiceItem(choice, choiceItem);
    }

    public void LoadChoiceItem(AppContentChoice choice, AppContentChoiceItem choiceItem) {
        appContentChoice = choice;
        appContentChoiceItem = choiceItem;

        LoadChoice(
            appContentChoice.code,
            appContentChoice.type,
            appContentChoiceItem.IsTypeCorrect(),
            appContentChoiceItem.display,
            appContentChoiceItem.code, "barrel-1");
    }

    public void LoadChoice(
        string choiceCode,
        string choiceType,
        bool choiceItemIsCorrect,
        string choiceItemDisplay,
        string choiceItemCode,
        string choiceItemAssetCode) {

        choiceData = new GameObjectChoiceData();
        choiceData.choiceCode = choiceCode;
        choiceData.choiceType = choiceType;
        choiceData.choiceItemIsCorrect = choiceItemIsCorrect;
        choiceData.choiceItemDisplay = choiceItemDisplay;
        choiceData.choiceItemCode = choiceItemCode;
        choiceData.choiceItemAssetCode = choiceItemAssetCode;

        appContentChoice = AppContentChoices.Instance.GetByCode(choiceCode);
        if (appContentChoice != null) {
            foreach (AppContentChoiceItem choiceItem in appContentChoice.choices) {
                if (choiceItem.code == choiceItemCode) {
                    appContentChoiceItem = choiceItem;
                }
            }
        }

        //LogUtil.Log("LoadChoice:choiceCode:" + choiceCode);
        //LogUtil.Log("LoadChoice:choiceType:" + choiceType);
        //LogUtil.Log("LoadChoice:choiceItemIsCorrect:" + choiceItemIsCorrect);
        //LogUtil.Log("LoadChoice:choiceItemDisplay:" + choiceItemDisplay);
        LogUtil.Log("LoadChoice:choiceItemCode:" + choiceItemCode);
        //LogUtil.Log("LoadChoice:choiceItemAssetCode:" + choiceItemAssetCode);

        LoadAsset(choiceItemAssetCode);

        LogUtil.Log("LoadChoice:SetLabel:choiceData.choiceItemDisplay:" + choiceData.choiceItemDisplay);

        UIUtil.SetLabelValue(labelResponse, choiceData.choiceItemDisplay);
        UIUtil.SetLabelValue(labelResponseShadow, choiceData.choiceItemDisplay);
        //LogUtil.Log("LoadChoice:SetLabel:labelResponse:" + labelResponse.text);

        hasLoadedChoice = true;
    }

    public void BroadcastChoice() {

        if (!hasBroadcasted) {

            hasBroadcasted = true;

            LogUtil.Log("GameObjectChoice:BroadcastChoice:" + appContentChoiceItem.code);

            Messenger<GameObjectChoiceData>.Broadcast(
                GameObjectChoiceMessages.gameChoiceDataResponse, choiceData);

            //Messenger<AppContentChoiceItem>.Broadcast(
            //    AppContentChoiceMessages.appContentChoiceItem, appContentChoiceItem);

            // Messenger<AppContentChoiceItem>.RemoveListener(AppContentChoiceMessages.appContentChoiceItem, OnAppContentChoiceItemHandler);
        }
    }

    public void BroadcastChoiceDelayed(float delay) {
        if (!hasBroadcasted) {
            StartCoroutine(BroadcastChoiceDelayedCo(delay));
        }
    }

    public IEnumerator BroadcastChoiceDelayedCo(float delay) {
        yield return new WaitForSeconds(delay);
        BroadcastChoice();
    }

    public void HandleChoiceData() {

        LogUtil.Log("GameObjectChoice:HandleChoiceData:" + name);

        if (choiceData != null) {

            if (choiceData.choiceItemIsCorrect) {
                PlayCorrect();
            }
            else {
                PlayIncorrect();

                if (gamePlayerController != null) {
                    gamePlayerController.AddImpact(-gamePlayerController.gameObject.transform.forward, 10f);
                }
            }
        }
    }

    GamePlayerController gamePlayerController = null;

    public void HandleCollision(Collision collision) {

        // If the human player hit us, check the score/choice and correct or incorrect message broadcast

        //LogUtil.Log("GameObjectChoice:OnCollisionEnter:" + collision.transform.name);

        GameObject go = collision.collider.transform.gameObject;

        //LogUtil.Log("GameObjectChoice:go:" + go.name);

        if (go.name.Contains("GamePlayerObject")) {

            gamePlayerController = GameController.GetGamePlayerController(go);

            if (gamePlayerController != null) {

                if (gamePlayerController.IsPlayerControlled) {

                    HandleChoiceData();
                }
            }
        }

        if (gamePlayerController == null
            && (go.name.Contains("GamePlayerCollider"))) {

            LogUtil.Log("GameObjectChoice:GamePlayerCollider:" + go.name);

            gamePlayerController = GameController.GetGamePlayerControllerParent(go);

            if (gamePlayerController != null) {

                LogUtil.Log("GameObjectChoice:gamePlayerController:" + gamePlayerController.name);

                if (gamePlayerController.IsPlayerControlled) {

                    HandleChoiceData();
                }
            }
        }
    }

    public void OnCollisionEnter(Collision collision) {

        if (!GameConfigs.isGameRunning) {
            return;
        }

        HandleCollision(collision);
    }
}
