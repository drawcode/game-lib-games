using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Animation;
using Engine.Data.Json;
using Engine.Events;
using Engine.Game;
using Engine.Game.Actor;
using Engine.Game.Controllers;
using Engine.Utility;

public enum GamePlayerControllerState {
    ControllerAgent = 0,
    ControllerPlayer = 1,
    ControllerUI = 2,
    ControllerNetwork = 3
}

public enum GamePlayerActorState {
    ActorMe,
    ActorEnemy,
    ActorFriend
}

public enum GamePlayerContextState {
    ContextInput = 0,
    ContextFollowAgent,
    ContextFollowAgentAttack,
    ContextFollowInput,
    ContextScript,
    ContextRandom,
    ContextUI,
    ContextNetwork
}

public class BaseGamePlayerSlots {
    public static string slotPrimary = "primary";
    public static string slotSecondary = "secondary";
    public static string slotExtra = "extra";
}

public class BaseGamePlayerRuntimeData {
    public double health = 1f;
    public double energy = 1f;
    public double speed = 1f;
    public double scale = 1f;
    public double attack = 1f;
    public double defense = 1f;
    public double scores = 0;
    public double score = 0;
    public double coins = 0;
    public double hits = 8;
    public double hitCount = 0;
    public double hitLimit = 10;
    public double mass = 1;
    public double evades = 0;
    public double ammo = 10;
    public double collectedAmmo = 0;
    public double saves = 0;
    public double savesLaunched = 0;

    public virtual float totalScoreValue {
        get {
            return (float)(score + (coins * 50) + (scores * 500));
        }
    }
}

public class BaseGamePlayerRuntimeRPGData {
    public double modifierSpeed = .5;
    public double modifierHealth = .5;
    public double modifierEnergy = .5;
    public double modifierMagic = .5;
    public double modifierBoost = .5;
    public double modifierPower = .5;
    public double modifierAttack = .5;
    public double modifierDefend = .5;
}

public class BaseGamePlayerController : GameActor {
 
    public string uuid = "";
    public string prefabName = "bot1";
    public string lastPrefabName = "--";
 
    // navigation/movement
    public NavMeshAgent navMeshAgent;
    public GamePlayerNavMeshAgentFollowController navMeshAgentFollowController;
    public GamePlayerNavMeshAgentController navMeshAgentController;
    public CharacterController characterController;
    public GamePlayerThirdPersonController thirdPersonController;
    public Transform currentTarget;
 
    // animation
    public GamePlayerControllerAnimation gamePlayerControllerAnimation;

    // asset
    public GamePlayerControllerAsset gamePlayerControllerAsset;
     
    // player effects
    public GameObject gamePlayerEffectParticleObjects;
    public ParticleSystem gamePlayerEffectWarp;
    public ParticleSystem gamePlayerEffectCircleFollow;
    public ParticleSystem gamePlayerEffectCircle;
    public ParticleSystem gamePlayerEffectCircleStars;
    public ParticleSystem gamePlayerEffectAttack;
    public ParticleSystem gamePlayerEffectSkill;
    public ParticleSystem gamePlayerEffectHit;
    public ParticleSystem gamePlayerEffectDeath; 
 
    // appearance/context
    public ActorShadow actorShadow;
 
    // models/objects
    public GameObject gamePlayerModel;
    public GameObject gamePlayerHolder;
    public GameObject gamePlayerShadow;
    public GameObject gamePlayerEnemyTarget;
    public GameObject gamePlayerModelHolder;
    public GameObject gamePlayerModelHolderModel;
    public GameObject gamePlayerModelHolderWeapons;
    public GameObject gamePlayerModelHolderItems;
    public GameObject gamePlayerModelHolderSkills;
 
    // attack
    public GameObject weaponObject;
 
    // skill
    public GameObject skillObject;
 
    // states
    public GamePlayerControllerState controllerState = GamePlayerControllerState.ControllerPlayer;
    public GamePlayerContextState contextState = GamePlayerContextState.ContextInput;
 
    // runtime data
    public GamePlayerRuntimeData runtimeData = new GamePlayerRuntimeData();
    public GamePlayerRuntimeRPGData runtimeRPGData = new GamePlayerRuntimeRPGData();
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
    public GamePlayerAttributes gamePlayerAttributes = new GamePlayerAttributes();
     
    // weapons
    // TODO work in full list dynamic.
    public Dictionary<string, GamePlayerWeapon> weapons = new Dictionary<string, GamePlayerWeapon>();
    public GamePlayerWeapon weaponPrimary;
    public GamePlayerWeapon weaponSecondary;
 
    // network
    public Gameverses.GameNetworkPlayerContainer currentNetworkPlayerContainer;
 
    //float lastUpdate = 0f;
    public bool visible = true;
    List<SkinnedMeshRenderer> renderers;
    public bool initialized = false;
    public bool dying = false;

    // effects

    // effects - warps

    public bool effectWarpEnabled = false;
    public float effectWarpStart = 0f;
    public float effectWarpEnd = 200f;
    public float effectWarpCurrent = 0f;

    // efects - lines

    public GameObject gamePlayerTrailContainer;
    public GameObject gamePlayerTrailGround;
    public GameObject gamePlayerTrailBoost;
    public TrailRenderer trailRendererBoost;
    public TrailRenderer trailRendererGround;
    public Vector3 impact = Vector3.zero;
    float lastDie = 0f;
    Gameverses.GameNetworkAniStates currentNetworkAniState = Gameverses.GameNetworkAniStates.walk;
    Gameverses.GameNetworkAniStates lastNetworkAniState = Gameverses.GameNetworkAniStates.run;
    [HideInInspector]
    public float incrementScore = 1f;
        
    GameObject audioObjectFootsteps;
    AudioClip audioObjectFootstepsClip;
    AudioSource audioObjectFootstepsSource;

    float lastUpdateCommon = 0f;
    float lastAttackTime = 0;
    float lastBoostTime = 0;
    float lastStrafeLeftTime = 0;
    float lastSpinTime = 0;
    float lastStrafeRightTime = 0;
    float lastAudioPlayedAttack = 0f;
    float lastAudioPlayedHit = 0f;
    float lastAudioPlayedDie = 0f;
    float lastNetworkContainerFind = 0f;
    bool navAgentRunning = true;
    public float currentTimeBlock = 0.0f;
    public float actionInterval = .33f;
    public bool initLoaded = false;
    float lastCollision = 0f;
    float lastHit = 0f;
    Vector3 positionPlayer;
    Vector3 positionTackler;
    float lastTackle = 0f;
    public GameProfileRPGItem currentRPGItem;
    public GameProfilePlayerProgressItem currentPlayerProgressItem;
    public double rpgModifierDefault = .4f;
    public float lastRPGModTime = 0f;
    bool playerSpin = false;
    public GameCameraSmoothFollow gameCameraSmoothFollow;
    public GameCameraSmoothFollow gameCameraSmoothFollowGround;
    public Vectrosity.VectorLine lineAim = null;
    public Vector3 positionStart = Vector3.zero;
    public Vector3 positionRelease = Vector3.zero;
    public Vector3 positionLastTouch = Vector3.zero;
    public Vector3 currentStartPoint = Vector3.zero;
    public Vector3 currentEndPoint = Vector3.zero;
    public Vector3 currentPosition = Vector3.zero;
    public Vector3 currentAimPosition = Vector3.zero;
    public ParticleSystem gamePlayerEffectAim;
    public float distanceToPlayerControlledGamePlayer;
    public float distanceEvade = 5f;
    public bool isWithinEvadeRange = false;
    public bool lastIsWithinEvadeRange = false;
    
    // IDLE ACTIONS AFTER INACTION
    public float delayIdleActions = 3.0f;
    public float lastIdleActions = 0f;
 
    // --------------------------------------------------------------------
    // INIT
 
    public virtual void Awake() {
        uuid = Gameverses.UniqueUtil.Instance.currentUniqueId;
    }
 
    public override void Start() {
        Init(controllerState);
    }
 
    public override void OnEnable() {
        //MessengerObject<InputTouchInfo>.AddListener(MessengerObjectMessageType.OnEventInputDown, OnInputDown);
        //MessengerObject<InputTouchInfo>.AddListener(MessengerObjectMessageType.OnEventInputUp, OnInputUp);
     
     
        Messenger<string, Vector3>.AddListener("input-axis", OnInputAxis);//"input-axis-" + axisName, axisInput);
     
        Messenger<string, string>.AddListener(GamePlayerMessages.PlayerAnimation, OnPlayerAnimation);
     
        Gameverses.GameMessenger<string, Gameverses.GameNetworkAniStates>.AddListener(Gameverses.GameNetworkPlayerMessages.PlayerAnimation, OnNetworkPlayerAnimation);
        Gameverses.GameMessenger<string, float>.AddListener(Gameverses.GameNetworkPlayerMessages.PlayerInputAxisHorizontal, OnNetworkPlayerInputAxisHorizontal);
        Gameverses.GameMessenger<string, float>.AddListener(Gameverses.GameNetworkPlayerMessages.PlayerInputAxisVertical, OnNetworkPlayerInputAxisVertical);
        Gameverses.GameMessenger<string, float>.AddListener(Gameverses.GameNetworkPlayerMessages.PlayerSpeed, OnNetworkPlayerSpeed);
     
        Gameverses.GameMessenger<Gameverses.GameNetworkingAction, Vector3, Vector3>.AddListener(Gameverses.GameNetworkingMessages.ActionEvent, OnNetworkActionEvent);
    }
 
    public override void OnDisable() {
        //MessengerObject<InputTouchInfo>.RemoveListener(MessengerObjectMessageType.OnEventInputDown, OnInputDown);
        //MessengerObject<InputTouchInfo>.RemoveListener(MessengerObjectMessageType.OnEventInputUp, OnInputUp);
        Messenger<string, string>.RemoveListener(GamePlayerMessages.PlayerAnimation, OnPlayerAnimation);
     
        Messenger<string, Vector3>.RemoveListener("input-axis", OnInputAxis);//"input-axis-" + axisName, axisInput); 
     
        Gameverses.GameMessenger<string, Gameverses.GameNetworkAniStates>.RemoveListener(Gameverses.GameNetworkPlayerMessages.PlayerAnimation, OnNetworkPlayerAnimation);
        Gameverses.GameMessenger<string, float>.RemoveListener(Gameverses.GameNetworkPlayerMessages.PlayerInputAxisHorizontal, OnNetworkPlayerInputAxisHorizontal);
        Gameverses.GameMessenger<string, float>.RemoveListener(Gameverses.GameNetworkPlayerMessages.PlayerInputAxisVertical, OnNetworkPlayerInputAxisVertical);
        Gameverses.GameMessenger<string, float>.RemoveListener(Gameverses.GameNetworkPlayerMessages.PlayerSpeed, OnNetworkPlayerSpeed);
     
        Gameverses.GameMessenger<Gameverses.GameNetworkingAction, Vector3, Vector3>.RemoveListener(Gameverses.GameNetworkingMessages.ActionEvent, OnNetworkActionEvent);
 
    }
 
    public virtual void SetRuntimeData(GamePlayerRuntimeData data) {
        if (data == null) {
            data = new GamePlayerRuntimeData();
        }
     
        runtimeData = data;
     
        // TODO sync if needed... to update 
        // runtime expensive states that can't be polled.
    }
 
    public virtual void Init(GamePlayerControllerState controlState) {
        controllerState = controlState;
     
        // TODO wire in network/local unique ids.
        uuid = System.Guid.NewGuid().ToString();
     
        SetRuntimeData(new GamePlayerRuntimeData());
             
        InitControls();
     
        LoadCharacter(prefabName);       
        LoadWeapons();
     
        // Add weapons and modifiers
     
        // modifierSpeed = gamePlayerAttributes.GetDefense();
     
        // effects init
     
        HidePlayerEffectWarp();
     
        initialized = true;      
    }

    // SPEED

    public virtual float gamePlayerMoveSpeed {
        get {
            return GamePlayerMoveSpeed();
        }
    }

    public virtual float GamePlayerMoveSpeed() {

        float currentSpeed = 0f;
        
        if(thirdPersonController != null) {
            currentSpeed = thirdPersonController.GetSpeed();
        }
        
        if(contextState == GamePlayerContextState.ContextFollowAgent
           || contextState == GamePlayerContextState.ContextFollowAgentAttack
           || contextState == GamePlayerContextState.ContextRandom) {

            if(gamePlayerControllerAnimation.navAgent != null) {

                if(gamePlayerControllerAnimation.navAgent.enabled) {                       
                    //currentSpeed = navAgent.velocity.magnitude + 20;
                    
                    if(gamePlayerControllerAnimation.navAgent.velocity.magnitude > 0f) {
                        currentSpeed = 15f;
                    }
                    else {
                        currentSpeed = 0;    
                    }
                    
                    if(gamePlayerControllerAnimation.navAgent.remainingDistance < gamePlayerControllerAnimation.navAgent.stoppingDistance + 1) {
                        currentSpeed = 0;
                    }
                    
                    if(currentSpeed < gamePlayerControllerAnimation.navAgent.speed) {
                        //currentSpeed = 0;
                    }
                }
            }
        }

        return currentSpeed;
    }

 
    // --------------------------------------------------------------------
    // EFFECTS

    // LINE RENDERERS

    public virtual void PlayerEffectTrailGroundFadeIn() {
        UITweenerUtil.FadeTo(gamePlayerTrailGround,
            UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 1f);
    }

    public virtual void PlayerEffectTrailGroundFadeOut() {
        UITweenerUtil.FadeTo(gamePlayerTrailGround,
            UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 0f);
    }

    public virtual void PlayerEffectTrailBoostFadeIn() {
        UITweenerUtil.FadeTo(gamePlayerTrailBoost,
            UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 1f);
    }

    public virtual void PlayerEffectTrailBoostFadeOut() {
        UITweenerUtil.FadeTo(gamePlayerTrailBoost,
            UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 0f);
    }

    public virtual bool CheckTrailRendererBoost() {
        if (trailRendererBoost == null && gamePlayerTrailBoost != null) {
            trailRendererBoost = gamePlayerTrailBoost.Get<TrailRenderer>();
            return true;
        }
        return false;
    }

    public virtual bool CheckTrailRendererGround() {
        if (trailRendererGround == null && gamePlayerTrailGround != null) {
            trailRendererGround = gamePlayerTrailGround.Get<TrailRenderer>();
            return true;
        }
        return false;
    }

    public virtual void PlayerEffectTrailBoostTime(float time) {
        if (gamePlayerTrailBoost != null) {
            CheckTrailRendererBoost();

            if (trailRendererBoost != null) {
                trailRendererBoost.time = time;
            }
        }
    }

    public virtual void PlayerEffectTrailGroundTime(float time) {
        if (gamePlayerTrailGround != null) {
            CheckTrailRendererGround();

            if (trailRendererGround != null) {
                trailRendererGround.time = time;
            }
        }
    }


    // WARP
     
    public virtual void PlayerEffectWarpFadeOut() {
        PlayerEffectWarpAnimate(200, 0);
    }
 
    public virtual void PlayerEffectWarpFadeIn() {
        PlayerEffectWarpAnimate(0, 200);
    }
 
    public virtual void PlayerEffectWarpAnimate(float fromEmission, float toEmission) {
        effectWarpStart = fromEmission;
        effectWarpEnd = toEmission;
        effectWarpEnabled = true;
    }
 
    public virtual void HandlePlayerEffectWarpAnimateTick() {
        if (effectWarpEnabled && visible) {
            float fadeSpeed = 200f;
            if (effectWarpCurrent < effectWarpEnd) {
                effectWarpCurrent += (Time.deltaTime * fadeSpeed);
                SetPlayerEffectWarp(effectWarpCurrent);
            }
            else if (effectWarpCurrent > effectWarpEnd) {
                effectWarpCurrent -= (Time.deltaTime * fadeSpeed);
                SetPlayerEffectWarp(effectWarpCurrent);
            }
            else {
                effectWarpEnabled = false;
                effectWarpCurrent = effectWarpEnd;
            }
        }
    }
 
    public virtual void SetPlayerEffectWarp(float rate) {
        if (gamePlayerEffectWarp != null) {
            gamePlayerEffectWarp.emissionRate = rate;
        }
    }
 
    public virtual void ShowPlayerEffectWarp() {
        if (gamePlayerEffectWarp != null) {
            SetPlayerEffectWarp(200);
        }
    }
 
    public virtual void HidePlayerEffectWarp() {
        if (gamePlayerEffectWarp != null) {
            SetPlayerEffectWarp(0);
        }
    }

    // PLAYER CIRCLE INDICATOR GROUND
 
    public virtual void ShowPlayerEffectCircleFollow() {
        if (gamePlayerEffectCircleFollow != null) {
            gamePlayerEffectCircleFollow.Play();
        }
    }
 
    public virtual void HidePlayerEffectCircleFollow() {
        if (gamePlayerEffectCircleFollow != null) {
            gamePlayerEffectCircleFollow.Pause();
        }
    }
 
    public virtual void ShowPlayerEffectCircle() {
        if (gamePlayerEffectCircle != null) {
            gamePlayerEffectCircle.Play();
        }
    }
 
    public virtual void HidePlayerEffectCircle() {
        if (gamePlayerEffectCircle != null) {
            gamePlayerEffectCircle.Pause();
        }
    }
 
    public virtual void ShowPlayerEffectCircleStars() {
        if (gamePlayerEffectCircleStars != null) {
            gamePlayerEffectCircleStars.Play();
        }
    }
 
    public virtual void HidePlayerEffectCircleStars() {
        if (gamePlayerEffectCircleStars != null) {
            gamePlayerEffectCircleStars.Pause();
        }
    }
 
    // --------------------------------------------------------------------
    // RUNTIME STATES
 
    public virtual void HandlePlayerAliveState() {
 
        if (runtimeData.health <= 0f) {
            Die();
        }

        //HandlePlayerInactionState();
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

        if(lastIdleActions + UnityEngine.Random.Range(3, 7) < Time.time) {
            lastIdleActions = Time.time;
            if(thirdPersonController.moveSpeed == 0f) {
                update = true;
            }
        }

        if(!update) {
            return;
        }

        // Look at camera

        OnInputAxis(GameTouchInputAxis.inputAxisMove, Vector3.zero.WithY(-1));

        // Randomize animations that are ok for idle

        int randomize = UnityEngine.Random.Range(0, 5);

        if(randomize == 0) {
            for(int i = 0; i < UnityEngine.Random.Range(1, 4); i++) {
                Jump();
            }
        }
        else if(randomize == 1) {
            for(int i = 0; i < UnityEngine.Random.Range(1, 4); i++) {
                Idle();
            }
        }
        else if(randomize == 2) {
            for(int i = 0; i < UnityEngine.Random.Range(1, 4); i++) {
                //StrafeLeft();
            }
        }
        else if(randomize == 3) {
            for(int i = 0; i < UnityEngine.Random.Range(1, 4); i++) {
                //StrafeRight();
            }
        }


    }
 
    // --------------------------------------------------------------------
    // CHARACTER
     
    public virtual bool isMe {
        get {
            if (uuid == Gameverses.UniqueUtil.Instance.currentUniqueId) {
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
            return runtimeData.health > 0f ? true : false;
        }
    }
 
    public virtual bool isRanged {
        get {
            return prefabName.IndexOf("playerGirl") > -1 ? true : false;
        }
    }

    public virtual bool isMelee {
        get {
            return prefabName.IndexOf("playerBoy") > -1 ? false : true;
        }
    }

    public virtual bool isGirl {
        get {
            return prefabName.IndexOf("playerGirl") > -1 ? true : false;
        }
    }

    public virtual bool isBoy {
        get {
            return prefabName.IndexOf("playerBoy") > -1 ? true : false;
        }
    }

    public virtual bool isBotZombie {
        get {
            return (prefabName.ToLower().IndexOf("bot1") > -1)
             ? true : false;
        }
    }
 
    public virtual bool IsPlayerControlled {
        get {
            if (controllerState == GamePlayerControllerState.ControllerPlayer
                || contextState == GamePlayerContextState.ContextInput
                || contextState == GamePlayerContextState.ContextFollowInput
                || uuid == UniqueUtil.Instance.currentUniqueId) {
                return true;
            }
            return false;
        }
    }
 
    public virtual bool IsAgentState() {
        if (controllerState == GamePlayerControllerState.ControllerAgent) {
            return true;
        }
        return false;
    }
 
    public virtual bool IsPlayerState() {
        if (controllerState == GamePlayerControllerState.ControllerPlayer) {
            return true;
        }
        return false;
    }
 
    public virtual bool IsNetworkPlayerState() {
        if (controllerState == GamePlayerControllerState.ControllerNetwork) {
            return true;
        }
        return false;
    }
 
    public virtual bool IsUIState() {
        if (controllerState == GamePlayerControllerState.ControllerUI) {
            return true;
        }
        return false;
    }
 
    public virtual void ChangePlayerState(GamePlayerControllerState controllerStateTo) {
        if (controllerStateTo != controllerState) {
            controllerState = controllerStateTo;
         
            InitControls();
         
            if (controllerState == GamePlayerControllerState.ControllerAgent) {
                if (navMeshAgent != null) {
                    // TODO load script or look for character input.
                    navMeshAgent.enabled = true;
                }
            }
            else if (controllerState == GamePlayerControllerState.ControllerPlayer) {
                if (navMeshAgent != null) {
                    navMeshAgent.Stop();
                    //navMeshAgent.enabled = false;
                }
            }
            else if (controllerState == GamePlayerControllerState.ControllerNetwork) {
                if (navMeshAgent != null) {
                    navMeshAgent.Stop();
                    //navMeshAgent.enabled = false;
                }
                ChangeContextState(GamePlayerContextState.ContextNetwork);
            }
            else if (controllerState == GamePlayerControllerState.ControllerUI) {
                if (navMeshAgent != null) {
                    navMeshAgent.Stop();
                    //navMeshAgent.enabled = false;  
                    if (thirdPersonController != null) {
                        thirdPersonController.getUserInput = true;
                    }                    
                }
            }
        }
    }
     
    public virtual GamePlayerController GetController(Transform transform) {
        if (transform != null) {
            GamePlayerController gamePlayerController = transform.GetComponentInChildren<GamePlayerController>();
            if (gamePlayerController != null) {
                return gamePlayerController;
            }
        }
        return null;
    }

    public virtual void LoadCharacterAttachedSounds() {
        // TODO footsteps over different terrain
        // Foosteps, breathing etc.

        if(audioObjectFootsteps == null) {
            audioObjectFootsteps = GameAudio.PlayEffectObject(transform, "audio_footsteps_default", true);
            if(audioObjectFootsteps != null) {
                if(audioObjectFootsteps.audio != null) {
                    audioObjectFootstepsSource = audioObjectFootsteps.audio;

                    if(audioObjectFootstepsClip == null && audioObjectFootstepsSource.clip != null) {
                        audioObjectFootstepsClip = audioObjectFootsteps.audio.clip;
                    }
                }
            }
        }
    }

    public virtual void HandleCharacterAttachedSounds() {
    
        if(!GameConfigs.isGameRunning) {
            audioObjectFootstepsSource.StopIfPlaying();
            return;
        }

        LoadCharacterAttachedSounds();

        if(gamePlayerMoveSpeed > .1f) {
            ////audioObjectFootstepsSource.volume = 1f;
            float playSpeed = Mathf.InverseLerp(0, initialMaxRunSpeed, gamePlayerMoveSpeed) + 1;
            //LogUtil.Log("playSpeed", playSpeed);
            audioObjectFootstepsSource.pitch = playSpeed;
        }
        else {
            audioObjectFootstepsSource.pitch = 0f;
        }
    
    }
 
    public virtual void LoadCharacter(string prefabNameObject) {
        prefabName = prefabNameObject;
        lastPrefabName = prefabNameObject;
        StartCoroutine(LoadCharacterFromPrefab(prefabName));
    }
 
    public virtual IEnumerator LoadCharacterFromPrefab(string prefabNameObject) {
        string path = 
         Contents.appCacheVersionSharedPrefabCharacters + prefabNameObject;

        GameObject prefabNameObjectItem = Resources.Load(path) as GameObject;
     
        if (prefabNameObjectItem != null) {
            // Remove all current characters
            foreach (Transform t in gamePlayerModelHolderModel.transform) {
                if (!initLoaded) {
                    initLoaded = true;
                    Destroy(t.gameObject);
                }
                else {

                    GameObjectHelper.DestroyGameObject(
                        t.gameObject, GameConfigs.usePooledGamePlayers);
                }
            }
     
            yield return new WaitForEndOfFrame();

            GameObject gameObjectLoad = GameObjectHelper.CreateGameObject(
                prefabNameObjectItem, Vector3.zero, Quaternion.identity, GameConfigs.usePooledGamePlayers);

            gameObjectLoad.transform.parent = gamePlayerModelHolderModel.transform;
            gameObjectLoad.transform.localScale = Vector3.one;
            gameObjectLoad.transform.position = Vector3.zero;
            gameObjectLoad.transform.localPosition = Vector3.zero;
            gameObjectLoad.transform.rotation = gamePlayerModelHolderModel.transform.rotation;
            //gameObjectLoad.transform.localRotation = gamePlayerHolder.transform.localRotation;

            foreach (Transform t in gameObjectLoad.transform) {
                t.localRotation = gamePlayerModelHolderModel.transform.rotation;
            }
         
            if (gamePlayerControllerAnimation != null) {
                gamePlayerControllerAnimation.ResetAnimatedActor(gameObjectLoad);
            }

            if (!gameObjectLoad.Has<GamePlayerControllerAsset>()) {
                gamePlayerControllerAsset = gameObjectLoad.AddComponent<GamePlayerControllerAsset>();
            }
        }
             
        ChangePlayerState(controllerState);
             
        LoadWeapons();
    }
 
    // --------------------------------------------------------------------
    // WEAPONS   
 
    public virtual void LoadWeapons() {
 
        //if(weapons == null) {
        //   weapons = new Dictionary<string, GamePlayerWeapon>();
        //}
     
        //if(weapons.Count == 0) {
        //   // Lock and load...
        //   GamePlayerWeapon weapon = gameObject.AddComponent<GamePlayerWeapon>();
        //   weapon.gameObject.transform.parent = gameObject.transform;
        //   weapons.Add(GamePlayerSlots.slotPrimary, weapon);
        //}
     
        // TODO attach for now, determinswitching.
    }
 
    // --------------------------------------------------------------------
    // EVENTS

    public virtual void OnInputAxis(string name, Vector3 axisInput) {
                     
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        // main
     
        //Debug.Log("OnInputAxis:" + name + "input:" + axisInput);
     
        if (name == GameTouchInputAxis.inputAxisMove) {
         
            //Debug.Log("OnInputAxis:" + name + "input:" + axisInput);
         
            if (thirdPersonController != null) {
             
                //Debug.Log("OnInputAxis:" + name + "input:" + axisInput);

                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {

                    thirdPersonController.horizontalInput = axisInput.x;
                    thirdPersonController.verticalInput = axisInput.y;
                }
            }
        }
        else if (name == GameTouchInputAxis.inputAxisAttack) {
         
            //Debug.Log("OnInputAxis:" + name + "input:" + axisInput);
         
            if (thirdPersonController != null) {
             
                //Debug.Log("OnInputAxis ATTACK:" + name + "input:" + axisInput);
                
                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {

                    thirdPersonController.horizontalInput2 = axisInput.x;
                    thirdPersonController.verticalInput2 = axisInput.y;
                }
            }
        }
        else if (name == GameTouchInputAxis.inputAxisMoveHorizontal) {
                
            //Debug.Log("OnInputAxis:" + name + "input:" + axisInput);
            
            if (thirdPersonController != null) {
                    
                //Debug.Log("OnInputAxis:" + name + "input:" + axisInput);
                
                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {

                    thirdPersonController.horizontalInput = axisInput.x;
                    thirdPersonController.verticalInput = 0f;//thirdPersonController.verticalInput;
                }

                if (axisInput.y > .7f) {
                    //Debug.Log("axisInput.y:" + axisInput.y);
                    Jump();
                }
                else {
                    JumpStop();
                }

            }
        }
        else if (name == GameTouchInputAxis.inputAxisMoveVertical) {
                
            //Debug.Log("OnInputAxis:" + name + "input:" + axisInput);
            
            if (thirdPersonController != null) {
                
                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    //Debug.Log("OnInputAxis:" + name + "input:" + axisInput);
                    thirdPersonController.horizontalInput = 0f;//axisInput.x;
                    thirdPersonController.verticalInput = axisInput.y;
                }
            }
        }
        else if (name == GameTouchInputAxis.inputAxisAttack2DSide2) {
                
            //Debug.Log("OnInputAxis:" + name + "input:" + axisInput);
            
            if (thirdPersonController != null) {
                    
                //Debug.Log("OnInputAxis:" + name + "input:" + axisInput);
                
                //thirdPersonController.horizontalInput = axisInput.x;
                //thirdPersonController.verticalInput = 0f;
                
                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    
                    if (controllerState == GamePlayerControllerState.ControllerPlayer) {
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
        else if (name == GameTouchInputAxis.inputAxisAttack2DSide) {
            
            //Debug.Log("OnInputAxis:" + name + "input:" + axisInput);
            
            if (thirdPersonController != null) {
                
                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    //Debug.Log("OnInputAxis ATTACK:" + name + "input:" + axisInput);
                    
                    thirdPersonController.horizontalInput2 = -axisInput.x;
                    thirdPersonController.verticalInput2 = 0f;//axisInput.y;
                    
                    //UpdateAim(axisInput.x, axisInput.y);
                }
            }
        }
    }
 
    public virtual void OnNetworkActionEvent(Gameverses.GameNetworkingAction actionEvent, Vector3 pos, Vector3 direction) {
         
        if (!GameConfigs.isGameRunning) {
            return;
        }
    
        if (actionEvent.uuidOwner == uuid) {
            AnimatePlayer(actionEvent.code);
        }
    }
     
    public virtual void OnNetworkPlayerAnimation(string uniqueId, Gameverses.GameNetworkAniStates aniState) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (uniqueId == uuid && !isMe) {
            if (lastNetworkAniState != currentNetworkAniState) {
                lastNetworkAniState = currentNetworkAniState;
             
                if (currentNetworkAniState == Gameverses.GameNetworkAniStates.walk) {
                 
                }
                else if (currentNetworkAniState == Gameverses.GameNetworkAniStates.run) {
                 
                }
                else if (currentNetworkAniState == Gameverses.GameNetworkAniStates.attack1) {
                 
                }
                else if (currentNetworkAniState == Gameverses.GameNetworkAniStates.attack2) {
                 
                }
                else if (currentNetworkAniState == Gameverses.GameNetworkAniStates.death) {
                 
                }
                else if (currentNetworkAniState == Gameverses.GameNetworkAniStates.skill1) {
                 
                }
                else if (currentNetworkAniState == Gameverses.GameNetworkAniStates.skill2) {
                 
                }
            }
        }
    }
 
    public virtual void OnNetworkPlayerInputAxisHorizontal(string uniqueId, float horizontalInput) {
        if (uniqueId == uuid && !isMe) {
            if (thirdPersonController != null) {
                thirdPersonController.horizontalInput = horizontalInput;
            }
        }
    }
 
    public virtual void OnNetworkPlayerInputAxisVertical(string uniqueId, float verticalInput) {
        if (uniqueId == uuid && !isMe) {
            if (thirdPersonController != null) {
                thirdPersonController.verticalInput = verticalInput;
            }
        }
    }
 
    public virtual void OnNetworkPlayerSpeed(string uniqueId, float speed) {
        if (uniqueId == uuid && !isMe) {
            if (thirdPersonController != null) {
                thirdPersonController.moveSpeed = speed;
            }            
        }
    }
 
    public override void OnInputDown(InputTouchInfo touchInfo) {
        LogUtil.Log("OnInputDown GameActor");        
     
    }
 
    // --------------------------------------------------------------------
    // COLLISIONS/TRIGGERS

    GamePlayerController collisionController = null;

    public virtual void HandleCollision(Collision collision) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (lastCollision + .2f < Time.time) {
            lastCollision = Time.time;
        }
        else {
            return;
        }

        if (collision.contacts.Length > 0) {
            foreach (ContactPoint contact in collision.contacts) {
                //Debug.DrawRay(contact.point, contact.normal, Color.white);
                     
                Transform t = contact.otherCollider.transform;
                string parentName = t.parent.name;

                bool isObstacle = parentName.Contains("GameObstacle");                  

                bool isLevelObject = parentName.Contains("GameItemObject")
                    || t.name.Contains("(Clone)")
                        || parentName.Contains("(Clone)");                

                bool isPlayerObject = 
                    parentName.Contains("HelmetContainer")
                    || parentName.Contains("Helmet")
                    || parentName.Contains("Facemask")
                        || parentName.Contains("HitCollider")
                        || parentName.Contains("GamePlayerCollider");
                        //|| t.name.Contains("GamePlayerObject")
                        //|| t.name.Contains("GamePlayerEnemy")
                        //|| t.name.Contains("GameEnemy");  

                if(isLevelObject) {
                    GameLevelSprite sprite = t.gameObject.FindTypeAboveRecursive<GameLevelSprite>();
                    if(sprite  == null) {
                        sprite = t.parent.gameObject.GetComponentInChildren<GameLevelSprite>();
                    }
                    if(sprite != null) {                        
                        isLevelObject = true;
                    }
                    else { 
                        isLevelObject = false;
                    }
                }
                                 
                if (isObstacle || isLevelObject) {
                    if (IsPlayerState()) {
                        AudioAttack();
                        Score(1);
                        GamePlayerProgress.SetStatHitsObstacles(1f);
                    }
                }
                else if (isPlayerObject){

                    // handle stat

                    //if (IsPlayerControlled) {
                    collisionController = GameController.GetGamePlayerControllerObject(
                        t.gameObject, false);

                    if(collisionController != null) {

                        if (!IsPlayerControlled) {
                            // we hit a player, so we are an enemy
                            GamePlayerProgress.SetStatHitsReceived(1f);

                        }
                        else {
                            // we hit an enemy, so we are the player
                            GamePlayerProgress.SetStatHits(1f);
                        }
                    }
                    //}

                    // handle hit

                    float power = .1f;

                    runtimeData.health -= power;

                    //contact.normal.magnitude

                    Hit(power);

                    //GamePlayerProgress.Instance.ProcessProgressSpins

                    //GameProfileCharacters.currentProgress.SubtractGamePlayerProgressHealth(power); // TODO get by skill upgrade
                    //GameProfileCharacters.currentProgress.SubtractGamePlayerProgressEnergy(power/2f); // TODO get by skill upgrade

                    Vector3 normal = contact.normal;
                    float magnitude = contact.point.sqrMagnitude;
                    float hitPower = (magnitude * (float)runtimeData.mass) / 110;
                    //Debug.Log("hitPower:" + hitPower);
                    AddImpact(normal, Mathf.Clamp(hitPower, 0f, 80f));
                }
                break;
            }
        }
     
        //if (collision.relativeVelocity.magnitude > 2)
        //    audio.Play();      
    }
 
    //GamePlayerController gamePlayerControllerHit;
        
    //void OnCollisionEnter(Collision collision) {
    //    if(!GameController.shouldRunGame) {
    //            return;
    //    }
        
    //GameObject target = collision.collider.gameObject;
    //LogUtil.Log("hit object:" + target);
        
    //if(target != null) {
    //    gamePlayerControllerHit = target.GetComponent<GamePlayerController>();
         
    //   if(gamePlayerControllerHit != null) {
             
    //DeviceUtil.Vibrate();
    //       LogUtil.Log("hit another game player");
    //   }
    //}
    // }
     
    public virtual void OnTriggerEnter(Collider collider) {
        // Check if we hit an actual destroyable sprite
        if (!GameController.shouldRunGame) {
            return;
        }
                     
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (controllerState == GamePlayerControllerState.ControllerPlayer) {
     
            string colliderName = collider.name;
         
            if (colliderName.Contains("GameGoalZone")) {
                LogUtil.Log("GameGoalZone: " + colliderName);
                GameController.GamePlayerGoalZone(collider.transform.gameObject);
            }
            else if (colliderName.Contains("GameBadZone")) {
                LogUtil.Log("GameBadZone: " + colliderName);
                GameController.GamePlayerOutOfBounds();
            }
            else if (colliderName.Contains("GameZone")) {
                LogUtil.Log("GameZone: " + colliderName);
                GameController.GamePlayerGoalZoneCountdown(collider.transform.gameObject);
            }
        }
        
        //GameObject target = collider.gameObject;
     
        //LogUtil.Log("hit object:" + target);
        
        //if(target != null) {
        //    GamePlayerController gamePlayerController = target.GetComponent<GamePlayerController>();
         
        //   if(gamePlayerController != null) {
             
        //DeviceUtil.Vibrate();
        //       LogUtil.Log("hit another game player");
        //   }
        //}
    
    }
 
    // --------------------------------------------------------------------
    // ANIMATION

    public virtual void AnimatePlayer(string animationName) {
        if (animationName == GamePlayerAnimationType.skill) {
            InputSkill();
        }
        else if (animationName == GamePlayerAnimationType.attack) {
            InputAttack();
        }
        else if (animationName == GamePlayerAnimationType.attackAlt) {
            InputAttackAlt();
        }
        else if (animationName == GamePlayerAnimationType.attackRight) {
            InputAttackRight();
        }
        else if (animationName == GamePlayerAnimationType.attackLeft) {
            InputAttackLeft();
        }
        else if (animationName == GamePlayerAnimationType.defend) {
            InputDefend();
        }
        else if (animationName == GamePlayerAnimationType.defendAlt) {
            InputDefendAlt();
        }
        else if (animationName == GamePlayerAnimationType.defendRight) {
            InputDefendRight();
        }
        else if (animationName == GamePlayerAnimationType.defendLeft) {
            InputDefendLeft();
        }
        else if (animationName == GamePlayerAnimationType.death
            || animationName == GamePlayerAnimationType.die) {
            InputDie();
        }
        else if (animationName == GamePlayerAnimationType.jump) {
            InputJump();
        }
        else if (animationName == GamePlayerAnimationType.strafeLeft) {
            InputStrafeLeft();
        }
        else if (animationName == GamePlayerAnimationType.strafeRight) {
            InputStrafeRight();
        }
        else if (animationName == GamePlayerAnimationType.use) {
            InputUse();
        }
    }
 
    public virtual void OnPlayerAnimation(string animationName, string uniqueId) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        LogUtil.Log("OnPlayerAnimation: " + animationName);
     
        /*
     if(Network.isClient || Network.isServer) {
     
         // call them over the network
         
         if(IsPlayerControlled) {
             Gameverses.GameNetworkingAction actionEvent = new Gameverses.GameNetworkingAction();
             actionEvent.uuid = UniqueUtil.Instance.CreateUUID4();
             actionEvent.uuidOwner = uuid;
             actionEvent.code = animationName;
             actionEvent.type = Gameverses.GameNetworkingPlayerTypeMessages.PlayerTypeAction;
             
             
             //Gameverses.GameversesGameAPI.Instance.SendActionMessage(actionEvent, Vector3.zero, Vector3.zero);
         }
     }
     else  {
     */
        if (IsPlayerControlled) {
            if (gamePlayerControllerAnimation != null) {
                AnimatePlayer(animationName);
            }
        }
        //}
    }
 
    // --------------------------------------------------------------------
    // STATE/RESET

    public virtual void ResetPosition() {

        foreach (Transform t in gamePlayerModelHolderModel.transform) {
            t.position.Reset();
            t.localPosition.Reset();
            t.rotation.Reset();
            t.localRotation.Reset();
            t.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        transform.position = Vector3.zero.WithY(1.5f);
    }
         
    public virtual void Reset() {
        dying = false;
        effectWarpEnabled = false;
        runtimeData = new GamePlayerRuntimeData();

        GetPlayerProgress();

        if (thirdPersonController != null) {
            thirdPersonController.Reset();
            if (gamePlayerControllerAnimation != null) {
                gamePlayerControllerAnimation.ResetPlayState();
            }
            if (controllerState == GamePlayerControllerState.ControllerAgent) {
                navMeshAgent.enabled = true;
            }
        }

        ResetPosition();

        impact = Vector3.zero;
        dying = false;
        effectWarpEnabled = true;
        effectWarpEnd = 0f;
    }
 
    public virtual void Remove() {
        if (!IsPlayerControlled) {

            foreach (Transform t in gamePlayerModelHolderModel.transform) {
                GameObjectHelper.DestroyGameObject(t.gameObject, GameConfigs.usePooledGamePlayers);
            }

            GameObjectHelper.DestroyGameObject(gameObject, GameConfigs.usePooledGamePlayers);
        }
    }
 
    // --------------------------------------------------------------------
    // COMBAT/HIT/ATTACK
 
    public virtual void SendUse() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.use,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }
    
    public virtual void SendJump() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.jump,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendStrafeLeft() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.strafeLeft,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendStrafeRight() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.strafeRight,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendBoost() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.boost,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendSkill() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.skill,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }
    
    public virtual void SendAttack() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.attack,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendAttackAlt() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.attackAlt,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendAttackRight() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.attackRight,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendAttackLeft() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.attackLeft,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendDefend() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.defend,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendDefendAlt() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.defendAlt,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendDefendRight() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.defendRight,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendDefendLeft() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.defendLeft,
            Gameverses.UniqueUtil.Instance.currentUniqueId);
    }
 
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

    public virtual void InputBoost() {
        Boost();
    }
 
    public virtual void InputUse() {
        // USE

    }

    public virtual void InputSkill() {
        Skill();
    }

    public virtual void InputMagic() {
        //Magic();
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

    public virtual void Hit(float power) {
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (controllerState == GamePlayerControllerState.ControllerPlayer) {          
            GameHUD.Instance.ShowHitOne((float)(1.5 - runtimeData.health));
            Score(2 * power);
            DeviceUtil.Vibrate();
        }
        else {
            //bool allow = false;

            if (lastHit + .3f < Time.time) {
                lastHit = Time.time;
                //allow = true;
            }
            else {
                return;
            }

            Score(-1);
        }
     
        //GameUIPanelOverlays.Instance.ShowOverlayWhiteFlash();
        //GameHUD.Instance.ShowHitOne();
             
        if (controllerState == GamePlayerControllerState.ControllerAgent) {
            power = power * 1;
        }
        else if (controllerState == GamePlayerControllerState.ControllerPlayer) {
            power = power * 1;
        }
     
        runtimeData.health -= power * .1f;

        AudioHit();

        if (runtimeData.health < 0) {
            Die();
        }
    }
 
    
    public virtual void Idle() {
        if (isDead) {
            return;
        }
        
        //thirdPersonController.Idle();
        
        gamePlayerControllerAnimation.DidIdle();
    }

    public virtual void Jump() {
        if (isDead) {
            return;
        }
        
        thirdPersonController.Jump();
        
        gamePlayerControllerAnimation.DidJump();
        
        if (gamePlayerEffectSkill != null) {
            gamePlayerEffectSkill.Emit(1);
        }
    }
    
    public virtual void JumpStop() {
        if (isDead) {
            return;
        }
        
        thirdPersonController.JumpStop();
    }

    // SKILL
 
    public virtual void Skill() {
        if (isDead) {
            return;
        }
     
        gamePlayerControllerAnimation.DidSkill();
     
        if (gamePlayerEffectSkill != null) {
            gamePlayerEffectSkill.Emit(1);
        }
    }

    // STRAFE

    // STRAFE LEFT

    public virtual void DidStrafeLeft(Vector3 dir) {
        StrafeLeft(dir);
    }

    public virtual void DidStrafeLeft(float power) {
        StrafeLeft(power);
    }

    public virtual void DidStrafeLeft(Vector3 dir, float power) {
        StrafeLeft(dir, power);
    }

    public virtual void StrafeLeft() {
        Vector3 dir = transform.TransformPoint(transform.localPosition.WithX(-1));//Vector3.zero.WithX(-1);
        float power = 10f + 5f * (float)runtimeRPGData.modifierAttack;
        StrafeLeft(dir, power);
    }

    public virtual void StrafeLeft(Vector3 dir) {
        float power = 10f + 5f * (float)runtimeRPGData.modifierAttack;
        StrafeLeft(dir, power);
    }

    public virtual void StrafeLeft(float power) {
        Vector3 dir = transform.TransformPoint(transform.localPosition.WithX(-1));//Vector3.zero.WithX(-1);
        StrafeLeft(dir, power);
    }

    public virtual void StrafeLeft(Vector3 dir, float power) {
        LogUtil.Log("GamePlayerController:DidStrafeLeft:");

        if (isDead) {
            return;
        }

        if (Time.time > lastStrafeLeftTime + 1f) {
            gamePlayerControllerAnimation.DidStrafeLeft();

            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cuts, 1f);
            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cutsLeft, 1f);

            lastStrafeLeftTime = Time.time;
            StartCoroutine(DidStrafeLeftCo(dir, power));
        }
    }

    public virtual IEnumerator DidStrafeLeftCo(Vector3 dir, float power) {
        AddForce(dir, power, false);
        yield return new WaitForEndOfFrame();
    }

    // STRAFE RIGHT


    public virtual void DidStrafeRight(Vector3 dir) {
        StrafeRight(dir);
    }

    public virtual void DidStrafeRight(float power) {
        StrafeRight(power);
    }

    public virtual void DidStrafeRight(Vector3 dir, float power) {
        StrafeRight(dir, power);
    }

    public virtual void StrafeRight() {
        Vector3 dir = transform.localPosition.WithX(1);
        float power = 10f + 5f * (float)runtimeRPGData.modifierAttack;
        StrafeRight(dir, power);
    }

    public virtual void StrafeRight(Vector3 dir) {
        float power = 10f + 5f * (float)runtimeRPGData.modifierAttack;
        StrafeRight(dir, power);
    }

    public virtual void StrafeRight(float power) {
        Vector3 dir = transform.localPosition.WithX(1);
        StrafeRight(dir, power);
    }

    public virtual void StrafeRight(Vector3 dir, float power) {
        LogUtil.Log("GamePlayerController:DidStrafeRight:");

        if (isDead) {
            return;
        }
        if (Time.time > lastStrafeRightTime + 1f) {

            gamePlayerControllerAnimation.DidStrafeRight();

            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cuts, 1f);
            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cutsRight, 1f);

            lastStrafeRightTime = Time.time;
            StartCoroutine(DidStrafeRightCo(dir, power));
        }
    }

    public virtual IEnumerator DidStrafeRightCo(Vector3 dir, float power) {
        AddForce(dir, power, false);
        yield return new WaitForEndOfFrame();
    }

    // BOOST

    public virtual void DidBoost(Vector3 dir) {
        Boost(dir);
    }

    public virtual void DidBoost(float power) {
        Boost(power);
    }

    public virtual void DidBoost(Vector3 dir, float power) {
        Boost(dir, power);
    }

    public virtual void Boost() {
        Vector3 dir = transform.forward;
        float power = 10f + 5f * (float)runtimeRPGData.modifierAttack;
        Boost(dir, power);
    }

    public virtual void Boost(Vector3 dir) {
        float power = 10f + 5f * (float)runtimeRPGData.modifierAttack;
        Boost(dir, power);
    }

    public virtual void Boost(float power) {
        Vector3 dir = transform.forward;
        Boost(dir, power);
    }

    public virtual void Boost(Vector3 dir, float power) {
        if (isDead) {
            return;
        }
        LogUtil.Log("GamePlayerController:Boost:");
        if (Time.time > lastBoostTime + 1f) {
            lastBoostTime = Time.time;

            gamePlayerControllerAnimation.DidBoost();
            GamePlayerProgress.SetStatBoosts(1f);
            StartCoroutine(DidBoostCo(dir, power));
        }
    }

    public virtual IEnumerator DidBoostCo(Vector3 dir, float power) {
        AddForce(dir, power, false);
        yield return new WaitForEndOfFrame();
    }

    // SPIN

    public virtual void DidSpin(Vector3 dir) {
        Spin(dir);
    }

    public virtual void DidSpin(float power) {
        Spin(power);
    }

    public virtual void DidSpin(Vector3 dir, float power) {
        Spin(dir, power);
    }

    public virtual void Spin() {
        Vector3 dir = transform.localPosition.WithZ(-1);
        float power = 10f + 5f * (float)runtimeRPGData.modifierAttack;
        Boost(dir, power);
    }

    public virtual void Spin(Vector3 dir) {
        float power = 10f + 5f * (float)runtimeRPGData.modifierAttack;
        Boost(dir, power);
    }

    public virtual void Spin(float power) {
        Vector3 dir = transform.localPosition.WithZ(-1);
        Boost(dir, power);
    }

    public virtual void Spin(Vector3 dir, float power) {
        if (isDead) {
            return;
        }
        LogUtil.Log("GamePlayerController:Spin:");
        if (Time.time > lastSpinTime + 1f) {
            lastSpinTime = Time.time;

            //UITweenerUtil.RotateTo(gamePlayerModelHolder, UITweener.Method.Linear, UITweener.Style.Once, .2f, 0f, Vector3.zero.WithY(179));
            //UITweenerUtil.RotateTo(gamePlayerModelHolder, UITweener.Method.Linear, UITweener.Style.Once, .2f, .2f, Vector3.zero.WithY(290));
            //UITweenerUtil.RotateTo(gamePlayerModelHolder, UITweener.Method.Linear, UITweener.Style.Once, .2f, .4f, Vector3.zero.WithY(0));

            playerSpin = true;

            //iTween.RotateTo(gamePlayerModelHolderModel, iTween.Hash("y", Vector3.zero.WithY(179).y, "time", .2f, "delay", 0f, "easetype", "linear", "space", "local"));
            //iTween.RotateTo(gamePlayerModelHolderModel, iTween.Hash("y", Vector3.zero.WithY(290).y, "time", .2f, "delay", .21f, "easetype", "linear", "space", "local"));
            //iTween.RotateTo(gamePlayerModelHolderModel, iTween.Hash("y", Vector3.zero.WithY(0).y, "time", .2f, "delay", .41f, "easetype", "linear", "space", "local"));

            gamePlayerControllerAnimation.DidSpin();
            GamePlayerProgress.SetStatSpins(1f);

            StartCoroutine(DidSpinCo(dir, power));
        }
    }

    public virtual IEnumerator DidSpinCo(Vector3 dir, float power) {
        AddForce(dir, power, false);
        yield return new WaitForEndOfFrame();
    }

    // DIE

    public virtual void DidDie(Vector3 dir) {
        Die(dir);
    }

    public virtual void DidDie(float power) {
        Die(power);
    }

    public virtual void DidDie(Vector3 dir, float power) {
        Die(dir, power);
    }

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
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (Time.time + 3f > lastDie) {
            lastDie = Time.time;
        }
        else {
            return;
        }

        if (thirdPersonController != null) {
            thirdPersonController.removing = true;
        }
             
        if (isDead && dying) {
            return;
        }

        dying = true;

        gamePlayerControllerAnimation.DidDie();

        if (IsPlayerControlled) {
            GamePlayerProgress.SetStatDeaths(1f);
        }
        else {
            GamePlayerProgress.SetStatKills(1f);
        }
     
        if (gamePlayerControllerAnimation != null) {
            gamePlayerControllerAnimation.isDead = true;
        }
     
        //if(controllerState == GamePlayerControllerState.ControllerAgent) {
        StopNavAgent();
        //}
     
        if (gamePlayerEffectDeath != null) {
            gamePlayerEffectDeath.Emit(1);
        }
     
        if (IsPlayerControlled) {
            PlayerEffectWarpFadeIn();
        }
     
        AudioDie();
     
        // fade out 
        UnityEngine.SkinnedMeshRenderer[] skinRenderersCharacter 
         = gamePlayerHolder.GetComponentsInChildren<SkinnedMeshRenderer>();
     
        foreach (SkinnedMeshRenderer skinRenderer in skinRenderersCharacter) {
         
            UITweenerUtil.FadeTo(skinRenderer.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 1f, 2f, 0f);        
        }
     
        Invoke("Remove", 5);
    }

    public virtual void StartNavAgent() {
        if (navMeshAgent != null) {
            navMeshAgent.enabled = true;
            navMeshAgent.Resume();
        }
        if (navMeshAgentController != null) {
            navMeshAgentController.StartAgent();
        }
        if (navMeshAgentFollowController != null) {
            navMeshAgentFollowController.StartAgent();
        }
    }

    public virtual void StopNavAgent() {

        if (navMeshAgent != null) {
            if(navMeshAgent.enabled) {
                navMeshAgent.Stop(true);
                navMeshAgent.enabled = false;
            }
        }
        if (navMeshAgentController != null) {
            navMeshAgentController.StopAgent();
        }
        if (navMeshAgentFollowController != null) {
            navMeshAgentFollowController.StopAgent();
        }
    }
 
    public virtual void AttackAlt() {    
        if (isDead) {
            return;
        }    
        gamePlayerControllerAnimation.DidAttackAlt();
        Invoke("AttackEffect", .5f);
    }
 
    public virtual void AttackLeft() {       
        if (isDead) {
            return;
        }
        gamePlayerControllerAnimation.DidAttackLeft();
        Invoke("AttackEffect", .5f);
    }
 
    public virtual void AttackRight() {      
        if (isDead) {
            return;
        }
        gamePlayerControllerAnimation.DidAttackRight();
        Invoke("AttackEffect", .5f);
    }
 
    public virtual void AttackEffect() {
        if (isDead) {
            return;
        }
        if (gamePlayerEffectAttack != null) {
            gamePlayerEffectAttack.Emit(1);
        }
    }
 
    public virtual void DidAttack() {    
        LogUtil.Log("GamePlayerController:DidAttack:");
        Attack();
    }
 
    public virtual void Attack() {       
        if (isDead) {
            return;
        }
        if (Time.time > lastAttackTime + 1f) {
            lastAttackTime = Time.time;
            StartCoroutine(DidAttackCo());
        }
    }
 
    public virtual IEnumerator DidAttackCo() {
        yield return new WaitForSeconds(.5f);
        ActionAttack();
    }
 
    public virtual void CastAttack() {       
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        // if (Physics.Linecast(transform.position, thirdPersonController.aimingDirection)) {
            
        //}
     
        float distance = 3f;
     
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
        if (thirdPersonController != null) {
            directionAttack = thirdPersonController.aimingDirection;
        }        
     
        Debug.DrawRay(transform.position, directionAttack * distance);
        hits = Physics.RaycastAll(transform.position, directionAttack, distance);
        int i = 0;
        while (i < hits.Length) {
            RaycastHit hit = hits[i];
            Transform hitObject = hit.transform;
         
            //if(hitObject.name.IndexOf("Game") > -1) {
            if (hitObject != null) {
                GamePlayerController playerController = GetController(hitObject);
                if (playerController != null) {
                 
                    ScoreAttack();
                 
                    playerController.Hit(1f);
                 
                    playerController.InputAttack();
                }
            }
            //}
         
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
                     
        LogUtil.Log("GamePlayerController:ActionAttack:");

        //thirdPersonController.ApplyAttack();
     
        //gamePlayerControllerAnimation.DidAttack();

        gamePlayerControllerAnimation.DidAttack();
     
        Invoke("AttackEffect", .5f);
     
        LogUtil.Log("Attacking:");
     
        // shoot ray for type of character
     
        CastAttack();

        AudioAttack();

        // Fire weapons
     
        if (weaponPrimary != null) {
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
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        Shoot(1);       
    }
    
    public virtual void Shoot(int number) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        AnimateShoot(); 
        
        //GameController.ProcessStatShot();
        
        runtimeData.savesLaunched += number;
        Messenger<double>.Broadcast(GameMessages.launch, number);
        Messenger<double>.Broadcast(GameMessages.ammo, -number);
    }
        
    public virtual void FindGamePlayerCamera() {
        if (gameCameraSmoothFollow == null || gameCameraSmoothFollowGround == null) {
            foreach (GameCameraSmoothFollow cam in ObjectUtil.FindObjects<GameCameraSmoothFollow>()) {
                if (cam.name.Contains("Ground")) {
                    gameCameraSmoothFollowGround = cam;
                }
                else {
                    gameCameraSmoothFollow = cam;
                }
            }
        }
    }

    float axisDeadZone = .05f;
    
    public virtual void UpdateAim(float x, float y) {
        
        FindGamePlayerCamera();
        
        GameObject model = gamePlayerModelHolder;
        float cameraAdjustment = 8f;
        axisDeadZone = .05f;
        
        if (Math.Abs(x) > axisDeadZone
            && Math.Abs(y) > axisDeadZone) {
            
            currentPosition = model.transform.position;
            
            currentAimPosition = -currentPosition
                .WithX(currentPosition.x + (x * 100))
                .WithY(currentPosition.y + (y * 100));
            
            //float angle = Vector3.Angle(currentPosition, currentAimPosition);
            float dist = Vector3.Distance(currentPosition, currentAimPosition);
            
            model.transform.localScale = Mathf.Clamp(dist * .1f, .5f, 1.3f) * Vector3.one;  
            
            Vector3 lookAtPos = model.transform.position + (currentAimPosition * 10);
            
            model.transform.LookAt(lookAtPos);
            
            float amount = Mathf.Abs(dist);
            
            if (gamePlayerEffectAim != null) {
                gamePlayerEffectAim.enableEmission = true;
                gamePlayerEffectAim.emissionRate = amount * 2;
                gamePlayerEffectAim.startLifetime = amount / 400f;
                gamePlayerEffectAim.startSpeed = amount;
                gamePlayerEffectAim.Play();
            }
            
            //lineAim..SetLine3D(Color.white, model.transform.position, lookAtPos);
            
            //LogUtil.Log("UpdateAim:currentAimPosition:", currentAimPosition);
            
            if (gameCameraSmoothFollow != null) {
                gameCameraSmoothFollow.offset.x = 
                    (gameCameraSmoothFollow.offsetInitial.x + -(x * cameraAdjustment));
                
                gameCameraSmoothFollow.offset.y = 
                    (gameCameraSmoothFollow.offsetInitial.y + -(y * cameraAdjustment));
                
                gameCameraSmoothFollow.SetZoom(-((x + y) * cameraAdjustment));
            }
            if (gameCameraSmoothFollowGround != null) {
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
            
            if (gamePlayerEffectAim != null) {
                gamePlayerEffectAim.enableEmission = false;
                gamePlayerEffectAim.emissionRate = 1;
                gamePlayerEffectAim.Stop();
            }
            
            if (gameCameraSmoothFollow != null) {
                gameCameraSmoothFollow.Reset();
            }
            if (gameCameraSmoothFollowGround != null) {
                gameCameraSmoothFollowGround.Reset();
            }
        }
    }
    
    public virtual void AnimateShoot() {
        
        if (gamePlayerModelHolderModel != null) {
            foreach (Animation anim in gamePlayerModelHolderModel.GetComponentsInChildren<Animation>()) {
                if (anim != null) {
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

    public virtual void Scores(double val) {

        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        runtimeData.scores += val;

        Messenger<double>.Broadcast(GameMessages.scores, val);

        GamePlayerProgress.SetStatScores(val);
    }
 
    public virtual void Score(double val) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        runtimeData.score += val;
        
        Messenger<double>.Broadcast(GameMessages.score, val);
        
        GamePlayerProgress.SetStatScore(val);
    }
        
    public virtual void Ammo(double val) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        runtimeData.ammo += val;
        runtimeData.collectedAmmo += val;

        Messenger<double>.Broadcast(GameMessages.ammo, val);

        GamePlayerProgress.SetStatAmmo(val);
    }
    
    public virtual void Save(double valAdd) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        runtimeData.saves += valAdd;
        Messenger<double>.Broadcast(GameMessages.save, valAdd);
    }
 
    public virtual void Tackle(GamePlayerController gamePlayerControllerTo) {
        Tackle(gamePlayerControllerTo, 1f);
    }

    public virtual void Tackle(GamePlayerController gamePlayerControllerTo, float power) {
     
        if (gamePlayerControllerTo == null) {
            return;
        }

        //bool allowTackle = false;

        if (lastTackle + .1f < Time.time) {
            lastTackle = Time.time;
            //allowTackle = true;
        }
        else {
            return;
        }

        //transform.LookAt(gamePlayerControllerTo.transform);

        positionPlayer = transform.position;
        positionTackler = gamePlayerControllerTo.transform.position;
     
        gamePlayerControllerAnimation.Attack();

        //Attack();
     
        AddImpact(positionTackler - positionPlayer, power, false);
     
    }

    public virtual void AddForce(Vector3 dir, float force) {
        AddImpact(dir, force, false);
    }

    public virtual void AddForce(Vector3 dir, float force, bool damage) {
        AddImpact(dir, force, damage);
    }

    public virtual void AddImpact(Vector3 dir, float force) {
        AddImpact(dir, force, true);
    }

    public virtual void UpdatePlayerProgress(float energy, float health) {
        StartCoroutine(UpdatePlayerProgressCo(energy, health));
    }

    public virtual IEnumerator UpdatePlayerProgressCo(float energy, float health) {
        yield return new WaitForEndOfFrame();
        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergyAndHealthRuntime(
            energy, health);
    }
 
    // call this function to add an impact force:
    public virtual void AddImpact(Vector3 dir, float force, bool damage) {

        dir.Normalize();
     
        if (dir.y < 0) {
            dir.y = 0;//-dir.y; // reflect down force on the ground
        }

        if (damage) {
            force = Mathf.Clamp(force, 0f, 100f);
        }

        impact += dir.normalized * force / (float)runtimeData.mass;

        if (damage) {
            runtimeData.hitCount++;

            if (IsPlayerControlled && damage) {
                UpdatePlayerProgress(
                    (float)(-.01f * Mathf.Clamp(force / 10f, .3f, 1f)),
                    (float)(-.01f * Mathf.Clamp(force / 10f, .3f, 1f)));
            }
        }
        //LogUtil.Log("AddImpact:name:", transform.name + "impact:" + impact.x);
    }
 
    public virtual void  UpdatePhysicsState() {

        //Vectrosity.VectorLine.SetLine (Color.red, transform.position, impact);

        // apply the impact force:
        //if (impact.magnitude > 0.3f) {
        characterController.Move(impact * Time.deltaTime);
        //}

        float trailTime =
            (Math.Abs(impact.x) +
            Math.Abs(impact.y) +
            Math.Abs(impact.z)) * 5f;

        if (IsPlayerControlled) {
            PlayerEffectTrailBoostTime(trailTime * thirdPersonController.moveSpeed);
            PlayerEffectTrailGroundTime(-trailTime + thirdPersonController.moveSpeed);
        }

        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }
 
    // --------------------------------------------------------------------
    // AUDIO
 
    public virtual void AudioAttack() {
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (lastAudioPlayedAttack + 1 > Time.time) {
            return;
        }
        else {
            lastAudioPlayedAttack = Time.time;
        }

        if (controllerState == GamePlayerControllerState.ControllerPlayer) {

            if (isGirl) {
                //GameAudio.PlayEffect(transform, "shotgun_shot");
                int randAudio = UnityEngine.Random.Range(1, 5);
                GameAudio.PlayEffect(transform, "audio_football_hit_good_" + randAudio.ToString());
            }
            else {
                //GameAudio.PlayEffect(transform, "attack-sword-hit-1");
                int randAudio = UnityEngine.Random.Range(1, 5);
                GameAudio.PlayEffect(transform, "audio_football_hit_good_" + randAudio.ToString());
            }
        }
        else {
            //if(isBotZombie) {
            int randAudio = UnityEngine.Random.Range(1, 5);
            GameAudio.PlayEffect(transform, "audio_football_hit_good_" + randAudio.ToString());
            //}
        }

    }

    public virtual void AudioHit() {
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (lastAudioPlayedHit + 1 > Time.time) {
            return;
        }
        else {
            lastAudioPlayedHit = Time.time;
        }

        if (controllerState == GamePlayerControllerState.ControllerPlayer) {

            if (isGirl) {
                //GameAudio.PlayEffect(transform, "hit-girl-grunt-2");
                int randAudio = UnityEngine.Random.Range(1, 5);
                GameAudio.PlayEffect(transform, "audio_football_hit_good_" + randAudio.ToString());
            }
            else {
                //GameAudio.PlayEffect(transform, "hit-grunt-3");
                int randAudio = UnityEngine.Random.Range(1, 5);
                GameAudio.PlayEffect(transform, "audio_football_hit_good_" + randAudio.ToString());
            }
        }
        else {
            //if(isBotZombie) {
            int randAudio = UnityEngine.Random.Range(1, 5);
            GameAudio.PlayEffect(transform, "audio_football_hit_good_" + randAudio.ToString());
            //}
        }

    }

    public virtual void AudioDie() {
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (lastAudioPlayedDie + 1 > Time.time) {
            return;
        }
        else {
            lastAudioPlayedDie = Time.time;
        }

        if (controllerState == GamePlayerControllerState.ControllerPlayer) {

            GameAudioController.Instance.PlayOh();
            GameAudioController.Instance.PlayWhistle();

            if (isGirl) {
                //GameAudio.PlayEffect(transform, "hit-girl-grunt-2");
                int randAudio = UnityEngine.Random.Range(1, 3);
                GameAudio.PlayEffect(transform, "audio_football_grunts_" + randAudio.ToString());
            }
            else {
                //GameAudio.PlayEffect(transform, "hit-grunt-3");
                int randAudio = UnityEngine.Random.Range(1, 3);
                GameAudio.PlayEffect(transform, "audio_football_grunts_" + randAudio.ToString());
            }
        }
        else {
            //if(isBotZombie) {
            int randAudio = UnityEngine.Random.Range(1, 3);
            GameAudio.PlayEffect(transform, "audio_football_grunts_" + randAudio.ToString());
            //}
        }
    }    
 
    // --------------------------------------------------------------------
    // NETWORK
 
 
    public virtual void UpdateNetworkContainer(string uniqueId) {
     
        uuid = uniqueId;
     
        FindNetworkContainer(uniqueId);      
     
        if (currentNetworkPlayerContainer != null) {
            currentNetworkPlayerContainer.networkViewObject.observed = currentNetworkPlayerContainer;
            currentNetworkPlayerContainer.gamePlayer = gameObject;
            if (thirdPersonController != null) {
                currentNetworkPlayerContainer.currentSpeedNetwork = thirdPersonController.moveSpeed;
                currentNetworkPlayerContainer.verticalInputNetwork = thirdPersonController.verticalInput;
                currentNetworkPlayerContainer.horizontalInputNetwork = thirdPersonController.horizontalInput;
            }
            currentNetworkPlayerContainer.running = true;                
        }    
    }
 
    public virtual Gameverses.GameNetworkPlayerContainer FindNetworkContainer(string uniqueId) {
     
     
        if (currentNetworkPlayerContainer != null) {
            if (currentNetworkPlayerContainer.uuid == uniqueId) {
                return currentNetworkPlayerContainer;
            }
        }
     
        if (Time.time > lastNetworkContainerFind + 5f) {
            lastNetworkContainerFind = Time.time;
            if (GameController.Instance.gameState == GameStateGlobal.GameStarted) {
                foreach (Gameverses.GameNetworkPlayerContainer playerContainer in ObjectUtil.FindObjects<Gameverses.GameNetworkPlayerContainer>()) {
                    if (playerContainer.uuid == uniqueId) {
                        currentNetworkPlayerContainer = playerContainer;
                        return currentNetworkPlayerContainer;
                    }
                }
            }
        }
     
        return null;
    }
 
    public virtual bool HasNetworkContainer(string uniqueId) {

        foreach (Gameverses.GameNetworkPlayerContainer playerContainer in ObjectUtil.FindObjects<Gameverses.GameNetworkPlayerContainer>()) {
            if (playerContainer.uuid == uniqueId) {
                currentNetworkPlayerContainer = playerContainer;
                return true;
            }
        }
     
        return false;
    }
 
    public virtual void UpdateNetworkContainerFromSource(string uniqueId) {
     
        uuid = uniqueId;
     
        FindNetworkContainer(uniqueId);
     
        if (currentNetworkPlayerContainer != null) {
            if (thirdPersonController != null) {
                currentNetworkPlayerContainer.currentSpeedNetwork = thirdPersonController.moveSpeed;
                currentNetworkPlayerContainer.verticalInputNetwork = thirdPersonController.verticalInput;
                currentNetworkPlayerContainer.horizontalInputNetwork = thirdPersonController.horizontalInput;
            }
            currentNetworkPlayerContainer.running = true;                
        }    
    }
 
    public virtual void ChangeContextState(GamePlayerContextState contextStateTo) {
        if (contextStateTo != contextState) {
            contextState = contextStateTo;
         
            if (thirdPersonController != null) {
                thirdPersonController.isNetworked = false;
            }
         
            if (contextState == GamePlayerContextState.ContextFollowAgent
                || contextState == GamePlayerContextState.ContextFollowAgentAttack
                || contextState == GamePlayerContextState.ContextRandom
                || contextState == GamePlayerContextState.ContextScript) {
                if (navMeshAgent != null) {
                    // TODO load script or look for character input.
                    navMeshAgent.enabled = true;
                }
            }
            else if (contextState == GamePlayerContextState.ContextInput
                || contextState == GamePlayerContextState.ContextFollowInput) {
                if (navMeshAgent != null) {
                    navMeshAgent.Stop();
                    //navMeshAgent.enabled = false;
                }
            }
            else if (contextState == GamePlayerContextState.ContextNetwork) {
                if (navMeshAgent != null) {
                    navMeshAgent.Stop();
                    //navMeshAgent.enabled = false;
                }
                thirdPersonController.isNetworked = true;
            }
            else if (contextState == GamePlayerContextState.ContextUI) {
                if (navMeshAgent != null) {
                    navMeshAgent.Stop();
                    //navMeshAgent.enabled = false;
                }
            }
        }
    }
 
    // --------------------------------------------------------------------
    // INPUT
 
    public override bool HitObject(GameObject go, InputTouchInfo inputTouchInfo) {
        Ray screenRay = Camera.main.ScreenPointToRay(inputTouchInfo.position3d);
        RaycastHit hit;
     
        if (Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {
            if (hit.transform.gameObject == go) {
                LogUtil.Log("HitObject GameActor");
                return true;
            }
        }
        return false;
    }
 
    public override void OnInputUp(InputTouchInfo touchInfo) {
        //LogUtil.Log("OnInputDown GameActor");
    }

    // HANDLE PROGRESS

    public virtual void GetPlayerProgress() {
        currentRPGItem = GameProfileCharacters.Current.GetCurrentCharacterRPG();
        currentPlayerProgressItem = GameProfileCharacters.Current.GetCurrentCharacterProgress();
    }

    public virtual void HandleRPGProperties() {

        if (IsPlayerControlled) {
            if (currentRPGItem == null
                || currentPlayerProgressItem == null
                || lastRPGModTime < Time.time) {
                lastRPGModTime = Time.time + 3f;
                GetPlayerProgress();
            }
    
            runtimeRPGData.modifierSpeed = (currentRPGItem.GetSpeed() / 3
                + rpgModifierDefault);

            runtimeRPGData.modifierEnergy = (currentRPGItem.GetEnergy() / 3
                + rpgModifierDefault)
                + currentPlayerProgressItem.GetGamePlayerProgressEnergy() * 1.2f;
    
            runtimeRPGData.modifierHealth = (currentRPGItem.GetHealth() / 3
                + rpgModifierDefault)
                + currentPlayerProgressItem.GetGamePlayerProgressHealth() * 1.2f;
    
            runtimeRPGData.modifierAttack = (currentRPGItem.GetAttack() / 3
                + rpgModifierDefault)
                + currentPlayerProgressItem.GetGamePlayerProgressEnergy() * 1.2f;
    
            if (thirdPersonController != null) {
                thirdPersonController.walkSpeed = Mathf.Clamp(
                    5 * (float)(runtimeRPGData.modifierSpeed + runtimeRPGData.modifierEnergy), 4, 8);
    
                thirdPersonController.trotSpeed = Mathf.Clamp(
                    12 * (float)(runtimeRPGData.modifierSpeed + runtimeRPGData.modifierEnergy), 9, 14);
    
                thirdPersonController.runSpeed = Mathf.Clamp(
                    20 * (float)(runtimeRPGData.modifierSpeed + runtimeRPGData.modifierEnergy), 14, 28);
    
                thirdPersonController.inAirControlAcceleration = 3;
                thirdPersonController.jumpHeight = .8f;
                thirdPersonController.extraJumpHeight = 1f;
                thirdPersonController.trotAfterSeconds = .5f;
                thirdPersonController.getUserInput = false;
                thirdPersonController.capeFlyGravity = 8f;
                thirdPersonController.gravity = 16f;
            }
        }

    }

    // HANDLE CONTROLS
 
    public virtual void InitControls() {
     
        if (gamePlayerHolder != null) {
         
            // base controller
         
            if (characterController == null) {
                characterController = gameObject.GetComponent<CharacterController>();

                if (characterController == null) {
                    characterController = gameObject.AddComponent<CharacterController>();
                    characterController.slopeLimit = 45;
                    characterController.stepOffset = .3f;
                    characterController.radius = 1.67f;
                    characterController.height = 2.42f;
                    characterController.center = new Vector3(0f, .8f, 0f);
                }
            }
         
            // controllers
                     
            if ((contextState == GamePlayerContextState.ContextInput
                || contextState == GamePlayerContextState.ContextFollowInput
                && !IsUIState())
                || IsNetworkPlayerState()) {
         
                if (thirdPersonController == null) {
                    thirdPersonController = gameObject.AddComponent<GamePlayerThirdPersonController>();
                    thirdPersonController.walkSpeed = 5 * (float)(runtimeRPGData.modifierSpeed + runtimeRPGData.modifierEnergy);
                    thirdPersonController.trotSpeed = 12 * (float)(runtimeRPGData.modifierSpeed + runtimeRPGData.modifierEnergy);
                    thirdPersonController.runSpeed = 20 * (float)(runtimeRPGData.modifierSpeed + runtimeRPGData.modifierEnergy);
                    thirdPersonController.inAirControlAcceleration = 3;
                    thirdPersonController.jumpHeight = .8f;
                    thirdPersonController.extraJumpHeight = 1f;
                    thirdPersonController.trotAfterSeconds = .5f;
                    thirdPersonController.getUserInput = false;
                    thirdPersonController.capeFlyGravity = 8f;
                    thirdPersonController.gravity = 16f;
                }
            }
                     
            // agent controllers
         
            if (navMeshAgent == null
                && !IsUIState()) {
                navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
                if (navMeshAgent == null) {
                    navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
                    if (navMeshAgent != null) {
                        //navMeshAgent.enabled = false;
                        navMeshAgent.height = 3.19f;
                        navMeshAgent.radius = 1.29f;
                        navMeshAgent.speed = 10 * (float)(runtimeRPGData.modifierSpeed + runtimeRPGData.modifierEnergy) + UnityEngine.Random.Range(1, 3);
                        navMeshAgent.acceleration = 8 * (float)(runtimeRPGData.modifierSpeed + runtimeRPGData.modifierEnergy) + UnityEngine.Random.Range(1, 3);
                        navMeshAgent.angularSpeed = 120 + UnityEngine.Random.Range(1, 3);
                        navMeshAgent.baseOffset = -0.34f;
                        navMeshAgent.stoppingDistance = 3f;
                    }
                }
            }
         
            if (contextState == GamePlayerContextState.ContextFollowAgent
                || contextState == GamePlayerContextState.ContextFollowAgentAttack
                && !IsUIState()) {
                if (navMeshAgentFollowController == null) {
                    navMeshAgentFollowController = gameObject.AddComponent<GamePlayerNavMeshAgentFollowController>();
                    navMeshAgentFollowController.agent = navMeshAgent;
                    navMeshAgentFollowController.targetFollow = GameController.CurrentGamePlayerController.gamePlayerEnemyTarget.transform;
                }
            }
         
            if (contextState == GamePlayerContextState.ContextRandom
                && !IsUIState()) {
                if (navMeshAgentController == null) {
                    navMeshAgentController = gameObject.AddComponent<GamePlayerNavMeshAgentController>();
                    navMeshAgentController.agent = navMeshAgent;
                    navMeshAgentController.nextDestination = navMeshAgentController.GetRandomLocation();
                }
            }
         
            // animation
         
            if (gamePlayerControllerAnimation == null) {
                gamePlayerControllerAnimation = gameObject.AddComponent<GamePlayerControllerAnimation>();
                gamePlayerControllerAnimation.torso = gamePlayerHolder.transform;
                gamePlayerControllerAnimation.actor = gamePlayerHolder;
                float smoothing = .8f;
                if (thirdPersonController != null) {
                    smoothing = thirdPersonController.speedSmoothing;
                }
                else {
                    smoothing = navMeshAgent.velocity.magnitude + 10f;
                }
                gamePlayerControllerAnimation.runSpeedScale = (smoothing * .15f) * (float)(runtimeRPGData.modifierSpeed + runtimeRPGData.modifierEnergy);// thirdPersonController.trotSpeed / thirdPersonController.walkSpeed / 2;
                gamePlayerControllerAnimation.walkSpeedScale = 1f * (float)(runtimeRPGData.modifierSpeed + runtimeRPGData.modifierEnergy);//thirdPersonController.walkSpeed / thirdPersonController.walkSpeed;
                gamePlayerControllerAnimation.isRunning = true;
            }
         
            if (actorShadow == null) {
                actorShadow = gameObject.AddComponent<ActorShadow>();
                actorShadow.objectParent = gamePlayerHolder;
                if (gamePlayerShadow != null) {
                    actorShadow.objectShadow = gamePlayerShadow;
                }
            }
        }
    }
 
    // --------------------------------------------------------------------
    // GAME STATE
 
    public virtual void CheckIfShouldRemove() {
        if (IsNetworkPlayerState()) {
            // if network container is gone remove the player...
         
            if (HasNetworkContainer(uuid)) {
                // no prob
            }
            else {
                // remove
             
                if (thirdPersonController) {
                    thirdPersonController.ApplyDie(true);
                }
             
                Tweens.Instance.FadeToObject(gameObject, 0f, .3f, 5f);
                Invoke("RemoveMe", 6);
            }
        }
    }
 
    public virtual void RemoveMe() {
        //Destroy(gameObject);
        //ObjectPoolManager.destroyPooled(gameObject);
    }
 
    public virtual bool CheckVisibility() {
     
        if (renderers == null) {
            renderers = new List<SkinnedMeshRenderer>(); 
        }
     
        if (renderers.Count == 0) {           
            foreach (SkinnedMeshRenderer rendererSkinned in gamePlayerHolder.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                renderers.Add(rendererSkinned);
            }
        }            
     
        visible = false;
     
        if (renderers.Count > 0) {
            foreach (SkinnedMeshRenderer rendererSkinned in renderers) {
                if (rendererSkinned != null) {
                    if (!rendererSkinned.isVisible) {// || !rendererSkinned.IsVisibleFrom(Camera.main)) {
                        visible = false;
                    }
                    else {
                        visible = true;
                        break;
                    }
                }        
            }
        }
     
        //LogUtil.Log("visible:" + visible);
     
        return visible;
    }
     
    // --------------------------------------------------------------------
    // AGENTS
 
    public virtual void TurnOffNavAgent() {
        if (navAgentRunning) {
            if (navMeshAgent != null) {
                navMeshAgent.Stop();
                navAgentRunning = false;
            }        
        }
    }
 
    public virtual void SyncNavAgent() {
     
        if (IsAgentState()) {
         
            /*
         if(navMeshAgent != null) {
             if(navMeshAgent.enabled) {
                 if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 1) {
                     //navMeshAgent.Stop();
                 }
             }
         }
         */
            navAgentRunning = true;
        }
        else if (IsPlayerState()) {
            TurnOffNavAgent();
        }
        else if (IsUIState()) {
            TurnOffNavAgent();
        }
        else if (IsNetworkPlayerState()) {
            TurnOffNavAgent();           
            CheckIfShouldRemove();
        }    
    }
 
    // --------------------------------------------------------------------
    // UPDATE/GAME TICK
 
    public virtual void UpdateCommonState() {
     
        if (Application.isEditor) {
            if (Input.GetKeyDown(KeyCode.M)) {
                PlayerEffectWarpFadeIn();
            }
            else if (Input.GetKeyDown(KeyCode.N)) {
                PlayerEffectWarpFadeOut();
            }
        }

        // visibility
        CheckVisibility();
     
        // fast stuff    
        HandlePlayerAliveState();
        HandlePlayerEffectWarpAnimateTick();
     
        if (IsAgentState()) {         
         
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
         
            if (runtimeData != null) {
                if (runtimeData.hitCount > 5) {
                    Die();
                }
            }            
         
            UpdateNetworkContainerFromSource(uuid);
        }
        else if (IsPlayerState()) {           
            if (thirdPersonController.aimingDirection != Vector3.zero) {

                //gamePlayerHolder.transform.rotation = Quaternion.LookRotation(thirdPersonController.aimingDirection);
                gamePlayerModelHolder.transform.rotation = Quaternion.LookRotation(thirdPersonController.aimingDirection);

                foreach (Transform t in gamePlayerModelHolderModel.transform) {
                    t.localRotation = Quaternion.identity;
                }

                if(thirdPersonController.aimingDirection.IsBiggerThanDeadzone(axisDeadZone)) {

                    if(thirdPersonController.aimingDirection != thirdPersonController.movementDirection) {
                        SendAttack();
                    }
                }
            }
            else {
                foreach (Transform t in gamePlayerModelHolderModel.transform) {
                    t.localRotation = Quaternion.identity;
                }
            }
         
            if (runtimeData.hitCount > 10) {
                Die();
            }
         
            UpdateNetworkContainerFromSource(uuid);          
        }
        else if (IsUIState()) {       
         
        }
        else if (IsNetworkPlayerState()) {            

        }

        // periodic      
     
        bool runUpdate = false;
        if (Time.time > lastUpdateCommon + 1f) {
            lastUpdateCommon = Time.time;
            runUpdate = true;

            if (IsPlayerControlled) {
                HandleRPGProperties();
                Score(2 * 1);
            }
        }
     
        if (!runUpdate) {
            return;
        }        
                 
        SyncNavAgent();  
     
    }
 
    public virtual void UpdateVisibleState() {
     
        if (thirdPersonController != null) {
            if (thirdPersonController.IsJumping()) {
                if (navMeshAgent != null) {
                    if(navMeshAgent.enabled) {
                        navMeshAgent.Stop(true);
                    }
                }
                
                if (gamePlayerShadow != null) {
                    /////gamePlayerShadow.Hide();
                }
            }
            else {
                if (navMeshAgent != null) {
                    if(!navMeshAgent.enabled) {
                        navMeshAgent.enabled = true;
                    }
                    navMeshAgent.Resume();
                }                       
                
                if (gamePlayerShadow != null) {
                    /////gamePlayerShadow.Show();
                }
            }
        }

        if (dying) {
            transform.position = Vector3.Lerp(transform.position, transform.position.WithY(1.3f), 1 + Time.deltaTime);
        }
     
        // If this is an enemy see if we should attack
     
        float attackRange = 10f;  // wihtin 6 yards
        //bool runUpdate = false;

        if (currentTimeBlock + actionInterval < Time.time) {
            currentTimeBlock = Time.time;
            // runUpdate = true;
        }
     
        if (controllerState == GamePlayerControllerState.ControllerAgent
            && (contextState == GamePlayerContextState.ContextFollowAgentAttack
            || contextState == GamePlayerContextState.ContextFollowAgent)
            && GameController.Instance.gameState == GameStateGlobal.GameStarted) {

            //if(runUpdate) {
            GameObject go = GameController.CurrentGamePlayerController.gameObject;

            if (go != null) {

                // check distance for evades

                distanceToPlayerControlledGamePlayer = Vector3.Distance(
                        go.transform.position,
                        transform.position);

                if (distanceToPlayerControlledGamePlayer <= distanceEvade) {
                    isWithinEvadeRange = true;
                }
                else {
                    isWithinEvadeRange = false;
                }

                if (lastIsWithinEvadeRange != isWithinEvadeRange) {
                    if (lastIsWithinEvadeRange && !isWithinEvadeRange) {
                        // evaded!
                        GameController.CurrentGamePlayerController.Score(5);
                        GamePlayerProgress.SetStatEvaded(1f);
                    }
                    lastIsWithinEvadeRange = isWithinEvadeRange;
                }

                // check attack/lunge range

                if (distanceToPlayerControlledGamePlayer <= attackRange) {
                    //foreach(Collider collide in Physics.OverlapSphere(transform.position, attackRange)) {

                    // Turn towards player and attack!

                    GamePlayerController gamePlayerControllerHit
                        = GameController.GetGamePlayerControllerObject(go, true);

                    if (gamePlayerControllerHit != null
                        && !gamePlayerControllerHit.dying) {


                        if (distanceToPlayerControlledGamePlayer < attackRange / 2.5f) {
                            // LEAP AT THEM within three
                            Tackle(gamePlayerControllerHit, Mathf.Clamp(20f - distanceToPlayerControlledGamePlayer / 2, 1f, 20f));
                        }
                        else {
                            // PURSUE FASTER
                            Tackle(gamePlayerControllerHit, 3.33f);
                        }
                    }
                }
            }
            //}
        }
        else if (controllerState == GamePlayerControllerState.ControllerPlayer
            && GameController.Instance.gameState == GameStateGlobal.GameStarted) {
            float currentSpeed = thirdPersonController.moveSpeed;
            //LogUtil.Log("currentSpeed:", currentSpeed);
         
            if (gamePlayerEnemyTarget != null) {
                Vector3 pos = Vector3.zero;
                pos.z = Mathf.Clamp(currentSpeed / 3, .3f, 4.5f);
                //LogUtil.Log("currentSpeedzz:", position.z);
                gamePlayerEnemyTarget.transform.localPosition = pos;
            }

            if (playerSpin) {
                // Clamps automatically angles between 0 and 360 degrees.
                float y = 360 * Time.deltaTime;

                gamePlayerModelHolder.transform.localRotation =
                    Quaternion.Euler(0, gamePlayerModelHolder.transform.localRotation.eulerAngles.y + y, 0);

                if (gamePlayerModelHolder.transform.localRotation.eulerAngles.y > 330) {
                    playerSpin = false;
                    gamePlayerModelHolder.transform.localRotation =
                        Quaternion.Euler(0, 0, 0);
                }
            }
        }

        /*
     // periodic stuff
     
     bool runUpdate = false;
     if(Time.time > lastUpdate + .3f) {
         lastUpdate = Time.time;
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

    public virtual void FixedUpdate() {
        if (!initialized) {
            return;
        }
     
        if (!GameController.IsGameRunning) {
            return;
        }

        HandlePlayerAliveStateFixed();
    }

    public virtual void LateUpdate() {
        if (!initialized) {
            return;
        }
     
        if (!GameController.IsGameRunning) {
            return;
        }

        HandlePlayerAliveStateLate();
    }

    public virtual void UpdateAlways() {        
        HandleCharacterAttachedSounds(); // always run to turn off audio when not playing.
        HandlePlayerInactionState();
    }
 
    public override void Update() {
     
        if (!initialized) {
            return;
        }

        UpdateAlways();

        if (!GameController.IsGameRunning) {
            return;
        }

        if (IsPlayerControlled) {
            if (Input.GetKey(KeyCode.LeftControl)) {

                //Debug.Log("GamePlayer:moveDirection:" + GameController.CurrentGamePlayerController.thirdPersonController.movementDirection);
                //Debug.Log("GamePlayer:aimDirection:" + GameController.CurrentGamePlayerController.thirdPersonController.aimingDirection);
                //Debug.Log("GamePlayer:rotation:" + GameController.CurrentGamePlayerController.transform.rotation);
                //Vector3 point1 = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
                //Vector3 point2 = Camera.main.ScreenToWorldPoint(new Vector3(1, 0, 1));

                //Debug.Log("GamePlayer:point1:" + point1);
                //Debug.Log("GamePlayer:point2:" + point2);

                float power = 100f;
                if (Input.GetKey(KeyCode.V)) {
                    Boost(Vector3.zero.WithZ(1),
                        power);
                }
                else if (Input.GetKey(KeyCode.B)) {
                    Boost(Vector3.zero.WithZ(-1),
                        power);
                }
                else if (Input.GetKey(KeyCode.N)) {
                    StrafeLeft(Vector3.zero.WithX(-1),
                        power);
                }
                else if (Input.GetKey(KeyCode.M)) {
                    StrafeRight(Vector3.zero.WithX(1),
                        power);
                }
            }
        }


        UpdateVisibleState();
        
        UpdateCommonState();

        //if(!visible) {
        //UpdateOffscreenState();
        //return;
        //}
        //else {
        //UpdateVisibleState();
        //}

    }        
}

