using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;
using Engine.Game.Data;
using Engine.Game.App;

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
    public GameDamageManager gameDamageManager;
    public float hitPoints = 5000f;

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

    // -----------------------------------------------------------
    // ASSET EFFECTS

    // ASSET EFFECTS REPAIR

    public void AssetEffectRepairPlayNormalizedFlipped(float val) {

        if (containerEffectsRepair == null) {
            return;
        }

        containerEffectsRepair.SetParticleSystemEmission(true, true);
        containerEffectsRepair.SetParticleSystemEmissionRateNormalizedFlipped(val, true);
    }

    public void AssetEffectRepairPlay() {

        if (containerEffectsRepair == null) {
            return;
        }

        containerEffectsRepair.PlayParticleSystem(true);
    }

    //

    public void AssetEffectBuildPlayNormalizedFlipped(float val) {

        if (containerEffectsBuild == null) {
            return;
        }

        containerEffectsBuild.SetParticleSystemEmission(true, true);
        containerEffectsBuild.SetParticleSystemEmissionRateNormalizedFlipped(val, true);
    }

    public void AssetEffectBuildPlay() {

        if (containerEffectsBuild == null) {
            return;
        }

        containerEffectsBuild.PlayParticleSystem(true);
    }

    // -----------------------------------------------------------
    // ASSET ANIMATION

    public void AssetAnimationIdle() {

        if (containerAssets == null) {
            return;
        }

        containerAssets.PlayAnimation(assetAnimationNameIdle);
    }

    public void AssetAnimationPlayNormalized(float time) {

        if (containerAssets == null) {
            return;
        }

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
        if (gamePlayerIndicator == null) {
            gamePlayerIndicator = GamePlayerIndicator.AddIndicator(gameObject, actionCode);
            //gamePlayerIndicator.alwaysVisible = true;
        }
    }

    public void LoadIcons() {

        if (containerIcons == null) {
            return;
        }

        // TODO icon from data

        string spriteName = "";

        if (isActionCodeSave) {
            spriteName = "icon-arrow-64";
        }
        else if (isActionCodeRepair) {
            spriteName = "icon-wrench-64";
        }
        else if (isActionCodeBuild) {
            spriteName = "icon-magic-64";
        }
        else if (isActionCodeAttack) {
            spriteName = "icon-weapon-64";
        }
        else if (isActionCodeDefend) {
            spriteName = "icon-shield-64";
        }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3

        foreach (UISprite spriteIcon in
                containerIcons.GetList<UISprite>("Icon")) {

            if (spriteName.IsNotNullOrEmpty()) {
                spriteIcon.spriteName = spriteName;
            }
        }
#else
        // TODO Unity UI

        foreach(SpriteRenderer spriteIcon in
               containerIcons.GetList<SpriteRenderer>("Icon")) {

            if(spriteName.IsNotNullOrEmpty()) {
                spriteIcon.sprite.name = spriteName;
            }
        }

#endif

    }

    public void LoadAsset() {

        if (containerAssets == null) {
            return;
        }

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


                if (isActionCodeAttack || isActionCodeDefend) {

                    // Add game damage

                    Collider goCollider = go.GetOrSet<Collider>();

                    if (goCollider != null) {

                        gameDamageManager =
                            goCollider.gameObject.GetOrSet<GameDamageManager>();
                        gameDamageManager.audioHit = "attack-hit-1";
                        gameDamageManager.effectDestroy = "effect-explosion";
                        gameDamageManager.enableObjectRemove = false;
                        gameDamageManager.HP = hitPoints;

                        gameDamageManager.UpdateGameObjects();

                    }
                }

                AssetAnimationReset();
            }
        }
    }

    public float GetDamageValue() {

        if (gameDamageManager != null) {
            return gameDamageManager.HP;
        }

        return 0;
    }

    public void LoadAssetPlatform() {

        if (containerAssetsPlatforms == null) {
            return;
        }

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

    public void LoadAssetEffectProgressRepair() {

        if (containerEffectsRepair == null) {
            return;
        }

        if (assetEffectProgressRepairCode != BaseDataObjectKeys.none
            && lastAssetEffectProgressRepairCode != assetEffectProgressRepairCode) {

            lastAssetEffectProgressRepairCode = assetEffectProgressRepairCode;

            GameObject go = AppContentAssets.LoadAssetEffects(assetEffectProgressRepairCode);

            if (go != null) {
                containerEffectsRepair.DestroyChildren();
                go.transform.parent = containerEffectsRepair.transform;
                go.TrackObject(containerEffectsRepair);
            }
        }
    }

    public void LoadAssetEffectProgressBuild() {

        if (containerEffectsBuild == null) {
            return;
        }

        if (assetEffectProgressBuildCode != BaseDataObjectKeys.none
            && lastAssetEffectProgressBuildCode != assetEffectProgressBuildCode) {

            lastAssetEffectProgressBuildCode = assetEffectProgressBuildCode;

            GameObject go = AppContentAssets.LoadAssetEffects(assetEffectProgressBuildCode);

            if (go != null) {
                containerEffectsBuild.DestroyChildren();
                go.transform.parent = containerEffectsBuild.transform;
                go.TrackObject(containerEffectsBuild);
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
        if (isActionCodeAttack) {
            currentCreateProgress = 1f;
            assetEffectProgressRepairCode = "effect-fire-smoke-2";

            if (!string.IsNullOrEmpty(assetEffectProgressRepairCode)) {
                LoadAssetEffectProgressRepair();
            }
        }
        else if (isActionCodeRepair) {
            currentCreateProgress = UnityEngine.Random.Range(0.1f, 0.2f);
            assetEffectProgressRepairCode = "effect-fire-smoke-2";

            if (!string.IsNullOrEmpty(assetEffectProgressRepairCode)) {
                LoadAssetEffectProgressRepair();
            }
        }
        else if (isActionCodeBuild || isActionCodeDefend) {
            currentCreateProgress = 0;
        }

        //GamePlayerIndicator.AddIndicator(GameHUD.Instance.containerOffscreenIndicators, 
        //                                 t.gameObject, "bot1");
    }

    public void HandleUpdateAction() {

        Mathf.Clamp(currentCreateProgress, 0f, 1f);

        if (!actionCompleted) {

            bool scoreIt = false;

            if (isActionCodeBuild
                || isActionCodeRepair
                || isActionCodeDefend) {

                if (currentCreateProgress >= 1) {

                    actionCompleted = true;

                    AssetAnimationIdle();

                    currentCreateState = GameZoneActionAssetState.created;

                    if (isActionCodeBuild) {

                        scoreIt = true;

                        GameController.CurrentGamePlayerController.ProgressBuild(1);
                    }

                    else if (isActionCodeRepair) {

                        scoreIt = true;

                        AssetEffectRepairPlayNormalizedFlipped(1f);

                        GameController.CurrentGamePlayerController.ProgressRepair(1);
                    }

                    else if (isActionCodeDefend) {

                        scoreIt = true;

                        GameController.CurrentGamePlayerController.ProgressDefend(1);
                    }
                }
            }
            else if (isActionCodeAttack) {

                float normalizedValue = GetDamageValue() / hitPoints;
                currentCreateProgress = Mathf.Clamp(normalizedValue, .2f, 1f);

                AssetAnimationPlayNormalized(currentCreateProgress);
                AssetEffectRepairPlayNormalizedFlipped(normalizedValue);

                if (currentCreateProgress <= 0 || normalizedValue == 0) {

                    currentCreateProgress = 0f;

                    actionCompleted = true;

                    ChangeStateDestroying();

                    scoreIt = true;

                    GameController.CurrentGamePlayerController.ProgressAttack(1);
                }
            }

            if (scoreIt) {
                GameController.CurrentGamePlayerController.ProgressScore(1);
                GameController.CurrentGamePlayerController.ProgressScores(1);
            }

            if (currentCreateProgress != lastCreateProgress) {

                lastCreateProgress = currentCreateProgress;

                if (!actionCompleted) {

                    AssetAnimationPlayNormalized(currentCreateProgress);

                    if (isActionCodeRepair) {
                        AssetEffectRepairPlayNormalizedFlipped(currentCreateProgress);
                    }
                    else if (isActionCodeAttack) {
                        AssetEffectRepairPlayNormalizedFlipped(currentCreateProgress);
                    }
                }
            }
        }
    }

    public void Update() {

        if (!GameConfigs.isGameRunning) {
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
                    //Load(
                    //    GameZoneKeys.action_attack, 
                    //    GameZoneActions.action_attack,
                    //    "level-building-1",// + UnityEngine.Random.Range(1, 10),
                    //    "platform-large-1");
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