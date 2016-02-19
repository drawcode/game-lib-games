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
    public bool loaded = false;
    public GameCharacter gameCharacter;
    public GamePlayerIndicator gamePlayerIndicator;

    public override void Start() {
        base.Start();

        if (loadOnStart) {
            Load();
        }
    }
    
    public override void OnEnable() {
        base.OnEnable();
                
        // Messenger<string>.Broadcast(GameMessages.gameInitLevelStart, levelCode);
        Messenger<string>.AddListener(GameMessages.gameInitLevelStart, OnGameInitLevelStart);
    }
    
    public override void OnDisable() {
        base.OnDisable();
        
        Messenger<string>.RemoveListener(GameMessages.gameInitLevelStart, OnGameInitLevelStart);
    }

    public override void Reset() {
        base.Reset();

        actionCompleted = false;
        actionStarted = false;
        lastCreateProgress = 0;
        currentCreateProgress = 0;
        currentCreateState = GameZoneActionAssetState.none;
        actionGoalState = GameZoneActionAssetState.none;
        gameCharacter = null;
    }

    private void OnGameInitLevelStart(string levelCode) {
     
        // Add indicator after level loaded and player is ready

        LoadPlayerIndicator();
    }

    public void AssetAnimationIdle() {
        containerAssets.PlayAnimation(assetAnimationNameIdle);
    }

    public void AssetAnimationPlayNormalized(float time) {
        containerAssets.StepAnimationFrame(assetAnimationNamePlay, time);
    }

    public void AssetAnimationReset() {
        currentCreateProgress = 0;
        AssetAnimationPlayNormalized(currentCreateProgress);
    }

    public void Load(string gameZoneTypeTo, string actionCodeTo, string assetCodeTo, string assetPlatformCodeTo) {

        loaded = false;
        
        Reset();

        currentCreateState = GameZoneActionAssetState.none;
        assetCode = assetCodeTo;
        assetPlatformCode = assetPlatformCodeTo;
        gameZoneType = gameZoneTypeTo;
        actionCode = actionCodeTo;

        Load();
    }

    public void Load() {

        loaded = false;

        //if(!alwaysVisible) {
        container.Hide();
        //}

        //containerAssets.Hide();
        //containerAssetsPlatforms.Hide();
        //containerIcons.Hide();
        //containerEffects.Hide();
        //containerEffectsDamage.Hide();

        if (gameZoneType == GameZoneKeys.action_none) {
            return;
        }

        gameCharacter = GameCharacters.Instance.GetById(assetCode);
                
        //containerAssets.Show();
        //containerAssetsPlatforms.Show();
        //containerIcons.Show();
        //containerEffects.Show();
        //containerEffectsDamage.Show();

        container.Show();

        LoadAsset();

        LoadAssetPlatform();

        LoadIcons();

        HandleActionInit();
                
        //if(gamePlayerIndicator == null) {
        //    gamePlayerIndicator = GamePlayerIndicator.AddIndicator(gameObject, actionCode);
        //}

        loaded = true;
    }

    public void LoadPlayerIndicator() {
        if(gamePlayerIndicator == null) {
            gamePlayerIndicator = GamePlayerIndicator.AddIndicator(gameObject, actionCode);
            gamePlayerIndicator.alwaysVisible = true;
        }
    }

    public void LoadIcons() {

        // TODO icon from data

        foreach (UISprite spriteIcon in 
                containerIcons.GetList<UISprite>("Icon")) {
        
            if (isActionCodeSave) {
                spriteIcon.spriteName = "icon-arrow-64";
            }
            else if (isActionCodeRepair) {
                spriteIcon.spriteName = "icon-wrench-64";
            }
            else if (isActionCodeBuild) {
                spriteIcon.spriteName = "icon-magic-64";
            }
            else if (isActionCodeAttack) {
                spriteIcon.spriteName = "icon-weapon-64";
            }
        }
    }

    public void LoadAsset() {

        if (isActionCodeSave) {
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

                // Add game damage

                Collider goCollider = go.GetOrSet<Collider>();

                if(goCollider != null) {
                    
                    GameDamageManager gameDamageManager = 
                        goCollider.gameObject.GetOrSet<GameDamageManager>();
                    gameDamageManager.audioHit = "attack-hit-1";
                    gameDamageManager.effectDestroy = "effect-explosion";
                    gameDamageManager.enableObjectRemove = true;
                    gameDamageManager.HP = 1000;
                    
                    gameDamageManager.UpdateGameObjects();
                    
                }

                AssetAnimationReset();
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

    public void HandleActionInit() {
        if(isActionCodeAttack) {
            currentCreateProgress = 1f;
        }
        else if(isActionCodeRepair) {
            currentCreateProgress = UnityEngine.Random.Range(0.1f,0.5f);
        }
        else if(isActionCodeBuild || isActionCodeDefend) {
            currentCreateProgress = 0;
        }


        
        //GamePlayerIndicator.AddIndicator(GameHUD.Instance.containerOffscreenIndicators, 
        //                                 t.gameObject, "bot1");
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

        if(!GameConfigs.isGameRunning) {
            return;
        }

        float power = .1f;

        if (Application.isEditor) {
            if (Input.GetKey(KeyCode.LeftControl)) {

                if (Input.GetKey(KeyCode.KeypadPlus)) {
                    currentCreateProgress += power * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.KeypadMinus)) {

                    currentCreateProgress -= power * Time.deltaTime;
                }

                
                if (Input.GetKeyDown(KeyCode.Slash)) {
                    Load(
                        GameZoneKeys.action_attack, 
                        GameZoneActions.action_attack,
                        "level-building-1",// + UnityEngine.Random.Range(1, 10),
                        "platform-large-1");
                }
            }
        }
        
        if (!loaded) {
            return;
        }


        if (isActionCodeNone) {

            return;
        }

        HandleUpdateState(power, Time.deltaTime);

        HandleUpdateAction();
    }

}