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

        if (string.IsNullOrEmpty(gameZoneType)) {
            gameZoneType = GameZoneKeys.action_build;
        }

        if (string.IsNullOrEmpty(actionCode)) {
            actionCode = GameZoneActions.action_build;
        }

        if (loadOnStart) {
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

        LoadIcons();
    }

    public void LoadIcons() {

        // TODO icon from data

        foreach(UISprite spriteIcon in 
                containerIcons.GetList<UISprite>("Icon")) {
        
            if(isActionCodeSave) {
                spriteIcon.spriteName = "icon-arrow-64";
            }
            else if(isActionCodeRepair) {
                spriteIcon.spriteName = "icon-wrench-064";
            }
            else if(isActionCodeBuild) {
                spriteIcon.spriteName = "icon-magic-64";
            }
            else if(isActionCodeAttack) {
                spriteIcon.spriteName = "icon-weapon-64";
            }
        }
    }

    public void LoadAsset() {

        if(isActionCodeSave) {
            return;
        }
        
        if (assetCode != BaseDataObjectKeys.none
            && lastAssetCode != assetCode) {

            lastAssetCode = assetCode;

            if (gameCharacter == null) {
                return;
            }

            assetAnimationNameIdle = 
                gameCharacter.data.GetAnimationsByTypeIdle().code;

            assetAnimationNamePlay = 
                gameCharacter.data.GetAnimationsByTypeStart().code;
            
            GameObject go = AppContentAssets.LoadAssetLevelAssets(
                gameCharacter.data.GetModel().code);
            
            if (go != null) {
                containerAssets.DestroyChildren();
                go.transform.parent = containerAssets.transform;
                go.TrackObject(containerAssets);

                AssetAnimationPlayNormalized(currentCreateProgress);
            }
        }
    }

    public void LoadAssetPlatform() {        
        
        if (assetPlatformCode != BaseDataObjectKeys.none
            && lastAssetPlatformCode != assetPlatformCode) {

            lastAssetPlatformCode = assetPlatformCode;
            
            GameObject go = AppContentAssets.LoadAssetLevelAssets(assetPlatformCode);
            
            if (go != null) {
                containerAssetsPlatforms.DestroyChildren();
                go.transform.parent = containerAssetsPlatforms.transform;
                go.TrackObject(containerAssetsPlatforms);
            }
        }
    }

    public void ChangeState(GameZoneActionAssetState stateTo) {
        if (stateTo != currentCreateState) {
            currentCreateState = stateTo;
        }
    }

    public void ChangeStateNone() {
        ChangeState(GameZoneActionAssetState.none);
    }

    public void ChangeStateCreating() {
        if (currentCreateState != GameZoneActionAssetState.creating
            && (currentCreateState != GameZoneActionAssetState.created
            && currentCreateState != GameZoneActionAssetState.destroyed)) {
            ChangeState(GameZoneActionAssetState.creating);
        }
    }

    public void ChangeStateDestroying() {
        if (currentCreateState != GameZoneActionAssetState.destroying
            && (currentCreateState != GameZoneActionAssetState.destroyed
            && currentCreateState != GameZoneActionAssetState.created)) {
            ChangeState(GameZoneActionAssetState.creating);
        }
    }

    public void HandleUpdateState(float power, float time = 1f) {
        
        
        if (currentCreateState != GameZoneActionAssetState.created) {
            if (currentCreateState == GameZoneActionAssetState.creating) {            
                currentCreateProgress += power * Time.deltaTime;
            }        
        }
        
        if (currentCreateState != GameZoneActionAssetState.destroying) {
            if (currentCreateState == GameZoneActionAssetState.destroying) {            
                currentCreateProgress -= power * Time.deltaTime;
            }
        }
    }

    public void HandleUpdateAction() {
        
        Mathf.Clamp(currentCreateProgress, 0f, 1f);
        
        if (!actionCompleted) {
            
            if (isActionCodeBuild || isActionCodeRepair || isActionCodeDefend) {
                if (currentCreateProgress >= 1) {
                    actionCompleted = true;
                    
                    AssetAnimationIdle();
                    
                    GameController.CurrentGamePlayerController.ProgressScores(1);
                    
                    currentCreateState = GameZoneActionAssetState.created;
                    
                    if (isActionCodeBuild) {                        
                        GameController.CurrentGamePlayerController.ProgressAssetBuild(1);
                    }
                    
                    if (isActionCodeRepair) {                        
                        GameController.CurrentGamePlayerController.ProgressAssetRepair(1);
                    }
                    
                    if (isActionCodeDefend) {                        
                        GameController.CurrentGamePlayerController.ProgressAssetDefend(1);
                    }
                }
            }
            else if (isActionCodeAttack) {
                if (currentCreateProgress <= 0) {
                    actionCompleted = true;

                    ChangeStateDestroying();

                    GameController.CurrentGamePlayerController.ProgressScores(1);
                    GameController.CurrentGamePlayerController.ProgressAssetAttack(1);
                    
                    //RemoveMe();
                    //AssetAnimationPlayNormalized(currentCreateProgress);
                }
            }
            
            if (currentCreateProgress != lastCreateProgress) {
                lastCreateProgress = currentCreateProgress;
                
                if (!actionCompleted) {
                    AssetAnimationPlayNormalized(currentCreateProgress);
                }
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

        HandleUpdateState(power, Time.deltaTime);

        HandleUpdateAction();
    }

}