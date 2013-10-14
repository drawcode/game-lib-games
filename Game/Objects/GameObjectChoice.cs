using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class GameObjectChoiceData {
    public string code = "";
    public string responseAnswer = "";
    public string displayAnswer = "";
    public string assetCode = "";
    public bool isCorrect = true;
}

public class GameObjectChoice : GameObjectLevelBase {

    public AppContentChoice appContentChoice;

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

        LoadChoice("question-1", true, "truer", "true","barrel-1");

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

    public void LoadChoice(AppContentChoice choice) {
        appContentChoice = choice;

        LoadChoice(
            appContentChoice.code,
            true,
            appContentChoice.display_name,
            appContentChoice.display_name, "barrel-1");
    }

    public void LoadChoice(
        string code,
        bool isCorrect,
        string displayAnswer,
        string responseAnswer,
        string assetCode) {

        choiceData = new GameObjectChoiceData();
        choiceData.code = code;
        choiceData.isCorrect = isCorrect;
        choiceData.displayAnswer = displayAnswer;
        choiceData.responseAnswer = responseAnswer;
        choiceData.assetCode = assetCode;

        LoadAsset(assetCode);

        UIUtil.SetLabelValue(labelResponse, choiceData.displayAnswer);
    }

    public void BroadcastChoice() {
        Messenger<GameObjectChoiceData>.Broadcast("game-choice-data-response", choiceData);
    }

    public void OnCollisionEnter(Collision collision) {

        // If the human player hit us, check the score/choice and correct or incorrect message broadcast

        if(choiceData != null) {
            if(choiceData.isCorrect) {
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
