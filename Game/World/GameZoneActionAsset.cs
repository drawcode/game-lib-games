using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameZoneActionAsset : GameZoneAction {

    float lastCreateProgress = 0;
    public float currentCreateProgress = 0;
    public GameZoneActionAssetState currentCreateState = GameZoneActionAssetState.none;
    public GameZoneActionAssetState actionGoalState = GameZoneActionAssetState.none;
    public bool actionCompleted = false;
    public bool actionStarted = false;
    public bool loadOnStart = true;

    public GameCharacter gameCharacter;

    public override void Start() {
        base.Start();

        if(string.IsNullOrEmpty(gameZoneType)) {
            gameZoneType = GameZoneKeys.action_build;
        }

        if(string.IsNullOrEmpty(actionCode)) {
            actionCode = GameZoneActions.action_build;
        }

        if(loadOnStart) {
            Load();
        }
    }
    
    public override void OnEnable() {
        base.OnEnable();
    }
    
    public override void OnDisable() {
        base.OnDisable();
    }

    public void AssetAnimationIdle() {
        containerAssets.PlayAnimation(assetAnimationNameIdle);
    }

    public void AssetAnimationPlayNormalized(float time) {
        containerAssets.StepAnimationFrame(assetAnimationNamePlay, time);
    }

    public void Load(string gameZoneTypeTo, string actionCodeTo, string assetCodeTo, string assetPlatformCodeTo) {
    
        gameZoneType = gameZoneTypeTo;
        actionCode = actionCodeTo;
        assetCode = assetCodeTo;
        assetPlatformCode = assetPlatformCodeTo;
        Load();
    }

    public void Load() {

        gameCharacter = GameCharacters.Instance.GetById(assetCode);

        LoadAsset();

        LoadAssetPlatform();
    }

    public void LoadAsset() {
        
        if(assetCode != lastAssetCode 
           && assetCode != BaseDataObjectKeys.none) {

            lastAssetCode = assetCode;

            if(gameCharacter == null) {
                return;
            }

            assetAnimationNameIdle = 
                gameCharacter.data.GetAnimationsByTypeIdle().code;

            assetAnimationNamePlay = 
                gameCharacter.data.GetAnimationsByTypeStart().code;
            
            GameObject go = AppContentAssets.LoadAssetLevelAssets(
                gameCharacter.data.GetModel().code);
            
            if(go != null) {
                containerAssets.DestroyChildren();
                go.transform.parent = containerAssets.transform;
                go.TrackObject(containerAssets);

                AssetAnimationPlayNormalized(currentCreateProgress);
            }
        }
    }

    public void LoadAssetPlatform() {        
        
        if(assetPlatformCode != lastAssetPlatformCode 
           && assetCode != BaseDataObjectKeys.none) {

            lastAssetPlatformCode = assetPlatformCode;
            
            GameObject go = AppContentAssets.LoadAssetLevelAssets(assetPlatformCode);
            
            if(go != null) {
                containerAssetsPlatforms.DestroyChildren();
                go.transform.parent = containerAssetsPlatforms.transform;
                go.TrackObject(containerAssetsPlatforms);
            }
        }
    }

    public void Update() {

        float power = .1f;

        if (Application.isEditor) {
            if (Input.GetKey(KeyCode.LeftControl)) {

                if (Input.GetKey(KeyCode.KeypadPlus)) {
                    currentCreateProgress += power * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.KeypadMinus)) {

                    currentCreateProgress -= power * Time.deltaTime;
                }
            }
        }        

        Mathf.Clamp(currentCreateProgress, 0f, 1f);

        if (!actionCompleted) {

            if (isActionCodeBuild || isActionCodeRepair) {
                if (currentCreateProgress >= 1) {
                    actionCompleted = true;

                    AssetAnimationIdle();

                    GameController.CurrentGamePlayerController.ProgressScores(1);
                }
            }
            else if (isActionCodeAttack) {
                if (currentCreateProgress <= 0) {
                    actionCompleted = true;
                    
                    GameController.CurrentGamePlayerController.ProgressScores(1);
                }
            }

            if (currentCreateProgress != lastCreateProgress) {
                lastCreateProgress = currentCreateProgress;
                        
                if(!actionCompleted) {
                    AssetAnimationPlayNormalized(currentCreateProgress);
                }
            }   
        }

    }

}