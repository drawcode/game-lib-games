using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using Engine.Data.Json;
using Engine.Events;
using Engine.Game;
using Engine.Game.Actor;
using Engine.Game.Controllers;
using Engine.Utility;

public class BaseGamePlayerController : GameActor {

    public float intervalUpdateAll = 0.033f;
    public float lastUpdateAll = 0;

    public bool AllowUpdateAll {

        get {

            if(lastUpdateAll +
                intervalUpdateAll < Time.time) {
                lastUpdateAll = Time.time;
                return true;
            }

            return false;
        }
    }

    //
    //

    //public string uuid = "";
    public string characterCode = ProfileConfigs.defaultGameCharacterCode;
    public Transform currentTarget;

    // data
    public GameCharacter gameCharacter;

    // asset
    public GameCustomPlayer gameCustomPlayer;


    // player effects
    public GameObject gamePlayerEffectParticleObjects;
    public ParticleSystem gamePlayerEffectWarp;
    public ParticleSystem gamePlayerEffectCircleFollow;
    public ParticleSystem gamePlayerEffectCircle;
    public ParticleSystem gamePlayerEffectCircleStars;
    public ParticleSystem gamePlayerEffectAttack;
    public ParticleSystem gamePlayerEffectSkill;
    public ParticleSystem gamePlayerEffectSlide;
    public GameObject gamePlayerEffectMarker;
    public GameObject gamePlayerEffectHit;
    public ParticleSystem gamePlayerEffectDeath;

    // appearance/context
    public ActorShadow actorShadow;

    // models/objects
    public GameObject gamePlayerModel;
    public GameObject gamePlayerHolder;
    public GameObject gamePlayerShadow;
    public GameObject gamePlayerEnemyTarget;
    public GameObject gamePlayerSidekickTarget;
    public GameObject gamePlayerModelTarget;
    public GameObject gamePlayerModelHolder;
    public GameObject gamePlayerModelHolderModel;
    public GameObject gamePlayerModelHolderWeapons;
    public GameObject gamePlayerModelHolderWeaponsHolder;
    public GameObject gamePlayerModelHolderItems;
    public GameObject gamePlayerModelHolderSkills;
    public GameObject gamePlayerSpawner;
    public Vector3 initialGamePlayerWeaponContainer = Vector3.zero;
    public Vector3 currentGamePlayerWeaponContainer = Vector3.zero;

    //gamePlayerModelHolderWeapons.transform.position

    // attack
    public GameObject weaponObject;

    // skill
    public GameObject skillObject;

    // states
    public GamePlayerControllerState controllerState = GamePlayerControllerState.ControllerNotSet;
    public GamePlayerContextState contextState = GamePlayerContextState.ContextNotSet;

    // controller runtime state
    public GamePlayerControllerData controllerData;

    // runtime data
    public GamePlayerRuntimeData runtimeData;
    public GamePlayerItemsData itemsData;

    // initialize
    public float initialMaxWalkSpeed = 5f;
    public float initialMaxTrotSpeed = 15f;
    public float initialMaxRunSpeed = 20f;
    public float initialMaxJumpHeight = .5f;
    public float initialMaxExtraJumpHeight = 1f;
    public float characterSlopeLimit = 45;
    public float characterStepOffset = .3f;
    public float characterRadius = 1f;
    public float characterHeight = 2.5f;
    public Vector3 characterCenter = new Vector3(0f, 0f, 0f);
    GameObject gameObjectLoad = null;

    // weapons
    public Dictionary<string, GamePlayerWeapon> weapons = new Dictionary<string, GamePlayerWeapon>();
    public GamePlayerWeapon weaponPrimary;
    public GamePlayerWeapon weaponSecondary;

    // network
    public Gameverses.GameNetworkPlayerContainer currentNetworkPlayerContainer;

    // CAMS
    public GameCameraSmoothFollow gameCameraSmoothFollow;
    public GameCameraSmoothFollow gameCameraSmoothFollowGround;
    //

    public GameObject gamePlayerEffectsBoost;
    public GameObject gamePlayerEffectsContainer;
    public GameObject gamePlayerEffectsGround;
    public GameObject gamePlayerTrailContainer;
    public GameObject gamePlayerTrailGround;
    public GameObject gamePlayerTrailBoost;

    // If this is an enemy see if we should attack

    public float attackRange = 12f;
    // within 6 yards
    public float attackDistance = 10f;
    public float lastStateEvaded = 0f;
    public float lastCollision = 0f;
    public float intervalCollision = .2f;

    // quality settings

    public float currentFPS = 60f;

    GameDamageManager gameDamageManager;

    // --------------------------------------------------------------------
    // INIT

    public virtual void Awake() {

    }

    public override void Start() {
        //Init(controllerState);

        HidePlayerShadow();
        ShowPlayerSpawner();
    }

    public override void OnEnable() {
        //MessengerObject<InputTouchInfo>.AddListener(MessengerObjectMessageType.OnEventInputDown, OnInputDown);
        //MessengerObject<InputTouchInfo>.AddListener(MessengerObjectMessageType.OnEventInputUp, OnInputUp);

        Messenger<string, Vector3>.AddListener(
            InputSystemEvents.inputAxis, OnInputAxis);//"input-axis-" + axisName, axisInput);

        Messenger<string, string>.AddListener(
            GamePlayerMessages.PlayerAnimation, OnPlayerAnimation);

        Messenger<string>.AddListener(GameMessages.gameLevelStart, OnGameLevelStart);

        Messenger<string>.AddListener(GameMessages.gameInitLevelStart, OnGameInitLevelStart);

        Messenger.AddListener(GameMessages.gameLevelPlayerReady, OnGameLevelPlayerReady);

        Gameverses.GameMessenger<string, Gameverses.GameNetworkAniStates>.AddListener(
            Gameverses.GameNetworkPlayerMessages.PlayerAnimation, OnNetworkPlayerAnimation);

        Gameverses.GameMessenger<string, float>.AddListener(
            Gameverses.GameNetworkPlayerMessages.PlayerInputAxisHorizontal, OnNetworkPlayerInputAxisHorizontal);

        Gameverses.GameMessenger<string, float>.AddListener(
            Gameverses.GameNetworkPlayerMessages.PlayerInputAxisVertical, OnNetworkPlayerInputAxisVertical);

        Gameverses.GameMessenger<string, float>.AddListener(
            Gameverses.GameNetworkPlayerMessages.PlayerSpeed, OnNetworkPlayerSpeed);

        Gameverses.GameMessenger<Gameverses.GameNetworkingAction, Vector3, Vector3>.AddListener(
            Gameverses.GameNetworkingMessages.ActionEvent, OnNetworkActionEvent);

    }

    public override void OnDisable() {
        //MessengerObject<InputTouchInfo>.RemoveListener(MessengerObjectMessageType.OnEventInputDown, OnInputDown);
        //MessengerObject<InputTouchInfo>.RemoveListener(MessengerObjectMessageType.OnEventInputUp, OnInputUp);
        Messenger<string, Vector3>.RemoveListener(
            InputSystemEvents.inputAxis, OnInputAxis);//"input-axis-" + axisName, axisInput); 

        Messenger<string, string>.RemoveListener(
            GamePlayerMessages.PlayerAnimation, OnPlayerAnimation);

        Messenger<string>.RemoveListener(GameMessages.gameLevelStart, OnGameLevelStart);

        Messenger<string>.RemoveListener(GameMessages.gameInitLevelStart, OnGameInitLevelStart);

        Messenger.RemoveListener(GameMessages.gameLevelPlayerReady, OnGameLevelPlayerReady);

        Gameverses.GameMessenger<string, Gameverses.GameNetworkAniStates>.RemoveListener(
            Gameverses.GameNetworkPlayerMessages.PlayerAnimation, OnNetworkPlayerAnimation);

        Gameverses.GameMessenger<string, float>.RemoveListener(
            Gameverses.GameNetworkPlayerMessages.PlayerInputAxisHorizontal, OnNetworkPlayerInputAxisHorizontal);

        Gameverses.GameMessenger<string, float>.RemoveListener(
            Gameverses.GameNetworkPlayerMessages.PlayerInputAxisVertical, OnNetworkPlayerInputAxisVertical);

        Gameverses.GameMessenger<string, float>.RemoveListener(
            Gameverses.GameNetworkPlayerMessages.PlayerSpeed, OnNetworkPlayerSpeed);

        Gameverses.GameMessenger<Gameverses.GameNetworkingAction, Vector3, Vector3>.RemoveListener(
            Gameverses.GameNetworkingMessages.ActionEvent, OnNetworkActionEvent);

    }

    public virtual void OnInputAxis(string name, Vector3 axisInput) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(controllerData == null) {
            return;
        }

        //float distance = Math.Abs(currentControllerData.effectWarpEnd - currentControllerData.effectWarpCurrent);

        //bool effectWarpOn = distance < 5 ? false : true;

        // main
        //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

        if(name == InputSystemEvents.inputAxisMove) {

            // INITIAL D-PAD

            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

            //if(isEntering || isExiting || effectWarpOn) { // || currentControllerData.effectWarpEnabled) {
            //    return;
            //}

            if(currentControllerData.thirdPersonController != null) {

                //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

                if(!axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    axisInput.x = 0f;
                    axisInput.y = 0f;
                }

                //if(axisInput.x != 0 || axisInput.y != 0) {
                //Debug.Log("axisInput x:" + axisInput.x + " y:" + axisInput.y);
                //}


                if(axisInput.x != 0 || axisInput.y != 0) {
                    //Debug.Log("axisInput x:" + axisInput.x + " y:" + axisInput.y);
                }

                //if(!GameController.isFingerNavigating) {
                HandleThirdPersonControllerAxis(axisInput);
            }
        }
        else if(name == InputSystemEvents.inputAxisAttack) {

            // SECONDARY D-PAD for DUAL STICK

            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

            if(currentControllerData.thirdPersonController != null) {

                //LogUtil.Log("OnInputAxis ATTACK:" + name + "input:" + axisInput);

                if(!axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    axisInput.x = 0f;
                    axisInput.y = 0f;
                }

                currentControllerData.thirdPersonController.horizontalInput2 = axisInput.x;
                currentControllerData.thirdPersonController.verticalInput2 = axisInput.y;

            }
        }
        else if(name == InputSystemEvents.inputAxisMoveHorizontal) {

            // INITIAL D-PAD ONLY FOR HORIZONTAL MOVEMENT

            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

            if(currentControllerData.thirdPersonController != null) {

                //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {

                    currentControllerData.thirdPersonController.horizontalInput = axisInput.x;
                    currentControllerData.thirdPersonController.verticalInput = 0f;//currentControllerData.thirdPersonController.verticalInput;
                }

                if(axisInput.y > .7f) {
                    //LogUtil.Log("axisInput.y:" + axisInput.y);
                    Jump();
                }
                else {
                    JumpStop();
                }

            }
        }
        else if(name == InputSystemEvents.inputAxisMoveVertical) {

            // INITIAL D-PAD ONLY FOR VERTICAL MOVEMENT

            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

            if(currentControllerData.thirdPersonController != null) {

                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
                    currentControllerData.thirdPersonController.horizontalInput = 0f;//axisInput.x;
                    currentControllerData.thirdPersonController.verticalInput = axisInput.y;
                }
            }
        }
        else if(name == InputSystemEvents.inputAxisAttack2DSide2) {

            // INITIAL D-PAD ONLY FOR SIDE SCROLLER ATTACK 2 

            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

            if(currentControllerData.thirdPersonController != null) {

                //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

                //currentControllerData.thirdPersonController.horizontalInput = axisInput.x;
                //currentControllerData.thirdPersonController.verticalInput = 0f;

                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {

                    if(controllerState == GamePlayerControllerState.ControllerPlayer) {
                        gamePlayerModelHolderModel
                                .transform.LookAt(-Vector3.zero.WithX(axisInput.x).WithY(axisInput.y));
                    }
                }
                else {
                    //GameController.CurrentGamePlayerController.gamePlayerModel.transform.rotation
                    //       = Quaternion.Euler(Vector3.zero);
                }


            }
        }
        else if(name == InputSystemEvents.inputAxisAttack2DSide) {

            // INITIAL D-PAD ONLY FOR SIDE SCROLLER ATTACK 1

            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

            if(currentControllerData.thirdPersonController != null) {

                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    //LogUtil.Log("OnInputAxis ATTACK:" + name + "input:" + axisInput);

                    currentControllerData.thirdPersonController.horizontalInput2 = -axisInput.x;
                    currentControllerData.thirdPersonController.verticalInput2 = 0f;//axisInput.y;

                    //UpdateAim(axisInput.x, axisInput.y);
                }
            }
        }
    }

    public virtual void HandleThirdPersonControllerAxis(Vector3 axisInput) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(currentControllerData.mountData.isMountedVehicle) {

            currentControllerData.mountData.SetMountVehicleAxis(axisInput.x, axisInput.y);
        }
        else {

            currentControllerData.thirdPersonController.horizontalInput = axisInput.x;
            currentControllerData.thirdPersonController.verticalInput = axisInput.y;

            if(axisInput.x != 0 && axisInput.y != 0) {
                Debug.Log("HandleThirdPersonControllerAxis: axisInput.x:" + axisInput.x + " axisInput.y:" + axisInput.y);
            }
        }
    }

    public virtual void SetThirdPersonControllerAxisAlt(Vector3 axisInput) {

    }

    public virtual void OnNetworkActionEvent(Gameverses.GameNetworkingAction actionEvent, Vector3 pos, Vector3 direction) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(actionEvent.uuidOwner == uniqueId) {
            AnimatePlayer(actionEvent.code);
        }
    }

    public virtual void OnNetworkPlayerAnimation(string uid, Gameverses.GameNetworkAniStates aniState) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(uniqueId == uid && !isMe) {
            if(currentControllerData.lastNetworkAniState != currentControllerData.currentNetworkAniState) {
                currentControllerData.lastNetworkAniState = currentControllerData.currentNetworkAniState;

                if(currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.walk) {

                }
                else if(currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.run) {

                }
                else if(currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.attack1) {

                }
                else if(currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.attack2) {

                }
                else if(currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.death) {

                }
                else if(currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.skill1) {

                }
                else if(currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.skill2) {

                }
            }
        }
    }

    public virtual void OnNetworkPlayerInputAxisHorizontal(string uid, float horizontalInput) {
        if(uniqueId == uid && !isMe) {
            if(currentControllerData.thirdPersonController != null) {
                currentControllerData.thirdPersonController.horizontalInput = horizontalInput;
            }
        }
    }

    public virtual void OnNetworkPlayerInputAxisVertical(string uid, float verticalInput) {
        if(uniqueId == uid && !isMe) {
            if(currentControllerData.thirdPersonController != null) {
                currentControllerData.thirdPersonController.verticalInput = verticalInput;
            }
        }
    }

    public virtual void OnNetworkPlayerSpeed(string uid, float speed) {
        if(uniqueId == uid && !isMe) {
            if(currentControllerData.thirdPersonController != null) {
                currentControllerData.thirdPersonController.moveSpeed = speed;
            }
        }
    }

    public override void OnInputDown(InputTouchInfo touchInfo) {
        LogUtil.Log("OnInputDown GameActor");

    }

    public virtual void OnGameLevelStart(string levelCode) {
        // Button pressed to run game after load

        if(IsPlayerControlled) {
            //GamePlayerModelHolderEaseOut(5, .1f, 0);
            //GamePlayerModelHolderEaseIn(0, 1, .2f);
        }
    }

    public virtual void OnGameInitLevelStart(string levelCode) {
        // Button pressed to run game after load

        //if(IsPlayerControlled) {
        GamePlayerModelHolderEaseStartByType();
        //}
    }

    public virtual void OnGameLevelPlayerReady() {

        if(IsPlayerControlled) {
            LoadPlayerReadyState();
        }
    }

    // ------------------------------------------------------------------------
    // DATA

    public virtual void SetRuntimeData(GamePlayerRuntimeData data) {
        if(data == null) {
            data = new GamePlayerRuntimeData();
        }

        runtimeData = data;

        // TODO sync if needed... to update 
        // runtime expensive states that can't be polled.
    }

    public virtual void SetControllerData(GamePlayerControllerData data) {
        if(data == null) {
            data = new GamePlayerControllerData();
        }

        controllerData = data;

        // TODO sync if needed... to update 
        // runtime expensive states that can't be polled.
    }

    public virtual void SetItemsData(GamePlayerItemsData data) {
        if(data == null) {
            data = new GamePlayerItemsData();
        }

        itemsData = data;

        // TODO sync if needed... to update 
        // runtime expensive states that can't be polled.
    }

    // ------------------------------------------------------------------------

    public virtual void Init(
        GamePlayerControllerState controllerStateTo,
        GamePlayerContextState contextStateTo, bool overrideLoading = false) {


        if(overrideLoading) {
            StopAllCoroutines();
            currentControllerData.loadingCharacter = false;

            //Debug.Log("GameCharacter:Init:" +
            //" overrideLoading:" +
            //overrideLoading);

            //Debug.Log("GameCharacter:Init:" +
            //" currentControllerData.loadingCharacter:" +
            //currentControllerData.loadingCharacter);

        }

        SetUp(controllerStateTo, contextStateTo);

    }

    // ------------------------------------------------------------------------
    // SPEED

    public virtual float gamePlayerMoveSpeed {
        get {
            return GamePlayerMoveSpeedGet();
        }
        set {
            GamePlayerMoveSpeedSet(value);
        }
    }

    public virtual void GamePlayerMoveSpeedSet(float moveSpeedTo) {

        if(currentControllerData.thirdPersonController != null) {
            controllerData.thirdPersonController.SetSpeed(moveSpeedTo);
        }
    }

    public virtual float GamePlayerMoveSpeedGet() {

        float currentSpeed = 0f;

        if(controllerData == null) {
            return currentSpeed;
        }

        if(currentControllerData.thirdPersonController != null) {
            currentSpeed = currentControllerData.thirdPersonController.GetSpeed();
        }

        if(contextState == GamePlayerContextState.ContextFollowAgent
            || contextState == GamePlayerContextState.ContextFollowAgentAttack
            || contextState == GamePlayerContextState.ContextRandom) {

            if(currentControllerData.navMeshAgent != null) {

                if(currentControllerData.navMeshAgent.enabled) {
                    //currentSpeed = navAgent.velocity.magnitude + 20;

                    if(currentControllerData.navMeshAgent.velocity.magnitude > 0f) {
                        currentSpeed = 15f;
                    }
                    else {
                        currentSpeed = 0;
                    }

                    if(currentControllerData.navMeshAgent.remainingDistance <
                        currentControllerData.navMeshAgent.stoppingDistance + 1) {
                        currentSpeed = 0;
                    }

                    if(currentSpeed <
                        currentControllerData.navMeshAgent.speed) {
                        //currentSpeed = 0;
                    }
                }
            }
        }

        return currentSpeed;
    }

    // ------------------------------------------------------------------------
    // REWARDS / ITEMS

    public virtual void HandleItemStateGoalFly(double val) {

        // TODO add item multi collect here

        //if(runtimeData.goalFly > 0) {
        //    return; // only one fly at a time for now...
        //}

        if(runtimeData.goalFly > 0 && val > 0) {
            return;
        }

        runtimeData.goalFly += val;
        runtimeData.goalFly = (double)Mathf.Clamp((float)runtimeData.goalFly, 0f, 5f);
    }

    public virtual void HandleItemStateCurrency(double val) {

        runtimeData.coins += val;

        if(GameController.IsGameplayWorldTypeStationary()) {
            SpeedUp(Vector3.zero.WithZ(.10f));
        }
    }

    public virtual void HandleItemStateSpecial(double val) {

        runtimeData.specials += val;

        if(GameController.IsGameplayWorldTypeStationary()) {
            SpeedUp(Vector3.zero.WithZ(.5f));
        }
    }

    public virtual void HandleItemStateScore(double val) {

        runtimeData.score += val;
    }

    public virtual void HandleItemStateScores(double val) {

        runtimeData.scores += val;
    }

    public virtual void HandleItemStateHitCount(double val) {
        runtimeData.hitCount += val;
    }

    public virtual void HandleItemStateHealth(double val) {
        HandleItemStateHitCount(-1);
        runtimeData.health += (float)val;
    }

    public virtual void HandleItemStateSpeedModifier(double val, double duration) {
        currentControllerData.modifierItemSpeedCurrent *= (float)val;
        currentControllerData.modifierItemSpeedLerpTime = (float)duration;

        currentControllerData.modifierItemSpeedLerp = 0f;
    }

    public virtual void HandleItemStateScaleModifier(double val, double duration) {
        currentControllerData.modifierItemScaleCurrent *= (float)val;
        currentControllerData.modifierItemScaleLerpTime = (float)duration;

        currentControllerData.modifierItemScaleLerp = 0f;

        ////Debug.Log("HandleItemStateScaleModifier::" + " val:" + val + " duration:" + duration);
        ////Debug.Log("HandleItemStateScaleModifier::" + " currentControllerData.modifierItemScaleCurrent:" + currentControllerData.modifierItemScaleCurrent);
        ////Debug.Log("HandleItemStateScaleModifier::" + " currentControllerData.modifierItemScaleLerpTime:" + currentControllerData.modifierItemScaleLerpTime);
        ////Debug.Log("HandleItemStateScaleModifier::" + " currentControllerData.modifierItemScaleLerp:" + currentControllerData.modifierItemScaleLerp);
    }

    public virtual void HandleItemStateFlyModifier(double val, double duration) {
        currentControllerData.modifierItemFlyCurrent *= (float)val;
        currentControllerData.modifierItemFlyLerpTime = (float)duration;

        currentControllerData.modifierItemFlyLerp = 0f;

        ////Debug.Log("HandleItemStateFlyModifier::" + " val:" + val + " duration:" + duration);
        ////Debug.Log("HandleItemStateFlyModifier::" + " currentControllerData.modifierItemFlyCurrent:" + currentControllerData.modifierItemFlyCurrent);
        ////Debug.Log("HandleItemStateFlyModifier::" + " currentControllerData.modifierItemFlyLerpTime:" + currentControllerData.modifierItemFlyLerpTime);
        ////Debug.Log("HandleItemStateFlyModifier::" + " currentControllerData.modifierItemFlyLerp:" + currentControllerData.modifierItemFlyLerp);
    }

    public virtual void HandleItemUse(GameItem gameItem) {

        GameDataObjectItem data = gameItem.data;

        if(data == null) {
            return;
        }

        //Messenger.Broadcast(GameMessages.gameActionItem)

        float modifier = 1f;

        GameDataItemRPG rpg = new GameDataItemRPG();

        if(data.HasRPGs()) {
            rpg = data.GetRPG();

            ////Debug.Log("HandleItemUse::" + " rpg:" + rpg.ToJson());

            HandleItemStateSpeedModifier(rpg.speed, rpg.duration);
            HandleItemStateScaleModifier(rpg.scale, rpg.duration);
            HandleItemStateFlyModifier(rpg.fly, rpg.duration);
        }

        // rewards

        if(data.HasRewards()) {

            List<GameDataItemReward> items = data.rewards;

            bool broadcastEvent = false;
            object broadcastVal = null;

            foreach(GameDataItemReward item in items) {

                broadcastEvent = false;
                broadcastVal = null;

                if(item.val == null) {
                    continue;
                }

                if(item.type.IsEqualLowercase(GameDataItemReward.xp)) {

                    double val = item.valDouble * modifier;

                    GamePlayerProgress.SetStatXP(val);

                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressXP(val);

                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if(item.type.IsEqualLowercase(GameDataItemReward.currency)) {

                    double val = item.valDouble * modifier;
                                        
                    GamePlayerProgress.SetStatCoins(val);
                    GamePlayerProgress.SetStatCoinsPickup(val);

                    HandleItemStateCurrency(val);

                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if(item.type.IsEqualLowercase(GameDataItemReward.health)) {

                    double val = item.valDouble * modifier;

                    HandleItemStateHealth(val);

                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(val); // refill
                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(val); // refill                        

                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if(item.type.IsEqualLowercase(GameDataItemReward.weapon)) {

                    double val = item.valDouble * modifier;

                    //GamePlayerProgress.SetStatCoins(val);
                    //GamePlayerProgress.SetStatCoinsPickup(val);     

                    //HandleItemStateCurrency(val);

                    GameController.CurrentGamePlayerController.LoadWeapon(item.code);//.LoadWeaponNext();

                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if(item.type.IsEqualLowercase(GameDataItemReward.goalFly)) {

                    double val = item.valDouble * modifier;

                    HandleItemStateGoalFly(val);

                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(val); // refill
                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(val); // refill                        

                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if(item.type.IsEqualLowercase(GameDataItemReward.letter)) {

                    string val = item.valString;

                    GamePlayerProgress.SetStatLetters(1);

                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if(item.type.IsEqualLowercase(GameDataItemReward.specials)) {

                    double val = item.valDouble;

                    GamePlayerProgress.SetStatSpecials(val);

                    HandleItemStateSpecial(val);

                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if(item.type.IsEqualLowercase(GameDataItemReward.score)) {

                    double val = item.valDouble;

                    GamePlayerProgress.SetStatScore(val);

                    HandleItemStateScore(val);

                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if(item.type.IsEqualLowercase(GameDataItemReward.scores)) {

                    double val = item.valDouble;

                    GamePlayerProgress.SetStatScores(val);

                    HandleItemStateScores(val);

                    broadcastEvent = true;
                    broadcastVal = val;
                }

                if(broadcastEvent) {
                    Messenger<string, string, object>.Broadcast(GameMessages.gameActionItem, gameItem.code, item.type, broadcastVal);
                }

            }
        }

        // sounds

        GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);

        if(data.HasSounds()) {

            data.PlaySoundType(GameDataActionKeys.reward);
        }
    }

    // ------------------------------------------------------------------------
    // EFFECTS

    // LINE RENDERERS

    public virtual void PlayerEffectTrailGroundFadeIn() {

        TweenUtil.FadeToObject(gamePlayerTrailGround, 1f, 1f, .5f);

        //UITweenerUtil.FadeTo(gamePlayerTrailGround,
        //    UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 1f);
    }

    public virtual void PlayerEffectTrailGroundFadeOut() {

        TweenUtil.FadeToObject(gamePlayerTrailGround, 0f, 1f, .5f);

        //UITweenerUtil.FadeTo(gamePlayerTrailGround,
        //    UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 0f);
    }

    public virtual void PlayerEffectTrailBoostFadeIn() {

        TweenUtil.FadeToObject(gamePlayerTrailBoost, 1f, 1f, .5f);

        //UITweenerUtil.FadeTo(gamePlayerTrailBoost,
        //    UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 1f);
    }

    public virtual void PlayerEffectTrailBoostFadeOut() {

        TweenUtil.FadeToObject(gamePlayerTrailBoost, 0f, 1f, .5f);

        //UITweenerUtil.FadeTo(gamePlayerTrailBoost,
        //    UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 0f);
    }

    public virtual bool CheckTrailRendererBoost() {
        if(currentControllerData.trailRendererBoost == null && gamePlayerTrailBoost != null) {
            currentControllerData.trailRendererBoost = gamePlayerTrailBoost.Get<TrailRenderer>();
            return true;
        }
        return false;
    }

    public virtual bool CheckTrailRendererGround() {
        if(currentControllerData.trailRendererGround == null && gamePlayerTrailGround != null) {
            currentControllerData.trailRendererGround = gamePlayerTrailGround.Get<TrailRenderer>();
            return true;
        }
        return false;
    }

    public virtual void PlayerEffectTrailBoostTime(float time) {
        if(gamePlayerTrailBoost != null) {
            CheckTrailRendererBoost();

            if(currentControllerData.trailRendererBoost != null) {
                currentControllerData.trailRendererBoost.time = time;
            }
        }
    }

    public virtual void PlayerEffectTrailGroundTime(float time) {
        if(gamePlayerTrailGround != null) {
            CheckTrailRendererGround();

            if(currentControllerData.trailRendererGround != null) {
                currentControllerData.trailRendererGround.time = time;
            }
        }
    }

    public virtual void HandlePlayerEffectTrailGroundTick() {
        if(gamePlayerTrailGround != null) {

            // UPDATE color randomly
            // TODO add other conditions to get colors, health, power etc
            // Currently randomize player colors for effect

            CheckTrailRendererGround();

            if(currentControllerData.trailRendererGround != null) {

                //Color colorTo = GameCustomController.GetRandomizedColorFromContextEffects();
                //currentControllerData.trailRendererGround.gameObject.ColorTo(colorTo, 1f, 0f);
            }
        }
    }

    public virtual void HandlePlayerEffectTrailBoostTick() {
        if(gamePlayerTrailBoost != null) {

            // UPDATE color randomly
            // TODO add other conditions to get colors, health, power etc
            // Currently randomize player colors for effect

            CheckTrailRendererBoost();

            if(gamePlayerTrailBoost != null) {

                //Color colorTo = GameCustomController.GetRandomizedColorFromContextEffects();
                //currentControllerData.trailRendererBoost.gameObject.ColorTo(colorTo, 1f, 0f);
            }
        }
    }

    // ------------------------------------------------------------------------
    // EFFECTS ACTION

    public virtual void PlayEffectHit() {

        if(gamePlayerEffectHit != null) {
            gamePlayerEffectHit.PlayParticleSystem(true);
            ;
        }
    }

    // ------------------------------------------------------------------------
    // EFFECTS FOLLOW - GROUND/BACK/HEAD ETC

    public bool playerEffectsGroundShow {
        get {

            if(FPSDisplay.isUnder20FPS) {
                return false;
            }

            return true;
        }
    }

    public virtual void PlayerEffectsGroundFadeIn() {

        if(!playerEffectsGroundShow) {
            return;
        }

        TweenUtil.FadeToObject(gamePlayerEffectsGround, 1f, 1f, .5f);

        //UITweenerUtil.FadeTo(gamePlayerEffectsGround,
        //    UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 1f);
    }

    public virtual void PlayerEffectsGroundFadeOut() {

        if(!playerEffectsGroundShow) {
            return;
        }

        TweenUtil.FadeToObject(gamePlayerEffectsGround, 0f, 1f, .5f);

        //UITweenerUtil.FadeTo(gamePlayerEffectsGround,
        //    UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 0f);
    }

    public virtual void PlayerEffectEffectsBoostFadeIn() {

        TweenUtil.FadeToObject(gamePlayerEffectsBoost, 1f, 1f, .5f);

        //UITweenerUtil.FadeTo(gamePlayerEffectsBoost,
        //    UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 1f);
    }

    public virtual void PlayerEffectEffectsBoostFadeOut() {

        TweenUtil.FadeToObject(gamePlayerEffectsBoost, 0f, 1f, .5f);

        //UITweenerUtil.FadeTo(gamePlayerEffectsBoost,
        //    UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 0f);
    }

    public virtual void PlayerEffectsBoostTime(float time) {
        if(gamePlayerEffectsBoost != null) {
            gamePlayerEffectsBoost.SetParticleSystemEmissionRate(time, true);
        }
    }

    public virtual void PlayerEffectsGroundTime(float time) {

        if(!playerEffectsGroundShow) {
            return;
        }

        if(gamePlayerEffectsGround != null) {
            gamePlayerEffectsGround.SetParticleSystemEmissionRate(time, true);
        }
    }

    public virtual void HandlePlayerEffectsObjectTick(
        ref GameObject effectsObject,
        ref Color colorCurrent,
        ref Color colorLast,
        ref float lastTime,
        float speed,
        float alpha = 1.0f) {

        // UPDATE color randomly
        // TODO add other conditions to get colors, health, power etc
        // Currently randomize player colors for effect

        float updateTime = 0;
        bool immediate = lastTime < 0 ? true : false;


        if(lastTime < 5 && lastTime >= 0) {
            lastTime += Time.deltaTime;
        }
        else {
            lastTime = 0;
            colorCurrent =
                GameCustomController.GetRandomizedColorFromContextEffects();
        }

        if(immediate) {
            updateTime = 1;
        }
        else {
            updateTime = lastTime / speed;
        }

        colorCurrent.a = alpha;
        colorLast.a = alpha;

        colorLast =
            Color.Lerp(colorLast,
            colorCurrent,
            updateTime);

        effectsObject.SetParticleSystemStartColor(colorLast, true);
    }

    public virtual void HandlePlayerEffectsTick() {

        //if (currentControllerData.lastPlayerEffectsTrailUpdate + 4 < Time.time) {
        //currentControllerData.lastPlayerEffectsTrailUpdate = Time.time;
        //HandlePlayerEffectTrailBoostTick();            
        //HandlePlayerEffectTrailGroundTick();  
        //}

        HandlePlayerEffectsGroundTick();
        HandlePlayerEffectsBoostTick();
        HandlePlayerEffectsIndicatorTick();
    }

    public virtual void HandlePlayerEffectsGroundTick() {

        if(!playerEffectsGroundShow) {
            return;
        }

        if(gamePlayerEffectsGround != null) {

            HandlePlayerEffectsObjectTick(ref gamePlayerEffectsGround,
                ref currentControllerData.gamePlayerEffectsGroundColorCurrent,
                ref currentControllerData.gamePlayerEffectsGroundColorLast,
                ref currentControllerData.lastPlayerEffectsGroundUpdate,
                5, .95f);
        }
    }

    public virtual void HandlePlayerEffectsBoostTick() {
        if(gamePlayerEffectsBoost != null) {

            HandlePlayerEffectsObjectTick(ref gamePlayerEffectsBoost,
                ref currentControllerData.gamePlayerEffectsBoostColorCurrent,
                ref currentControllerData.gamePlayerEffectsBoostColorLast,
                ref currentControllerData.lastPlayerEffectsBoostUpdate,
                3, .3f);
        }
    }

    public virtual void HandlePlayerEffectsIndicatorTick() {
        if(gamePlayerShadow != null) {

            HandlePlayerEffectsObjectTick(ref gamePlayerShadow,
                ref currentControllerData.gamePlayerEffectsIndicatorColorCurrent,
                ref currentControllerData.gamePlayerEffectsIndicatorColorLast,
                ref currentControllerData.lastPlayerEffectsIndicatorUpdate,
                3,
                .3f);
        }
    }

    // ------------------------------------------------------------------------
    // ACTOR ENTERING/EXITING

    void GamePlayerModelActorEnteringSet(float delay, bool val) {
        StartCoroutine(GamePlayerModelActorEnteringSetCo(delay, val));
    }

    IEnumerator GamePlayerModelActorEnteringSetCo(float delay, bool val) {
        yield return new WaitForSeconds(delay);

        currentControllerData.actorEntering = val;
    }

    public virtual void GamePlayerModelHolderEase(float height = 0, float time = .5f, float delay = 0) {

        //LeanTween.cancel(gamePlayerModelHolder);

        LeanTween
            .moveLocalY(gamePlayerModelHolder, height, time)
                .setEase(LeanTweenType.easeInOutQuad)
                    .setDelay(delay);
    }

    public virtual void GamePlayerModelHolderEaseIn(float height = 0, float time = .5f, float delay = .1f) {

        PlayerEffectWarpFadeOut();
        GamePlayerModelHolderEase(height, time, delay);

        GamePlayerModelActorEnteringSet(delay, false);
    }

    public virtual void GamePlayerModelHolderEaseOut(float height = 0, float time = .5f, float delay = 0) {
        PlayerEffectWarpFadeIn();
        GamePlayerModelHolderEase(height, time, delay);
    }

    public virtual void GamePlayerModelHolderEaseStartByType() {

        if(GameController.IsGameplayTypeRunner()) {
            GamePlayerModelHolderEaseOut(5f, .1f, 0f);
            if(!IsPlayerControlled) {
                GamePlayerModelHolderEaseIn(0, .3f, .3f);
            }
        }
        else if(GameController.IsGameplayTypeDasher()) {
            GamePlayerModelHolderEaseOut(5f, .1f, 0f);
            if(!IsPlayerControlled) {
                GamePlayerModelHolderEaseIn(0, .3f, .3f);
            }
        }
    }

    public virtual void GamePlayerModelHolderEaseReadyByType() {

        if(GameController.IsGameplayTypeRunner()) {
            //if(IsPlayerControlled) {
            GamePlayerModelHolderEaseIn(0, .3f, .3f);
            //}
        }
        else if(GameController.IsGameplayTypeDasher()) {
            //if(IsPlayerControlled) {
            GamePlayerModelHolderEaseIn(0, .3f, .3f);
            //}
        }
    }

    public virtual void GamePlayerModelHolderEaseEndByType() {

        if(GameController.IsGameplayTypeRunner()) {

            GamePlayerModelHolderEaseOut(0, .1f, 0);
        }
        else if(GameController.IsGameplayTypeDasher()) {

            GamePlayerModelHolderEaseOut(5, .1f, 0);
        }
    }

    // ------------------------------------------------------------------------
    // WARP

    public virtual void PlayerEffectWarpFadeOut() {
        PlayerEffectWarpAnimate(200, 0);
    }

    public virtual void PlayerEffectWarpFadeIn() {
        PlayerEffectWarpAnimate(0, 200);
    }

    public virtual void PlayerEffectWarpAnimate(float fromEmission, float toEmission) {
        currentControllerData.effectWarpStart = fromEmission;
        currentControllerData.effectWarpEnd = toEmission;
        currentControllerData.effectWarpEnabled = true;
    }

    public virtual void HandlePlayerEffectWarpAnimateTick() {

        if(!IsPlayerControlled) {
            //currentControllerData.visible
        }

        if(currentControllerData.effectWarpEnabled) {
            float fadeSpeed = currentControllerData.effectWarpFadeSpeed;

            if(!IsPlayerControlled) {
                fadeSpeed = currentControllerData.effectWarpFadeSpeed * 1.5f;
            }

            if(currentControllerData.effectWarpCurrent < currentControllerData.effectWarpEnd) {
                currentControllerData.effectWarpCurrent += (Time.deltaTime * fadeSpeed);
                SetPlayerEffectWarp(currentControllerData.effectWarpCurrent);
            }
            else if(currentControllerData.effectWarpCurrent > currentControllerData.effectWarpEnd) {
                currentControllerData.effectWarpCurrent -= (Time.deltaTime * fadeSpeed);
                SetPlayerEffectWarp(currentControllerData.effectWarpCurrent);
            }
            else {
                if(currentControllerData.effectWarpCurrent != currentControllerData.effectWarpEnd) {
                    currentControllerData.effectWarpEnabled = false;
                    currentControllerData.effectWarpCurrent = currentControllerData.effectWarpEnd;
                    SetPlayerEffectWarp(currentControllerData.effectWarpCurrent);
                }
            }
        }
    }

    public virtual void SetPlayerEffectWarp(float rate) {
        if(gamePlayerEffectWarp != null) {

            if(gamePlayerEffectWarp.GetEmissionRate() == rate) {
                return;
            }

            if(rate > 0
                && !gamePlayerEffectWarp.isPlaying) {
                gamePlayerEffectWarp.Play();
            }
            else if(rate <= 0
                     && gamePlayerEffectWarp.isPlaying) {
                gamePlayerEffectWarp.Stop();
            }

            gamePlayerEffectWarp.SetEmissionRate(rate);
        }
    }

    public virtual void ShowPlayerEffectWarp() {
        if(gamePlayerEffectWarp != null) {
            SetPlayerEffectWarp(200);
        }
    }

    public virtual void HidePlayerEffectWarp() {
        if(gamePlayerEffectWarp != null) {
            SetPlayerEffectWarp(0);
        }
    }

    // ------------------------------------------------------------------------
    // PLAYER SHADOW + EFFECTS

    public virtual void ShowPlayerShadow() {
        if(gamePlayerShadow != null) {
            gamePlayerShadow.Show();
        }
    }

    public virtual void HidePlayerShadow() {
        if(gamePlayerShadow != null) {
            gamePlayerShadow.Hide();
        }
    }

    public virtual void ShowPlayerSpawner() {
        if(gamePlayerSpawner != null) {
            gamePlayerSpawner.Show();
        }
    }

    public virtual void HidePlayerSpawner() {
        if(gamePlayerSpawner != null) {
            gamePlayerSpawner.Hide();
        }
    }

    // ------------------------------------------------------------------------
    // PLAYER CIRCLE INDICATOR GROUND

    public virtual void ShowPlayerEffectCircleFollow() {
        if(gamePlayerEffectCircleFollow != null) {
            gamePlayerEffectCircleFollow.Play();
        }
    }

    public virtual void HidePlayerEffectCircleFollow() {
        if(gamePlayerEffectCircleFollow != null) {
            gamePlayerEffectCircleFollow.Pause();
        }
    }

    public virtual void ShowPlayerEffectCircle() {
        if(gamePlayerEffectCircle != null) {
            gamePlayerEffectCircle.Play();
        }
    }

    public virtual void HidePlayerEffectCircle() {
        if(gamePlayerEffectCircle != null) {
            gamePlayerEffectCircle.Pause();
        }
    }

    public virtual void ShowPlayerEffectCircleStars() {
        if(gamePlayerEffectCircleStars != null) {
            gamePlayerEffectCircleStars.Play();
        }
    }

    public virtual void HidePlayerEffectCircleStars() {
        if(gamePlayerEffectCircleStars != null) {
            gamePlayerEffectCircleStars.Pause();
        }
    }

    // ------------------------------------------------------------------------
    // RUNTIME STATES

    public virtual void HandlePlayerAliveState() {

        if(currentControllerData.lastCharacterLoadedCheck + 1 < Time.time) {
            currentControllerData.lastCharacterLoadedCheck = Time.time;

            if(!isCharacterLoaded) {
                //LoadCharacter(characterCode);
            }
        }

        if(runtimeData.health <= 0f) {
            Die();
        }

        //UpdatePhysicsState();
    }

    public virtual void HandlePlayerAliveStateLate() {

        UpdatePhysicsState();
    }

    public virtual void HandlePlayerAliveStateFixed() {

    }

    public virtual void HandlePlayerInactionState() {

        if(!IsPlayerControlled) {
            return;
        }

        // update player controlled players to look at player and animate it inactive

        bool update = false;

        if(controllerData != null) {

            if(currentControllerData.lastIdleActions + UnityEngine.Random.Range(3, 7) < Time.time) {

                currentControllerData.lastIdleActions = Time.time;

                if(currentControllerData.thirdPersonController != null) {

                    if(currentControllerData.thirdPersonController.GetSpeed() == 0f) {
                        update = true;
                    }
                }
            }

            if(!update) {
                return;
            }
        }

        // Look at camera

        // TODO set default
        //OnInputAxis(GameTouchInputAxis.inputAxisMove, Vector3.zero.WithY(-1));

        Idle();
    }

    // ------------------------------------------------------------------------
    // CHARACTER

    public virtual bool isMe {
        get {
            if(uniqueId == UniqueUtil.Instance.currentUniqueId) {
                return true;
            }
            return false;
        }
    }

    public virtual bool isDead {
        get {
            return !isAlive;
        }
    }

    public virtual bool isAlive {
        get {

            bool hasHealth = runtimeData.health > 0f;

            bool notDying = true;

            if(currentControllerData != null) {
                notDying = !currentControllerData.dying;
            }

            return hasHealth && notDying ? true : false;
        }
    }

    public virtual bool isExiting {
        get {

            bool exiting = true;

            if(currentControllerData != null) {
                exiting = currentControllerData.actorExiting;
            }

            return exiting;
        }
    }

    public virtual bool isEntering {
        get {

            bool entering = true;

            if(currentControllerData != null) {
                entering = currentControllerData.actorEntering;
            }

            return entering;
        }
    }

    public virtual bool IsPlayerControlled {
        get {
            if(controllerState == GamePlayerControllerState.ControllerPlayer
                || contextState == GamePlayerContextState.ContextInput
                || contextState == GamePlayerContextState.ContextInputVehicle
                || contextState == GamePlayerContextState.ContextFollowInput
                || uniqueId == UniqueUtil.Instance.currentUniqueId) {
                return true;
            }
            return false;
        }
    }

    public virtual bool IsSidekickControlled {
        get {
            return IsSidekickState();
        }
    }

    public virtual bool IsAssetControlled {
        get {
            return IsAssetState();
        }
    }

    public virtual bool IsAgentControlled {
        get {
            return IsAgentState();
        }
    }

    public virtual bool IsSidekickState() {
        if(controllerState == GamePlayerControllerState.ControllerSidekick) {
            return true;
        }
        return false;
    }

    public virtual bool IsAssetState() {
        if(controllerState == GamePlayerControllerState.ControllerAsset) {
            return true;
        }
        return false;
    }

    public virtual bool IsAgentState() {
        if(controllerState == GamePlayerControllerState.ControllerAgent) {
            return true;
        }
        return false;
    }

    public virtual bool IsPlayerState() {
        if(controllerState == GamePlayerControllerState.ControllerPlayer) {
            return true;
        }
        return false;
    }

    public virtual bool IsNetworkPlayerState() {
        if(controllerState == GamePlayerControllerState.ControllerNetwork) {
            return true;
        }
        return false;
    }

    public virtual bool IsUIState() {
        if(controllerState == GamePlayerControllerState.ControllerUI) {
            return true;
        }
        return false;
    }

    public virtual void ChangePlayerState(GamePlayerControllerState controllerStateTo) {
        StartCoroutine(ChangePlayerStateCo(controllerStateTo));
    }

    public virtual IEnumerator ChangePlayerStateCo(GamePlayerControllerState controllerStateTo) {
        //if (controllerStateTo != controllerState) {
        controllerState = controllerStateTo;

        yield return StartCoroutine(InitControlsCo());

        if(controllerState == GamePlayerControllerState.ControllerAgent) {
            if(currentControllerData.navMeshAgent != null) {
                // TODO load script or look for character input.
                currentControllerData.navMeshAgent.enabled = true;
            }
        }
        else if(controllerState == GamePlayerControllerState.ControllerPlayer) {
            if(currentControllerData.navMeshAgent != null) {

                if(currentControllerData.navMeshAgent.isActiveAndEnabled) {
                    currentControllerData.navMeshAgent.isStopped = true;
                }
                //navMeshAgent.enabled = false;
            }
        }
        else if(controllerState == GamePlayerControllerState.ControllerNetwork) {
            if(currentControllerData.navMeshAgent != null) {
                if(currentControllerData.navMeshAgent.isActiveAndEnabled) {
                    currentControllerData.navMeshAgent.isStopped = true;
                }
                //navMeshAgent.enabled = false;
            }
            ChangeContextState(GamePlayerContextState.ContextNetwork);
        }
        else if(controllerState == GamePlayerControllerState.ControllerUI) {
            if(currentControllerData.navMeshAgent != null) {
                if(currentControllerData.navMeshAgent.isActiveAndEnabled) {
                    currentControllerData.navMeshAgent.isStopped = true;
                }
                //navMeshAgent.enabled = false;  
                if(currentControllerData.thirdPersonController != null) {
                    currentControllerData.thirdPersonController.getUserInput = true;
                }
            }
        }
        //}
    }

    // ------------------------------------------------------------------------
    // CONTROLLER

    public virtual GamePlayerController GetController(Transform transform) {
        if(transform != null) {
            GamePlayerController gamePlayerController =
                transform.GetComponentInChildren<GamePlayerController>();
            if(gamePlayerController != null) {
                return gamePlayerController;
            }
        }
        return null;
    }

    // ------------------------------------------------------------------------
    // CHARACTER

    public virtual void LoadCharacterAttachedSounds() {

        // TODO footsteps over different terrain
        // Foosteps, breathing etc.

        if(currentControllerData.audioObjectFootsteps == null) {

            string soundFootsteps = "audio_footsteps_default";

            if(gameCharacter != null) {

                GameDataSound dataSound = gameCharacter.data.GetSoundByType(GameDataActionKeys.footsteps);

                if(dataSound != null) {
                    soundFootsteps = dataSound.code;
                }
            }

            currentControllerData.audioObjectFootsteps = GameAudio.PlayEffectObject(transform, soundFootsteps, true);

            if(currentControllerData.audioObjectFootsteps != null) {

                AudioSource audioObject = currentControllerData.audioObjectFootsteps.GetComponent<AudioSource>();

                if(audioObject != null) {
                    currentControllerData.audioObjectFootstepsSource = audioObject;

                    if(currentControllerData.audioObjectFootstepsClip == null && currentControllerData.audioObjectFootstepsSource.clip != null) {
                        currentControllerData.audioObjectFootstepsClip = audioObject.clip;
                    }
                }
            }
        }
    }

    public virtual void HandleCharacterAttachedSounds() {

        if(!GameConfigs.isGameRunning) {
            if(controllerData != null) {
                if(currentControllerData.audioObjectFootstepsSource != null) {
                    currentControllerData.audioObjectFootstepsSource.StopIfPlaying();
                }
            }
            return;
        }

        LoadCharacterAttachedSounds();

        if(currentControllerData.audioObjectFootstepsSource == null) {
            return;
        }

        currentControllerData.audioObjectFootstepsSource.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();

        if(gamePlayerMoveSpeed > .1f) {
            ////currentControllerData.audioObjectFootstepsSource.volume = 1f;
            float playSpeed = Mathf.InverseLerp(0, initialMaxRunSpeed, gamePlayerMoveSpeed) + .8f;
            //LogUtil.Log("playSpeed", playSpeed);
            currentControllerData.audioObjectFootstepsSource.pitch = playSpeed;
        }
        else {
            currentControllerData.audioObjectFootstepsSource.pitch = 0f;
        }
    }

    public virtual bool isCharacterLoaded {
        get {
            return gamePlayerModelHolderModel.transform.childCount > 0;
        }
    }

    public virtual void LoadAnimatedActor() {
        if(controllerData == null) {
            return;
        }

        if(currentControllerData.gamePlayerControllerAnimation == null) {
            return;
        }

        currentControllerData.gamePlayerControllerAnimation.LoadAnimatedActor();
    }

    public virtual void ChangeCharacter(string characterCodeTo) {
        characterCode = characterCodeTo;

        Init(
            GamePlayerControllerState.ControllerPlayer,
            GamePlayerContextState.ContextInput);

        LoadCharacter(characterCode);
    }

    public virtual void LoadCharacter(string characterCodeTo) {

        if(currentControllerData.loadingCharacter) {
            StopAllCoroutines();
            currentControllerData.loadingCharacter = false;
        }

        characterCode = characterCodeTo;

        UpdateCharacterStates();

        //if (currentControllerData.lastCharacterCode != characterCode 
        //    || currentControllerData.lastCharacterCode == null) {

        currentControllerData.lastCharacterCode = characterCode;

        //if (gameObject.activeInHierarchy) {
        StartCoroutine(LoadCharacterCo());
        //}
        //}
    }

    public virtual void UpdateCharacterStates() {

        UpdateCharacterRuntimeState();

        SetControllerData(new GamePlayerControllerData());
    }

    public virtual void UpdateCharacterRuntimeState() {

        ResetScale();
        ResetPosition();

        SetRuntimeData(new GamePlayerRuntimeData());

        currentControllerData.ResetRuntime();

        currentControllerData.dying = false;
        currentControllerData.actorExiting = false;

        if(currentControllerData.thirdPersonController != null) {
            currentControllerData.thirdPersonController.Reset();//.controllerData.removing = false;
        }

        if(currentControllerData.gamePlayerControllerAnimation != null) {
            currentControllerData.gamePlayerControllerAnimation.Reset();
        }
    }

    public virtual IEnumerator LoadCharacterCo() {

        gameCharacter =
            GameCharacters.Instance.GetById(characterCode);

        if(gameCharacter == null) {
            yield break;
        }

        //Debug.Log("LoadCharacterCo:" + " code:" + gameCharacter.code);

        currentControllerData.loadingCharacter = true;

        string prefabCode = gameCharacter.data.GetModel().code;

        //Debug.Log("LoadCharacterCo:" + " prefabCode:" + prefabCode);

        //LogUtil.Log("LoadCharacter:path:" + path);
        if(!string.IsNullOrEmpty(prefabCode)) {
            if(gamePlayerModelHolderModel.transform.childCount > 0) {
                // Remove all current characters
                foreach(Transform t in gamePlayerModelHolderModel.transform) {
                    // Pool safely destroys either way
                    GameObjectHelper.DestroyGameObject(
                        t.gameObject, GameConfigs.usePooledGamePlayers);

                    //LogUtil.Log("LoadCharacter:destroy pooled:t.name:" + t.name);
                }
            }

            yield return new WaitForSeconds(.5f);

            gameObjectLoad = AppContentAssets.LoadAsset(prefabCode);

            if(gameObjectLoad != null) {

                // Wire up custom objects

                gameCustomPlayer = gameObjectLoad.Set<GameCustomPlayer>();

                if(IsPlayerControlled) {

                    GameCustomCharacterData customInfo = gameCustomPlayer.customCharacterData;

                    if(customInfo == null) {
                        customInfo = new GameCustomCharacterData();
                        customInfo.type = GameCustomTypes.defaultType;
                    }

                    gameCustomPlayer.SetActorHero();
                    gameCustomPlayer.Load(customInfo);

                    if(gamePlayerEffectsContainer != null) {
                        gamePlayerEffectsContainer.Show();
                    }
                }
                else {

                    if(gamePlayerEffectsContainer != null) {
                        gamePlayerEffectsContainer.Hide();
                    }

                    gameCustomPlayer.SetActorEnemy();
                }

                if(!IsPlayerControlled
                    && GameAIController.generateType == GameAICharacterGenerateType.team) {
                    //&& contextState == GamePlayerContextState.) {
                    // apply team colors and textures

                    GameTeam team = GameTeams.Current;

                    if(team != null) {
                        if(team.data != null) {

                            GameCustomCharacterData customInfo = new GameCustomCharacterData();

                            customInfo.actorType = GameCustomActorTypes.enemyType;
                            customInfo.presetColorCode = team.data.GetColorPreset().code;//GetColorPresetCode();
                            customInfo.presetTextureCode = team.data.GetTexturePreset().code;
                            customInfo.type = GameCustomTypes.teamType;
                            customInfo.teamCode = team.code;

                            gameCustomPlayer.Load(customInfo);
                        }
                    }
                }

                gameObjectLoad.transform.parent = gamePlayerModelHolderModel.transform;
                gameObjectLoad.transform.localScale = Vector3.one;
                gameObjectLoad.transform.position = Vector3.zero;
                gameObjectLoad.transform.localPosition = Vector3.zero;
                gameObjectLoad.transform.rotation = gamePlayerModelHolderModel.transform.rotation;
                gameObjectLoad.transform.localRotation = gamePlayerHolder.transform.localRotation;

                initialScale = transform.localScale;

                // Wire up collision object

                GamePlayerCollision gamePlayerCollision;

                if(gameObjectLoad.Has<GamePlayerCollision>()) {
                    gamePlayerCollision = gameObjectLoad.Get<GamePlayerCollision>();

                    if(IsPlayerControlled) {
                        gamePlayerCollision.tag = "Player";
                        tag = "Player";
                    }
                    else if(IsSidekickControlled) {
                        gamePlayerCollision.tag = "Sidekick";
                        tag = "Sidekick";
                    }
                    else {
                        gamePlayerCollision.tag = "Enemy";
                        tag = "Enemy";
                    }

                    gamePlayerCollision.UpdateGameObjects();
                }

                //Debug.Log("LoadCharacterCo:" + " gameObjectLoad:" + gameObjectLoad.name);

                // load items

                foreach(GamePlayerObjectItem objectItem
                        in gameObjectLoad.GetComponentsInChildren<GamePlayerObjectItem>(true)) {

                    if(IsPlayerControlled) {
                        objectItem.gameObject.Show();
                    }
                    else {
                        objectItem.gameObject.Hide();
                    }
                }

                //LogUtil.Log("LoadCharacter:create game object:gameObjectLoad.name:" + gameObjectLoad.name);

                foreach(Transform t in gameObjectLoad.transform) {
                    //t.localRotation = gamePlayerModelHolderModel.transform.rotation;

                    // TODO config

                    if(IsSidekickControlled || IsPlayerControlled) {
                        GamePlayerIndicator.AddIndicator(GameHUD.Instance.containerOffscreenIndicators,
                            t.gameObject, "friend1");
                    }
                    else {
                        GamePlayerIndicator.AddIndicator(GameHUD.Instance.containerOffscreenIndicators,
                            t.gameObject, "bot1");
                    }
                    break;

                }

                yield return new WaitForEndOfFrame();
            }
        }

        yield return StartCoroutine(ChangePlayerStateCo(controllerState));

        LoadWorldIndicator();

        LoadWeapons();

        currentControllerData.loadingCharacter = false;

        ResetPosition();

        currentControllerData.initialized = true;

        if(IsPlayerControlled) {
            GetPlayerProgress();
            currentControllerData.lastPlayerEffectsTrailUpdate = 0;
            HandlePlayerEffectsTick();
        }

        LoadEnterExitState();
    }

    // ------------------------------------------------------------------------
    // LOADING

    public virtual void LoadEnterExitState() {

        GamePlayerModelHolderEaseStartByType();
    }

    public virtual void LoadPlayerReadyState() {
        //if(IsPlayerControlled) {

        UpdateCharacterRuntimeState();

        GamePlayerModelHolderEaseReadyByType();
        //}
    }

    public void LoadWorldIndicator() {

        // Visible elements after model loaded

        ShowPlayerShadow();
        HidePlayerSpawner();

        if(!IsPlayerControlled) {
            if(gamePlayerShadow != null) {

                Color colorTo = UIColors.colorRed;
                colorTo.a = .3f;

                gamePlayerShadow.SetParticleSystemStartColor(colorTo, true);
            }

            if(FPSDisplay.isUnder30FPS) {
                if(gamePlayerEffectMarker != null) {
                    gamePlayerEffectMarker.Hide();
                }
            }
        }
    }

    // ------------------------------------------------------------------------
    // WEAPONS

    public List<string> weaponInventory;
    public int weaponInventoryIndex = 0;

    public virtual void LoadInventory() {

        ////LogUtil.Log("LoadInventory");

        if(weaponInventory == null) {
            weaponInventory = new List<string>();
        }

        weaponInventory.Clear();

        foreach(GameWeapon weapon in GameWeapons.Instance.GetAll()) {
            if(weapon.active) {
                weaponInventory.Add(weapon.code);
            }
        }

        // TODO load from data
        SetItemsData(new GamePlayerItemsData());
    }

    public virtual void UnloadWeapons() {
        if(gamePlayerModelHolderWeaponsHolder != null) {
            gamePlayerModelHolderWeaponsHolder.DestroyChildren();
        }

        if(weapons == null) {
            weapons = new Dictionary<string, GamePlayerWeapon>();
        }

        weapons.Clear();
    }

    public virtual void LoadWeapons() {

        foreach(GameObjectMountWeaponHolder holder in
                gamePlayerModelHolderModel.GetComponentsInChildren<GameObjectMountWeaponHolder>(true)) {
            gamePlayerModelHolderWeaponsHolder = holder.gameObject;
            gamePlayerModelHolderWeapons = gamePlayerModelHolderWeaponsHolder.transform.parent.gameObject;
        }

        //LogUtil.Log("LoadWeapons");
        if(gamePlayerModelHolderWeaponsHolder != null) {
            // check if character or vehicle has holder for weapons placement

            initialGamePlayerWeaponContainer = gamePlayerModelHolderWeaponsHolder.transform.position;
            currentGamePlayerWeaponContainer = gamePlayerModelHolderWeaponsHolder.transform.position;

            LoadInventory();

            if(weaponInventory != null
                && weaponInventory.Count != 0) {
                LoadWeapon(weaponInventory[weaponInventoryIndex]);
            }
        }
    }

    public virtual void LoadWeaponNext() {
        LoadWeapon(weaponInventoryIndex + 1);
    }

    public virtual void LoadWeaponPrevious() {
        LoadWeapon(weaponInventoryIndex - 1);
    }

    public virtual void LoadWeapon(int index) {
        if(index < 0) {
            index = weaponInventory.Count - 1;
        }
        else if(index > weaponInventory.Count - 1) {
            index = 0;
        }

        weaponInventoryIndex = index;
        LoadWeapon(weaponInventory[weaponInventoryIndex]);
    }

    public virtual void LoadWeapon(string code) {

        UnloadWeapons();

        if(!IsPlayerControlled) {
            return; // TODO enemy weapons
        }

        GameWeapon gameWeaponData = GameWeapons.Instance.GetByCode(code);

        if(gameWeaponData == null) {
            LogUtil.LogWarning("LoadWeapon: NULL gameWeaponData");
            return;
        }

        if(gameWeaponData.data == null) {
            LogUtil.LogWarning("LoadWeapon: NULL gameWeaponData.data");
            return;
        }

        GameDataModel dataModel = gameWeaponData.data.GetModel();

        if(dataModel == null) {
            LogUtil.LogWarning("LoadWeapon: NULL dataModel");
            return;
        }

        GameObject go = AppContentAssets.LoadAsset("weapon", dataModel.code);

        if(go == null) {
            return;
        }

        go.transform.parent = gamePlayerModelHolderWeaponsHolder.transform;
        go.ResetPosition();
        go.ResetRotation();

        if(GameConfigs.isGameRunning && IsPlayerControlled) {
            UINotificationDisplayTip.Instance.QueueTip(
                "Weapon Loaded: " + gameWeaponData.display_name,
                gameWeaponData.description, true);

        }

        if(go != null && weapons.Count == 0) {

            foreach(GamePlayerWeapon weapon in
                    gamePlayerModelHolderWeaponsHolder.GetComponentsInChildren<GamePlayerWeapon>()) {

                weapon.gameWeaponData = gameWeaponData;

                LogUtil.Log("LoadWeapon:weapon.name:" + weapon.name);

                weapons.Add(GamePlayerSlots.slotPrimary, weapon);
                weaponPrimary = weapon;
                break;
            }
        }
    }

    // ------------------------------------------------------------------------
    // OBJECTS IN SPHERE

    public virtual T FindNearest<T>(string codeLike, float distanceRange = 10f) where T : Component {

        T nearest = default(T);
        bool found = false;
        float shortestDistance = distanceRange * 2;

        foreach(Collider collide in Physics.OverlapSphere(transform.position, distanceRange)) {

            Transform t = collide.transform;

            if(t != null) {

                if(t.name.ToLower().Contains(codeLike)) {

                    if(!t.gameObject.Has<T>()) {
                        continue;
                    }

                    T nearObject = t.gameObject.Get<T>();

                    if(nearObject != null) {

                        float currentDistance = Vector3.Distance(
                                                    transform.position,
                                                    nearObject.transform.position);

                        if(currentDistance < shortestDistance) {
                            found = true;
                            shortestDistance = currentDistance;
                            nearest = nearObject;
                        }
                    }
                }
            }
        }

        if(found && nearest != null) {
            return nearest;
        }

        return default(T);
    }

    // ------------------------------------------------------------------------
    // MOUNT VEHICLE

    public virtual void SetControllersState(bool running) {

        if(currentControllerData.characterController == null) {
            return;
        }

        currentControllerData.characterController.enabled = running;
    }

    public virtual void MountNearest() {

        if(currentControllerData.mountData == null) {
            return;
        }

        if(currentControllerData.mountData.isMountedVehicle) {
            Unmount();
        }
        else {
            MountNearest<GameObjectMountVehicle>();
        }
    }

    public virtual void MountNearest<T>() where T : Component {

        //List<T> mounts = new List<T>();
        T nearest = default(T);
        bool found = false;
        float mountRange = 10f;
        float shortestDistance = mountRange * 2;

        foreach(Collider collide in Physics.OverlapSphere(transform.position, mountRange)) {

            Transform t = collide.transform;

            if(t != null) {

                if(t.name.ToLower().Contains("mount")) {

                    T mountObject = t.gameObject.Get<T>();

                    if(mountObject != null) {

                        float currentDistance = Vector3.Distance(
                                                    transform.position,
                                                    mountObject.transform.position);

                        if(currentDistance < shortestDistance) {
                            found = true;
                            shortestDistance = currentDistance;
                            nearest = mountObject;
                        }
                    }

                }
            }
        }

        if(found && nearest != null) {
            Mount(nearest.gameObject);
        }
    }

    public virtual void Mount(GameObject go) {
        if(go.Has<GameObjectMountVehicle>()) {        // MOUNT VEHICLES     
            if(!currentControllerData.mountData.isMountedVehicleObject) {
                currentControllerData.mountData.MountVehicle(gameObject,
                    go.Get<GameObjectMountVehicle>());

                if(currentControllerData.gameModelVisible) {
                    gamePlayerModelHolderModel.Hide();
                    currentControllerData.gameModelVisible = false;
                }

                GameObjectMountWeaponHolder weaponHolder = currentControllerData.mountData.mountVehicle.GetWeaponHolder();

                if(weaponHolder != null) {
                    currentGamePlayerWeaponContainer = weaponHolder.transform.position;
                    //gamePlayerModelHolderWeaponsHolder.transform.position = currentGamePlayerWeaponContainer;

                    gamePlayerModelHolderWeaponsHolder.transform.parent = weaponHolder.transform;
                    gamePlayerModelHolderWeaponsHolder.transform.position = weaponHolder.transform.position;
                    gamePlayerModelHolderWeaponsHolder.transform.rotation = weaponHolder.transform.rotation;
                }

                SetControllersState(false);

                StopNavAgent();
            }
        }
    }

    public virtual void Unmount() {
        if(currentControllerData.mountData.isMountedVehicleObject) {
            currentControllerData.mountData.UnmountVehicle();

            if(!currentControllerData.gameModelVisible) {
                gamePlayerModelHolderModel.Show();
                currentControllerData.gameModelVisible = true;
            }

            //gamePlayerModelHolderWeaponsHolder.transform.position = initialGamePlayerWeaponContainer;

            gamePlayerModelHolderWeaponsHolder.transform.parent = gamePlayerModelHolderWeapons.transform;
            gamePlayerModelHolderWeaponsHolder.transform.localPosition = Vector3.zero;
            gamePlayerModelHolderWeaponsHolder.transform.rotation = gamePlayerModelHolderWeapons.transform.rotation;

            SetControllersState(true);

            StartNavAgent();
        }
    }

    // ------------------------------------------------------------------------
    // CONTROLLER

    public bool controllerReady {
        get {

            if(!GameController.shouldRunGame) {
                return false;
            }

            if(!GameConfigs.isGameRunning) {
                return false;
            }

            if(controllerData == null) {
                return false;
            }

            //return true;

            //if(!gameObject.activeSelf && !gameObject.activeInHierarchy) {
            //return false;
            //}

            if(controllerData != null) {
                if(!currentControllerData.loadingCharacter) {
                    return true;
                }
            }

            return false;
        }
    }

    public virtual bool AllowControllerInteraction(
        GamePlayerController otherGamePlayer) {

        if(controllerData == null) {
            return false;
        }

        if(controllerData.gamePlayerController == null
            || otherGamePlayer == null) {
            return false;
        }

        return AllowControllerInteraction(controllerData.gamePlayerController, otherGamePlayer);
    }

    public virtual bool AllowControllerInteraction(
        GamePlayerController currentGamePlayer,
        GamePlayerController otherGamePlayer) {

        if(currentGamePlayer == null || otherGamePlayer == null) {
            return false;
        }

        if((currentGamePlayer.IsPlayerControlled && otherGamePlayer.IsSidekickControlled)
            || (otherGamePlayer.IsPlayerControlled && currentGamePlayer.IsSidekickControlled)) {
            return false;
        }

        if(currentGamePlayer.IsSidekickControlled && otherGamePlayer.IsSidekickControlled) {
            return false;
        }

        if(currentGamePlayer.isDead || otherGamePlayer.isDead) {
            return false;
        }

        if(currentGamePlayer.isExiting || otherGamePlayer.isExiting) {
            return false;
        }

        if(currentGamePlayer.isEntering || otherGamePlayer.isEntering) {
            return false;
        }

        return true;
    }

    // ------------------------------------------------------------------------
    // COLLISIONS/TRIGGERS

    string nameGameObstacle = "GameObstacle";
    string nameGameDamageObstacle = "GameDamageObstacle";
    string nameGameItemObject = "GameItemObject";
    string nameGamePlayerCollider = "GamePlayerCollider";
    string nameGameColliderObstacle = "GameColliderObstacle";

    public virtual void HandleCollision(Collision collision) {

        if(!controllerReady) {
            return;
        }

        if(collision.contacts.Length > 0) {
            foreach(ContactPoint contact in collision.contacts) {
                //Debug.DrawRay(contact.point, contact.normal, Color.white);

                Transform t = contact.otherCollider.transform;

                if(t.parent != null) {
                    string parentName = t.parent.name;

                    // TODO make name recursion by depth limit, for now check three above.
                    string parentParentName = "";
                    string parentParentParentName = "";

                    if(t.parent.parent != null) {
                        parentParentName = t.parent.parent.name;
                        if(t.parent.parent.parent != null) {
                            parentParentParentName = t.parent.parent.parent.name;
                        }
                    }

                    bool isGameColliderObstacle = parentName.Contains(nameGameColliderObstacle)
                                      || t.name.Contains(nameGameColliderObstacle);

                    if(isGameColliderObstacle) {
                        Physics.IgnoreCollision(contact.thisCollider, contact.otherCollider);
                        return;
                    }

                    bool isObstacle = parentName.Contains(nameGameObstacle)
                                      || t.name.Contains(nameGameObstacle);

                    bool isDamageObstacle = parentName.Contains(nameGameDamageObstacle)
                                      || t.name.Contains(nameGameDamageObstacle);


                    bool isLevelObject = parentName.Contains(nameGameItemObject)
                                         || parentParentName.Contains(nameGameItemObject)
                                         || parentParentParentName.Contains(nameGameItemObject)
                                         || t.name.Contains(nameGameItemObject);

                    bool isPlayerObject =
                        t.name.Contains(nameGamePlayerCollider);
                    //|| t.name.Contains("GamePlayerObject");

                    if(!isObstacle && !isLevelObject && !isPlayerObject && !isDamageObstacle) {
                        Physics.IgnoreCollision(contact.thisCollider, contact.otherCollider);
                    }
                    else {

                        if(currentControllerData.lastCollision + currentControllerData.intervalCollision < Time.time) {
                            currentControllerData.lastCollision = Time.time;
                        }
                        else {
                            //    return;
                        }
                    }

                    if(isLevelObject) {
                        GameLevelSprite sprite = t.gameObject.FindTypeAboveRecursive<GameLevelSprite>();
                        if(sprite == null) {
                            sprite = t.parent.gameObject.GetComponentInChildren<GameLevelSprite>();
                        }
                        if(sprite != null) {
                            isLevelObject = true;
                        }
                        else {
                            isLevelObject = false;
                        }
                    }

                    if(isDamageObstacle) {

                        if(IsPlayerControlled) {
                            // If stationary aff move back

                            //  if(GameController.IsGameplayWorldTypeStationary()) {

                            float power = .1f;
                            runtimeData.health -= power;

                            Hit(power);

                            //controllerData.currentGamePlayerPosition = t.position.WithZ(16);

                            //GamePlayerBounceSet(250);

                            ////Debug.Log("isDamageObstacle:" + isDamageObstacle);
                        }
                    }

                    if(isObstacle || isLevelObject) {
                        if(IsPlayerControlled) {
                            AudioAttack();
                            ProgressScore(1);
                            GamePlayerProgress.SetStatHitsObstacles(1f);
                        }
                    }
                    else if(isDamageObstacle) {
                        if(IsPlayerControlled) {

                            /*
                                                        
                            float power = .35f;
                            runtimeData.health -= power;

                            //GamePlayerProgress.Instance.ProcessProgressSpins
                            //GameProfileCharacters.currentProgress.SubtractGamePlayerProgressHealth(power); // TODO get by skill upgrade
                            //GameProfileCharacters.currentProgress.SubtractGamePlayerProgressEnergy(power/2f); // TODO get by skill upgrade

                            Vector3 normal = contact.normal;
                            float magnitude = contact.point.sqrMagnitude;

                            // TODO config

                            float hitPower = (magnitude * (float)runtimeData.mass) / 50;

                            //LogUtil.Log("hitPower:" + hitPower);

                            // TODO config

                            AddImpact(normal, Mathf.Clamp(hitPower, 0f, 80f));

                            // we hit an enemy, so we are the player
                            GamePlayerProgress.SetStatHits(1f);
                            Hit(power);
                            */
                        }

                    }
                    else if(isPlayerObject) {

                        if(IsAgentControlled) {
                            ////Debug.Log("HandleCollision:Agent");
                        }
                        else {
                        }

                        // handle stat

                        currentControllerData.collisionController = GameController.GetGamePlayerControllerObject(
                            t.gameObject, false);

                        if(currentControllerData.collisionController != null) {

                            if(!currentControllerData.collisionController.controllerReady) {
                                //break;
                            }

                            // make sure it isn't own colliders or a friend
                            if(currentControllerData.collisionController.uniqueId == uniqueId
                                || !AllowControllerInteraction(currentControllerData.collisionController)) {
                                // It is me, leave it be.
                                break;
                            }

                            // handle hit

                            // TODO config

                            float power = .1f;
                            runtimeData.health -= power;

                            //GamePlayerProgress.Instance.ProcessProgressSpins
                            //GameProfileCharacters.currentProgress.SubtractGamePlayerProgressHealth(power); // TODO get by skill upgrade
                            //GameProfileCharacters.currentProgress.SubtractGamePlayerProgressEnergy(power/2f); // TODO get by skill upgrade

                            Vector3 normal = contact.normal;
                            float magnitude = contact.point.sqrMagnitude;

                            // TODO config

                            float hitPower = (magnitude * (float)runtimeData.mass) / 110;

                            //LogUtil.Log("hitPower:" + hitPower);

                            // TODO config

                            AddImpact(normal, Mathf.Clamp(hitPower, 0f, 80f));

                            if(IsPlayerControlled) {
                                // we hit an enemy, so we are the player
                                GamePlayerProgress.SetStatHits(1f);
                                Hit(power);

                            }
                            else {
                                Hit(power);
                                // we hit a player, so we are an enemy
                                GamePlayerProgress.SetStatHitsReceived(1f);
                            }
                        }
                    }
                }
                //break;
            }
        }

        //if (collision.relativeVelocity.magnitude > 2)
        //    audio.Play();      
    }

    // ------------------------------------------------------------------------
    // TRIGGERS PARTICLES

    public virtual void OnParticleCollision(GameObject other) {

        if(!controllerReady) {
            return;
        }

        if(other.name.Contains("projectile-")) {

            if(lastCollision + intervalCollision < Time.time) {
                lastCollision = Time.time;
            }
            else {
                return;
            }

            if(gameDamageManager == null) {
                gameDamageManager = GetComponentInChildren<GameDamageManager>();
            }

            if(gameDamageManager != null) {
                // todo lookup projectile and power to subtract.                    
                float projectilePower = 3;
                gameDamageManager.ApplyDamage(projectilePower);
            }
        }
    }

    // ------------------------------------------------------------------------
    // TRIGGERS OBJECTS

    public virtual void OnTriggerEnter(Collider collider) {

        if(!controllerReady) {
            return;
        }

        HandleZones(
            GameActionTriggerState.TriggerEnter, collider.gameObject);
    }

    public virtual void OnTriggerExit(Collider collider) {

        if(!controllerReady) {
            return;
        }

        HandleZones(
            GameActionTriggerState.TriggerExit, collider.gameObject);
    }

    // ------------------------------------------------------------------------
    // ZONES HANDLING

    public virtual void HandleZones(
        GameActionTriggerState triggerState, GameObject go) {

        if(go == null) {
            return;
        }

        string goName = go.name;

        // IF WITHIN ACTION RANGE TRIGGER

        if(goName.Contains(GameActionKeys.GameZoneActionTrigger)) {
            //Debug.Log(GameActionKeys.GameZoneActionTrigger + ":" + goName);

            HandleZonesActionsController(triggerState, go, goName);
        }
        else if(goName.Contains(GameActionKeys.GameZoneActionArea)) {
            //Debug.Log(GameActionKeys.GameZoneActionArea + ":" + goName);

            if(triggerState == GameActionTriggerState.TriggerExit) {
                triggerState = GameActionTriggerState.ActionTriggerExit;
            }
            else {
                triggerState = GameActionTriggerState.ActionTriggerEnter;
            }

            HandleZonesActionsController(triggerState, go, goName);
        }
        else {
            HandleZonesController(triggerState, go, goName);
        }
    }

    // GOALS

    public virtual void HandleZonesController(
        GameActionTriggerState triggerState, GameObject go, string goName) {

        // GENERIC OR GOAL ZONES separate from actions

        if(IsPlayerControlled) {

            // GOALS

            if(goName.Contains(GameActionKeys.GameGoalZone)) {
                LogUtil.Log(GameActionKeys.GameGoalZone + ":" + goName);
                GameController.GamePlayerGoalZone(go);
            }
            else if(goName.Contains(GameActionKeys.GameBadZone)) {
                LogUtil.Log(GameActionKeys.GameBadZone + ":" + goName);
                GameController.GamePlayerOutOfBounds();
            }
            else if(goName.Contains(GameActionKeys.GameBoundaryZone)) {
                LogUtil.Log(GameActionKeys.GameBoundaryZone + ":" + goName);
                // Nothing it is a wall...
            }
            else if(goName.Contains(GameActionKeys.GameZone)) {
                LogUtil.Log(GameActionKeys.GameZone + ":" + goName);
                GameController.GamePlayerGoalZoneCountdown(go);
            }
        }
    }

    // ACTIONS

    public virtual void HandleZonesActionsController(
        GameActionTriggerState triggerState, GameObject go, string goName) {

        GameZoneAction actionItem =
            go.transform.GetComponentInParent<GameZoneAction>();

        if(actionItem == null) {
            return;
        }

        //Debug.Log("HandleZonesActionsController: actionCode:" + actionItem.actionCode);

        GameZoneActionAsset actionTypeItem =
            go.transform.GetComponentInParent<GameZoneActionAsset>();

        if(actionTypeItem == null) {
            return;
        }

        // CHECK action type

        string actionCode = actionItem.actionCode;

        // GET ACTION CODE

        if(IsPlayerControlled) {

            // ACTIONS
            // TRIGGER ACTION ENTER

            // TRIGGER ENTER

            if(triggerState == GameActionTriggerState.TriggerEnter) {

                // SAVE

                // BUILD

                if(actionTypeItem.isActionCodeBuild) {
                    actionTypeItem.ChangeStateCreating();
                }

                // REPAIR

                if(actionTypeItem.isActionCodeRepair) {
                    actionTypeItem.ChangeStateCreating();
                }

                // ATTACK

                // DEFEND
            }

            // TRIGGER EXIT

            if(triggerState == GameActionTriggerState.TriggerExit) {

                // SAVE

                // BUILD

                if(actionTypeItem.isActionCodeBuild) {
                    actionTypeItem.ChangeStateNone();
                }

                // REPAIR

                if(actionTypeItem.isActionCodeRepair) {
                    actionTypeItem.ChangeStateNone();
                }

                // ATTACK

            }

        }

        if(IsSidekickControlled) {

            // TRIGGER

            if(triggerState == GameActionTriggerState.TriggerEnter) {

                if(actionTypeItem.isActionCodeSave) {
                    SetNavAgentDestination(go);
                }
            }
            else if(triggerState == GameActionTriggerState.TriggerExit) {

                if(actionTypeItem.isActionCodeSave) {
                    SetNavAgentDestination(
                        GameController.CurrentGamePlayerController.gameObject);
                }
            }

            // ACTION TRIGGER

            if(triggerState == GameActionTriggerState.ActionTriggerEnter) {

                if(actionTypeItem.isActionCodeSave) {

                    AppContentCollect appContentCollect =
                        AppContentCollects.GetByTypeAndCode(BaseDataObjectKeys.action, actionCode);

                    if(appContentCollect != null) {

                        //Debug.Log(GameActionKeys.GameZoneActionArea + ":" +
                        //"appContentCollect:" +
                        //appContentCollect.code);

                        ExitPlayer();
                    }
                }
            }
        }

        if(IsAgentControlled) {

        }

    }

    // ------------------------------------------------------------------------
    // ANIMATION

    public virtual void AnimatePlayer(string animationName) {
        if(animationName == GameDataActionKeys.skill) {
            InputSkill();
        }
        else if(animationName == GameDataActionKeys.attack) {
            InputAttack();
        }
        else if(animationName == GameDataActionKeys.attack_alt) {
            InputAttackAlt();
        }
        else if(animationName == GameDataActionKeys.attack_right) {
            InputAttackRight();
        }
        else if(animationName == GameDataActionKeys.attack_left) {
            InputAttackLeft();
        }
        else if(animationName == GameDataActionKeys.defend) {
            InputDefend();
        }
        else if(animationName == GameDataActionKeys.defend_alt) {
            InputDefendAlt();
        }
        else if(animationName == GameDataActionKeys.defend_right) {
            InputDefendRight();
        }
        else if(animationName == GameDataActionKeys.defend_left) {
            InputDefendLeft();
        }
        else if(animationName == GameDataActionKeys.death) {
            InputDie();
        }
        else if(animationName == GameDataActionKeys.jump) {
            InputJump();
        }
        else if(animationName == GameDataActionKeys.strafe_left) {
            InputStrafeLeft();
        }
        else if(animationName == GameDataActionKeys.strafe_right) {
            InputStrafeRight();
        }
        else if(animationName == GameDataActionKeys.use) {
            InputUse();
        }
        else if(animationName == GameDataActionKeys.mount) {
            InputMount();
        }
        else if(animationName == GameDataActionKeys.slide) {
            InputSlide();
        }
    }

    public virtual void OnPlayerAnimation(string animationName, string uniqueId) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        LogUtil.Log("OnPlayerAnimation: " + animationName);

        /*
         if(Network.isClient || Network.isServer) {
     
             // call them over the network
         
             if(IsPlayerControlled) {
                 Gameverses.GameNetworkingAction actionEvent = new Gameverses.GameNetworkingAction();
                 actionEvent.uuid = UniqueUtil.CreateUUID4();
                 actionEvent.uuidOwner = uuid;
                 actionEvent.code = animationName;
                 actionEvent.type = Gameverses.GameNetworkingPlayerTypeMessages.PlayerTypeAction;             
             
                 //Gameverses.GameversesGameAPI.Instance.SendActionMessage(actionEvent, Vector3.zero, Vector3.zero);
             }
         }
         else  {
     */
        if(IsPlayerControlled) {
            if(currentControllerData.gamePlayerControllerAnimation != null) {
                AnimatePlayer(animationName);
            }
        }
        //}
    }

    // ------------------------------------------------------------------------
    // STATE/RESET

    public virtual void ResetPositionAir(float y) {

        //if(IsPlayerControlled) {

        //if (GameController.IsGameplayType(GameplayType.gameRunner)) {
        //    return;
        //}

        if(currentControllerData.lastAirCheck > 1f) {
            currentControllerData.lastAirCheck = 0;

            gameObject.transform.position = Vector3.Lerp(
                gameObject.transform.position,
                gameObject.transform.position.WithY(y),
                1 * Time.deltaTime);
        }

        //}
    }

    public virtual void ResetPosition() {

        foreach(Transform t in gamePlayerModelHolderModel.transform) {
            t.position.Reset();
            t.localPosition.Reset();
            t.rotation.Reset();
            t.localRotation.Reset();
            t.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if(IsPlayerControlled) {
            if(GameController.IsGameplayTypeRunner()) {
                //return;
            }
            gameObject.ResetPosition();
        }
    }

    public virtual void ResetScale() {

        foreach(Transform t in gamePlayerModelHolderModel.transform) {
            t.localScale = Vector3.one;
        }

        if(IsPlayerControlled) {
            gameObject.ResetScale(1f);
        }
    }

    public virtual void SetUp(
        GamePlayerControllerState controllerStateTo,
        GamePlayerContextState contextStateTo) {

        //if (controllerState != controllerStateTo 
        //    || controllerState == GamePlayerControllerState.ControllerNotSet) {

        controllerState = controllerStateTo;
        contextState = contextStateTo;

        //}

        Reset();
    }

    public virtual void Reset() {

        if(IsPlayerControlled) {
            uniqueId = UniqueUtil.Instance.currentUniqueId;
        }
        else {
            uniqueId = UniqueUtil.CreateUUID4();
        }

        ResetPosition();
    }

    public virtual void Remove() {
        if(!IsPlayerControlled) {

            foreach(Transform t in gamePlayerModelHolderModel.transform) {
                GameObjectHelper.DestroyGameObject(t.gameObject, GameConfigs.usePooledGamePlayers);
            }

            GameObjectHelper.DestroyGameObject(gameObject, GameConfigs.usePooledGamePlayers);
        }
    }

    // ------------------------------------------------------------------------
    // COMBAT/HIT/ATTACK

    //MountNearest

    public virtual void SendMount() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.mount,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendUse() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.use,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendJump() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.jump,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendStrafeLeft() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.strafe_left,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendStrafeRight() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.strafe_right,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendBoost() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.boost,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendSkill() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.skill,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendAttack() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.attack,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendAttackAlt() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.attack_alt,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendAttackRight() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.attack_right,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendAttackLeft() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.attack_left,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendDefend() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.defend,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendDefendAlt() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.defend_alt,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendDefendRight() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.defend_right,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendDefendLeft() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GameDataActionKeys.defend_left,
            UniqueUtil.Instance.currentUniqueId);
    }

    // ------------------------------------------------------------------------
    // ANIMATIONS/TYPES

    public virtual void InputAttack() {
        Attack();
    }

    public virtual void InputAttackAlt() {
        AttackAlt();
    }

    public virtual void InputAttackLeft() {
        AttackLeft();
    }

    public virtual void InputAttackRight() {
        AttackRight();
    }

    public virtual void InputDefend() {
        //Defend();
    }

    public virtual void InputDefendAlt() {
        //DefendAlt();
    }

    public virtual void InputDefendLeft() {
        //DefendLeft();
    }

    public virtual void InputDefendRight() {
        //DefendRight();
    }

    public virtual void InputJump() {
        Jump();
    }

    public virtual void InputMove(Vector3 amount) {
        Move(amount);
    }

    public virtual void InputBoost() {
        Boost();
    }

    public virtual void InputUse() {
        // USE

    }

    public virtual void InputMount() {
        // MOUNT
        MountNearest();
    }

    public virtual void InputSkill() {
        Skill();
    }

    public virtual void InputMagic() {
        //Magic();
    }

    public virtual void InputSlide() {
        InputSlide(Vector3.zero.WithZ(3f));
    }

    public virtual void InputSlide(Vector3 amount) {
        Slide(amount);
    }

    public virtual void InputStrafe(Vector3 amount) {
        Strafe(amount);
    }

    public virtual void InputMove(Vector3 amount, Vector3 rangeStart, Vector3 rangeEnd, bool append = true) {
        Move(amount, rangeStart, rangeEnd, append);
    }

    public virtual void InputStrafeLeft() {
        StrafeLeft();
    }

    public virtual void InputStrafeRight() {
        StrafeRight();
    }

    public virtual void InputDie() {
        Die();
    }

    public virtual void InputSpin() {
        //Skill();
        // Straightarm
    }

    // ------------------------------------------------------------------------
    // HIT

    public virtual void Hit(float power) {
        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(IsPlayerControlled) {
            GameHUD.Instance.ShowHitOne((float)(1.5 - runtimeData.health));
            ProgressScore(2 * power);
            DeviceUtil.Vibrate();
        }
        else {
            //bool allow = false;

            if(currentControllerData.lastHit + .3f < Time.time) {
                currentControllerData.lastHit = Time.time;
                //allow = true;
            }
            else {
                return;
            }

            ProgressScore(-1);
        }

        if(GameController.IsGameplayTypeRunner()) {
            currentControllerData.moveGamePlayerPosition =
                currentControllerData.moveGamePlayerPosition.WithZ(-.15f);
        }

        //GameUIPanelOverlays.Instance.ShowOverlayWhiteFlash();
        //GameHUD.Instance.ShowHitOne();

        //if(controllerState == GamePlayerControllerState.ControllerAgent) {
        //    power = power * 1;
        //}
        //else if(IsPlayerControlled) {
        //    power = power * 1;
        //}

        double powerCharacter = 1f;

        if(gameCharacter.data.HasRPGs()) {
            if(controllerData.characterRPG == null) {
                controllerData.characterRPG = gameCharacter.data.GetRPG();
            }
            powerCharacter = 1 / controllerData.characterRPG.power;
        }

        runtimeData.health -= power * (float)powerCharacter;

        PlayEffectHit();

        AudioHit();

        if(runtimeData.health < 0) {
            Die();
        }
    }

    // ------------------------------------------------------------------------
    // IDLE

    public virtual void Idle() {
        if(isDead) {
            return;
        }

        //currentControllerData.thirdPersonController.Idle();

        if(currentControllerData.gamePlayerControllerAnimation != null) {
            currentControllerData.gamePlayerControllerAnimation.Idle();
        }
    }

    // ------------------------------------------------------------------------
    // JUMP

    public virtual void Jump() {
        Jump(.5f);
    }

    public virtual void Jump(float duration) {
        if(isDead) {
            return;
        }

        if(currentControllerData.thirdPersonController != null) {
            currentControllerData.thirdPersonController.Jump(duration);
        }

        if(currentControllerData.gamePlayerControllerAnimation != null) {
            currentControllerData.gamePlayerControllerAnimation.Jump();
        }

        if(gamePlayerEffectSkill != null) {
            gamePlayerEffectSkill.Emit(1);
        }
    }

    public virtual void JumpStop() {
        if(isDead) {
            return;
        }

        if(currentControllerData.thirdPersonController != null) {
            currentControllerData.thirdPersonController.JumpStop();
        }
    }

    // ------------------------------------------------------------------------
    // SLIDE

    public virtual void Slide() {
        Slide(Vector3.zero.WithZ(-.1f));
    }

    public virtual void Slide(Vector3 amount) {
        if(isDead) {
            return;
        }

        //GamePlayerBounceSet(-100);

        if(currentControllerData.thirdPersonController != null) {

            if(currentControllerData.thirdPersonController.IsGrounded()) {

                //GamePlayerCollisionEnable(false);
                //GamePlayerCollisionEnableDelayed(true, 1f);

                //GamePlayerCollisionPosition(Vector3.zero.WithY(-0.5f));
                //GamePlayerCollisionPositionDelayed(Vector3.zero, 1f);

                GamePlayerCollisionScale(Vector3.one.WithY(0.70f));
                GamePlayerCollisionScaleDelayed(Vector3.one, 1f);

                currentControllerData.thirdPersonController.Slide(amount);

                currentControllerData.gamePlayerControllerAnimation.Slide();

                //Move(amount);

                currentControllerData.moveGamePlayerPosition += amount;
            }
        }

        if(gamePlayerEffectSlide != null) {
            gamePlayerEffectSlide.Emit(1);
        }
    }

    public virtual void SlideStop() {
        if(isDead) {
            return;
        }

        if(currentControllerData.thirdPersonController != null) {
            currentControllerData.thirdPersonController.SlideStop();
        }
    }


    // ------------------------------------------------------------------------
    // SPeedUp

    public virtual void SpeedUp() {
        SpeedUp(Vector3.zero.WithZ(.15f));
    }

    public virtual void SpeedUp(Vector3 amount) {
        if(isDead) {
            return;
        }

        //GamePlayerBounceSet(-100);

        if(currentControllerData.thirdPersonController != null) {

            currentControllerData.thirdPersonController.Slide(amount);

            currentControllerData.moveGamePlayerPosition += amount;
        }

        //if(gamePlayerEffectBoost != null) {
        //    gamePlayerEffectBoost.Emit(1);
        //}
    }

    // ------------------------------------------------------------------------
    // MOVE

    public virtual void Move() {
        Move(Vector3.zero.WithZ(-.5f));
    }

    public virtual void Move(Vector3 amount) {
        if(isDead) {
            return;
        }

        controllerData.moveGamePlayerPositionTo = amount;

        //if (currentControllerData.thirdPersonController != null) {
        //currentControllerData.gamePlayerControllerAnimation.Move();
        //}

        //if (gamePlayerEffectSlide != null) {
        //    gamePlayerEffectSlide.Emit(1);
        //}
    }

    // ------------------------------------------------------------------------
    // SKILL

    public virtual void Skill() {
        if(isDead) {
            return;
        }

        if(currentControllerData.thirdPersonController != null) {
            currentControllerData.gamePlayerControllerAnimation.Skill();
        }

        if(gamePlayerEffectSkill != null) {
            gamePlayerEffectSkill.Emit(1);
        }
    }

    // ------------------------------------------------------------------------
    // STRAFE

    // STRAFE LEFT

    public virtual void StrafeLeft() {
        Vector3 dir = transform.TransformPoint(transform.localPosition.WithX(-1));//Vector3.zero.WithX(-1);
        float power = 10f + 5f * (float)currentControllerData.runtimeRPGData.modifierAttack;
        Strafe(dir, power);
    }

    public virtual void StrafeLeft(float power) {
        Vector3 dir = transform.TransformPoint(transform.localPosition.WithX(-1));//Vector3.zero.WithX(-1);
        Strafe(dir, power);
    }

    // STRAFE RIGHT

    public virtual void StrafeRight() {
        Vector3 dir = transform.localPosition.WithX(1);
        float power = 10f + 5f * (float)currentControllerData.runtimeRPGData.modifierAttack;
        Strafe(dir, power);
    }

    public virtual void StrafeRight(float power) {
        Vector3 dir = transform.localPosition.WithX(1);
        Strafe(dir, power);
    }

    // STRAFE BASE

    public virtual void Strafe(Vector3 dir) {
        float power = 10f + 5f * (float)currentControllerData.runtimeRPGData.modifierAttack;
        Strafe(dir, power);
    }

    public virtual void Strafe(Vector3 dir, float power) {

        if(isDead) {
            return;
        }

        if(Time.time > currentControllerData.lastStrafeLeftTime + 1f) {

            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cuts, 1f);

            if(dir.x < 0) {
                //LogUtil.Log("GamePlayerController:StrafeLeft:");
                currentControllerData.gamePlayerControllerAnimation.StrafeLeft();
                GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cutsLeft, 1f);
                currentControllerData.lastStrafeLeftTime = Time.time;
            }

            StartCoroutine(StrafeCo(dir, power));
        }

        if(Time.time > currentControllerData.lastStrafeRightTime + 1f) {

            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cuts, 1f);

            if(dir.x > 0) {
                //LogUtil.Log("GamePlayerController:StrafeRight:");
                currentControllerData.gamePlayerControllerAnimation.StrafeRight();
                GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cutsRight, 1f);
                currentControllerData.lastStrafeRightTime = Time.time;
            }

            StartCoroutine(StrafeCo(dir, power));
        }
    }

    public virtual IEnumerator StrafeCo(Vector3 dir, float power) {
        AddForce(dir, power, false);
        yield return new WaitForEndOfFrame();
    }

    // ------------------------------------------------------------------------
    // MOVE BASE

    public virtual void Move(Vector3 dir, Vector3 rangeStart, Vector3 rangeEnd, bool append = true) {

        if(isDead) {
            return;
        }

        if(Time.time > currentControllerData.lastMoveLeftTime + .1f) {

            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cuts, 1f);

            if(dir.x < 0) {
                //LogUtil.Log("GamePlayerController:StrafeLeft:");
                currentControllerData.gamePlayerControllerAnimation.StrafeLeft();
                GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cutsLeft, 1f);
                currentControllerData.lastMoveLeftTime = Time.time;
            }

            StartCoroutine(MoveCo(dir, rangeStart, rangeEnd));
        }

        if(Time.time > currentControllerData.lastMoveRightTime + .1f) {

            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cuts, 1f);

            if(dir.x > 0) {
                //LogUtil.Log("GamePlayerController:StrafeRight:");
                currentControllerData.gamePlayerControllerAnimation.StrafeRight();
                GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cutsRight, 1f);
                currentControllerData.lastMoveRightTime = Time.time;
            }

            StartCoroutine(MoveCo(dir, rangeStart, rangeEnd));
        }
    }

    public virtual IEnumerator MoveCo(Vector3 dir, Vector3 rangeStart, Vector3 rangeEnd, bool append = true) {
        //AddForce(dir, power, false);

        if(append) {
            dir = gameObject.transform.localPosition + dir;
        }

        if(dir.x >= rangeEnd.x) {
            dir.x = rangeEnd.x;
        }

        if(dir.x <= rangeStart.x) {
            dir.x = rangeStart.x;
        }

        if(dir.y >= rangeEnd.y) {
            dir.y = rangeEnd.y;
        }

        if(dir.y <= rangeStart.y) {
            dir.y = rangeStart.y;
        }

        if(dir.z >= rangeEnd.z) {
            dir.z = rangeEnd.z;
        }

        if(dir.z <= rangeStart.z) {
            dir.z = rangeStart.z;
        }

        TweenUtil.MoveToObject(gameObject, dir, .3f, 0f, true, TweenCoord.local);
        yield return new WaitForEndOfFrame();
    }

    // ------------------------------------------------------------------------
    // BOOST

    public virtual void Boost() {
        Vector3 dir = transform.forward;
        float power = 10f + 5f * (float)currentControllerData.runtimeRPGData.modifierAttack;
        Boost(dir, power);
    }

    public virtual void Boost(Vector3 dir) {
        float power = 10f + 5f * (float)currentControllerData.runtimeRPGData.modifierAttack;
        Boost(dir, power);
    }

    public virtual void Boost(float power) {
        Vector3 dir = transform.forward;
        Boost(dir, power);
    }

    public virtual void Boost(Vector3 dir, float power, bool allowY = false) {
        if(isDead) {
            return;
        }
        //LogUtil.Log("GamePlayerController:Boost:");
        if(Time.time > currentControllerData.lastBoostTime + 1f) {
            currentControllerData.lastBoostTime = Time.time;

            currentControllerData.gamePlayerControllerAnimation.Boost();
            GamePlayerProgress.SetStatBoosts(1f);
            StartCoroutine(BoostCo(dir, power, allowY));
        }
    }

    public virtual IEnumerator BoostCo(Vector3 dir, float power, bool allowY = false) {
        AddForce(dir, power, false, allowY);
        yield return new WaitForEndOfFrame();
    }

    // ------------------------------------------------------------------------
    // SPIN

    public virtual void Spin() {
        Vector3 dir = transform.localPosition.WithZ(-1);
        float power = 10f + 5f * (float)currentControllerData.runtimeRPGData.modifierAttack;
        Boost(dir, power);
    }

    public virtual void Spin(Vector3 dir) {
        float power = 10f + 5f * (float)currentControllerData.runtimeRPGData.modifierAttack;
        Boost(dir, power);
    }

    public virtual void Spin(float power) {
        Vector3 dir = transform.localPosition.WithZ(-1);
        Boost(dir, power);
    }

    public virtual void Spin(Vector3 dir, float power) {
        if(isDead) {
            return;
        }
        //LogUtil.Log("GamePlayerController:Spin:");
        if(Time.time > currentControllerData.lastSpinTime + 1f) {
            currentControllerData.lastSpinTime = Time.time;

            //UITweenerUtil.RotateTo(gamePlayerModelHolder, UITweener.Method.Linear, UITweener.Style.Once, .2f, 0f, Vector3.zero.WithY(179));
            //UITweenerUtil.RotateTo(gamePlayerModelHolder, UITweener.Method.Linear, UITweener.Style.Once, .2f, .2f, Vector3.zero.WithY(290));
            //UITweenerUtil.RotateTo(gamePlayerModelHolder, UITweener.Method.Linear, UITweener.Style.Once, .2f, .4f, Vector3.zero.WithY(0));

            currentControllerData.playerSpin = true;

            //iTween.RotateTo(gamePlayerModelHolderModel, iTween.Hash("y", Vector3.zero.WithY(179).y, "time", .2f, "delay", 0f, "easetype", "linear", "space", "local"));
            //iTween.RotateTo(gamePlayerModelHolderModel, iTween.Hash("y", Vector3.zero.WithY(290).y, "time", .2f, "delay", .21f, "easetype", "linear", "space", "local"));
            //iTween.RotateTo(gamePlayerModelHolderModel, iTween.Hash("y", Vector3.zero.WithY(0).y, "time", .2f, "delay", .41f, "easetype", "linear", "space", "local"));

            currentControllerData.gamePlayerControllerAnimation.Spin();
            GamePlayerProgress.SetStatSpins(1f);

            StartCoroutine(SpinCo(dir, power));
        }
    }

    public virtual IEnumerator SpinCo(Vector3 dir, float power) {
        AddForce(dir, power, false);
        yield return new WaitForEndOfFrame();
    }

    // ------------------------------------------------------------------------
    // DIE

    public virtual void Die() {
        Vector3 dir = Vector3.zero.WithZ(1);
        float power = 50f;
        Die(dir, power);
    }

    public virtual void Die(Vector3 dir) {
        float power = 10f;
        Die(dir, power);
    }

    public virtual void Die(float power) {
        Vector3 dir = Vector3.zero.WithZ(1);
        Die(dir, power);
    }

    public virtual void Die(Vector3 dir, float power) {
        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(currentControllerData.lastDie + .8f < Time.time) {
            currentControllerData.lastDie = Time.time;
        }
        else {
            return;
        }

        if(isDead) {
            //return;
        }

        if(currentControllerData != null) {

            if(currentControllerData.dying || currentControllerData.actorExiting) {
                return;
            }

            StopNavAgent();

            ResetPositionAir(0f);

            if(currentControllerData.thirdPersonController != null) {
                currentControllerData.thirdPersonController.controllerData.removing = true;
            }

            if(currentControllerData.gamePlayerControllerAnimation != null) {
                currentControllerData.gamePlayerControllerAnimation.Die();
            }

            currentControllerData.impact = Vector3.zero;

            currentControllerData.dying = true;
        }

        if(IsPlayerControlled) {
            GamePlayerProgress.SetStatDeaths(1f);
        }
        else {

            // update players kill runtime value

            GameController.CurrentGamePlayerController.runtimeData.kills += 1;

            GamePlayerProgress.SetStatKills(1f);
        }

        /*
        if (gamePlayerEffectDeath != null) {
            gamePlayerEffectDeath.Emit(1);
        }
        */


        if(IsPlayerControlled) {
            GamePlayerModelHolderEaseEndByType();
        }

        runtimeData.health = 0;

        AudioDie();

        // TODO FADE OUT CLEANLY
        /*
        // fade out 
        UnityEngine.SkinnedMeshRenderer[] skinRenderersCharacter 
         = gamePlayerHolder.GetComponentsInChildren<SkinnedMeshRenderer>();
     
        foreach (SkinnedMeshRenderer skinRenderer in skinRenderersCharacter) {
         
            UITweenerUtil.FadeTo(skinRenderer.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 1f, 2f, 0f);        
        }
        */

        Invoke("Remove", 3);

    }

    public virtual void ExitPlayer() {
        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(currentControllerData.lastExit + 3f < Time.time) {
            currentControllerData.lastExit = Time.time;
        }
        else {
            return;
        }

        if(isDead) {
            return;
        }

        if(currentControllerData != null) {


            if(currentControllerData.actorExiting) {
                return;
            }

            StopNavAgent();

            if(currentControllerData.thirdPersonController != null) {
                currentControllerData.thirdPersonController.controllerData.removing = true;
            }

            if(currentControllerData.gamePlayerControllerAnimation != null) {
                currentControllerData.gamePlayerControllerAnimation.Jump();
            }

            currentControllerData.actorExiting = true;

            GameController.CurrentGamePlayerController.ProgressSave(1);
            GameController.CurrentGamePlayerController.ProgressScores(1);
            GameController.CurrentGamePlayerController.ProgressScore(1 * 10);

            runtimeData.health = 10;

            AudioAttack();

            GamePlayerModelHolderEaseEndByType();

            //Jump(50);

            //ResetPositionAir(10);

            // TODO FADE OUT CLEANLY
            /*
            // fade out 
            UnityEngine.SkinnedMeshRenderer[] skinRenderersCharacter 
             = gamePlayerHolder.GetComponentsInChildren<SkinnedMeshRenderer>();
         
            foreach (SkinnedMeshRenderer skinRenderer in skinRenderersCharacter) {
             
                UITweenerUtil.FadeTo(skinRenderer.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 1f, 2f, 0f);        
            }
            */

            Invoke("Remove", 2);

        }

    }

    // ------------------------------------------------------------------------
    // ATTACK

    public virtual void AttackAlt() {
        if(isDead) {
            return;
        }
        currentControllerData.gamePlayerControllerAnimation.AttackAlt();
        Invoke("AttackEffect", .5f);
    }

    public virtual void AttackLeft() {
        if(isDead) {
            return;
        }
        currentControllerData.gamePlayerControllerAnimation.AttackLeft();
        Invoke("AttackEffect", .5f);
    }

    public virtual void AttackRight() {
        if(isDead) {
            return;
        }
        currentControllerData.gamePlayerControllerAnimation.AttackRight();
        Invoke("AttackEffect", .5f);
    }

    public virtual void AttackEffect() {
        if(isDead) {
            return;
        }
        if(gamePlayerEffectAttack != null) {
            gamePlayerEffectAttack.Emit(1);
        }
    }

    public virtual void Attack() {
        if(isDead) {
            return;
        }

        bool shouldShoot = true;

        if(weaponPrimary != null) {
            if(weaponPrimary.isAuto) {
                weaponPrimary.Attack();
                shouldShoot = false;
            }
        }

        if(shouldShoot) {
            if(Time.time > currentControllerData.lastAttackTime + 1f) {
                currentControllerData.lastAttackTime = Time.time;
                StartCoroutine(AttackCo());
            }
        }
    }

    public virtual IEnumerator AttackCo() {
        yield return new WaitForSeconds(.5f);
        ActionAttack();
    }

    public virtual void CastAttack() {

        if(controllerReady) {
            return;
        }

        if(!GameConfigs.isGameRunning) {
            return;
        }

        // if (Physics.Linecast(transform.position, currentControllerData.thirdPersonController.aimingDirection)) {

        //}

        //if(prefabName.IndexOf("norah") > -1) {
        //   distance = 300f;
        //}
        //else if(prefabName.IndexOf("jaime") > -1) {
        //   distance = 30f;
        //}
        //else {
        //   distance = 6f;
        //}

        RaycastHit[] hits;

        Vector3 directionAttack = transform.forward;
        if(currentControllerData.thirdPersonController != null) {
            directionAttack = currentControllerData.thirdPersonController.aimingDirection;
        }

        //Debug.DrawRay(transform.position, directionAttack * attackDistance);

        hits = Physics.RaycastAll(transform.position, directionAttack, attackDistance);
        int i = 0;
        while(i < hits.Length) {
            RaycastHit hit = hits[i];
            Transform hitObject = hit.transform;

            if(hitObject.name.IndexOf("Game") > -1) {
                if(hitObject != null) {
                    GamePlayerController playerController = GetController(hitObject);
                    if(playerController != null) {

                        ////Debug.Log("CastAttack:" + " currentUUID:" + uniqueId + " otherID:" + playerController.uniqueId);


                        if(AllowControllerInteraction(playerController)) {
                            return;
                        }

                        ScoreAttack();

                        playerController.Hit(1f);

                        playerController.InputAttack();
                    }
                }
            }

            // Renderer renderer = hit.collider.renderer;
            //if (renderer) {
            //    renderer.material.shader = Shader.Find("Transparent/Diffuse");
            //    renderer.material.color.a = 0.3F;
            //}
            i++;
        }

    }

    public virtual void ScoreAttack() {
        ScoreAttack(10);
    }

    public virtual void ScoreAttack(double score) {
        runtimeData.score += score;
    }

    public virtual void ActionAttack() {

        //LogUtil.Log("GamePlayerController:ActionAttack:");

        //currentControllerData.thirdPersonController.ApplyAttack();

        //gamePlayerControllerAnimation.Attack();

        currentControllerData.gamePlayerControllerAnimation.Attack();

        Invoke("AttackEffect", .5f);

        //LogUtil.Log("Attacking:");

        // shoot ray for type of character

        CastAttack();

        AudioAttack();

        // Fire weapons

        if(weaponPrimary != null) {
            weaponPrimary.Attack();
        }


        // TODO wire up to weapons


        /*
     //LoadWeapons();
     
     if(weapons != null) {
         
         LogUtil.Log("Attacking: weapons:" + weapons);
         
         LogUtil.Log("Attacking: weapons.Count:" + weapons.Count);
         
         if(weapons.Count > 0) {
             //weapons[GamePlayerSlots.slotPrimary].AttackPrimary();
             //LogUtil.Log("Attacking: AttackPrimary:" + weapons[GamePlayerSlots.slotPrimary]);
         }
     }
     */
    }

    public virtual void ShootOne() {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        Shoot(1);
    }

    public virtual void Shoot(int number) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        AnimateShoot();

        //GameController.ProcessStatShot();

        runtimeData.savesLaunched += number;
        Messenger<double>.Broadcast(GameMessages.gameActionLaunch, number);
        Messenger<double>.Broadcast(GameMessages.gameActionAmmo, -number);
    }

    public virtual void FindGamePlayerCamera() {
        if(gameCameraSmoothFollow == null || gameCameraSmoothFollowGround == null) {
            foreach(GameCameraSmoothFollow cam in UnityObjectUtil.FindObjects<GameCameraSmoothFollow>()) {
                if(cam.name.Contains("Ground")) {
                    gameCameraSmoothFollowGround = cam;
                }
                else {
                    gameCameraSmoothFollow = cam;
                }
            }
        }
    }

    // ------------------------------------------------------------------------
    // AIM/LAUNCH

    float axisDeadZone = .05f;

    public virtual void UpdateAim(float x, float y) {

        FindGamePlayerCamera();

        GameObject model = gamePlayerModelHolder;
        float cameraAdjustment = 8f;
        axisDeadZone = .05f;

        if(Math.Abs(x) > axisDeadZone
            && Math.Abs(y) > axisDeadZone) {

            currentControllerData.currentPosition = model.transform.position;

            currentControllerData.currentAimPosition = -currentControllerData.currentPosition
                .WithX(currentControllerData.currentPosition.x + (x * 100))
                .WithY(currentControllerData.currentPosition.y + (y * 100));

            //float angle = Vector3.Angle(currentControllerData.currentPosition, currentControllerData.currentAimPosition);
            float dist = Vector3.Distance(currentControllerData.currentPosition, currentControllerData.currentAimPosition);

            model.transform.localScale = Mathf.Clamp(dist * .1f, .5f, 1.3f) * Vector3.one;

            Vector3 lookAtPos = model.transform.position + (currentControllerData.currentAimPosition * 10);

            model.transform.LookAt(lookAtPos);

            float amount = Mathf.Abs(dist);

            if(currentControllerData.gamePlayerEffectAim != null) {
                
                ParticleSystem.MainModule main = currentControllerData.gamePlayerEffectAim.main;
                main.startLifetime = amount / 400f;
                main.startSpeed = amount;

                currentControllerData.gamePlayerEffectAim.EnableEmission(true);
                currentControllerData.gamePlayerEffectAim.SetEmissionRate(amount * 2);
                //currentControllerData.gamePlayerEffectAim.startLifetime = amount / 400f;
                //currentControllerData.gamePlayerEffectAim.startSpeed = amount;
                currentControllerData.gamePlayerEffectAim.Play();
            }

            //currentControllerData.lineAim..SetLine3D(Color.white, model.transform.position, lookAtPos);

            //LogUtil.Log("UpdateAim:currentControllerData.currentAimPosition:", currentControllerData.currentAimPosition);

            if(gameCameraSmoothFollow != null) {
                gameCameraSmoothFollow.offset.x =
                    (gameCameraSmoothFollow.offsetInitial.x + -(x * cameraAdjustment));

                gameCameraSmoothFollow.offset.y =
                    (gameCameraSmoothFollow.offsetInitial.y + -(y * cameraAdjustment));

                gameCameraSmoothFollow.SetZoom(-((x + y) * cameraAdjustment));
            }
            if(gameCameraSmoothFollowGround != null) {
                gameCameraSmoothFollowGround.offset.x =
                    (gameCameraSmoothFollowGround.offsetInitial.x + -(x * cameraAdjustment));

                gameCameraSmoothFollowGround.offset.y =
                    (gameCameraSmoothFollowGround.offsetInitial.y + -(y * cameraAdjustment));

                gameCameraSmoothFollowGround.SetZoom(-((x + y) * cameraAdjustment));
            }

        }
        else {
            //AnimateShoot();           
            model.transform.localScale = Vector3.one;
            model.transform.rotation = gamePlayerHolder.transform.rotation;
            //model.transform.LookAt(gamePlayerModelHolder.transform.position);

            if(currentControllerData.gamePlayerEffectAim != null) {
                currentControllerData.gamePlayerEffectAim.EnableEmission(false);
                currentControllerData.gamePlayerEffectAim.SetEmissionRate(1);
                currentControllerData.gamePlayerEffectAim.Stop();
            }

            if(gameCameraSmoothFollow != null) {
                gameCameraSmoothFollow.Reset();
            }
            if(gameCameraSmoothFollowGround != null) {
                gameCameraSmoothFollowGround.Reset();
            }
        }
    }

    public virtual void AnimateShoot() {

        if(gamePlayerModelHolderModel != null) {
            foreach(Animation anim in gamePlayerModelHolderModel.GetComponentsInChildren<Animation>()) {
                if(anim != null) {
                    //gamePlayerControllerObject.animation.Play("emo_06");
                    //anim["emo_09"].normalizedSpeed = 2f;
                    anim["emo_09"].normalizedSpeed = 2f;
                    anim.Play("emo_09", PlayMode.StopAll);
                    //anim.Blend("emo_08");
                    //gamePlayerControllerObject.animation.Play("emo_10");
                    //gamePlayerControllerObject.animation.CrossFade("emo_09");
                    break;
                }
            }
        }
    }

    // ------------------------------------------------------------------------
    // AI/NAVAGENT

    public virtual void StartNavAgent() {

        if(!IsPlayerControlled && !isExiting) {
            //&& gameObject.Has<CharacterController>() && !isExiting) {

            if(currentControllerData.navMeshAgent != null) {
                currentControllerData.navMeshAgent.StartAgent();
            }
            if(currentControllerData.navMeshAgentController != null) {
                currentControllerData.navMeshAgentController.StartAgent();
            }
            if(currentControllerData.navMeshAgentFollowController != null) {
                currentControllerData.navMeshAgentFollowController.StartAgent();
            }

            currentControllerData.navAgentRunning = true;
        }
    }

    public virtual void StopNavAgent() {

        if(currentControllerData.navMeshAgent != null) {
            currentControllerData.navMeshAgent.StopAgent();
        }
        if(currentControllerData.navMeshAgentController != null) {
            currentControllerData.navMeshAgentController.StopAgent();
        }
        if(currentControllerData.navMeshAgentFollowController != null) {
            currentControllerData.navMeshAgentFollowController.StopAgent();
        }

        currentControllerData.navAgentRunning = false;
    }

    public virtual void SetNavAgentDestination(GameObject go) {

        currentTarget = go.transform;

        //StopNavAgent();

        //if (currentControllerData.navMeshAgent != null) {
        //    currentControllerData.navMeshAgent.destination = currentTarget.position;
        //}
        if(currentControllerData.navMeshAgentController != null) {
            currentControllerData.navMeshAgentController.nextDestination = currentTarget.position;
        }
        if(currentControllerData.navMeshAgentFollowController != null) {
            currentControllerData.navMeshAgentFollowController.targetFollow = currentTarget;
        }

        //StartNavAgent();
    }
    public virtual void TurnOffNavAgent() {
        if(currentControllerData.navAgentRunning) {
            if(currentControllerData.navMeshAgent != null) {
                //currentControllerData.navMeshAgent.Stop();
                StopNavAgent();
                currentControllerData.navAgentRunning = false;
            }
        }
    }

    public virtual void SyncNavAgent() {

        if(IsAgentState()) {

            /*
         if(navMeshAgent != null) {
             if(navMeshAgent.enabled) {
                 if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 1) {
                     //navMeshAgent.Stop();
                 }
             }
         }
         */
            if(isDead) {
                currentControllerData.navAgentRunning = false;
            }
        }
        else if(IsPlayerState()) {
            TurnOffNavAgent();
        }
        else if(IsUIState()) {
            TurnOffNavAgent();
        }
        else if(IsNetworkPlayerState()) {
            TurnOffNavAgent();
            CheckIfShouldRemove();
        }
    }

    // ------------------------------------------------------------------------
    // PROGRESSION

    public virtual void ProgressScores(double val) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        runtimeData.scores += val;

        Messenger<double>.Broadcast(GameMessages.gameActionScores, val);

        GamePlayerProgress.SetStatScores(val);
    }

    public virtual void ProgressScore(double val) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        runtimeData.score += val;

        Messenger<double>.Broadcast(GameMessages.gameActionScore, val);

        GamePlayerProgress.SetStatScore(val);
    }

    public virtual void ProgressAmmo(double val) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        runtimeData.ammo += val;
        runtimeData.collectedAmmo += val;

        Messenger<double>.Broadcast(GameMessages.gameActionAmmo, val);

        GamePlayerProgress.SetStatAmmo(val);
    }

    public virtual void ProgressSave(double val) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        runtimeData.saves += val;
        Messenger<double>.Broadcast(GameMessages.gameActionSave, val);

        GamePlayerProgress.SetStatSaves(val);
    }

    public virtual void ProgressAttack(double val) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        runtimeData.attacks += val;
        Messenger<double>.Broadcast(GameMessages.gameActionAssetAttack, val);

        GamePlayerProgress.SetStatAttacks(val);
    }

    public virtual void ProgressDefend(double val) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        runtimeData.defends += val;
        Messenger<double>.Broadcast(GameMessages.gameActionAssetDefend, val);

        GamePlayerProgress.SetStatDefends(val);
    }

    public virtual void ProgressRepair(double val) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        runtimeData.repairs += val;
        Messenger<double>.Broadcast(GameMessages.gameActionAssetRepair, val);

        GamePlayerProgress.SetStatRepairs(val);
    }

    public virtual void ProgressBuild(double val) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        runtimeData.builds += val;
        Messenger<double>.Broadcast(GameMessages.gameActionAssetBuild, val);

        GamePlayerProgress.SetStatBuilds(val);
    }

    public virtual void GetPlayerProgress() {
        currentControllerData.currentRPGItem = GameProfileCharacters.Current.GetCurrentCharacterRPG();
        currentControllerData.currentPlayerProgressItem = GameProfileCharacters.Current.GetCurrentCharacterProgress();
    }

    public virtual void UpdatePlayerProgress(float energy, float health) {
        StartCoroutine(UpdatePlayerProgressCo(energy, health));
    }

    public virtual IEnumerator UpdatePlayerProgressCo(float energy, float health) {
        yield return new WaitForEndOfFrame();
        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergyAndHealthRuntime(
            energy, health);
    }

    // ------------------------------------------------------------------------
    // FORCE

    public virtual void Tackle(GamePlayerController gamePlayerControllerTo) {
        Tackle(gamePlayerControllerTo, 1f);
    }

    public virtual void Tackle(GamePlayerController gamePlayerControllerTo, float power) {

        if(gamePlayerControllerTo == null) {
            return;
        }

        if(isDead) {
            return;
        }

        if(!AllowControllerInteraction(gamePlayerControllerTo)) {
            return;
        }


        if(currentControllerData.lastTackle + 1f < Time.time) {
            currentControllerData.lastTackle = Time.time;
        }
        else {
            return;
        }

        transform.LookAt(gamePlayerControllerTo.transform);

        //Jump();

        StopNavAgent();

        //Jump();

        currentControllerData.positionPlayer = transform.position;//.WithY(10);
        currentControllerData.positionTackler = gamePlayerControllerTo.transform.position;

        currentControllerData.gamePlayerControllerAnimation.Attack();

        //Attack();

        AddImpact(currentControllerData.positionTackler - currentControllerData.positionPlayer, power, false);

        StartNavAgent();
    }

    public virtual void AddForce(Vector3 dir, float force) {
        AddImpact(dir, force, false);
    }

    public virtual void AddForce(Vector3 dir, float force, bool damage) {
        AddImpact(dir, force, damage);
    }

    public virtual void AddForce(Vector3 dir, float force, bool damage, bool allowY) {
        AddImpact(dir, force, damage, allowY);
    }

    public virtual void AddImpact(Vector3 dir, float force) {
        AddImpact(dir, force, true);
    }
    // call this function to add an currentControllerData.impact force:
    public virtual void AddImpact(Vector3 dir, float force, bool damage, bool allowY = false) {

        if(isDead || isExiting) {
            currentControllerData.impact = Vector3.zero;
            return;
        }

        dir.Normalize();

        if(dir.y < 0 && allowY) {
            dir.y = 0;//-dir.y; // reflect down force on the ground
        }
        else if(!allowY) {
            dir.y = 0;
        }

        if(damage) {
            force = Mathf.Clamp(force, 0f, 100f);
        }

        //if(IsPlayerControlled 
        //   && runtimeData.goalFly == 0) {
        currentControllerData.impact += dir.normalized * force / (float)runtimeData.mass;
        //}
        if(damage) {
            runtimeData.hitCount++;

            if(IsPlayerControlled && damage) {

                HandlePlayerEffectsStateChange();

                UpdatePlayerProgress(
                    (float)(-.01f * Mathf.Clamp(force / 10f, .3f, 1f)),
                    (float)(-.01f * Mathf.Clamp(force / 10f, .3f, 1f)));
            }
        }
        //LogUtil.Log("AddImpact:name:", transform.name + "currentControllerData.impact:" + currentControllerData.impact.x);
    }

    // call this function to add an currentControllerData.impact force:
    public virtual void AddImpactForce(Vector3 dir, float force) {

        Vector3 delta = dir * force / (float)runtimeData.mass;

        currentControllerData.impact += delta;

        ////Debug.Log("AddImpactForce:" + " delta:" + delta + " dir:" + dir + " force:" + force); 

    }

    internal virtual void GamePlayerBounceSet(float bounce) {

        if(runtimeData == null) {
            return;
        }

        if(GameController.IsGameplayWorldTypeStationary()) {
            // If stationary aff move back

            controllerData.currentGamePlayerPositionBounce =
                controllerData.currentGamePlayerPositionBounce.WithZ(bounce);
        }
    }

    // ------------------------------------------------------------------------
    // UPDATE / GAMEPLAY TYPES

    internal virtual void UpdateStationary() {

        if(GameController.IsGameplayWorldTypeStationary()) {

            if(GameConfigs.isGameRunning) {

                ////Debug.Log("UpdateStationary");

                controllerData.speedInfinite =
                    Mathf.Clamp(
                        Mathf.Lerp(
                            controllerData.speedInfinite,
                            controllerData.speedInfiniteTo, 1f * Time.deltaTime),
                        0, controllerData.speedInfiniteMax);

                GamePlayerMoveSpeedSet(controllerData.speedInfinite);

                ////Debug.Log("controllerData.speedInfinite:" + controllerData.speedInfinite);
                ////Debug.Log("controllerData.GamePlayerMoveSpeedGet:" + GamePlayerMoveSpeedGet());

                /*
                controllerData.currentGamePlayerPositionBounce =
                    Vector3.Lerp(
                        controllerData.currentGamePlayerPositionBounce,
                        Vector3.zero, 1000 * Time.deltaTime);
                        */
                //Messenger<Vector3, float>.Broadcast(
                //    GamePlayerMessages.PlayerCurrentDistance,
                //    controllerData.moveGamePlayerPosition,
                //    controllerData.speedInfinite);
            }
        }
    }

    internal virtual void UpdateStationaryLate() {

        if(GameController.IsGameplayWorldTypeStationary()) {

            if(GameConfigs.isGameRunning) {

                controllerData.currentGamePlayerPosition.z = transform.position.z;

                controllerData.overallGamePlayerPosition.z += controllerData.currentGamePlayerPosition.z;
                //overallGamePlayerPosition.z = currentGamePlayerPosition.z;

                controllerData.moveGamePlayerPosition.z =
                    Mathf.Lerp(controllerData.moveGamePlayerPosition.z,
                        controllerData.currentGamePlayerPosition.z, .3f * Time.deltaTime);

                controllerData.moveGamePlayerPosition.x =
                    Mathf.Lerp(controllerData.moveGamePlayerPosition.x,
                        controllerData.moveGamePlayerPositionTo.x, gamePlayerMoveSpeed / 10f * Time.deltaTime);

                if(controllerData.currentGamePlayerPosition.y < -1) {

                    GamePlayerBounceSet(0);

                    GamePlayerMoveSpeedSet(0);

                    controllerData.moveGamePlayerPosition =
                        controllerData.moveGamePlayerPosition.WithY(-4).WithZ(0);

                    Die();
                }

                transform.position =
                    Vector3.Lerp(
                        transform.position,
                        transform.position
                            .WithX(controllerData.moveGamePlayerPosition.x)
                            .WithZ(-controllerData.moveGamePlayerPosition.z),// * controllerData.currentGamePlayerPositionBounce.z),
                        controllerData.speedInfinite * Time.deltaTime);

            }
        }
    }

    // ------------------------------------------------------------------------
    // UPDATE PHYSICS

    public virtual void UpdatePhysicsState() {

        if(!controllerReady) {
            return;
        }

        if(controllerData.lastUpdatePhysics + .2f < Time.time) {
            controllerData.lastUpdatePhysics = Time.time;
        }
        else {
            if(!IsPlayerControlled) {
                //return;
            }
        }

        //if (GameController.IsGameplayType(GameplayType.gameDasher)) {

        if(currentControllerData.characterController.enabled) {
            currentControllerData.characterController.Move(currentControllerData.impact * Time.deltaTime);
        }

        // consumes the currentControllerData.impact energy each cycle:
        currentControllerData.impact = Vector3.Lerp(currentControllerData.impact, Vector3.zero, 5 * Time.deltaTime);
        //}
        //}

        UpdatePlayerEffectsState();

        //StartCoroutine(UpdatePhysicStateCo());
    }

    public virtual IEnumerator UpdatePhysicStateCo() {

        //Vectrosity.VectorLine.SetLine (Color.red, transform.position, currentControllerData.impact);

        //if (currentControllerData.characterController.enabled) {
        //    currentControllerData.characterController.Move(currentControllerData.impact * Time.deltaTime);
        // }

        // consumes the currentControllerData.impact energy each cycle:
        //currentControllerData.impact = Vector3.Lerp(currentControllerData.impact, Vector3.zero, 5 * Time.deltaTime);
        //}

        //UpdatePlayerEffectsState();

        yield return null;
        //new WaitForFixedUpdate();

    }

    // ------------------------------------------------------------------------
    // UPDATE / EFFECTS

    public virtual void HandlePlayerEffectsStateChange() {
        currentControllerData.lastPlayerEffectsTrailUpdate = 0;
        currentControllerData.lastPlayerEffectsBoostUpdate = -1f;
        currentControllerData.lastPlayerEffectsGroundUpdate = -1f;
        UpdatePlayerEffectsState();
    }

    public virtual void UpdatePlayerEffectsState() {

        if(!controllerReady) {
            return;
        }

        if(IsPlayerControlled) {

            float trailTime =
                (Math.Abs(currentControllerData.impact.x) +
                Math.Abs(currentControllerData.impact.y) +
                Math.Abs(currentControllerData.impact.z)) * 5f;

            PlayerEffectTrailBoostTime(trailTime * currentControllerData.thirdPersonController.moveSpeed);
            PlayerEffectTrailGroundTime(-trailTime + currentControllerData.thirdPersonController.moveSpeed);

            HandlePlayerEffectsTick();
        }

        // consumes the currentControllerData.impact energy each cycle:
        //currentControllerData.impact = Vector3.Lerp(currentControllerData.impact, Vector3.zero, 5 * Time.deltaTime);
    }

    // ------------------------------------------------------------------------
    // AUDIO

    public virtual void AudioAttack() {
        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(currentControllerData.lastAudioPlayedAttack + 1 > Time.time) {
            return;
        }
        else {
            currentControllerData.lastAudioPlayedAttack = Time.time;
        }

        GameDataSound soundAttack = gameCharacter.data.GetSoundByType(GameDataActionKeys.attack);

        GameAudio.PlayEffect(soundAttack.code);
    }

    public virtual void AudioHit() {
        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(currentControllerData.lastAudioPlayedHit + 1 > Time.time) {
            return;
        }
        else {
            currentControllerData.lastAudioPlayedHit = Time.time;
        }

        GameDataSound dataItem = gameCharacter.data.GetSoundByType(GameDataActionKeys.hit);
        GameAudio.PlayEffect(transform, dataItem.code);
    }

    public virtual void AudioDie() {
        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(currentControllerData.lastAudioPlayedDie + 1 > Time.time) {
            return;
        }
        else {
            currentControllerData.lastAudioPlayedDie = Time.time;
        }

        if(controllerState == GamePlayerControllerState.ControllerPlayer) {

            GameAudioController.PlaySoundPlayerEnd();
        }

        GameDataSound dataItem = gameCharacter.data.GetSoundByType(GameDataActionKeys.death);
        GameAudio.PlayEffect(transform, dataItem.code);
    }

    // ------------------------------------------------------------------------
    // NETWORK

    public virtual void UpdateNetworkContainer(string uid) {

        uniqueId = uid;

        if(!AppConfigs.featureEnableNetworking || !GameConfigs.useNetworking) {
            return;
        }

        FindNetworkContainer(uniqueId);

        if(currentNetworkPlayerContainer != null) {
#if NETWORK_UNITY || NETWORK_PHOTON
            currentNetworkPlayerContainer.networkViewObject.observed = currentNetworkPlayerContainer;
#endif
            currentNetworkPlayerContainer.gamePlayer = gameObject;
            if(currentControllerData.thirdPersonController != null) {
                currentNetworkPlayerContainer.currentSpeedNetwork = currentControllerData.thirdPersonController.moveSpeed;
                currentNetworkPlayerContainer.verticalInputNetwork = currentControllerData.thirdPersonController.verticalInput;
                currentNetworkPlayerContainer.horizontalInputNetwork = currentControllerData.thirdPersonController.horizontalInput;
            }
            currentNetworkPlayerContainer.running = true;
        }
    }

    public virtual Gameverses.GameNetworkPlayerContainer FindNetworkContainer(string uid) {

        if(!AppConfigs.featureEnableNetworking || !GameConfigs.useNetworking) {
            return null;
        }

        if(currentNetworkPlayerContainer != null) {
            if(currentNetworkPlayerContainer.uniqueId == uid) {
                return currentNetworkPlayerContainer;
            }
        }

        if(Time.time > currentControllerData.lastNetworkContainerFind + 5f) {
            currentControllerData.lastNetworkContainerFind = Time.time;
            if(GameController.Instance.gameState == GameStateGlobal.GameStarted) {
                foreach(Gameverses.GameNetworkPlayerContainer playerContainer
                         in UnityObjectUtil.FindObjects<Gameverses.GameNetworkPlayerContainer>()) {
                    if(playerContainer.uniqueId == uid) {
                        currentNetworkPlayerContainer = playerContainer;
                        return currentNetworkPlayerContainer;
                    }
                }
            }
        }

        return null;
    }

    public virtual bool HasNetworkContainer(string uid) {

        foreach(Gameverses.GameNetworkPlayerContainer playerContainer
                 in UnityObjectUtil.FindObjects<Gameverses.GameNetworkPlayerContainer>()) {
            if(playerContainer.uniqueId == uid) {
                currentNetworkPlayerContainer = playerContainer;
                return true;
            }
        }

        return false;
    }

    public virtual void UpdateNetworkContainerFromSource(string uid) {

        uniqueId = uid;

        FindNetworkContainer(uniqueId);

        if(currentNetworkPlayerContainer != null) {
            if(currentControllerData.thirdPersonController != null) {
                currentNetworkPlayerContainer.currentSpeedNetwork = currentControllerData.thirdPersonController.moveSpeed;
                currentNetworkPlayerContainer.verticalInputNetwork = currentControllerData.thirdPersonController.verticalInput;
                currentNetworkPlayerContainer.horizontalInputNetwork = currentControllerData.thirdPersonController.horizontalInput;
            }
            currentNetworkPlayerContainer.running = true;
        }
    }

    // ------------------------------------------------------------------------
    // CONTEXT CONTROLLER

    public virtual void ChangeContextState(GamePlayerContextState contextStateTo) {
        //if (contextStateTo != contextState) {
        contextState = contextStateTo;

        if(currentControllerData.thirdPersonController != null) {
            currentControllerData.thirdPersonController.isNetworked = false;
        }

        if(contextState == GamePlayerContextState.ContextFollowAgent
            || contextState == GamePlayerContextState.ContextFollowAgentAttack
            || contextState == GamePlayerContextState.ContextRandom
            || contextState == GamePlayerContextState.ContextScript) {
            if(currentControllerData.navMeshAgent != null) {
                // TODO load script or look for character input.
                currentControllerData.navMeshAgent.enabled = true;
            }
        }
        else if(contextState == GamePlayerContextState.ContextInput
                 || contextState == GamePlayerContextState.ContextInputVehicle
                 || contextState == GamePlayerContextState.ContextFollowInput) {
            if(currentControllerData.navMeshAgent != null) {
                currentControllerData.navMeshAgent.isStopped = true;
                //navMeshAgent.enabled = false;
            }
        }
        else if(contextState == GamePlayerContextState.ContextNetwork) {
            if(currentControllerData.navMeshAgent != null) {
                currentControllerData.navMeshAgent.isStopped = true;
                //navMeshAgent.enabled = false;
            }
            currentControllerData.thirdPersonController.isNetworked = true;
        }
        else if(contextState == GamePlayerContextState.ContextUI) {
            if(currentControllerData.navMeshAgent != null) {
                currentControllerData.navMeshAgent.isStopped = true;
                //navMeshAgent.enabled = false;
            }
        }
        //}
    }

    // ------------------------------------------------------------------------
    // INPUT

    public override bool HitObject(GameObject go, InputTouchInfo inputTouchInfo) {
        Ray screenRay = Camera.main.ScreenPointToRay(inputTouchInfo.position3d);
        RaycastHit hit;

        if(Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {
            if(hit.transform.gameObject == go) {
                LogUtil.Log("HitObject GameActor");
                return true;
            }
        }
        return false;
    }

    public override void OnInputUp(InputTouchInfo touchInfo) {
        //LogUtil.Log("OnInputDown GameActor");
    }

    // ------------------------------------------------------------------------
    // ITEMS

    public virtual void HandleItemProperties() {

        //float tParam = 0f;
        //float valToBeLerped = 15f;
        //float speed = 0.3f;
        //if (tParam < 1) {
        //    tParam += Time.deltaTime * speed; //This will increment tParam based on Time.deltaTime multiplied by a speed multiplier
        //    //valToBeLerped = Mathf.Lerp(0, 3, tParam);
        //}

        // speed

        if(currentControllerData.modifierItemSpeedLerp < 1f) {

            currentControllerData.modifierItemSpeedLerp += Time.deltaTime / (currentControllerData.modifierItemSpeedLerpTime * 1000);

            currentControllerData.modifierItemSpeedCurrent = Mathf.Lerp(
                currentControllerData.modifierItemSpeedCurrent,
                currentControllerData.modifierItemSpeedMin,
                currentControllerData.modifierItemSpeedLerp);

            currentControllerData.modifierItemSpeedCurrent = Mathf.Clamp(
                currentControllerData.modifierItemSpeedCurrent, 0, 5);
        }

        // scale

        if(currentControllerData.modifierItemScaleLerp < 1f) {

            currentControllerData.modifierItemScaleLerp += Time.deltaTime / (currentControllerData.modifierItemScaleLerpTime * 1000);

            currentControllerData.modifierItemScaleCurrent = Mathf.Lerp(
                currentControllerData.modifierItemScaleCurrent,
                currentControllerData.modifierItemScaleMin,
                currentControllerData.modifierItemScaleLerp);

            currentControllerData.modifierItemScaleCurrent = Mathf.Clamp(
                currentControllerData.modifierItemScaleCurrent, 0, 5);
        }

        // fly

        /*
        if (currentControllerData.modifierItemFlyLerp < 1f) {
            
            currentControllerData.modifierItemFlyLerp += Time.deltaTime / (currentControllerData.modifierItemFlyLerpTime * 1000);
            
            currentControllerData.modifierItemFlyCurrent = Mathf.Lerp(
                currentControllerData.modifierItemFlyCurrent, 
                currentControllerData.modifierItemFlyMin, 
                currentControllerData.modifierItemFlyLerp);     
            
            currentControllerData.modifierItemFlyCurrent = Mathf.Clamp(
                currentControllerData.modifierItemFlyCurrent, 0, 5);
        }
        */

        HandleActionGoalNextFlyFlap();
    }

    // ------------------------------------------------------------------------
    // ITEMS - FLY GOALS

    public void HandleActionGoalNextFlyFlap() {

        if(runtimeData.goalFly > 0) {

            GameZoneGoalMarker marker = GameZoneGoalMarker.GetMarker();

            if(marker != null) {

                ////Debug.Log("marker:" + marker.name);

                // goalFly position

                GameObject gameGoalNext = marker.gameObject;

                Vector3 modifierItemGoalNextPosCurrent =
                    GetPositionValue(
                        GamePlayerModifierKeys.modifierItemGoalNextPosCurrent, gameObject.transform.position);

                Vector3 modifierItemGoalNextPosMin =
                    GetPositionValue(
                        GamePlayerModifierKeys.modifierItemGoalNextPosMin, gameObject.transform.position);

                Vector3 modifierItemGoalNextPosMax =
                    GetPositionValue(
                        GamePlayerModifierKeys.modifierItemGoalNextPosMax, gameGoalNext.transform.position);

                float modifierItemGoalNextPosStartTime =
                    GetModifierValue(
                        GamePlayerModifierKeys.modifierItemGoalNextPosStartTime, 0);

                modifierItemGoalNextPosCurrent = gameObject.transform.position;
                modifierItemGoalNextPosMax = gameGoalNext.transform.position;

                //float distanceFull = Vector3.Distance(modifierItemGoalNextPosMin, modifierItemGoalNextPosMax);
                float distanceCurrent = Vector3.Distance(modifierItemGoalNextPosCurrent, modifierItemGoalNextPosMax);

                //float modifierItemGoalNextPosDistance = 
                //        GetModifierValue(
                //           GamePlayerModifierKeys.modifierItemGoalNextPosDistance, distanceFull);

                //float lastTimeGoalFlyFlap = 
                //    GetModifierValue(
                //        GamePlayerTimeKeys.lastTimeGoalFlyFlap, 0);

                // float currentFlyFactor = 0f;
                //float duration = Mathf.RoundToInt(distanceFull / 5f);

                //float steps = Mathf.RoundToInt(distanceFull / 5f);
                //float step = 0.1f;

                // handle step

                //float modifierItemGoalNextPosSteps = 
                //    GetModifierValue(
                //        GamePlayerModifierKeys.modifierItemGoalNextPosSteps, steps);

                //float modifierItemGoalNextPosStep = 
                //    GetModifierValue(
                //       GamePlayerModifierKeys.modifierItemGoalNextPosStep, step);

                //

                if(Mathf.Abs(distanceCurrent) > 3f) {
                    Jump(.75f);
                }

                if(modifierItemGoalNextPosStartTime == 0) {
                    modifierItemGoalNextPosStartTime = Time.time;
                    SetModifierValue(GamePlayerModifierKeys.modifierItemGoalNextPosStartTime, modifierItemGoalNextPosStartTime);
                }

                // easing

                //if (Time.time - modifierItemGoalNextPosStartTime <= duration) {
                //    currentFlyFactor = (float)AnimationEasing.QuadEaseInOut(
                //        Time.time - modifierItemGoalNextPosStartTime, 0, 1, duration);
                //}

                if(gameObject.transform.position.y < UnityEngine.Random.Range(1.3f, 1.8f)) { //UnityEngine.Random.Range(1f, 2f)) {

                    // jagged jumps
                    //if (Mathf.Abs(distanceCurrent) > .5f) {
                    //    Jump(.05f);
                    //}

                    if(GameController.IsGameplayWorldTypeStationary()) {

                    }
                    else {

                        Vector3 dir = gameGoalNext.transform.position - transform.position;
                        dir.y = distanceCurrent / 2f;//UnityEngine.Random.Range(120f, 200f);
                        currentControllerData.impact = Vector3.zero;
                        AddImpactForce(dir, UnityEngine.Random.Range(1.3f, 1.8f));
                    }
                }


                //// TODO - hook to controller type - update controls temporarily
                //Vector3 axisInput = Vector3.zero;
                //float directionX = transform.position.x;

                //axisInput.WithX(directionX/Math.Abs(directionX));

                //OnInputAxis(GameTouchInputAxis.inputAxisMove, axisInput);

                //}

                //modifierItemGoalNextPosMax = MathUtil.LerpPercent(modifierItemGoalNextPosMax, gameGoalNext.transform.position, .1f);

                SetPositionValue(GamePlayerModifierKeys.modifierItemGoalNextPosCurrent, modifierItemGoalNextPosCurrent);
                SetPositionValue(GamePlayerModifierKeys.modifierItemGoalNextPosMin, modifierItemGoalNextPosMin);
                SetPositionValue(GamePlayerModifierKeys.modifierItemGoalNextPosMax, modifierItemGoalNextPosMax);

                //SetModifierValue(GamePlayerModifierKeys.modifierItemGoalNextPosStep, modifierItemGoalNextPosStep);

                if(currentControllerData.thirdPersonController != null) {
                    //if (currentControllerData.thirdPersonController.IsJumping()) {
                    //transform.position = modifierItemGoalNextPosCurrent;
                    //}
                }

                if(Math.Abs(distanceCurrent) < 1) {
                    ResetActionFlyFlap();
                }
            }
        }
    }

    public void ResetActionFlyFlap() {

        HandleItemStateGoalFly(-1);

        SetPositionValue(GamePlayerModifierKeys.modifierItemGoalNextPosCurrent, gameObject.transform.position);
        SetPositionValue(GamePlayerModifierKeys.modifierItemGoalNextPosMin, gameObject.transform.position);
        SetPositionValue(GamePlayerModifierKeys.modifierItemGoalNextPosMax, gameObject.transform.position);
        SetModifierValue(GamePlayerModifierKeys.modifierItemGoalNextPosLerp, 0);

        SetModifierValue(GamePlayerModifierKeys.modifierItemGoalNextPosStartTime, 0);
        SetModifierValue(GamePlayerModifierKeys.modifierItemGoalNextPosCurrent, 0);
        SetModifierValue(GamePlayerModifierKeys.modifierItemGoalNextPosDistance, 0);
        SetModifierValue(GamePlayerModifierKeys.modifierItemGoalNextPosStep, 0);
        SetModifierValue(GamePlayerModifierKeys.modifierItemGoalNextPosSteps, 0);
    }

    public class GamePlayerModifierKeys {
        public static string modifierItemGoalNextCurrent = "modifierItemGoalNextCurrent";
        public static string modifierItemGoalNextMin = "modifierItemGoalNextMin";
        public static string modifierItemGoalNextMax = "modifierItemGoalNextMax";
        public static string modifierItemGoalNextLerp = "modifierItemGoalNextLerp";
        public static string modifierItemGoalNextPosCurrent = "modifierItemGoalNextPosCurrent";
        public static string modifierItemGoalNextPosMin = "modifierItemGoalNextPosMin";
        public static string modifierItemGoalNextPosMax = "modifierItemGoalNextPosMax";
        public static string modifierItemGoalNextPosLerp = "modifierItemGoalNextPosLerp";
        public static string modifierItemGoalNextPosStartTime = "modifierItemGoalNextPosStartTime";
        public static string modifierItemGoalNextPosDistance = "modifierItemGoalNextPosDistance";
        public static string modifierItemGoalNextPosSteps = "modifierItemGoalNextPosSteps";
        public static string modifierItemGoalNextPosStep = "modifierItemGoalNextPosStep";
    }

    public class GamePlayerTimeKeys {

        public static string lastTimeFlyFlap = "lastTimeFlyFlap";
        public static string lastTimeGoalFlyFlap = "lastTimeGoalFlyFlap";
    }

    public GamePlayerControllerData currentControllerData {
        get {
            return controllerData;
        }
    }

    void SetModifierValue(string key, float val) {

        currentControllerData.modifiers.Set(key, val);

    }

    float GetModifierValue(string key, float defaultValue) {

        if(!currentControllerData.modifiers.ContainsKey(key)) {
            currentControllerData.modifiers.Set(key, defaultValue);
        }

        return currentControllerData.modifiers.Get(key);
    }

    void SetPositionValue(string key, Vector3 val) {

        if(controllerData == null) {
            return;
        }

        currentControllerData.positions.Set(key, val);

    }

    Vector3 GetPositionValue(string key, Vector3 defaultValue) {

        if(!currentControllerData.positions.ContainsKey(key)) {
            currentControllerData.positions.Set(key, defaultValue);
        }

        return currentControllerData.positions.Get(key);
    }

    public Vector3 initialScale = Vector3.one;

    // ------------------------------------------------------------------------
    // ITEMS RPG

    public virtual void HandleRPGProperties() {

        if(IsPlayerControlled) {
            if(currentControllerData.currentRPGItem == null
                || currentControllerData.currentPlayerProgressItem == null
                || currentControllerData.lastRPGModTime < Time.time) {
                currentControllerData.lastRPGModTime = Time.time + 3f;
                GetPlayerProgress();
            }

            currentControllerData.runtimeRPGData.modifierSpeed =
                currentControllerData.currentRPGItem.GetSpeed();

            currentControllerData.runtimeRPGData.modifierEnergy =
                currentControllerData.currentRPGItem.GetEnergy()
                + currentControllerData.currentPlayerProgressItem.GetGamePlayerProgressEnergy();

            currentControllerData.runtimeRPGData.modifierHealth =
                currentControllerData.currentRPGItem.GetHealth()
                + currentControllerData.currentPlayerProgressItem.GetGamePlayerProgressHealth();

            currentControllerData.runtimeRPGData.modifierAttack =
                currentControllerData.currentRPGItem.GetAttack()
                + currentControllerData.currentPlayerProgressItem.GetGamePlayerProgressEnergy();

            currentControllerData.runtimeRPGData.modifierScale =
                currentControllerData.currentRPGItem.GetScale();

            // HANDLE ITEM RUNTIME

            // SCALE

            ////Debug.Log("modifierScale:" + currentControllerData.runtimeRPGData.modifierScale);
            ////Debug.Log("modifierItemScaleCurrent:" + currentControllerData.modifierItemScaleCurrent);

            Vector3 scalePos =
                initialScale
                * Mathf.Clamp((float)currentControllerData.runtimeRPGData.modifierScale
                * currentControllerData.modifierItemScaleCurrent, .4f, 2.4f);

            ////Debug.Log("scalePos:" + scalePos);

            transform.localScale = scalePos;

            // SPEED

            float modifiedItem = Mathf.Clamp
                (currentControllerData.modifierItemSpeedCurrent, .3f, 4f);

            // FLY / JUMP

            float modifiedFly = Mathf.Clamp(
                currentControllerData.modifierItemFlyCurrent, .3f, 4f);

            if(modifiedFly > 1.0) {
                ///Jump();
            }

            // POWER

            float modifiedPower =
                (float)(currentControllerData.runtimeRPGData.modifierSpeed +
                currentControllerData.runtimeRPGData.modifierEnergy);

            float baseWalkSpeed = 5f;
            float baseTrotSpeed = 12f;
            float baseRunSpeed = 24f;

            float modifiedRunSpeed = Mathf.Clamp(baseRunSpeed * modifiedPower, 14, 24) * modifiedItem;
            float modifiedTrotSpeed = Mathf.Clamp(baseTrotSpeed * modifiedPower, 9, 14) * modifiedItem;
            float modifiedWalkSpeed = Mathf.Clamp(baseWalkSpeed * modifiedPower, 4, 8) * modifiedItem;

            if(currentControllerData.thirdPersonController != null) {

                currentControllerData.thirdPersonController.walkSpeed = modifiedRunSpeed;
                currentControllerData.thirdPersonController.trotSpeed = modifiedTrotSpeed;
                currentControllerData.thirdPersonController.runSpeed = modifiedRunSpeed;

                currentControllerData.thirdPersonController.inAirControlAcceleration = 3;
                currentControllerData.thirdPersonController.jumpHeight = .8f;
                currentControllerData.thirdPersonController.extraJumpHeight = 1f;
                currentControllerData.thirdPersonController.trotAfterSeconds = .5f;
                currentControllerData.thirdPersonController.getUserInput = false;
                currentControllerData.thirdPersonController.capeFlyGravity = 8f;
                currentControllerData.thirdPersonController.gravity = 16f;

                if(GameController.IsGameplayWorldTypeStationary()) {
                    currentControllerData.thirdPersonController.inAirControlAcceleration = 7f;
                    currentControllerData.thirdPersonController.jumpHeight = 7f;
                    currentControllerData.thirdPersonController.extraJumpHeight = 7f;
                    currentControllerData.thirdPersonController.gravity = 100f;
                    currentControllerData.thirdPersonController.capeFlyGravity = 100f;


                    currentControllerData.thirdPersonController.walkSpeed = 40f;
                    currentControllerData.thirdPersonController.trotSpeed = 40f;
                    currentControllerData.thirdPersonController.runSpeed = 50f;
                }
            }

            if(currentControllerData.gamePlayerControllerAnimation != null) {

                if(currentControllerData.gamePlayerControllerAnimation.animationData != null) {
                    currentControllerData.gamePlayerControllerAnimation.animationData.runSpeedScale =
                        Mathf.Clamp(0.1f * modifiedRunSpeed / baseRunSpeed, 1.5f, 2.8f);

                    currentControllerData.gamePlayerControllerAnimation.animationData.walkSpeedScale =
                        Mathf.Clamp(0.1f * modifiedWalkSpeed / baseWalkSpeed, 1.2f, 1.8f);
                }
            }
        }

    }

    // ------------------------------------------------------------------------
    // HANDLE CONTROLS

    public virtual IEnumerator InitControlsCo() {

        if(gamePlayerHolder != null) {

            // 
            // CHARACTER

            if(gameObject == null) {
                yield break;
            }

            // remove all components

            //Destroy(gameObject.GetComponent<GamePlayerNavMeshAgentFollowController>());
            //Destroy(gameObject.GetComponent<GamePlayerNavMeshAgentController>());
            //Destroy(gameObject.GetComponent<NavMeshAgent>());
            //Destroy(gameObject.GetComponent<GamePlayerControllerAnimation>());
            //Destroy(gameObject.GetComponent<GamePlayerThirdPersonController>());
            //Destroy(gameObject.GetComponent<CharacterController>());

            yield return new WaitForEndOfFrame();

            // COLLISION

            if(currentControllerData.gamePlayerCollision == null) {
                currentControllerData.gamePlayerCollision = gameObject.Get<GamePlayerCollision>();
            }

            // CURRENT GAME CONTROLLER

            currentControllerData.gamePlayerController = GetController(transform);

            // CHARACTER CONTROLLER

            currentControllerData.characterController = gameObject.GetOrSet<CharacterController>();

            // TODO config

            //public float initialMaxWalkSpeed = 5f;
            //public float initialMaxTrotSpeed = 15f;
            //public float initialMaxRunSpeed = 20f;
            //public float initialMaxJumpHeight = .5f;
            //public float initialMaxExtraJumpHeight = 1f;
            //public float characterSlopeLimit = 45;
            //public float characterStepOffset = .3f;
            //public float characterRadius = 1f;
            //public float characterHeight = 2.5f;

            currentControllerData.characterController.slopeLimit = 45;
            currentControllerData.characterController.stepOffset = .3f;
            currentControllerData.characterController.radius = 1.67f;
            currentControllerData.characterController.height = 2.42f;
            currentControllerData.characterController.center = new Vector3(0f, 1.79f, 0f);
            //currentControllerData.characterController.center = new Vector3(0f, 2.22f, 0f);

            /*
            
            currentControllerData.characterController.slopeLimit = 45;
            currentControllerData.characterController.stepOffset = .3f;
            currentControllerData.characterController.radius = 2.6f;// 1.67f;
            currentControllerData.characterController.height = 2.42f;
            //currentControllerData.characterController.center = new Vector3(0f, 1.79f, 0f);
            currentControllerData.characterController.center = new Vector3(0f, 2.22f, 0f);
             */

            // 
            // PLAYER CONTROLLERS

            if((contextState == GamePlayerContextState.ContextInput
                || contextState == GamePlayerContextState.ContextInputVehicle
                || contextState == GamePlayerContextState.ContextFollowInput
                && !IsUIState())
                || IsNetworkPlayerState()) {

                currentControllerData.thirdPersonController =
                    gameObject.GetOrSet<GamePlayerThirdPersonController>();

                currentControllerData.thirdPersonController.Init();

                HandleRPGProperties();
            }

            // 
            // AGENTS

            if(ShouldHaveNavAgent()) {

                currentControllerData.navMeshAgent = gameObject.GetOrSet<NavMeshAgent>();

                if(currentControllerData.navMeshAgent != null) {
                    //navMeshAgent.enabled = false;
                    //if(!IsPlayerControlled) {
                    currentControllerData.navMeshAgent.height = 3.19f;
                    currentControllerData.navMeshAgent.radius = 1.29f;
                    currentControllerData.navMeshAgent.baseOffset = -0.30f;
                    currentControllerData.navMeshAgent.stoppingDistance = 2f;
                    //}

                    currentControllerData.navMeshAgent.speed =
                        12 * (float)(currentControllerData.runtimeRPGData.modifierSpeed +
                    currentControllerData.runtimeRPGData.modifierEnergy) +
                    UnityEngine.Random.Range(1, 5);

                    currentControllerData.navMeshAgent.acceleration =
                        8 * (float)(currentControllerData.runtimeRPGData.modifierSpeed +
                    currentControllerData.runtimeRPGData.modifierEnergy) +
                    UnityEngine.Random.Range(1, 5);

                    currentControllerData.navMeshAgent.angularSpeed =
                        120 + UnityEngine.Random.Range(1, 5);

                    if(IsSidekickControlled) {
                        // Speed up sidekicks to take cover better behind you.
                        float adjust = 2.0f;
                        currentControllerData.navMeshAgent.speed *= adjust;
                        currentControllerData.navMeshAgent.acceleration *= adjust;
                        currentControllerData.navMeshAgent.angularSpeed *= adjust;
                    }
                }
            }

            if(contextState == GamePlayerContextState.ContextFollowAgent
                || contextState == GamePlayerContextState.ContextFollowAgentAttack
                && ShouldHaveNavAgent()) {

                currentControllerData.navMeshAgentFollowController = gameObject.GetOrSet<GamePlayerNavMeshAgentFollowController>();
                currentControllerData.navMeshAgentFollowController.agent = currentControllerData.navMeshAgent;

                if(IsSidekickControlled) {
                    currentControllerData.navMeshAgentFollowController.agentDistance = 10;
                    currentControllerData.navMeshAgentFollowController.targetAttractRange = 20;
                    currentControllerData.navMeshAgentFollowController.targetLimitRange = 40;
                    currentControllerData.navMeshAgentFollowController.targetFollow =
                        GameController.CurrentGamePlayerController.gamePlayerSidekickTarget.transform;
                }
                else {
                    currentControllerData.navMeshAgentFollowController.targetFollow =
                        GameController.CurrentGamePlayerController.gamePlayerEnemyTarget.transform;
                }
            }

            if(contextState == GamePlayerContextState.ContextRandom
                && ShouldHaveNavAgent()) {

                currentControllerData.navMeshAgentController = gameObject.GetOrSet<GamePlayerNavMeshAgentController>();
                currentControllerData.navMeshAgentController.agent = currentControllerData.navMeshAgent;

                currentControllerData.navMeshAgentController.nextDestination =
                    currentControllerData.navMeshAgentController.GetRandomLocation();
            }

            // 
            // ANIMATION

            currentControllerData.gamePlayerControllerAnimation = gameObject.GetOrSet<GamePlayerControllerAnimation>();

            currentControllerData.gamePlayerControllerAnimation.Init();

            float smoothing = .8f;

            if(currentControllerData.thirdPersonController != null) {
                smoothing = currentControllerData.thirdPersonController.speedSmoothing;
            }
            else {
                smoothing = currentControllerData.navMeshAgent.velocity.magnitude + 10f;
            }

            currentControllerData.gamePlayerControllerAnimation.animationData.runSpeedScale =
                (smoothing * .15f) * (float)(currentControllerData.runtimeRPGData.modifierSpeed +
                currentControllerData.runtimeRPGData.modifierEnergy);// currentControllerData.thirdPersonController.trotSpeed / currentControllerData.thirdPersonController.walkSpeed / 2;

            currentControllerData.gamePlayerControllerAnimation.animationData.walkSpeedScale =
                1f * (float)(currentControllerData.runtimeRPGData.modifierSpeed +
                currentControllerData.runtimeRPGData.modifierEnergy);//currentControllerData.thirdPersonController.walkSpeed / currentControllerData.thirdPersonController.walkSpeed;

            currentControllerData.gamePlayerControllerAnimation.animationData.isRunning = true;

            // 
            // SHADOW

            actorShadow = gameObject.GetOrSet<ActorShadow>();

            if(actorShadow != null) {

                actorShadow.objectParent = gamePlayerModelHolderModel;

                if(gamePlayerShadow != null) {
                    actorShadow.objectShadow = gamePlayerShadow;
                }
            }

            StartNavAgent();

            if(currentControllerData.thirdPersonController != null) {
                currentControllerData.thirdPersonController.Reset();
            }

            if(controllerState == GamePlayerControllerState.ControllerAgent
                || controllerState == GamePlayerControllerState.ControllerSidekick) {
                currentControllerData.navMeshAgent.enabled = true;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    // ------------------------------------------------------------------------
    // GAME STATE

    public bool ShouldHaveNavAgent() {

        return (!IsUIState()
            && !(IsPlayerControlled
                && GameController.IsGameplayWorldTypeStationary()));
    }

    public virtual void CheckIfShouldRemove() {
        if(IsNetworkPlayerState()) {
            // if network container is gone remove the player...

            if(HasNetworkContainer(uniqueId)) {
                // no prob
            }
            else {
                // remove

                if(currentControllerData.thirdPersonController) {
                    currentControllerData.thirdPersonController.ApplyDie(true);
                }

                TweenUtil.FadeToObject(gameObject, 0f, .3f, .5f);

                Invoke("RemoveMe", 6);
            }
        }
    }

    public override void RemoveMe() {
        gamePlayerModelHolderModel.DestroyChildren(GameConfigs.usePooledGamePlayers);
        gameObject.DestroyGameObject(3f, GameConfigs.usePooledGamePlayers);
    }

    public virtual bool CheckVisibility() {

        if(!controllerReady) {
            return false;
        }

        if(currentControllerData.renderers == null) {
            currentControllerData.renderers = new List<SkinnedMeshRenderer>();
        }

        if(currentControllerData.renderers.Count == 0) {
            foreach(SkinnedMeshRenderer rendererSkinned in gamePlayerHolder.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                currentControllerData.renderers.Add(rendererSkinned);
            }
        }

        currentControllerData.visible = false;

        if(currentControllerData.renderers.Count > 0) {
            foreach(SkinnedMeshRenderer rendererSkinned in currentControllerData.renderers) {
                if(rendererSkinned != null) {
                    if(!rendererSkinned.isVisible) {// || !rendererSkinned.IscurrentControllerData.visibleFrom(Camera.main)) {
                        currentControllerData.visible = false;
                    }
                    else {
                        currentControllerData.visible = true;
                        break;
                    }
                }
            }
        }

        //LogUtil.Log("currentControllerData.visible:" + currentControllerData.visible);

        return currentControllerData.visible;
    }

    // ------------------------------------------------------------------------
    // COLLISION

    // enable

    public virtual void GamePlayerCollisionEnable(bool enabled) {

        if(!controllerReady) {
            return;
        }

        currentControllerData.gamePlayerCollision.gameObject.SetActive(enabled);
    }

    public virtual void GamePlayerCollisionEnableDelayed(bool enabled, float delay) {
        StartCoroutine(GamePlayerCollisionEnableDelayedCo(enabled, delay));
    }

    public virtual IEnumerator GamePlayerCollisionEnableDelayedCo(bool enabled, float delay) {

        yield return new WaitForSeconds(delay);

        GamePlayerCollisionEnable(enabled);
    }

    // -0.5
    // position

    public virtual void GamePlayerCollisionPosition(Vector3 pos) {

        if(!controllerReady) {
            return;
        }

        currentControllerData.gamePlayerCollision.gameObject.transform.position = pos;
    }

    public virtual void GamePlayerCollisionPositionDelayed(Vector3 pos, float delay) {
        StartCoroutine(GamePlayerCollisionPositionDelayedCo(pos, delay));
    }

    public virtual IEnumerator GamePlayerCollisionPositionDelayedCo(Vector3 pos, float delay) {

        yield return new WaitForSeconds(delay);

        GamePlayerCollisionScale(pos);
    }

    // scale

    public virtual void GamePlayerCollisionScale(Vector3 scale) {

        if(!controllerReady) {
            return;
        }

        currentControllerData.gamePlayerCollision.gameObject.transform.localScale = scale;
    }

    public virtual void GamePlayerCollisionScaleDelayed(Vector3 scale, float delay) {
        StartCoroutine(GamePlayerCollisionScaleDelayedCo(scale, delay));
    }

    public virtual IEnumerator GamePlayerCollisionScaleDelayedCo(Vector3 scale, float delay) {

        yield return new WaitForSeconds(delay);

        GamePlayerCollisionScale(scale);
    }

    // ------------------------------------------------------------------------
    // UPDATE/GAME TICK

    public virtual void UpdateCommonState() {

        if(!controllerReady) {
            return;
        }

        currentFPS = FPSDisplay.GetCurrentFPS();

        if(InputSystem.isMouseSecondaryPressed) {
            Jump();
        }

        if(Application.isEditor && IsPlayerControlled) {
            if(Input.GetKeyDown(KeyCode.M)) {
                PlayerEffectWarpFadeIn();
            }
            else if(Input.GetKeyDown(KeyCode.N)) {
                PlayerEffectWarpFadeOut();
            }
            else if(Input.GetKeyDown(KeyCode.J)) {
                GamePlayerModelHolderEaseStartByType();
            }
            else if(Input.GetKeyDown(KeyCode.K)) {
                GamePlayerModelHolderEaseReadyByType();
            }
            else if(Input.GetKeyDown(KeyCode.L)) {
                GamePlayerModelHolderEaseEndByType();
            }
        }

        // visibility
        CheckVisibility();

        // fast stuff    
        HandlePlayerAliveState();
        //HandlePlayerEffectWarpAnimateTick();
        //HandleGamePlayerModelHolderEaseTick();

        if(IsAgentState()) {

            //if(Input.GetMouseButtonDown(0)) {

            //   if(navMeshAgent != null) {
            //       if(navMeshAgent.enabled) {

            //Vector3 worldPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y,Camera.main.nearClipPlane);
            //var mousePos = Camera.main.ScreenToWorldPoint (worldPoint);
            //var ray = Camera.main.ScreenPointToRay(mousePos);
            //RaycastHit hit;
            //if(Physics.Raycast(ray.origin,ray.direction, out hit)){
            // print("world point on terrain: "+hit.point+", distance to point: "+hit.distance);
            //navMeshAgent.destination = hit.point;
            //}
            //       }   
            //   }
            //}

            if(runtimeData != null) {
                if(runtimeData.hitCount > UnityEngine.Random.Range(2, 4)) {
                    Die();
                }
            }

            UpdateNetworkContainerFromSource(uniqueId);
        }
        else if(IsPlayerState()) {
            if(currentControllerData.thirdPersonController.aimingDirection != Vector3.zero) {

                //gamePlayerHolder.transform.rotation = Quaternion.LookRotation(currentControllerData.thirdPersonController.aimingDirection);
                gamePlayerModelHolder.transform.rotation =
                    Quaternion.LookRotation(currentControllerData.thirdPersonController.aimingDirection);

                if(currentControllerData.mountData.isMountedVehicle) {
                    currentControllerData.mountData.mountVehicle.SetMountWeaponRotator(
                        Quaternion.LookRotation(currentControllerData.thirdPersonController.aimingDirection));
                }

                foreach(Transform t in gamePlayerModelHolderModel.transform) {
                    t.localRotation = Quaternion.identity;
                }

                if(currentControllerData.thirdPersonController.aimingDirection.IsBiggerThanDeadzone(axisDeadZone)) {

                    if(currentControllerData.thirdPersonController.aimingDirection != currentControllerData.thirdPersonController.movementDirection) {
                        SendAttack();
                    }
                }
            }
            else {

                if(currentControllerData.mountData.isMountedVehicle) {
                    //currentControllerData.mountData.mountVehicle.SetMountWeaponRotatorLocal(Vector3.zero);
                }

                foreach(Transform t in gamePlayerModelHolderModel.transform) {
                    t.localRotation = Quaternion.identity;
                }
            }

            if(runtimeData.hitCount > runtimeData.hitLimit) {
                Die();
            }

            UpdateNetworkContainerFromSource(uniqueId);
        }
        else if(IsUIState()) {

        }
        else if(IsNetworkPlayerState()) {

        }

        bool shouldBeGrounded = true;
        if(currentControllerData.thirdPersonController != null) {
            if(currentControllerData.thirdPersonController.IsJumping()) {
                shouldBeGrounded = false;
            }
        }

        if(shouldBeGrounded && !isEntering && !isExiting) {
            ResetPositionAir(0f);
        }

        if(IsPlayerControlled) {
            HandleItemProperties();
            HandleRPGProperties();
        }

        // periodic      

        bool runUpdate = false;
        if(Time.time > currentControllerData.lastUpdateCommon + 1f) {
            currentControllerData.lastUpdateCommon = Time.time;
            runUpdate = true;

            if(IsPlayerControlled) {
                ProgressScore(1);
            }
        }

        if(!runUpdate) {
            return;
        }

        SyncNavAgent();

    }

    bool shadowActive = false;

    public virtual void UpdateVisibleState() {

        // Handle navagent on/off while jumping

        if(currentControllerData.thirdPersonController != null) {
            if(currentControllerData.thirdPersonController.IsJumping()) {
                if(currentControllerData.navMeshAgent != null) {
                    if(currentControllerData.navMeshAgent.enabled) {
                        currentControllerData.navMeshAgent.isStopped = true;
                    }
                }

                if(gamePlayerShadow != null) {
                    /////gamePlayerShadow.Hide();
                }
            }
            else {
                if(currentControllerData.navMeshAgent != null) {
                    if(!currentControllerData.navMeshAgent.enabled) {
                        currentControllerData.navMeshAgent.enabled = true;
                    }
                    currentControllerData.navMeshAgent.isStopped = false;
                }

                if(gamePlayerShadow != null) {
                    /////gamePlayerShadow.Show();
                }
            }
        }

        if(isCharacterLoaded) {
            if(!shadowActive) {
                actorShadow.gameObject.Show();
                shadowActive = true;
            }
        }
        else {
            if(shadowActive) {
                actorShadow.gameObject.Hide();
                shadowActive = false;
            }
        }

        //if (currentControllerData.dying) {
        //transform.position = Vector3.Lerp(transform.position, transform.position.WithY(1.3f), 1 + Time.deltaTime);
        //}

        // fix after jump
        if(gamePlayerModelHolderModel != null) {
            foreach(Transform t in gamePlayerModelHolderModel.transform) {
                if(currentControllerData.thirdPersonController != null) {
                    if(!currentControllerData.thirdPersonController.IsJumping()) {
                        t.localPosition = Vector3.Lerp(t.localPosition, t.localPosition.WithY(0), 2 + Time.deltaTime);
                    }
                }
                break;
            }
        }

        bool runUpdate = false;

        if(currentControllerData.currentTimeBlock + currentControllerData.actionInterval < Time.time) {
            currentControllerData.currentTimeBlock = Time.time;
            runUpdate = true;
        }

        if(controllerState == GamePlayerControllerState.ControllerAgent
            && (contextState == GamePlayerContextState.ContextFollowAgentAttack
            || contextState == GamePlayerContextState.ContextFollowAgent)
            && GameController.Instance.gameState == GameStateGlobal.GameStarted
            && (isAlive || !currentControllerData.actorExiting)) {

            if(runUpdate) {
                GameObject go = GameController.CurrentGamePlayerController.gameObject;

                if(go != null) {

                    currentControllerData.distanceToPlayerControlledGamePlayer = Vector3.Distance(
                        go.transform.position,
                        transform.position);

                    // check distance for evades

                    if(lastStateEvaded > .3f) {

                        lastStateEvaded += Time.deltaTime;

                        if(currentControllerData.distanceToPlayerControlledGamePlayer <= currentControllerData.distanceEvade) {
                            currentControllerData.isWithinEvadeRange = true;
                        }
                        else {
                            currentControllerData.isWithinEvadeRange = false;
                        }

                        if(currentControllerData.lastIsWithinEvadeRange != currentControllerData.isWithinEvadeRange) {
                            if(currentControllerData.lastIsWithinEvadeRange && !currentControllerData.isWithinEvadeRange) {
                                // evaded!
                                GamePlayerProgress.SetStatEvaded(1f);
                            }
                            currentControllerData.lastIsWithinEvadeRange = currentControllerData.isWithinEvadeRange;
                        }
                    }

                    // CHECK ATTACK/LUNGE/TACKLE/MELEE RANGE

                    if(currentControllerData.distanceToPlayerControlledGamePlayer <= attackRange
                        && currentControllerData.shouldTackle) {
                        //foreach(Collider collide in Physics.OverlapSphere(transform.position, attackRange)) {

                        // Turn towards player and attack!

                        /// TODO

                        GamePlayerController gamePlayerControllerHit
                            = GameController.GetGamePlayerControllerObject(go, true);

                        if(IsAgentControlled) {

                            if(gamePlayerControllerHit != null
                                && !gamePlayerControllerHit.isDead
                                && !currentControllerData.gamePlayerController.isDead) {

                                if(AllowControllerInteraction(gamePlayerControllerHit)) {

                                    float power = 0;

                                    if(currentControllerData.distanceToPlayerControlledGamePlayer < attackRange / 2f) {
                                        // LEAP AT THEM within three
                                        power = Mathf.Clamp(150f - currentControllerData.distanceToPlayerControlledGamePlayer / 2, 0f, 80f);
                                    }
                                    else if(currentControllerData.distanceToPlayerControlledGamePlayer < attackRange * 2) {
                                        // PURSUE FASTER
                                        //power = Mathf.Clamp(3.5f + currentControllerData.distanceToPlayerControlledGamePlayer / 2, 0f, 40f);
                                    }

                                    power = 5f;

                                    if(power > 0 && currentControllerData.shouldTackle) {
                                        Tackle(gamePlayerControllerHit, power);
                                    }
                                }
                            }
                        }
                    }

                    // CHECK RANDOM DIE RANGE

                    bool shouldRandomlyDie = false;

                    if(shouldRandomlyDie) {

                        if(currentControllerData.distanceToPlayerControlledGamePlayer >= currentControllerData.distanceRandomDie) {
                            currentControllerData.isInRandomDieRange = true;
                        }
                        else {
                            currentControllerData.isInRandomDieRange = false;
                        }

                        if(currentControllerData.isInRandomDieRange) {
                            if(currentControllerData.lastRandomDie > UnityEngine.Random.Range(
                                    currentControllerData.timeMinimumRandomDie,
                                    currentControllerData.timeMinimumRandomDie + currentControllerData.timeMinimumRandomDie / 2)) {

                                currentControllerData.lastRandomDie = 0;
                                //shouldRandomlyDie = true;
                            }

                            currentControllerData.lastRandomDie += Time.deltaTime;
                        }

                        // TODO review evaded

                        //public float currentControllerData.distanceRandomDie = 30f;
                        //public float currentControllerData.timeMinimumRandomDie = 5f;

                        if(currentControllerData.lastIsInRandomDieRange != currentControllerData.isInRandomDieRange) {
                            if(currentControllerData.lastIsInRandomDieRange && !currentControllerData.isInRandomDieRange) {
                                // out of range random!
                                //GameController.CurrentGamePlayerController.Score(5);
                                //GamePlayerProgress.SetStatEvaded(1f);
                            }
                            currentControllerData.lastIsInRandomDieRange = currentControllerData.isInRandomDieRange;
                        }

                        // TODO mod
                        runtimeData.hitCount += 10;
                    }
                }
            }
        }
        else if(IsPlayerControlled
                 && GameController.Instance.gameState == GameStateGlobal.GameStarted) {
            float currentSpeed = 0;

            if(currentControllerData.mountData.isMountedVehicle) {
                currentSpeed = currentControllerData.mountData.mountVehicle.driver.currentSpeed;
            }
            else {
                currentSpeed = currentControllerData.thirdPersonController.moveSpeed;
            }
            //LogUtil.Log("currentSpeed:", currentSpeed);

            // Target glides in front of character by speed, enemy targeting

            Vector3 pos = Vector3.zero;
            pos.z = Mathf.Clamp(currentSpeed / 3, 0.3f, 3.5f);

            // Target glides in back of character by speed, sidekick targeting
            // allows player to protect rescue bots/sidekicks/co-bot that are on auto.

            Vector3 posBack = Vector3.zero;
            posBack.z = -Mathf.Clamp(currentSpeed / 3, 1.3f, 4.5f);

            if(gamePlayerEnemyTarget != null) {
                gamePlayerEnemyTarget.transform.localPosition = pos;
            }

            if(gamePlayerSidekickTarget != null) {
                gamePlayerSidekickTarget.transform.localPosition = posBack;
            }

            if(gamePlayerModelTarget != null) {
                gamePlayerModelTarget.transform.localPosition = pos;
            }

            if(currentControllerData.playerSpin) {
                // Clamps automatically angles between 0 and 360 degrees.
                float y = 360 * Time.deltaTime;

                gamePlayerModelHolder.transform.localRotation =
                    Quaternion.Euler(0, gamePlayerModelHolder.transform.localRotation.eulerAngles.y + y, 0);

                if(gamePlayerModelHolder.transform.localRotation.eulerAngles.y > 330) {
                    currentControllerData.playerSpin = false;
                    gamePlayerModelHolder.transform.localRotation =
                        Quaternion.Euler(0, 0, 0);
                }
            }
        }

        /*
            // periodic stuff
     
            bool runUpdate = false;
            if(Time.time > currentControllerData.lastUpdate + .3f) {
                currentControllerData.lastUpdate = Time.time;
                runUpdate = true;
            }
     
            if(!runUpdate) {
                return;
            }
     */
    }

    public virtual void UpdateOffscreenState() {

    }

    //void OnDrawGizmosSelected() {
    //    Vector3 p = camera.ScreenToWorldPoint(new Vector3(100, 100, camera.nearClipPlane));
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(p, 0.1F);
    //}

    public virtual void UpdateAlways() {

        currentControllerData.lastAirCheck += Time.deltaTime;

        HandleCharacterAttachedSounds(); // always run to turn off audio when not playing.
        HandlePlayerInactionState();

        //handleGameInput();
    }

    public virtual void UpdateEditorTools() {

        if(IsPlayerControlled) {

            if(Application.isEditor) {

                if(Input.GetKey(KeyCode.LeftControl)) {

                    float power = 100f;
                    if(Input.GetKey(KeyCode.V)) {
                        Boost(Vector3.zero.WithZ(1),
                            power);
                    }
                    else if(Input.GetKey(KeyCode.B)) {
                        Boost(Vector3.zero.WithZ(-1),
                            power);
                    }
                    else if(Input.GetKey(KeyCode.N)) {
                        Strafe(Vector3.zero.WithX(-1),
                            power);
                    }
                    else if(Input.GetKey(KeyCode.M)) {
                        Strafe(Vector3.zero.WithX(1),
                            power);
                    }

                    if(Input.GetKey(KeyCode.RightBracket)) {
                        if(!IsPlayerControlled) {
                            Die();
                        }
                    }
                }
            }
        }
    }

    public virtual void FixedUpdate() {

        if(!gameObjectTimer.IsTimerPerf(
                GameObjectTimerKeys.gameFixedUpdateAll, IsPlayerControlled ? 1 : 2)) {
            return;
        }

        if(!controllerReady) {
            return;
        }

        if(!currentControllerData.initialized) {
            return;
        }

        HandlePlayerAliveStateFixed();
    }

    public virtual void LateUpdate() {

        UpdateStationaryLate();

        if(!gameObjectTimer.IsTimerPerf(
                GameObjectTimerKeys.gameLateUpdateAll, IsPlayerControlled ? 1 : 2)) {
            return;
        }

        if(!controllerReady) {
            return;
        }

        if(controllerData == null) {
            return;
        }

        if(!currentControllerData.initialized) {
            return;
        }

        HandlePlayerAliveStateLate();
    }

    public override void Update() {

        // Run only in game state

        if(!controllerReady) {
            return;
        }

        if(!GameConfigs.isGameRunning) {
            return;
        }

        UpdateStationary();

        if(!gameObjectTimer.IsTimerPerf(
                GameObjectTimerKeys.gameUpdateAll, IsPlayerControlled ? 1 : 2)) {
            return;
        }

        // Run outside game state as well

        HandlePlayerEffectWarpAnimateTick();

        if(controllerData != null && !currentControllerData.initialized) {
            return;
        }

        UpdateAlways();

        UpdateVisibleState();

        UpdateCommonState();

        UpdateEditorTools();

        //if(!currentControllerData.visible) {
        //UpdateOffscreenState();
        //return;
        //}
        //else {
        //UpdateVisibleState();
        //}
    }
}