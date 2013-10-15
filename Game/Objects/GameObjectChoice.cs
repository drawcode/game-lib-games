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
}

public class GameObjectChoiceData {
    public string choiceCode = "";
    public string choiceType = "";
    public string choiceItemCode = "";
    public string choiceItemDisplay = "";
    public string choiceItemAssetCode = "";
    public bool choiceItemIsCorrect = true;
}

public class GameObjectChoice : GameObjectLevelBase {

    public AppContentChoice appContentChoice;
    public AppContentChoiceItem appContentChoiceItem;

    public GameObjectChoiceData choiceData;

    public GameObject containerLabel;
    public UILabel labelResponse;

    public GameObject containerEffects;
    public GameObject containerEffectsAlwaysOn;
    public GameObject containerEffectsCorrect;
    public GameObject containerEffectsIncorrect;

    public GameObject containerAsset;

    public Color startColor = Color.red;

    public bool isUI = false;

    public override void Start() {
        base.Start();

        LoadData();
    }

    public override void LoadData() {
        base.LoadData();

        LoadChoice("question-1", "correct", true, code, "true","barrel-1");

        if(containerEffectsCorrect != null) {
            containerEffectsCorrect.StopParticleSystem(true);
        }

        SetChoiceParticleSystemColors();

        if(isUI) {
            gameObject.SetLayerRecursively("UIOverlay");
        }
    }

    public void SetChoiceParticleSystemColors() {
        if(containerEffectsAlwaysOn != null) {
            containerEffectsAlwaysOn.SetParticleSystemStartColor(startColor, true);
        }
    }

    public void LoadAsset(string assetCode) {

        if(containerAsset != null) {

            containerAsset.DestroyChildren();

            GameObject assetItem = GameObjectHelper.LoadFromResources(
                Path.Combine(
                Contents.appCacheVersionSharedPrefabLevelAssets,
                assetCode));

            if(assetItem != null) {
                if(isUI) {
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

                foreach(Rigidbody rigidbody in assetItem.GetComponentsInChildren<Rigidbody>(true)) {

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

        LoadAsset(choiceItemAssetCode);

        UIUtil.SetLabelValue(labelResponse, choiceData.choiceItemDisplay);
    }

    public void BroadcastChoice() {
        Messenger<GameObjectChoiceData>.Broadcast(
            GameObjectChoiceMessages.gameChoiceDataResponse, choiceData);
    }

    public void OnCollisionEnter(Collision collision) {

        // If the human player hit us, check the score/choice and correct or incorrect message broadcast


        if(choiceData != null) {
            if(choiceData.choiceItemIsCorrect) {
                // Play correct
                if(containerEffectsCorrect != null) {
                    containerEffectsCorrect.PlayParticleSystem(true);
                    BroadcastChoice();
                }

            }
            else {
                // Play incorrect
                BroadcastChoice();
                if(rigidbody != null) {
                    //rigidbody.AddExplosionForce(10f, transform.position, 10f);
                }
            }
        }

    }


}
