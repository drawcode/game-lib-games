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
    ControllerNetwork = 3,
    ControllerNotSet = 4,
}

public enum GamePlayerActorState {
    ActorMe,
    ActorEnemy,
    ActorFriend
}

public enum GamePlayerContextState {
    ContextInput = 0,
    ContextInputVehicle,
    ContextFollowAgent,
    ContextFollowAgentAttack,
    ContextFollowAgentAttackVehicle,
    ContextFollowInput,
    ContextScript,
    ContextRandom,
    ContextUI,
    ContextNetwork,
    ContextNotSet
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
    public double goalFly = 0;

    public virtual float totalScoreValue {
        get {
            return (float)(score + (coins * 50) + (scores * 500));
        }
    }

    public double hitHealthRemaining {
        get {
            return hitCount > 0 ? ((hitLimit - hitCount) / hitLimit) : 1;
        }
    }
}

[System.Serializable]
public class BaseGamePlayerControllerData {  
    public bool loadingCharacter = false;
    public bool gameModelVisible = true;
    public bool paused = true;

    // player
    public bool visible = true;
    public bool initialized = false;
    public bool dying = false;
    public float lastDie = 0f;
    public string lastCharacterCode = null;
    
    // animation
    public GamePlayerControllerAnimation gamePlayerControllerAnimation;
        
    // gameplay    
    public float lastAirCheck = 0f;
    public float lastUpdateCommon = 0f;
    public float lastAttackTime = 0;
    public float lastBoostTime = 0;
    public float lastStrafeLeftTime = 0;
    public float lastSpinTime = 0;
    public float lastStrafeRightTime = 0;
    public float lastAudioPlayedAttack = 0f;
    public float lastAudioPlayedHit = 0f;
    public float lastAudioPlayedDie = 0f;
    public float lastNetworkContainerFind = 0f;
        
    public float lastUpdateAll = 0;
    public float lastUpdateAlways = 0;
    public float lastUpdatePhysics = 0;


    public bool navAgentRunning = true;
    public float currentTimeBlock = 0.0f;
    public float actionInterval = .33f;
    public bool initLoaded = false;
    public float lastCollision = 0f;
    public float intervalCollision = .05f;
    public float lastHit = 0f;
    public Vector3 positionPlayer;
    public Vector3 positionTackler;
    public float lastTackle = 0f;
    public float incrementScore = 1f;
    
    // effects
    
    // effects - warps
    
    public bool effectWarpEnabled = false;
    public float effectWarpStart = 0f;
    public float effectWarpEnd = 200f;
    public float effectWarpCurrent = 0f;
    
    // effects - lines
    
    public float lastPlayerEffectsTrailUpdate = 0f;
    public TrailRenderer trailRendererBoost;
    public TrailRenderer trailRendererGround;
    
    // effects - follow
    
    public float lastPlayerEffectsGroundUpdate = 0f;
    public float lastPlayerEffectsBoostUpdate = 0f;
    public float lastPlayerEffectsIndicatorUpdate = 0f;
    public Color gamePlayerEffectsGroundColorCurrent;
    public Color gamePlayerEffectsGroundColorLast;
    public Color gamePlayerEffectsBoostColorCurrent;
    public Color gamePlayerEffectsBoostColorLast;
    public Color gamePlayerEffectsIndicatorColorCurrent;
    public Color gamePlayerEffectsIndicatorColorLast;

    // navigation/movement
    public NavMeshAgent navMeshAgent;
    public GamePlayerNavMeshAgentFollowController navMeshAgentFollowController;
    public GamePlayerNavMeshAgentController navMeshAgentController;
    public CharacterController characterController;
    public GamePlayerThirdPersonController thirdPersonController;

    // physics
    public Vector3 impact = Vector3.zero;

    // networking
    public Gameverses.GameNetworkAniStates currentNetworkAniState = Gameverses.GameNetworkAniStates.walk;
    public Gameverses.GameNetworkAniStates lastNetworkAniState = Gameverses.GameNetworkAniStates.run;
        
    // RPG
    public GameProfileRPGItem currentRPGItem;
    public GameProfilePlayerProgressItem currentPlayerProgressItem;
    public double rpgModifierDefault = .4f;
    public float lastRPGModTime = 0f;
    public bool playerSpin = false;
    public GamePlayerRuntimeRPGData runtimeRPGData = new GamePlayerRuntimeRPGData();
    
    // LAUNCHING
    public Vectrosity.VectorLine lineAim = null;
    public Vector3 positionStart = Vector3.zero;
    public Vector3 positionRelease = Vector3.zero;
    public Vector3 positionLastTouch = Vector3.zero;
    public Vector3 currentStartPoint = Vector3.zero;
    public Vector3 currentEndPoint = Vector3.zero;
    public Vector3 currentPosition = Vector3.zero;
    public Vector3 currentAimPosition = Vector3.zero;
    public ParticleSystem gamePlayerEffectAim;

    // EVADING
    public float distanceToPlayerControlledGamePlayer;
    public float distanceEvade = 5f;
    public bool isWithinEvadeRange = false;
    public bool lastIsWithinEvadeRange = false;
    public float distanceRandomDie = 50f;
    public float timeMinimumRandomDie = 25f;
    public float lastRandomDie = 5f;
    public bool isInRandomDieRange = false;
    public bool lastIsInRandomDieRange = false;    
    
    // IDLE ACTIONS AFTER INACTION
    public float delayIdleActions = 3.0f;
    public float lastIdleActions = 0f;
    public float lastCharacterLoadedCheck = 0f;
    public GameObject currentPrefabNameObjectItem;
    public string currentPrefabNameObject = "";
    public GamePlayerAttributes gamePlayerAttributes = new GamePlayerAttributes();
    
    // audio
    public GameObject audioObjectFootsteps;
    public AudioClip audioObjectFootstepsClip;
    public AudioSource audioObjectFootstepsSource;

    // materials
    
    //float currentControllerData.lastUpdate = 0f;
    public List<SkinnedMeshRenderer> renderers;
    public GamePlayerController collisionController = null;

    // powerup modifiers

    // speed

    public float modifierItemSpeedCurrent = 1.0f;
    public float modifierItemSpeedMin = 1.0f;
    public float modifierItemSpeedMax = 3.0f;
    public float modifierItemSpeedLerpTime = 5f;
    public float modifierItemSpeedLerp = 0f;

    // scale
        
    public float modifierItemScaleCurrent = 1.0f;
    public float modifierItemScaleMin = 1.0f;
    public float modifierItemScaleMax = 3.0f;
    public float modifierItemScaleLerpTime = 5f;
    public float modifierItemScaleLerp = 0f;
        
    // fly
    
    public float modifierItemFlyCurrent = 1.0f;
    public float modifierItemFlyMin = 1.0f;
    public float modifierItemFlyMax = 3.0f;
    public float modifierItemFlyLerpTime = 5f;
    public float modifierItemFlyLerp = 0f;
        
    // goalNext
    
    public float modifierItemGoalNextCurrent = 1.0f;
    public float modifierItemGoalNextMin = 1.0f;
    public float modifierItemGoalNextMax = 3.0f;
    public float modifierItemGoalNextLerpTime = 5f;
    public float modifierItemGoalNextLerp = 0f;
    public Dictionary<string, float> modifiers = new Dictionary<string, float>();
    public Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();

    // items
    public GamePlayerItemsData itemsData = new GamePlayerItemsData();

    // mounts
    public GamePlayerMountData mountData = new GamePlayerMountData();

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
    public double modifierScale = .5;
}

public class GamePlayerItemsData : BaseGamePlayerItemsData {
    
    public GamePlayerItemsData() {
        
    }
}

public class BaseGamePlayerItemsData {    
    
    public BaseGamePlayerItemsData() {
        
    }
    
    // ---------------------------------------------------
    // ITEMS
    
    // powerups
    
    // boost
}

public class GamePlayerMountData : BaseGamePlayerMountData {

    public GamePlayerMountData() {
    
    }
}

public class BaseGamePlayerMountData {    
        
    public BaseGamePlayerMountData() {
        
    }

    // ---------------------------------------------------
    // MOUNTS

    // MOUNTS - vehicle

    public GameObjectMountVehicle mountVehicle;

    public bool isMountedVehicle {

        get {
            if (isMountedVehicleObject) {
                return true;
            }

            return false;
        }
    }

    public bool isMountedVehicleObject {
        get {            
            if (mountVehicle != null) {
                return true;
            }
            return false;
        }
    }

    public void MountVehicle(GameObject go, GameObjectMountVehicle mount) {
        if (!isMountedVehicleObject) {
            mountVehicle = mount;
            mountVehicle.Mount(go);
        }
    }

    public void UnmountVehicle() {
        if (isMountedVehicleObject) {
            mountVehicle.Unmount();
            mountVehicle = null;
        }
    }
    
    public void SetMountVehicleAxis(float h, float v) {
        if (mountVehicle != null) {
            mountVehicle.SetMountVehicleAxis(h, v);
        }
    }

    public void SetMountWeaponRotator(Vector3 rt) {
        if (mountVehicle != null) {
            mountVehicle.SetMountWeaponRotator(rt);
        }
    }
    
    public void SetMountWeaponRotator(Quaternion qt) {
        if (mountVehicle != null) {
            mountVehicle.SetMountWeaponRotator(qt);
        }
    }
}

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
    public string characterCode = "character-bot-1";
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
    public GameObject gamePlayerEffectMarker;
    public ParticleSystem gamePlayerEffectHit;
    public ParticleSystem gamePlayerEffectDeath; 
 
    // appearance/context
    public ActorShadow actorShadow;
 
    // models/objects
    public GameObject gamePlayerModel;
    public GameObject gamePlayerHolder;
    public GameObject gamePlayerShadow;
    public GameObject gamePlayerEnemyTarget;
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
    
    public float attackRange = 12f;  // within 6 yards    
    public float attackDistance = 10f;
    public float lastStateEvaded = 0f;
    public float lastCollision = 0f;
    public float intervalCollision = .2f;

    // quality settings
        
    public float currentFPS = 60f;

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
 
    public virtual void SetControllerData(GamePlayerControllerData data) {
        if (data == null) {
            data = new GamePlayerControllerData();
        }
     
        controllerData = data;
     
        // TODO sync if needed... to update 
        // runtime expensive states that can't be polled.
    }
    
    public virtual void SetItemsData(GamePlayerItemsData data) {
        if (data == null) {
            data = new GamePlayerItemsData();
        }
        
        itemsData = data;
        
        // TODO sync if needed... to update 
        // runtime expensive states that can't be polled.
    }
     
    public virtual void Init(
        GamePlayerControllerState controllerStateTo, 
        GamePlayerContextState contextStateTo) {
        
        SetUp(controllerStateTo, contextStateTo);

    }

    // SPEED

    public virtual float gamePlayerMoveSpeed {
        get {
            return GamePlayerMoveSpeed();
        }
    }

    public virtual float GamePlayerMoveSpeed() {

        float currentSpeed = 0f;

        if (controllerData == null) {
            return currentSpeed;
        }
        
        if (currentControllerData.thirdPersonController != null) {
            currentSpeed = currentControllerData.thirdPersonController.GetSpeed();
        }
        
        if (contextState == GamePlayerContextState.ContextFollowAgent
            || contextState == GamePlayerContextState.ContextFollowAgentAttack
            || contextState == GamePlayerContextState.ContextRandom) {

            if (currentControllerData.navMeshAgent != null) {

                if (currentControllerData.navMeshAgent.enabled) {                       
                    //currentSpeed = navAgent.velocity.magnitude + 20;
                    
                    if (currentControllerData.navMeshAgent.velocity.magnitude > 0f) {
                        currentSpeed = 15f;
                    }
                    else {
                        currentSpeed = 0;    
                    }
                    
                    if (currentControllerData.navMeshAgent.remainingDistance < 
                        currentControllerData.navMeshAgent.stoppingDistance + 1) {
                        currentSpeed = 0;
                    }
                    
                    if (currentSpeed < 
                        currentControllerData.navMeshAgent.speed) {
                        //currentSpeed = 0;
                    }
                }
            }
        }

        return currentSpeed;
    }

    // REWARDS / ITEMS

    public virtual void HandleItemStateGoalFly(double val) {    

        // TODO add item multi collect here

        //if(runtimeData.goalFly > 0) {
        //    return; // only one fly at a time for now...
        //}

        if (runtimeData.goalFly > 0 && val > 0) {
            return;
        }

        runtimeData.goalFly += val;
        runtimeData.goalFly = (double)Mathf.Clamp((float)runtimeData.goalFly, 0f, 5f);
    }
    
    public virtual void HandleItemStateCurrency(double val) {    

        runtimeData.coins += val;
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
                
        //Debug.Log("HandleItemStateScaleModifier::" + " val:" + val + " duration:" + duration);
        //Debug.Log("HandleItemStateScaleModifier::" + " currentControllerData.modifierItemScaleCurrent:" + currentControllerData.modifierItemScaleCurrent);
        //Debug.Log("HandleItemStateScaleModifier::" + " currentControllerData.modifierItemScaleLerpTime:" + currentControllerData.modifierItemScaleLerpTime);
        //Debug.Log("HandleItemStateScaleModifier::" + " currentControllerData.modifierItemScaleLerp:" + currentControllerData.modifierItemScaleLerp);
    }

    public virtual void HandleItemStateFlyModifier(double val, double duration) {         
        currentControllerData.modifierItemFlyCurrent *= (float)val;
        currentControllerData.modifierItemFlyLerpTime = (float)duration;

        currentControllerData.modifierItemFlyLerp = 0f;
        
        //Debug.Log("HandleItemStateFlyModifier::" + " val:" + val + " duration:" + duration);
        //Debug.Log("HandleItemStateFlyModifier::" + " currentControllerData.modifierItemFlyCurrent:" + currentControllerData.modifierItemFlyCurrent);
        //Debug.Log("HandleItemStateFlyModifier::" + " currentControllerData.modifierItemFlyLerpTime:" + currentControllerData.modifierItemFlyLerpTime);
        //Debug.Log("HandleItemStateFlyModifier::" + " currentControllerData.modifierItemFlyLerp:" + currentControllerData.modifierItemFlyLerp);
    }

    public virtual void HandleItemUse(GameItem gameItem) {
        
        GameDataObjectItem data = gameItem.data;
        
        if (data == null) {
            return;
        }
        
        float modifier = 1f;

        GameDataItemRPG rpg = new GameDataItemRPG();

        if (data.HasRPGs()) {
            rpg = data.GetRPG();            
            
            Debug.Log("HandleItemUse::" + " rpg:" + rpg.ToJson());

            HandleItemStateSpeedModifier(rpg.speed, rpg.duration);
            HandleItemStateScaleModifier(rpg.scale, rpg.duration);
            HandleItemStateFlyModifier(rpg.fly, rpg.duration);
        }
        
        // rewards
        
        if (data.HasRewards()) {
            
            List<GameDataItemReward> items = data.rewards;
            
            bool broadcastEvent = false;
            object broadcastVal = null;
            
            foreach (GameDataItemReward item in items) {
                
                broadcastEvent = false;
                broadcastVal = null;
                
                if (item.val == null) {
                    continue;
                }
                
                if (item.code == GameDataItemReward.xp) {

                    double val = item.valDouble * modifier;   
                    
                    GamePlayerProgress.SetStatXP(val);
                    
                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressXP(val);
                    
                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if (item.code == GameDataItemReward.currency) {
                    
                    double val = item.valDouble * modifier;   
                    
                    GamePlayerProgress.SetStatCoins(val);
                    GamePlayerProgress.SetStatCoinsPickup(val);     
                    
                    HandleItemStateCurrency(val);
                    
                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if (item.code == GameDataItemReward.health) {
                    
                    double val = item.valDouble * modifier;  
                    
                    HandleItemStateHealth(val);

                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(val); // refill
                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(val); // refill                        
                    
                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if (item.code == GameDataItemReward.goalFly) {
                    
                    double val = item.valDouble * modifier;  
                    
                    HandleItemStateGoalFly(val);
                    
                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(val); // refill
                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(val); // refill                        
                    
                    broadcastEvent = true;
                    broadcastVal = val;
                }
                
                if (broadcastEvent) {        
                    Messenger<string, object>.Broadcast(GameMessages.item, item.code, broadcastVal);
                }
                
            }
        }
        
        // sounds
        
        GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
        
        if (data.HasSounds()) {

            data.PlaySoundType(GameDataActionKeys.reward);
        }           
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
        if (currentControllerData.trailRendererBoost == null && gamePlayerTrailBoost != null) {
            currentControllerData.trailRendererBoost = gamePlayerTrailBoost.Get<TrailRenderer>();
            return true;
        }
        return false;
    }

    public virtual bool CheckTrailRendererGround() {
        if (currentControllerData.trailRendererGround == null && gamePlayerTrailGround != null) {
            currentControllerData.trailRendererGround = gamePlayerTrailGround.Get<TrailRenderer>();
            return true;
        }
        return false;
    }

    public virtual void PlayerEffectTrailBoostTime(float time) {
        if (gamePlayerTrailBoost != null) {
            CheckTrailRendererBoost();

            if (currentControllerData.trailRendererBoost != null) {
                currentControllerData.trailRendererBoost.time = time;
            }
        }
    }

    public virtual void PlayerEffectTrailGroundTime(float time) {
        if (gamePlayerTrailGround != null) {
            CheckTrailRendererGround();

            if (currentControllerData.trailRendererGround != null) {
                currentControllerData.trailRendererGround.time = time;
            }
        }
    }

    public virtual void HandlePlayerEffectTrailGroundTick() {
        if (gamePlayerTrailGround != null) {

            // UPDATE color randomly
            // TODO add other conditions to get colors, health, power etc
            // Currently randomize player colors for effect

            CheckTrailRendererGround();
            
            if (currentControllerData.trailRendererGround != null) {

                //Color colorTo = GameCustomController.GetRandomizedColorFromContextEffects();
                //currentControllerData.trailRendererGround.gameObject.ColorTo(colorTo, 1f, 0f);
            }
        }
    }
    
    public virtual void HandlePlayerEffectTrailBoostTick() {
        if (gamePlayerTrailBoost != null) {
            
            // UPDATE color randomly
            // TODO add other conditions to get colors, health, power etc
            // Currently randomize player colors for effect
            
            CheckTrailRendererBoost();
            
            if (gamePlayerTrailBoost != null) {
                
                //Color colorTo = GameCustomController.GetRandomizedColorFromContextEffects();
                //currentControllerData.trailRendererBoost.gameObject.ColorTo(colorTo, 1f, 0f);
            }
        }
    }

    
    // EFFECTS FOLLOW - GROUND/BACK/HEAD ETC

    public bool playerEffectsGroundShow {
        get {
            
            if (FPSDisplay.isUnder20FPS) {
                return false;
            }

            return true;
        }
    }

    public virtual void PlayerEffectsGroundFadeIn() {

        if (!playerEffectsGroundShow) {
            return;
        }

        UITweenerUtil.FadeTo(gamePlayerEffectsGround,
                             UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 1f);
    }
    
    public virtual void PlayerEffectsGroundFadeOut() {
        
        if (!playerEffectsGroundShow) {
            return;
        }

        UITweenerUtil.FadeTo(gamePlayerEffectsGround,
                             UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 0f);
    }
    
    public virtual void PlayerEffectEffectsBoostFadeIn() {
        UITweenerUtil.FadeTo(gamePlayerEffectsBoost,
                             UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 1f);
    }
    
    public virtual void PlayerEffectEffectsBoostFadeOut() {
        UITweenerUtil.FadeTo(gamePlayerEffectsBoost,
                             UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 0f);
    }
    
    public virtual void PlayerEffectsBoostTime(float time) {
        if (gamePlayerEffectsBoost != null) {
            gamePlayerEffectsBoost.SetParticleSystemEmissionRate(time, true);
        }
    }

    public virtual void PlayerEffectsGroundTime(float time) {
        
        if (!playerEffectsGroundShow) {
            return;
        }

        if (gamePlayerEffectsGround != null) {
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
        
        
        if (lastTime < 5 && lastTime >= 0) {    
            lastTime += Time.deltaTime; 
        }
        else {             
            lastTime = 0;
            colorCurrent = 
                GameCustomController.GetRandomizedColorFromContextEffects();
        }
        
        if (immediate) {
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
        
        if (!playerEffectsGroundShow) {
            return;
        }

        if (gamePlayerEffectsGround != null) {

            HandlePlayerEffectsObjectTick(ref gamePlayerEffectsGround, 
                                          ref currentControllerData.gamePlayerEffectsGroundColorCurrent, 
                                          ref currentControllerData.gamePlayerEffectsGroundColorLast, 
                                          ref currentControllerData.lastPlayerEffectsGroundUpdate,
                                          5, .95f);
        }
    }

    public virtual void HandlePlayerEffectsBoostTick() {
        if (gamePlayerEffectsBoost != null) {
            
            HandlePlayerEffectsObjectTick(ref gamePlayerEffectsBoost, 
                                          ref currentControllerData.gamePlayerEffectsBoostColorCurrent, 
                                          ref currentControllerData.gamePlayerEffectsBoostColorLast, 
                                          ref currentControllerData.lastPlayerEffectsBoostUpdate,
                                          3, .3f);
        }
    }

    public virtual void HandlePlayerEffectsIndicatorTick() {
        if (gamePlayerShadow != null) {

            HandlePlayerEffectsObjectTick(ref gamePlayerShadow, 
                                          ref currentControllerData.gamePlayerEffectsIndicatorColorCurrent, 
                                          ref currentControllerData.gamePlayerEffectsIndicatorColorLast, 
                                          ref currentControllerData.lastPlayerEffectsIndicatorUpdate,
                                          3,
                                          .3f);
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
        currentControllerData.effectWarpStart = fromEmission;
        currentControllerData.effectWarpEnd = toEmission;
        currentControllerData.effectWarpEnabled = true;
    }
 
    public virtual void HandlePlayerEffectWarpAnimateTick() {
        if (currentControllerData.effectWarpEnabled && currentControllerData.visible) {
            float fadeSpeed = 200f;
            if (currentControllerData.effectWarpCurrent < currentControllerData.effectWarpEnd) {
                currentControllerData.effectWarpCurrent += (Time.deltaTime * fadeSpeed);
                SetPlayerEffectWarp(currentControllerData.effectWarpCurrent);
            }
            else if (currentControllerData.effectWarpCurrent > currentControllerData.effectWarpEnd) {
                currentControllerData.effectWarpCurrent -= (Time.deltaTime * fadeSpeed);
                SetPlayerEffectWarp(currentControllerData.effectWarpCurrent);
            }
            else {
                currentControllerData.effectWarpEnabled = false;
                currentControllerData.effectWarpCurrent = currentControllerData.effectWarpEnd;
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

    // PLAYER SHADOW + EFFECTS
    
    public virtual void ShowPlayerShadow() {
        if (gamePlayerShadow != null) {
            gamePlayerShadow.Show();
        }
    }
    
    public virtual void HidePlayerShadow() {
        if (gamePlayerShadow != null) {
            gamePlayerShadow.Hide();
        }
    }
    
    public virtual void ShowPlayerSpawner() {
        if (gamePlayerSpawner != null) {
            gamePlayerSpawner.Show();
        }
    }
    
    public virtual void HidePlayerSpawner() {
        if (gamePlayerSpawner != null) {
            gamePlayerSpawner.Hide();
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
        
        if (currentControllerData.lastCharacterLoadedCheck + 1 < Time.time) {
            currentControllerData.lastCharacterLoadedCheck = Time.time;
            
            if (!isCharacterLoaded) {
                //LoadCharacter(characterCode);
            }
        }        

        if (runtimeData.health <= 0f) {
            Die();
        }
    }

    public virtual void HandlePlayerAliveStateLate() {

        UpdatePhysicsState();
    }

    public virtual void HandlePlayerAliveStateFixed() {

    }

    public virtual void HandlePlayerInactionState() {
        
        if (!IsPlayerControlled) {
            return;
        }

        // update player controlled players to look at player and animate it inactive

        bool update = false;

        if (controllerData != null) {
            if (currentControllerData.lastIdleActions + UnityEngine.Random.Range(3, 7) < Time.time) {
                currentControllerData.lastIdleActions = Time.time;
                if (currentControllerData.thirdPersonController != null) {
                    if (currentControllerData.thirdPersonController.moveSpeed == 0f) {
                        update = true;
                    }
                }
            }

            if (!update) {
                return;
            }
        }

        // Look at camera

        OnInputAxis(GameTouchInputAxis.inputAxisMove, Vector3.zero.WithY(-1));

        Idle();
    }
 
    // --------------------------------------------------------------------
    // CHARACTER
     
    public virtual bool isMe {
        get {
            if (uniqueId == UniqueUtil.Instance.currentUniqueId) {
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
  
    public virtual bool IsPlayerControlled {
        get {
            if (controllerState == GamePlayerControllerState.ControllerPlayer
                || contextState == GamePlayerContextState.ContextInput
                || contextState == GamePlayerContextState.ContextInputVehicle
                || contextState == GamePlayerContextState.ContextFollowInput
                || uniqueId == UniqueUtil.Instance.currentUniqueId) {
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
        StartCoroutine(ChangePlayerStateCo(controllerStateTo));
    }

    public virtual IEnumerator ChangePlayerStateCo(GamePlayerControllerState controllerStateTo) {
        //if (controllerStateTo != controllerState) {
        controllerState = controllerStateTo;
         
        yield return StartCoroutine(InitControlsCo());
         
        if (controllerState == GamePlayerControllerState.ControllerAgent) {
            if (currentControllerData.navMeshAgent != null) {
                // TODO load script or look for character input.
                currentControllerData.navMeshAgent.enabled = true;
            }
        }
        else if (controllerState == GamePlayerControllerState.ControllerPlayer) {
            if (currentControllerData.navMeshAgent != null) {
                currentControllerData.navMeshAgent.Stop();
                //navMeshAgent.enabled = false;
            }
        }
        else if (controllerState == GamePlayerControllerState.ControllerNetwork) {
            if (currentControllerData.navMeshAgent != null) {
                currentControllerData.navMeshAgent.Stop();
                //navMeshAgent.enabled = false;
            }
            ChangeContextState(GamePlayerContextState.ContextNetwork);
        }
        else if (controllerState == GamePlayerControllerState.ControllerUI) {
            if (currentControllerData.navMeshAgent != null) {
                currentControllerData.navMeshAgent.Stop();
                //navMeshAgent.enabled = false;  
                if (currentControllerData.thirdPersonController != null) {
                    currentControllerData.thirdPersonController.getUserInput = true;
                }                    
            }
        }
        //}
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

        if (currentControllerData.audioObjectFootsteps == null) {
        
            string soundFootsteps = "audio_footsteps_default";
            
            if (gameCharacter != null) {
                
                GameDataSound dataSound = gameCharacter.data.GetSoundByType(GameDataActionKeys.footsteps);
                
                if (dataSound != null) {
                    soundFootsteps = dataSound.code;
                }
            }

            currentControllerData.audioObjectFootsteps = GameAudio.PlayEffectObject(transform, soundFootsteps, true);
            
            if (currentControllerData.audioObjectFootsteps != null) {
                if (currentControllerData.audioObjectFootsteps.audio != null) {
                    currentControllerData.audioObjectFootstepsSource = currentControllerData.audioObjectFootsteps.audio;

                    if (currentControllerData.audioObjectFootstepsClip == null && currentControllerData.audioObjectFootstepsSource.clip != null) {
                        currentControllerData.audioObjectFootstepsClip = currentControllerData.audioObjectFootsteps.audio.clip;
                    }
                }
            }
        }
    }

    public virtual void HandleCharacterAttachedSounds() {
    
        if (!GameConfigs.isGameRunning) {
            if (controllerData != null) {
                if (currentControllerData.audioObjectFootstepsSource != null) {
                    currentControllerData.audioObjectFootstepsSource.StopIfPlaying();
                }
            }
            return;
        }

        LoadCharacterAttachedSounds();

        if (currentControllerData.audioObjectFootstepsSource == null) {
            return;
        }

        currentControllerData.audioObjectFootstepsSource.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();

        if (gamePlayerMoveSpeed > .1f) {
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
        if (controllerData == null) {
            return;
        }

        if (currentControllerData.gamePlayerControllerAnimation == null) {
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

        if (currentControllerData.loadingCharacter) {
            return;
        }

        characterCode = characterCodeTo;

        ResetScale();
        ResetPosition();

        SetControllerData(new GamePlayerControllerData());
        SetRuntimeData(new GamePlayerRuntimeData());

        //LogUtil.Log("LoadCharacter:prefabNameObject:" + prefabNameObject);
        //if (currentControllerData.lastCharacterCode != characterCode 
        //    || currentControllerData.lastCharacterCode == null) {

        currentControllerData.lastCharacterCode = characterCode;
                        
        //if (gameObject.activeInHierarchy) {
        StartCoroutine(LoadCharacterCo());
        //}
        //}
    }
 
    public virtual IEnumerator LoadCharacterCo() {
        
        gameCharacter = 
            GameCharacters.Instance.GetById(characterCode);

        if (gameCharacter == null) {
            yield break;
        }

        currentControllerData.loadingCharacter = true;

        string prefabCode = gameCharacter.data.GetModel().code;

        //LogUtil.Log("LoadCharacter:path:" + path);
        if (!string.IsNullOrEmpty(prefabCode)) {
            if (gamePlayerModelHolderModel.transform.childCount > 0) {
                // Remove all current characters
                foreach (Transform t in gamePlayerModelHolderModel.transform) {
                    // Pool safely destroys either way
                    GameObjectHelper.DestroyGameObject(
                        t.gameObject, GameConfigs.usePooledGamePlayers);

                    //LogUtil.Log("LoadCharacter:destroy pooled:t.name:" + t.name);
                }
            }

            yield return new WaitForSeconds(.5f);

            gameObjectLoad = AppContentAssets.LoadAsset(prefabCode);

            if (gameObjectLoad != null) {  

                // Wire up collision object

                GamePlayerCollision gamePlayerCollision;
                
                if (gameObjectLoad.Has<GamePlayerCollision>()) {
                    gamePlayerCollision = gameObjectLoad.Get<GamePlayerCollision>();

                    gamePlayerCollision.gamePlayerController = gameObject.Get<GamePlayerController>();
                    
                    if (IsPlayerControlled) {     
                        gamePlayerCollision.tag = "Player";
                        tag = "Player";
                    }
                    else {
                        gamePlayerCollision.tag = "Enemy";
                        tag = "Enemy";
                    }
                }

                // Wire up custom objects
                
                gameCustomPlayer = gameObjectLoad.Set<GameCustomPlayer>();

                if (IsPlayerControlled) {                    

                    gameCustomPlayer.SetActorHero();

                    if (gamePlayerEffectsContainer != null) {
                        gamePlayerEffectsContainer.Show();
                    }
                }
                else {
                    
                    if (gamePlayerEffectsContainer != null) {
                        gamePlayerEffectsContainer.Hide();
                    }
                    
                    gameCustomPlayer.SetActorEnemy();
                }
                                
                if (!IsPlayerControlled 
                    && GameAIController.generateType == GameAICharacterGenerateType.team) {
                    // apply team colors and textures

                    GameTeam team = GameTeams.Current;

                    if (team != null) {
                        if (team.data != null) {
                                                                                    
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

                // load items

                foreach (GamePlayerObjectItem objectItem 
                        in gameObjectLoad.GetComponentsInChildren<GamePlayerObjectItem>(true)) {

                    if (IsPlayerControlled) {
                        objectItem.gameObject.Show();
                    }
                    else {
                        objectItem.gameObject.Hide(); 
                    }
                }
                
                //LogUtil.Log("LoadCharacter:create game object:gameObjectLoad.name:" + gameObjectLoad.name);

                foreach (Transform t in gameObjectLoad.transform) {
                    //t.localRotation = gamePlayerModelHolderModel.transform.rotation;
                    GamePlayerIndicator.AddIndicator(GameHUD.Instance.containerOffscreenIndicators, 
                                                     t.gameObject, "bot1");
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
        
        HidePlayerEffectWarp();
        
        if (IsPlayerControlled) {
            GetPlayerProgress();
            currentControllerData.lastPlayerEffectsTrailUpdate = 0;
            HandlePlayerEffectsTick();
        }
    }

    public void LoadWorldIndicator() {
        
        // Visible elements after model loaded
        
        ShowPlayerShadow();
        HidePlayerSpawner();
        
        if (!IsPlayerControlled) {
            if (gamePlayerShadow != null) {

                Color colorTo = UIColors.colorRed;
                colorTo.a = .3f;

                gamePlayerShadow.SetParticleSystemStartColor(colorTo, true);
            }

            if (FPSDisplay.isUnder30FPS) {
                if (gamePlayerEffectMarker != null) {
                    gamePlayerEffectMarker.Hide();
                }
            }
        }
    }
 
    // --------------------------------------------------------------------
    // WEAPONS   

    public List<string> weaponInventory;
    public int weaponInventoryIndex = 0;

    public virtual void LoadInventory() {

        ////LogUtil.Log("LoadInventory");
    
        if (weaponInventory == null) {
            weaponInventory = new List<string>();
        }

        weaponInventory.Clear();

        foreach (GameWeapon weapon in GameWeapons.Instance.GetAll()) {
            if (weapon.active) {
                weaponInventory.Add(weapon.code);
            }
        }
        
        // TODO load from data
        SetItemsData(new GamePlayerItemsData());
    }
    
    public virtual void UnloadWeapons() {
        if (gamePlayerModelHolderWeaponsHolder != null) {
            gamePlayerModelHolderWeaponsHolder.DestroyChildren();
        }
        
        if (weapons == null) {
            weapons = new Dictionary<string, GamePlayerWeapon>();
        }

        weapons.Clear();
    }
 
    public virtual void LoadWeapons() {
                
        foreach (GameObjectMountWeaponHolder holder in 
                gamePlayerModelHolderModel.GetComponentsInChildren<GameObjectMountWeaponHolder>(true)) {    
            gamePlayerModelHolderWeaponsHolder = holder.gameObject;
            gamePlayerModelHolderWeapons = gamePlayerModelHolderWeaponsHolder.transform.parent.gameObject;
        }
        
        //LogUtil.Log("LoadWeapons");
        if (gamePlayerModelHolderWeaponsHolder != null) {
            // check if character or vehicle has holder for weapons placement
                            
            initialGamePlayerWeaponContainer = gamePlayerModelHolderWeaponsHolder.transform.position;
            currentGamePlayerWeaponContainer = gamePlayerModelHolderWeaponsHolder.transform.position;

            LoadInventory();

            LoadWeapon(weaponInventory[weaponInventoryIndex]);
        }
    }
    
    public virtual void LoadWeaponNext() {
        LoadWeapon(weaponInventoryIndex + 1);
    }
    
    public virtual void LoadWeaponPrevious() {
        LoadWeapon(weaponInventoryIndex - 1);
    }

    public virtual void LoadWeapon(int index) {
        if (index < 0) {
            index = weaponInventory.Count - 1;
        }
        else if (index > weaponInventory.Count - 1) {
            index = 0;
        }

        weaponInventoryIndex = index;            
        LoadWeapon(weaponInventory[weaponInventoryIndex]);
    }

    public virtual void LoadWeapon(string code) {
        
        UnloadWeapons();

        if (!IsPlayerControlled) {
            return; // TODO enemy weapons
        }

        GameWeapon gameWeaponData = GameWeapons.Instance.GetByCode(code);

        if (gameWeaponData == null) {
            LogUtil.LogWarning("LoadWeapon: NULL gameWeaponData");
            return;
        }

        if (gameWeaponData.data == null) {
            LogUtil.LogWarning("LoadWeapon: NULL gameWeaponData.data");
            return;
        }

        GameDataModel dataModel = gameWeaponData.data.GetModel();
        
        if (dataModel == null) {
            LogUtil.LogWarning("LoadWeapon: NULL dataModel");
            return;
        }

        GameObject go = AppContentAssets.LoadAsset("weapon", dataModel.code);

        if (go == null) {
            return;
        }

        go.transform.parent = gamePlayerModelHolderWeaponsHolder.transform;
        go.ResetPosition();
        go.ResetRotation();

        if (GameConfigs.isGameRunning && IsPlayerControlled) {
            UINotificationDisplayTip.Instance.QueueTip(
                "Weapon Loaded: " + gameWeaponData.display_name,
                gameWeaponData.description, true);
                
        }

        if (go != null && weapons.Count == 0) {
            
            foreach (GamePlayerWeapon weapon in 
                    gamePlayerModelHolderWeaponsHolder.GetComponentsInChildren<GamePlayerWeapon>()) {

                weapon.gameWeaponData = gameWeaponData;
                                
                LogUtil.Log("LoadWeapon:weapon.name:" + weapon.name);

                weapons.Add(GamePlayerSlots.slotPrimary, weapon);
                weaponPrimary = weapon;                
                break;
            }
        }
    }


 
    // --------------------------------------------------------------------
    // EVENTS

    public virtual void OnInputAxis(string name, Vector3 axisInput) {
                     
        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (controllerData == null) {
            return;
        }
     
        // main
     
        //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
     
        if (name == GameTouchInputAxis.inputAxisMove) {
         
            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
         
            if (currentControllerData.thirdPersonController != null) {
             
                //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
                
                if (!axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    axisInput.x = 0f;
                    axisInput.y = 0f;
                }

                //if(!GameController.isFingerNavigating) {
                HandleThirdPersonControllerAxis(axisInput);
            }
        }
        else if (name == GameTouchInputAxis.inputAxisAttack) {
         
            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
         
            if (currentControllerData.thirdPersonController != null) {
             
                //LogUtil.Log("OnInputAxis ATTACK:" + name + "input:" + axisInput);
                
                if (!axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    axisInput.x = 0f;
                    axisInput.y = 0f;
                }
                
                currentControllerData.thirdPersonController.horizontalInput2 = axisInput.x;
                currentControllerData.thirdPersonController.verticalInput2 = axisInput.y;

            }
        }
        else if (name == GameTouchInputAxis.inputAxisMoveHorizontal) {
                
            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
            
            if (currentControllerData.thirdPersonController != null) {
                    
                //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
                
                if (axisInput.IsBiggerThanDeadzone(axisDeadZone)) {

                    currentControllerData.thirdPersonController.horizontalInput = axisInput.x;
                    currentControllerData.thirdPersonController.verticalInput = 0f;//currentControllerData.thirdPersonController.verticalInput;
                }

                if (axisInput.y > .7f) {
                    //LogUtil.Log("axisInput.y:" + axisInput.y);
                    Jump();
                }
                else {
                    JumpStop();
                }

            }
        }
        else if (name == GameTouchInputAxis.inputAxisMoveVertical) {
                
            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
            
            if (currentControllerData.thirdPersonController != null) {
                
                if (axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
                    currentControllerData.thirdPersonController.horizontalInput = 0f;//axisInput.x;
                    currentControllerData.thirdPersonController.verticalInput = axisInput.y;
                }
            }
        }
        else if (name == GameTouchInputAxis.inputAxisAttack2DSide2) {
                
            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
            
            if (currentControllerData.thirdPersonController != null) {
                    
                //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
                
                //currentControllerData.thirdPersonController.horizontalInput = axisInput.x;
                //currentControllerData.thirdPersonController.verticalInput = 0f;
                
                if (axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    
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
            
            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
            
            if (currentControllerData.thirdPersonController != null) {
                
                if (axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    //LogUtil.Log("OnInputAxis ATTACK:" + name + "input:" + axisInput);
                    
                    currentControllerData.thirdPersonController.horizontalInput2 = -axisInput.x;
                    currentControllerData.thirdPersonController.verticalInput2 = 0f;//axisInput.y;
                    
                    //UpdateAim(axisInput.x, axisInput.y);
                }
            }
        }
    }

    public virtual void HandleThirdPersonControllerAxis(Vector3 axisInput) {
        if (currentControllerData.mountData.isMountedVehicle) {

            currentControllerData.mountData.SetMountVehicleAxis(axisInput.x, axisInput.y);
        }
        else {

            currentControllerData.thirdPersonController.horizontalInput = axisInput.x;
            currentControllerData.thirdPersonController.verticalInput = axisInput.y;
        }
    }
        
    public virtual void SetThirdPersonControllerAxisAlt(Vector3 axisInput) {
        
    }
 
    public virtual void OnNetworkActionEvent(Gameverses.GameNetworkingAction actionEvent, Vector3 pos, Vector3 direction) {
         
        if (!GameConfigs.isGameRunning) {
            return;
        }
    
        if (actionEvent.uuidOwner == uniqueId) {
            AnimatePlayer(actionEvent.code);
        }
    }
     
    public virtual void OnNetworkPlayerAnimation(string uid, Gameverses.GameNetworkAniStates aniState) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (uniqueId == uid && !isMe) {
            if (currentControllerData.lastNetworkAniState != currentControllerData.currentNetworkAniState) {
                currentControllerData.lastNetworkAniState = currentControllerData.currentNetworkAniState;
             
                if (currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.walk) {
                 
                }
                else if (currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.run) {
                 
                }
                else if (currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.attack1) {
                 
                }
                else if (currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.attack2) {
                 
                }
                else if (currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.death) {
                 
                }
                else if (currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.skill1) {
                 
                }
                else if (currentControllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.skill2) {
                 
                }
            }
        }
    }
 
    public virtual void OnNetworkPlayerInputAxisHorizontal(string uid, float horizontalInput) {
        if (uniqueId == uid && !isMe) {
            if (currentControllerData.thirdPersonController != null) {
                currentControllerData.thirdPersonController.horizontalInput = horizontalInput;
            }
        }
    }
 
    public virtual void OnNetworkPlayerInputAxisVertical(string uid, float verticalInput) {
        if (uniqueId == uid && !isMe) {
            if (currentControllerData.thirdPersonController != null) {
                currentControllerData.thirdPersonController.verticalInput = verticalInput;
            }
        }
    }
 
    public virtual void OnNetworkPlayerSpeed(string uid, float speed) {
        if (uniqueId == uid && !isMe) {
            if (currentControllerData.thirdPersonController != null) {
                currentControllerData.thirdPersonController.moveSpeed = speed;
            }            
        }
    }
 
    public override void OnInputDown(InputTouchInfo touchInfo) {
        LogUtil.Log("OnInputDown GameActor");        
     
    }
    
    // --------------------------------------------------------------------
    // MOUNT VEHICLE


    public virtual void SetControllersState(bool running) {

        if (currentControllerData.characterController == null) {
            return;
        }

        currentControllerData.characterController.enabled = running;
    }

    public virtual void MountNearest() {
        
        if (currentControllerData.mountData == null) {
            return;
        }

        if (currentControllerData.mountData.isMountedVehicle) {
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

        foreach (Collider collide in Physics.OverlapSphere(transform.position, mountRange)) {

            Transform t = collide.transform;

            if (t != null) {

                if (t.name.ToLower().Contains("mount")) {

                    T mountObject = t.gameObject.Get<T>();

                    if (mountObject != null) {
                       
                        float currentDistance = Vector3.Distance(
                            transform.position,
                            mountObject.transform.position);

                        if (currentDistance < shortestDistance) {
                            found = true;
                            shortestDistance = currentDistance; 
                            nearest = mountObject;
                        }
                    }
                    
                }
            }
        }

        if (found && nearest != null) {
            Mount(nearest.gameObject);
        }
    }

    public virtual void Mount(GameObject go) {
        if (go.Has<GameObjectMountVehicle>()) {        // MOUNT VEHICLES     
            if (!currentControllerData.mountData.isMountedVehicleObject) {
                currentControllerData.mountData.MountVehicle(gameObject, 
                    go.Get<GameObjectMountVehicle>());

                if (currentControllerData.gameModelVisible) {
                    gamePlayerModelHolderModel.Hide();
                    currentControllerData.gameModelVisible = false;
                }

                GameObjectMountWeaponHolder weaponHolder = currentControllerData.mountData.mountVehicle.GetWeaponHolder();

                if (weaponHolder != null) {
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
        if (currentControllerData.mountData.isMountedVehicleObject) {
            currentControllerData.mountData.UnmountVehicle();
                        
            if (!currentControllerData.gameModelVisible) {
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

    public bool controllerReady {
        get {

            if (!GameController.shouldRunGame) {
                return false;
            }
            
            if (!GameConfigs.isGameRunning) {
                return false;
            }

            if (controllerData == null) {
                return false;
            }

            //return true;

            //if(!gameObject.activeSelf && !gameObject.activeInHierarchy) {
            //return false;
            //}

            if (controllerData != null) {
                if (!currentControllerData.loadingCharacter) {
                    return true;
                }
            }

            return false;
        }
    }
 
    // --------------------------------------------------------------------
    // COLLISIONS/TRIGGERS

    public virtual void HandleCollision(Collision collision) {
                                
        if (!controllerReady) {
            return;
        }

        if (currentControllerData.lastCollision + currentControllerData.intervalCollision < Time.time) {
            currentControllerData.lastCollision = Time.time;
        }
        else {
            return;
        }

        if (collision.contacts.Length > 0) {
            foreach (ContactPoint contact in collision.contacts) {
                //Debug.DrawRay(contact.point, contact.normal, Color.white);
                     
                Transform t = contact.otherCollider.transform;

                if (t.parent != null) {
                    string parentName = t.parent.name;

                    // TODO make name recursion by depth limit, for now check three above.
                    string parentParentName = "";
                    string parentParentParentName = "";
                
                    if (t.parent.parent != null) {
                        parentParentName = t.parent.parent.name;                        
                        if (t.parent.parent.parent != null) {
                            parentParentParentName = t.parent.parent.parent.name;
                        }
                    }

                    bool isObstacle = parentName.Contains("GameObstacle")
                        || t.name.Contains("GameObstacle");                  

                    bool isLevelObject = parentName.Contains("GameItemObject")
                        || parentParentName.Contains("GameItemObject")
                        || parentParentParentName.Contains("GameItemObject")
                        || t.name.Contains("GameItemObject");                   

                    bool isPlayerObject = 
                        t.name.Contains("GamePlayerCollider");
                    //|| t.name.Contains("GamePlayerObject");

                    if (isLevelObject) {
                        GameLevelSprite sprite = t.gameObject.FindTypeAboveRecursive<GameLevelSprite>();
                        if (sprite == null) {
                            sprite = t.parent.gameObject.GetComponentInChildren<GameLevelSprite>();
                        }
                        if (sprite != null) {                        
                            isLevelObject = true;
                        }
                        else { 
                            isLevelObject = false;
                        }
                    }
                                     
                    if (isObstacle || isLevelObject) {
                        if (IsPlayerControlled) {
                            AudioAttack();
                            Score(1);
                            GamePlayerProgress.SetStatHitsObstacles(1f);
                        }
                    }
                    else if (isPlayerObject) {

                        // handle stat

                        currentControllerData.collisionController = GameController.GetGamePlayerControllerObject(
                            t.gameObject, false);

                        if (currentControllerData.collisionController != null) {
                        
                            if (!currentControllerData.collisionController.controllerReady) {
                                //break;
                            }

                            // make sure it isn't own colliders
                            if (currentControllerData.collisionController.uniqueId
                                == uniqueId) {
                                // It's me, leave it be.
                                continue;
                            }

                            // handle hit
                            
                            float power = .1f;                            
                            runtimeData.health -= power;
                            
                            //GamePlayerProgress.Instance.ProcessProgressSpins
                            //GameProfileCharacters.currentProgress.SubtractGamePlayerProgressHealth(power); // TODO get by skill upgrade
                            //GameProfileCharacters.currentProgress.SubtractGamePlayerProgressEnergy(power/2f); // TODO get by skill upgrade
                            
                            Vector3 normal = contact.normal;
                            float magnitude = contact.point.sqrMagnitude;
                            float hitPower = (magnitude * (float)runtimeData.mass) / 110;

                            //LogUtil.Log("hitPower:" + hitPower);

                            AddImpact(normal, Mathf.Clamp(hitPower, 0f, 80f));

                            if (IsPlayerControlled) {
                                // we hit an enemy, so we are the player
                                GamePlayerProgress.SetStatHits(1f);
                                Hit(power);

                            }
                            else {

                                if (currentControllerData.collisionController.IsPlayerControlled) {
                                    Hit(power);
                                    // we hit a player, so we are an enemy
                                    GamePlayerProgress.SetStatHitsReceived(1f);
                                }
                            }
                        }
                    }
                }
                break;
            }
        }
     
        //if (collision.relativeVelocity.magnitude > 2)
        //    audio.Play();      
    }
 
    //GamePlayerController gamePlayerControllerHit;
        
    //public void OnCollisionEnter(Collision collision) {
    //    if(!GameController.shouldRunGame) {
    //            return;
    //    }
    //
    //    HandleCollision(collision);
    //}
        
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
    //private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];
    
    public virtual void OnParticleCollision(GameObject other) {

        if (!controllerReady) {
            return;
        }
        
        if (lastCollision + intervalCollision < Time.time) {
            lastCollision = Time.time;
        }
        else {
            return;
        }
                
        if (other.name.Contains("projectile-")) {
            
            LogUtil.Log("OnParticleCollision:" + other.name);
            
            // todo lookup projectile and power to subtract.
            
            float projectilePower = 1;
            float power = projectilePower / 10f;
            
            if (IsPlayerControlled) {
                // 1/20th power for friendly fire
                power = power / 20f;
            }
            
            runtimeData.health -= power;
            
            //contact.normal.magnitude
            
            Hit(power);
        }
            
        /*
            ParticleSystem particleSystem;
            particleSystem = other.GetComponent<ParticleSystem>();
            int safeLength = particleSystem.safeCollisionEventSize;
            if (collisionEvents.Length < safeLength)
                collisionEvents = new ParticleSystem.CollisionEvent[safeLength];
            
            int numCollisionEvents = particleSystem.GetCollisionEvents(gameObject, collisionEvents);
            int i = 0;
            while (i < numCollisionEvents) {
                if (gameObject.rigidbody) {
                    Vector3 pos = collisionEvents[i].intersection;
                    Vector3 force = collisionEvents[i].velocity * 10;
                    gamePlayerController.gameObject.rigidbody.AddForce(force);
                }
                i++;
            }
            */
            
        /*
            int safeLength = particleSystem.safeCollisionEventSize;
            if (collisionEvents.Length < safeLength)
                collisionEvents = new ParticleSystem.CollisionEvent[safeLength];
            
            int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
            int i = 0;
            while (i < numCollisionEvents) {
                if (other.rigidbody) {
                    Vector3 pos = collisionEvents[i].intersection;
                    Vector3 force = collisionEvents[i].velocity * 10;
                    rigidbody.AddForce(force);
                }
                i++;
            }
            */
    }
     
    public virtual void OnTriggerEnter(Collider collider) {
        
        if (!controllerReady) {
            return;
        }
     
        if (IsPlayerControlled) {
     
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
        if (animationName == GameDataActionKeys.skill) {
            InputSkill();
        }
        else if (animationName == GameDataActionKeys.attack) {
            InputAttack();
        }
        else if (animationName == GameDataActionKeys.attack_alt) {
            InputAttackAlt();
        }
        else if (animationName == GameDataActionKeys.attack_right) {
            InputAttackRight();
        }
        else if (animationName == GameDataActionKeys.attack_left) {
            InputAttackLeft();
        }
        else if (animationName == GameDataActionKeys.defend) {
            InputDefend();
        }
        else if (animationName == GameDataActionKeys.defend_alt) {
            InputDefendAlt();
        }
        else if (animationName == GameDataActionKeys.defend_right) {
            InputDefendRight();
        }
        else if (animationName == GameDataActionKeys.defend_left) {
            InputDefendLeft();
        }
        else if (animationName == GameDataActionKeys.death) {
            InputDie();
        }
        else if (animationName == GameDataActionKeys.jump) {
            InputJump();
        }
        else if (animationName == GameDataActionKeys.strafe_left) {
            InputStrafeLeft();
        }
        else if (animationName == GameDataActionKeys.strafe_right) {
            InputStrafeRight();
        }
        else if (animationName == GameDataActionKeys.use) {
            InputUse();
        }
        else if (animationName == GameDataActionKeys.mount) {
            InputMount();
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
            if (currentControllerData.gamePlayerControllerAnimation != null) {
                AnimatePlayer(animationName);
            }
        }
        //}
    }
 
    // --------------------------------------------------------------------
    // STATE/RESET

    public virtual void ResetPositionAir(float y) {    

        //if(IsPlayerControlled) {
        if (currentControllerData.lastAirCheck > 1f) {
            currentControllerData.lastAirCheck = 0;

            gameObject.transform.position = Vector3.Lerp(
            gameObject.transform.position, 
            gameObject.transform.position.WithY(y), 
            1 * Time.deltaTime);            
        }

        currentControllerData.lastAirCheck += Time.deltaTime;
        //}
    }

    public virtual void ResetPosition() {

        foreach (Transform t in gamePlayerModelHolderModel.transform) {
            t.position.Reset();
            t.localPosition.Reset();
            t.rotation.Reset();
            t.localRotation.Reset();
            t.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (IsPlayerControlled) {
            gameObject.ResetPosition();
        }
    }

    public virtual void ResetScale() {
        
        foreach (Transform t in gamePlayerModelHolderModel.transform) {
            t.localScale = Vector3.one;
        }
        
        if (IsPlayerControlled) {
            gameObject.ResetScale(1f);
        }
    }
    
    public virtual void SetUp(
        GamePlayerControllerState controllerStateTo, 
        GamePlayerContextState contextStateTo) {        
        
        if (controllerState != controllerStateTo 
            || controllerState == GamePlayerControllerState.ControllerNotSet) {
           
            controllerState = controllerStateTo;
            contextState = contextStateTo;
        }

        Reset();
    }
         
    public virtual void Reset() {

        if (IsPlayerControlled) {
            uniqueId = UniqueUtil.Instance.currentUniqueId;
        }
        else {
            uniqueId = UniqueUtil.Instance.CreateUUID4();
        }

        ResetPosition();

        //SetControllerData(new GamePlayerControllerData());
        //SetRuntimeData(new GamePlayerRuntimeData());
        
        LoadCharacter(characterCode);
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
     
        if (IsPlayerControlled) {          
            GameHUD.Instance.ShowHitOne((float)(1.5 - runtimeData.health));
            Score(2 * power);
            DeviceUtil.Vibrate();
        }
        else {
            //bool allow = false;

            if (currentControllerData.lastHit + .3f < Time.time) {
                currentControllerData.lastHit = Time.time;
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
        else if (IsPlayerControlled) {
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
        
        //currentControllerData.thirdPersonController.Idle();
        
        currentControllerData.gamePlayerControllerAnimation.Idle();
    }
    
    public virtual void Jump() {
        Jump(.5f);
    }

    public virtual void Jump(float duration) {
        if (isDead) {
            return;
        }
        
        if (currentControllerData.thirdPersonController != null) {
            currentControllerData.thirdPersonController.Jump(duration);
        }
        
        currentControllerData.gamePlayerControllerAnimation.Jump();
        
        if (gamePlayerEffectSkill != null) {
            gamePlayerEffectSkill.Emit(1);
        }
    }
    
    public virtual void JumpStop() {
        if (isDead) {
            return;
        }
        
        currentControllerData.thirdPersonController.JumpStop();
    }

    // SKILL
 
    public virtual void Skill() {
        if (isDead) {
            return;
        }
     
        currentControllerData.gamePlayerControllerAnimation.Skill();
     
        if (gamePlayerEffectSkill != null) {
            gamePlayerEffectSkill.Emit(1);
        }
    }

    // STRAFE

    // STRAFE LEFT

    public virtual void StrafeLeft() {
        Vector3 dir = transform.TransformPoint(transform.localPosition.WithX(-1));//Vector3.zero.WithX(-1);
        float power = 10f + 5f * (float)currentControllerData.runtimeRPGData.modifierAttack;
        StrafeLeft(dir, power);
    }

    public virtual void StrafeLeft(Vector3 dir) {
        float power = 10f + 5f * (float)currentControllerData.runtimeRPGData.modifierAttack;
        StrafeLeft(dir, power);
    }

    public virtual void StrafeLeft(float power) {
        Vector3 dir = transform.TransformPoint(transform.localPosition.WithX(-1));//Vector3.zero.WithX(-1);
        StrafeLeft(dir, power);
    }

    public virtual void StrafeLeft(Vector3 dir, float power) {
        //LogUtil.Log("GamePlayerController:StrafeLeft:");

        if (!controllerReady) {
            return;
        }

        if (isDead) {
            return;
        }

        if (Time.time > currentControllerData.lastStrafeLeftTime + 1f) {
            currentControllerData.gamePlayerControllerAnimation.StrafeLeft();

            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cuts, 1f);
            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cutsLeft, 1f);

            currentControllerData.lastStrafeLeftTime = Time.time;
            StartCoroutine(StrafeLeftCo(dir, power));
        }
    }

    public virtual IEnumerator StrafeLeftCo(Vector3 dir, float power) {
        AddForce(dir, power, false);
        yield return new WaitForEndOfFrame();
    }

    // STRAFE RIGHT


    public virtual void StrafeRight() {
        Vector3 dir = transform.localPosition.WithX(1);
        float power = 10f + 5f * (float)currentControllerData.runtimeRPGData.modifierAttack;
        StrafeRight(dir, power);
    }

    public virtual void StrafeRight(Vector3 dir) {
        float power = 10f + 5f * (float)currentControllerData.runtimeRPGData.modifierAttack;
        StrafeRight(dir, power);
    }

    public virtual void StrafeRight(float power) {
        Vector3 dir = transform.localPosition.WithX(1);
        StrafeRight(dir, power);
    }

    public virtual void StrafeRight(Vector3 dir, float power) {
        //LogUtil.Log("GamePlayerController:StrafeRight:");

        if (isDead) {
            return;
        }
        if (Time.time > currentControllerData.lastStrafeRightTime + 1f) {

            currentControllerData.gamePlayerControllerAnimation.StrafeRight();

            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cuts, 1f);
            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cutsRight, 1f);

            currentControllerData.lastStrafeRightTime = Time.time;
            StartCoroutine(StrafeRightCo(dir, power));
        }
    }

    public virtual IEnumerator StrafeRightCo(Vector3 dir, float power) {
        AddForce(dir, power, false);
        yield return new WaitForEndOfFrame();
    }

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
        if (isDead) {
            return;
        }
        //LogUtil.Log("GamePlayerController:Boost:");
        if (Time.time > currentControllerData.lastBoostTime + 1f) {
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
        if (isDead) {
            return;
        }
        //LogUtil.Log("GamePlayerController:Spin:");
        if (Time.time > currentControllerData.lastSpinTime + 1f) {
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
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (currentControllerData.lastDie + 3f < Time.time) {
            currentControllerData.lastDie = Time.time;
        }
        else {
            return;
        }
                     
        if (isDead && currentControllerData.dying) {
            return;
        }
                
        if (currentControllerData.thirdPersonController != null) {
            currentControllerData.thirdPersonController.controllerData.removing = true;
        }
        
        currentControllerData.gamePlayerControllerAnimation.Die();
                
        currentControllerData.impact = Vector3.zero;

        currentControllerData.dying = true;

        if (IsPlayerControlled) {
            GamePlayerProgress.SetStatDeaths(1f);
        }
        else {
            GamePlayerProgress.SetStatKills(1f);
        }
     
        /*
        if (gamePlayerEffectDeath != null) {
            gamePlayerEffectDeath.Emit(1);
        }
        */
     
        if (IsPlayerControlled) {
            PlayerEffectWarpFadeIn();
        }
                
        runtimeData.health = 0;
     
        AudioDie();
        
        StopNavAgent();
        
        ResetPositionAir(0f);
     
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

    public virtual void StartNavAgent() {
        
        if (!IsPlayerControlled || gameObject.Has<CharacterController>()) {
            if (currentControllerData.navMeshAgent != null) {
                currentControllerData.navMeshAgent.enabled = true;
                currentControllerData.navMeshAgent.Resume();
            }
            if (currentControllerData.navMeshAgentController != null) {
                currentControllerData.navMeshAgentController.StartAgent();
            }
            if (currentControllerData.navMeshAgentFollowController != null) {
                currentControllerData.navMeshAgentFollowController.StartAgent();
            }
        }
    }

    public virtual void StopNavAgent() {

        if (currentControllerData.navMeshAgent != null) {
            if (currentControllerData.navMeshAgent.enabled) {
                currentControllerData.navMeshAgent.Stop(true);
                currentControllerData.navMeshAgent.enabled = false;
            }
        }
        if (currentControllerData.navMeshAgentController != null) {
            currentControllerData.navMeshAgentController.StopAgent();
        }
        if (currentControllerData.navMeshAgentFollowController != null) {
            currentControllerData.navMeshAgentFollowController.StopAgent();
        }
    }
 
    public virtual void AttackAlt() {    
        if (isDead) {
            return;
        }    
        currentControllerData.gamePlayerControllerAnimation.AttackAlt();
        Invoke("AttackEffect", .5f);
    }
 
    public virtual void AttackLeft() {       
        if (isDead) {
            return;
        }
        currentControllerData.gamePlayerControllerAnimation.AttackLeft();
        Invoke("AttackEffect", .5f);
    }
 
    public virtual void AttackRight() {      
        if (isDead) {
            return;
        }
        currentControllerData.gamePlayerControllerAnimation.AttackRight();
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
 
    public virtual void Attack() {       
        if (isDead) {
            return;
        }

        bool shouldShoot = true;

        if (weaponPrimary != null) {
            if (weaponPrimary.isAuto) {
                weaponPrimary.Attack();
                shouldShoot = false;
            }
        }

        if (shouldShoot) {    
            if (Time.time > currentControllerData.lastAttackTime + 1f) {
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

        if (controllerReady) {
            return;
        }
             
        if (!GameConfigs.isGameRunning) {
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
        if (currentControllerData.thirdPersonController != null) {
            directionAttack = currentControllerData.thirdPersonController.aimingDirection;
        }        
     
        //Debug.DrawRay(transform.position, directionAttack * attackDistance);

        hits = Physics.RaycastAll(transform.position, directionAttack, attackDistance);
        int i = 0;
        while (i < hits.Length) {
            RaycastHit hit = hits[i];
            Transform hitObject = hit.transform;
         
            if (hitObject.name.IndexOf("Game") > -1) {
                if (hitObject != null) {
                    GamePlayerController playerController = GetController(hitObject);
                    if (playerController != null) {

                        //Debug.Log("CastAttack:" + " currentUUID:" + uniqueId + " otherID:" + playerController.uniqueId);
                                             
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
            foreach (GameCameraSmoothFollow cam in UnityObjectUtil.FindObjects<GameCameraSmoothFollow>()) {
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
            
            if (currentControllerData.gamePlayerEffectAim != null) {
                currentControllerData.gamePlayerEffectAim.enableEmission = true;
                currentControllerData.gamePlayerEffectAim.emissionRate = amount * 2;
                currentControllerData.gamePlayerEffectAim.startLifetime = amount / 400f;
                currentControllerData.gamePlayerEffectAim.startSpeed = amount;
                currentControllerData.gamePlayerEffectAim.Play();
            }
            
            //currentControllerData.lineAim..SetLine3D(Color.white, model.transform.position, lookAtPos);
            
            //LogUtil.Log("UpdateAim:currentControllerData.currentAimPosition:", currentControllerData.currentAimPosition);
            
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
            
            if (currentControllerData.gamePlayerEffectAim != null) {
                currentControllerData.gamePlayerEffectAim.enableEmission = false;
                currentControllerData.gamePlayerEffectAim.emissionRate = 1;
                currentControllerData.gamePlayerEffectAim.Stop();
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

        if(controllerData.dying) {
            return; 
        }

        //bool allowTackle = false;

        if (currentControllerData.lastTackle + 1f < Time.time) {
            currentControllerData.lastTackle = Time.time;
            //allowTackle = true;
        }
        else {
            return;
        }

        transform.LookAt(gamePlayerControllerTo.transform);
        
        Jump();

        currentControllerData.positionPlayer = transform.position.WithY(10);
        currentControllerData.positionTackler = gamePlayerControllerTo.transform.position;
     
        currentControllerData.gamePlayerControllerAnimation.Attack();

        //Attack();
     
        AddImpact(currentControllerData.positionTackler - currentControllerData.positionPlayer, power, false);
     
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

    public virtual void UpdatePlayerProgress(float energy, float health) {
        StartCoroutine(UpdatePlayerProgressCo(energy, health));
    }

    public virtual IEnumerator UpdatePlayerProgressCo(float energy, float health) {
        yield return new WaitForEndOfFrame();
        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergyAndHealthRuntime(
            energy, health);
    }
 
    // call this function to add an currentControllerData.impact force:
    public virtual void AddImpact(Vector3 dir, float force, bool damage, bool allowY = false) {

        if(currentControllerData.dying) {
            currentControllerData.impact = Vector3.zero;
            return;
        }

        dir.Normalize();
     
        if (dir.y < 0 && allowY) {
            dir.y = 0;//-dir.y; // reflect down force on the ground
        }

        if (damage) {
            force = Mathf.Clamp(force, 0f, 100f);
        }

        //if(IsPlayerControlled 
        //   && runtimeData.goalFly == 0) {
        currentControllerData.impact += dir.normalized * force / (float)runtimeData.mass;
        //}
        if (damage) {
            runtimeData.hitCount++;

            if (IsPlayerControlled && damage) {

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

        //Debug.Log("AddImpactForce:" + " delta:" + delta + " dir:" + dir + " force:" + force); 

    }
    
    float lastUpdatePhysics = 0;

    public virtual void UpdatePhysicsState() {
        
        if (!controllerReady) {
            return;
        }
                
        if (lastUpdatePhysics + .2f < Time.time) {
            lastUpdatePhysics = Time.time;
        }
        else {
            if (!IsPlayerControlled) {
                return;
            }
        }

        StartCoroutine(UpdatePhysicStateCo());
    }

    public virtual IEnumerator UpdatePhysicStateCo() {
        
        //Vectrosity.VectorLine.SetLine (Color.red, transform.position, currentControllerData.impact);

        if (currentControllerData.characterController.enabled) {
            currentControllerData.characterController.Move(currentControllerData.impact * Time.deltaTime);
        }
                
        // consumes the currentControllerData.impact energy each cycle:
        currentControllerData.impact = Vector3.Lerp(currentControllerData.impact, Vector3.zero, 5 * Time.deltaTime);
        //}

        UpdatePlayerEffectsState();
                
        yield return new WaitForFixedUpdate();

    }

    public virtual void HandlePlayerEffectsStateChange() {
        currentControllerData.lastPlayerEffectsTrailUpdate = 0;
        currentControllerData.lastPlayerEffectsBoostUpdate = -1f;
        currentControllerData.lastPlayerEffectsGroundUpdate = -1f;
        UpdatePlayerEffectsState();
    }

    public virtual void  UpdatePlayerEffectsState() { 
        
        if (!controllerReady) {
            return;
        }
        
        if (IsPlayerControlled) {
            
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


    // --------------------------------------------------------------------
    // AUDIO
 
    public virtual void AudioAttack() {
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (currentControllerData.lastAudioPlayedAttack + 1 > Time.time) {
            return;
        }
        else {
            currentControllerData.lastAudioPlayedAttack = Time.time;
        }

        GameDataSound soundAttack = gameCharacter.data.GetSoundByType(GameDataActionKeys.attack);

        GameAudio.PlayEffect(soundAttack.code);
    }

    public virtual void AudioHit() {
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (currentControllerData.lastAudioPlayedHit + 1 > Time.time) {
            return;
        }
        else {
            currentControllerData.lastAudioPlayedHit = Time.time;
        }

        GameDataSound dataItem = gameCharacter.data.GetSoundByType(GameDataActionKeys.hit);
        GameAudio.PlayEffect(transform, dataItem.code);
    }

    public virtual void AudioDie() {
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (currentControllerData.lastAudioPlayedDie + 1 > Time.time) {
            return;
        }
        else {
            currentControllerData.lastAudioPlayedDie = Time.time;
        }

        if (controllerState == GamePlayerControllerState.ControllerPlayer) {

            GameAudioController.PlaySoundPlayerEnd();
        }
        
        GameDataSound dataItem = gameCharacter.data.GetSoundByType(GameDataActionKeys.death);
        GameAudio.PlayEffect(transform, dataItem.code);
    }    
 
    // --------------------------------------------------------------------
    // NETWORK
 
 

    public virtual void UpdateNetworkContainer(string uid) {
     
        uniqueId = uid;
        
        if (!AppConfigs.featureEnableNetworking || !GameConfigs.useNetworking) {
            return;
        }
     
        FindNetworkContainer(uniqueId);      
     
        if (currentNetworkPlayerContainer != null) {
#if NETWORK_UNITY || NETWORK_PHOTON
            currentNetworkPlayerContainer.networkViewObject.observed = currentNetworkPlayerContainer;
#endif
            currentNetworkPlayerContainer.gamePlayer = gameObject;
            if (currentControllerData.thirdPersonController != null) {
                currentNetworkPlayerContainer.currentSpeedNetwork = currentControllerData.thirdPersonController.moveSpeed;
                currentNetworkPlayerContainer.verticalInputNetwork = currentControllerData.thirdPersonController.verticalInput;
                currentNetworkPlayerContainer.horizontalInputNetwork = currentControllerData.thirdPersonController.horizontalInput;
            }
            currentNetworkPlayerContainer.running = true;                
        }    
    }
 
    public virtual Gameverses.GameNetworkPlayerContainer FindNetworkContainer(string uid) {
     
        if (!AppConfigs.featureEnableNetworking || !GameConfigs.useNetworking) {
            return null;
        }
     
        if (currentNetworkPlayerContainer != null) {
            if (currentNetworkPlayerContainer.uniqueId == uid) {
                return currentNetworkPlayerContainer;
            }
        }
     
        if (Time.time > currentControllerData.lastNetworkContainerFind + 5f) {
            currentControllerData.lastNetworkContainerFind = Time.time;
            if (GameController.Instance.gameState == GameStateGlobal.GameStarted) {
                foreach (Gameverses.GameNetworkPlayerContainer playerContainer 
                         in UnityObjectUtil.FindObjects<Gameverses.GameNetworkPlayerContainer>()) {
                    if (playerContainer.uniqueId == uid) {
                        currentNetworkPlayerContainer = playerContainer;
                        return currentNetworkPlayerContainer;
                    }
                }
            }
        }
     
        return null;
    }
 
    public virtual bool HasNetworkContainer(string uid) {

        foreach (Gameverses.GameNetworkPlayerContainer playerContainer 
                 in UnityObjectUtil.FindObjects<Gameverses.GameNetworkPlayerContainer>()) {
            if (playerContainer.uniqueId == uid) {
                currentNetworkPlayerContainer = playerContainer;
                return true;
            }
        }
     
        return false;
    }
 
    public virtual void UpdateNetworkContainerFromSource(string uid) {
     
        uniqueId = uid;
     
        FindNetworkContainer(uniqueId);
     
        if (currentNetworkPlayerContainer != null) {
            if (currentControllerData.thirdPersonController != null) {
                currentNetworkPlayerContainer.currentSpeedNetwork = currentControllerData.thirdPersonController.moveSpeed;
                currentNetworkPlayerContainer.verticalInputNetwork = currentControllerData.thirdPersonController.verticalInput;
                currentNetworkPlayerContainer.horizontalInputNetwork = currentControllerData.thirdPersonController.horizontalInput;
            }
            currentNetworkPlayerContainer.running = true;                
        }    
    }
 
    public virtual void ChangeContextState(GamePlayerContextState contextStateTo) {
        //if (contextStateTo != contextState) {
        contextState = contextStateTo;
         
        if (currentControllerData.thirdPersonController != null) {
            currentControllerData.thirdPersonController.isNetworked = false;
        }
         
        if (contextState == GamePlayerContextState.ContextFollowAgent
            || contextState == GamePlayerContextState.ContextFollowAgentAttack
            || contextState == GamePlayerContextState.ContextRandom
            || contextState == GamePlayerContextState.ContextScript) {
            if (currentControllerData.navMeshAgent != null) {
                // TODO load script or look for character input.
                currentControllerData.navMeshAgent.enabled = true;
            }
        }
        else if (contextState == GamePlayerContextState.ContextInput
            || contextState == GamePlayerContextState.ContextInputVehicle
            || contextState == GamePlayerContextState.ContextFollowInput) {
            if (currentControllerData.navMeshAgent != null) {
                currentControllerData.navMeshAgent.Stop();
                //navMeshAgent.enabled = false;
            }
        }
        else if (contextState == GamePlayerContextState.ContextNetwork) {
            if (currentControllerData.navMeshAgent != null) {
                currentControllerData.navMeshAgent.Stop();
                //navMeshAgent.enabled = false;
            }
            currentControllerData.thirdPersonController.isNetworked = true;
        }
        else if (contextState == GamePlayerContextState.ContextUI) {
            if (currentControllerData.navMeshAgent != null) {
                currentControllerData.navMeshAgent.Stop();
                //navMeshAgent.enabled = false;
            }
        }
        //}
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
        currentControllerData.currentRPGItem = GameProfileCharacters.Current.GetCurrentCharacterRPG();
        currentControllerData.currentPlayerProgressItem = GameProfileCharacters.Current.GetCurrentCharacterProgress();
    }

    public virtual void HandleItemProperties() {    

        //float tParam = 0f;
        //float valToBeLerped = 15f;
        //float speed = 0.3f;
        //if (tParam < 1) {
        //    tParam += Time.deltaTime * speed; //This will increment tParam based on Time.deltaTime multiplied by a speed multiplier
        //    //valToBeLerped = Mathf.Lerp(0, 3, tParam);
        //}

        // speed

        if (currentControllerData.modifierItemSpeedLerp < 1f) {

            currentControllerData.modifierItemSpeedLerp += Time.deltaTime / (currentControllerData.modifierItemSpeedLerpTime * 1000);
                    
            currentControllerData.modifierItemSpeedCurrent = Mathf.Lerp(
                    currentControllerData.modifierItemSpeedCurrent, 
                    currentControllerData.modifierItemSpeedMin, 
                    currentControllerData.modifierItemSpeedLerp);     

            currentControllerData.modifierItemSpeedCurrent = Mathf.Clamp(
                currentControllerData.modifierItemSpeedCurrent, 0, 5);
        }

        // scale
        
        if (currentControllerData.modifierItemScaleLerp < 1f) {
            
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

    public void HandleActionGoalNextFlyFlap() {
                
        if (runtimeData.goalFly > 0) {

            GameGoalZoneMarker marker = GameGoalZoneMarker.GetMarker();

            if (marker != null) {

                //Debug.Log("marker:" + marker.name);

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
                                
                if (Mathf.Abs(distanceCurrent) > 3f) {
                    Jump(.75f);
                }
                
                if (modifierItemGoalNextPosStartTime == 0) {    
                    modifierItemGoalNextPosStartTime = Time.time;
                    SetModifierValue(GamePlayerModifierKeys.modifierItemGoalNextPosStartTime, modifierItemGoalNextPosStartTime);
                }

                // easing

                //if (Time.time - modifierItemGoalNextPosStartTime <= duration) {
                //    currentFlyFactor = (float)AnimationEasing.QuadEaseInOut(
                //        Time.time - modifierItemGoalNextPosStartTime, 0, 1, duration);
                //}

                if (gameObject.transform.position.y < UnityEngine.Random.Range(1.3f, 1.8f)) { //UnityEngine.Random.Range(1f, 2f)) {
                      
                    // jagged jumps
                    //if (Mathf.Abs(distanceCurrent) > .5f) {
                    //    Jump(.05f);
                    //}

                    Vector3 dir = gameGoalNext.transform.position - transform.position;
                    dir.y = distanceCurrent / 2f;//UnityEngine.Random.Range(120f, 200f);
                    currentControllerData.impact = Vector3.zero;
                    AddImpactForce(dir, UnityEngine.Random.Range(1.3f, 1.8f));
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
                                    
                if (currentControllerData.thirdPersonController != null) {
                    //if (currentControllerData.thirdPersonController.IsJumping()) {
                    //transform.position = modifierItemGoalNextPosCurrent;
                    //}
                }  

                if (Math.Abs(distanceCurrent) < 1) {
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
        
        if (!currentControllerData.modifiers.ContainsKey(key)) {
            currentControllerData.modifiers.Set(key, defaultValue);            
        }
        
        return currentControllerData.modifiers.Get(key);
    }

    void SetPositionValue(string key, Vector3 val) {
        
        if (controllerData == null) {
            return;
        }
        
        currentControllerData.positions.Set(key, val);
        
    }
    
    Vector3 GetPositionValue(string key, Vector3 defaultValue) {

        if (!currentControllerData.positions.ContainsKey(key)) {
            currentControllerData.positions.Set(key, defaultValue);            
        }
        
        return currentControllerData.positions.Get(key);
    }

    public Vector3 initialScale = Vector3.one;

    public virtual void HandleRPGProperties() {

        if (IsPlayerControlled) {
            if (currentControllerData.currentRPGItem == null
                || currentControllerData.currentPlayerProgressItem == null
                || currentControllerData.lastRPGModTime < Time.time) {
                currentControllerData.lastRPGModTime = Time.time + 3f;
                GetPlayerProgress();
            }
    
            currentControllerData.runtimeRPGData.modifierSpeed = currentControllerData.currentRPGItem.GetSpeed();

            currentControllerData.runtimeRPGData.modifierEnergy = currentControllerData.currentRPGItem.GetEnergy()
                + currentControllerData.currentPlayerProgressItem.GetGamePlayerProgressEnergy();

            currentControllerData.runtimeRPGData.modifierHealth = currentControllerData.currentRPGItem.GetHealth()
                + currentControllerData.currentPlayerProgressItem.GetGamePlayerProgressHealth();
    
            currentControllerData.runtimeRPGData.modifierAttack = currentControllerData.currentRPGItem.GetAttack()
                + currentControllerData.currentPlayerProgressItem.GetGamePlayerProgressEnergy();

            currentControllerData.runtimeRPGData.modifierScale = currentControllerData.currentRPGItem.GetScale();

            // HANDLE ITEM RUNTIME

            // SCALE

            //Debug.Log("modifierScale:" + currentControllerData.runtimeRPGData.modifierScale);
            //Debug.Log("modifierItemScaleCurrent:" + currentControllerData.modifierItemScaleCurrent);
                        
            Vector3 scalePos = initialScale * Mathf.Clamp((float)currentControllerData.runtimeRPGData.modifierScale
                * currentControllerData.modifierItemScaleCurrent, .4f, 2.4f);
            
            //Debug.Log("scalePos:" + scalePos);

            transform.localScale = scalePos;

            // SPEED
            
            float modifiedItem = Mathf.Clamp(currentControllerData.modifierItemSpeedCurrent, .3f, 4f);

            // FLY / JUMP

            float modifiedFly = Mathf.Clamp(currentControllerData.modifierItemFlyCurrent, .3f, 4f);

            if (modifiedFly > 1.0) {
                ///Jump();
            }

            // POWER

            float modifiedPower = (float)(currentControllerData.runtimeRPGData.modifierSpeed + 
                currentControllerData.runtimeRPGData.modifierEnergy);

            float baseWalkSpeed = 5f;
            float baseTrotSpeed = 12f;
            float baseRunSpeed = 24f;

            float modifiedRunSpeed = Mathf.Clamp(baseRunSpeed * modifiedPower, 14, 34) * modifiedItem;            
            float modifiedTrotSpeed = Mathf.Clamp(baseTrotSpeed * modifiedPower, 9, 14) * modifiedItem;            
            float modifiedWalkSpeed = Mathf.Clamp(baseWalkSpeed * modifiedPower, 4, 8) * modifiedItem;
    
            if (currentControllerData.thirdPersonController != null) {

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
            }                       
            
            if (currentControllerData.gamePlayerControllerAnimation != null) {
                
                if (currentControllerData.gamePlayerControllerAnimation.animationData != null) {
                    currentControllerData.gamePlayerControllerAnimation.animationData.runSpeedScale = 
                        Mathf.Clamp(0.1f * modifiedRunSpeed / baseRunSpeed, 1.5f, 2.8f);

                    currentControllerData.gamePlayerControllerAnimation.animationData.walkSpeedScale = 
                        Mathf.Clamp(0.1f * modifiedWalkSpeed / baseWalkSpeed, 1.2f, 1.8f);
                }
            }
        }

    }

    // HANDLE CONTROLS
 
    public virtual IEnumerator InitControlsCo() {
     
        if (gamePlayerHolder != null) {
            
            // 
            // CHARACTER

            if (gameObject == null) {
                yield break;
            }

            // remove all components
            
            //Destroy(gameObject.GetComponent<GamePlayerNavMeshAgentFollowController>());
            //Destroy(gameObject.GetComponent<GamePlayerNavMeshAgentController>());
            //Destroy(gameObject.GetComponent<NavMeshAgent>());
            //Destroy(gameObject.GetComponent<GamePlayerControllerAnimation>());
            //Destroy(gameObject.GetComponent<GamePlayerThirdPersonController>());
            //Destroy(gameObject.GetComponent<CharacterController>());

            //

            yield return new WaitForEndOfFrame();                        
                     
            currentControllerData.characterController = gameObject.GetComponent<CharacterController>();

            if (currentControllerData.characterController == null) {
                currentControllerData.characterController = gameObject.AddComponent<CharacterController>();
            }
            
            currentControllerData.characterController.slopeLimit = 45;
            currentControllerData.characterController.stepOffset = .3f;
            currentControllerData.characterController.radius = 1.67f;
            currentControllerData.characterController.height = 2.42f;
            currentControllerData.characterController.center = new Vector3(0f, 1.79f, 0f);
            
            // 
            // PLAYER CONTROLLERS
                     
            if ((contextState == GamePlayerContextState.ContextInput
                || contextState == GamePlayerContextState.ContextInputVehicle
                || contextState == GamePlayerContextState.ContextFollowInput
                && !IsUIState())
                || IsNetworkPlayerState()) {         
                    
                if (gameObject.Has<GamePlayerThirdPersonController>()) {
                
                    currentControllerData.thirdPersonController = 
                            gameObject.GetComponent<GamePlayerThirdPersonController>();
                }
                else {

                    currentControllerData.thirdPersonController = 
                            gameObject.AddComponent<GamePlayerThirdPersonController>();
                }

                currentControllerData.thirdPersonController.Init();

                HandleRPGProperties();
            }
            
            // 
            // AGENTS
         
            if (!IsUIState()) {

                currentControllerData.navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

                if (currentControllerData.navMeshAgent == null) {
                    currentControllerData.navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
                }

                if (currentControllerData.navMeshAgent != null) {
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

                }
            }
         
            if (contextState == GamePlayerContextState.ContextFollowAgent
                || contextState == GamePlayerContextState.ContextFollowAgentAttack
                && !IsUIState()) {

                currentControllerData.navMeshAgentFollowController = 
                    gameObject.GetComponent<GamePlayerNavMeshAgentFollowController>();
                
                if (currentControllerData.navMeshAgentFollowController == null) {
                    currentControllerData.navMeshAgentFollowController = 
                        gameObject.AddComponent<GamePlayerNavMeshAgentFollowController>();
                }

                currentControllerData.navMeshAgentFollowController.agent = currentControllerData.navMeshAgent;

                currentControllerData.navMeshAgentFollowController.targetFollow = 
                    GameController.CurrentGamePlayerController.gamePlayerEnemyTarget.transform;
            }
         
            if (contextState == GamePlayerContextState.ContextRandom
                && !IsUIState()) {

                currentControllerData.navMeshAgentController = 
                    gameObject.GetComponent<GamePlayerNavMeshAgentController>();
                
                if (currentControllerData.navMeshAgentController == null) {
                    currentControllerData.navMeshAgentController = 
                        gameObject.AddComponent<GamePlayerNavMeshAgentController>();
                }

                currentControllerData.navMeshAgentController.agent = currentControllerData.navMeshAgent;

                currentControllerData.navMeshAgentController.nextDestination = 
                    currentControllerData.navMeshAgentController.GetRandomLocation();
            }
            
            // 
            // ANIMATION
         
            if (gameObject.Has<GamePlayerControllerAnimation>()) {
                currentControllerData.gamePlayerControllerAnimation = 
                    gameObject.GetComponent<GamePlayerControllerAnimation>();  
            }
            else {
                currentControllerData.gamePlayerControllerAnimation = 
                    gameObject.AddComponent<GamePlayerControllerAnimation>();  
            }
            
            currentControllerData.gamePlayerControllerAnimation.Init(); 
                      
            float smoothing = .8f;

            if (currentControllerData.thirdPersonController != null) {
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
         
            if (actorShadow == null) {

                actorShadow = gameObject.AddComponent<ActorShadow>();
                actorShadow.objectParent = gamePlayerModelHolderModel;

                if (gamePlayerShadow != null) {
                    actorShadow.objectShadow = gamePlayerShadow;
                }
            }

            StartNavAgent();
                        
            if (currentControllerData.thirdPersonController != null) {
                currentControllerData.thirdPersonController.Reset();           
            }
                        
            if (controllerState == GamePlayerControllerState.ControllerAgent) {
                currentControllerData.navMeshAgent.enabled = true;
            } 
            
            yield return new WaitForEndOfFrame();
        }
    }
 
    // --------------------------------------------------------------------
    // GAME STATE
 
    public virtual void CheckIfShouldRemove() {
        if (IsNetworkPlayerState()) {
            // if network container is gone remove the player...
         
            if (HasNetworkContainer(uniqueId)) {
                // no prob
            }
            else {
                // remove
             
                if (currentControllerData.thirdPersonController) {
                    currentControllerData.thirdPersonController.ApplyDie(true);
                }

                UITweenerUtil.FadeTo(
                    gameObject, 
                    UITweener.Method.EaseIn, UITweener.Style.Once, .3f, .5f, 0);

                Invoke("RemoveMe", 6);
            }
        }
    }
 
    public virtual void RemoveMe() {
        gamePlayerModelHolderModel.DestroyChildren(GameConfigs.usePooledGamePlayers);
        gameObject.DestroyGameObject(3f, GameConfigs.usePooledGamePlayers);
    }
 
    public virtual bool CheckVisibility() {

        if (!controllerReady) {
            return false;
        }
     
        if (currentControllerData.renderers == null) {
            currentControllerData.renderers = new List<SkinnedMeshRenderer>(); 
        }
     
        if (currentControllerData.renderers.Count == 0) {           
            foreach (SkinnedMeshRenderer rendererSkinned in gamePlayerHolder.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                currentControllerData.renderers.Add(rendererSkinned);
            }
        }            
     
        currentControllerData.visible = false;
     
        if (currentControllerData.renderers.Count > 0) {
            foreach (SkinnedMeshRenderer rendererSkinned in currentControllerData.renderers) {
                if (rendererSkinned != null) {
                    if (!rendererSkinned.isVisible) {// || !rendererSkinned.IscurrentControllerData.visibleFrom(Camera.main)) {
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
     
    // --------------------------------------------------------------------
    // AGENTS
 
    public virtual void TurnOffNavAgent() {
        if (currentControllerData.navAgentRunning) {
            if (currentControllerData.navMeshAgent != null) {
                currentControllerData.navMeshAgent.Stop();
                currentControllerData.navAgentRunning = false;
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
            if (isDead) {
                currentControllerData.navAgentRunning = true;
            }
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

        if (!controllerReady) {
            return;
        }
        
        currentFPS = FPSDisplay.GetCurrentFPS();
             
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
                if (runtimeData.hitCount > UnityEngine.Random.Range(2, 4)) {
                    Die();
                }
            }            
         
            UpdateNetworkContainerFromSource(uniqueId);
        }
        else if (IsPlayerState()) {           
            if (currentControllerData.thirdPersonController.aimingDirection != Vector3.zero) {

                //gamePlayerHolder.transform.rotation = Quaternion.LookRotation(currentControllerData.thirdPersonController.aimingDirection);
                gamePlayerModelHolder.transform.rotation = 
                    Quaternion.LookRotation(currentControllerData.thirdPersonController.aimingDirection);

                if (currentControllerData.mountData.isMountedVehicle) {
                    currentControllerData.mountData.mountVehicle.SetMountWeaponRotator(
                        Quaternion.LookRotation(currentControllerData.thirdPersonController.aimingDirection));
                }

                foreach (Transform t in gamePlayerModelHolderModel.transform) {
                    t.localRotation = Quaternion.identity;
                }

                if (currentControllerData.thirdPersonController.aimingDirection.IsBiggerThanDeadzone(axisDeadZone)) {

                    if (currentControllerData.thirdPersonController.aimingDirection != currentControllerData.thirdPersonController.movementDirection) {
                        SendAttack();
                    }
                }
            }
            else {
                                
                if (currentControllerData.mountData.isMountedVehicle) {
                    //currentControllerData.mountData.mountVehicle.SetMountWeaponRotatorLocal(Vector3.zero);
                }

                foreach (Transform t in gamePlayerModelHolderModel.transform) {
                    t.localRotation = Quaternion.identity;
                }
            }
         
            if (runtimeData.hitCount > runtimeData.hitLimit) {
                Die();
            }
         
            UpdateNetworkContainerFromSource(uniqueId);          
        }
        else if (IsUIState()) {       
         
        }
        else if (IsNetworkPlayerState()) {            

        }

        bool shouldBeGrounded = true;
        if (currentControllerData.thirdPersonController != null) {
            if (currentControllerData.thirdPersonController.IsJumping()) {
                shouldBeGrounded = false;
            }
        }

        if (shouldBeGrounded) {
            ResetPositionAir(0f);
        }
        
        if (IsPlayerControlled) {
            HandleItemProperties();
            HandleRPGProperties();
        }

        // periodic      
     
        bool runUpdate = false;
        if (Time.time > currentControllerData.lastUpdateCommon + 1f) {
            currentControllerData.lastUpdateCommon = Time.time;
            runUpdate = true;

            if (IsPlayerControlled) {
                Score(2 * 1);
            }
        }
     
        if (!runUpdate) {
            return;
        }        
                 
        SyncNavAgent();  
     
    }

    bool shadowActive = false;
 
    public virtual void UpdateVisibleState() {
     
        // Handle navagent on/off while jumping

        if (currentControllerData.thirdPersonController != null) {
            if (currentControllerData.thirdPersonController.IsJumping()) {
                if (currentControllerData.navMeshAgent != null) {
                    if (currentControllerData.navMeshAgent.enabled) {
                        currentControllerData.navMeshAgent.Stop(true);
                    }
                }
                
                if (gamePlayerShadow != null) {
                    /////gamePlayerShadow.Hide();
                }
            }
            else {
                if (currentControllerData.navMeshAgent != null) {
                    if (!currentControllerData.navMeshAgent.enabled) {
                        currentControllerData.navMeshAgent.enabled = true;
                    }
                    currentControllerData.navMeshAgent.Resume();
                }                       
                
                if (gamePlayerShadow != null) {
                    /////gamePlayerShadow.Show();
                }
            }
        }

        if (isCharacterLoaded) {
            if (!shadowActive) {
                actorShadow.gameObject.Show();
                shadowActive = true;
            }
        }
        else {
            if (shadowActive) {
                actorShadow.gameObject.Hide();
                shadowActive = false;
            }
        }

        //if (currentControllerData.dying) {
        //transform.position = Vector3.Lerp(transform.position, transform.position.WithY(1.3f), 1 + Time.deltaTime);
        //}

        // fix after jump
        if (gamePlayerModelHolderModel != null) {
            foreach (Transform t in gamePlayerModelHolderModel.transform) { 
                if (currentControllerData.thirdPersonController != null) {
                    if (!currentControllerData.thirdPersonController.IsJumping()) {
                        t.localPosition = Vector3.Lerp(t.localPosition, t.localPosition.WithY(0), 2 + Time.deltaTime);
                    }
                }
                break;
            }
        }
     
        bool runUpdate = false;

        if (currentControllerData.currentTimeBlock + currentControllerData.actionInterval < Time.time) {
            currentControllerData.currentTimeBlock = Time.time;
            runUpdate = true;
        }
     
        if (controllerState == GamePlayerControllerState.ControllerAgent
            && (contextState == GamePlayerContextState.ContextFollowAgentAttack
            || contextState == GamePlayerContextState.ContextFollowAgent)
            && GameController.Instance.gameState == GameStateGlobal.GameStarted
            && isAlive) {

            if (runUpdate) {
                GameObject go = GameController.CurrentGamePlayerController.gameObject;

                if (go != null) {
                    
                    currentControllerData.distanceToPlayerControlledGamePlayer = Vector3.Distance(
                        go.transform.position,
                        transform.position);

                    // check distance for evades

                    if (lastStateEvaded > .3f) {

                        lastStateEvaded += Time.deltaTime;

                        if (currentControllerData.distanceToPlayerControlledGamePlayer <= currentControllerData.distanceEvade) {
                            currentControllerData.isWithinEvadeRange = true;
                        }
                        else {
                            currentControllerData.isWithinEvadeRange = false;
                        }

                        if (currentControllerData.lastIsWithinEvadeRange != currentControllerData.isWithinEvadeRange) {
                            if (currentControllerData.lastIsWithinEvadeRange && !currentControllerData.isWithinEvadeRange) {
                                // evaded!
                                GamePlayerProgress.SetStatEvaded(1f);
                            }
                            currentControllerData.lastIsWithinEvadeRange = currentControllerData.isWithinEvadeRange;
                        }
                    }

                    // check attack/lunge range

                    if (currentControllerData.distanceToPlayerControlledGamePlayer <= attackRange) {
                        //foreach(Collider collide in Physics.OverlapSphere(transform.position, attackRange)) {

                        // Turn towards player and attack!

                        /// TODO

                        GamePlayerController gamePlayerControllerHit
                            = GameController.GetGamePlayerControllerObject(go, true);

                        if (gamePlayerControllerHit != null
                            && !gamePlayerControllerHit.currentControllerData.dying) {


                            if (currentControllerData.distanceToPlayerControlledGamePlayer < attackRange / 3.5f) {
                                // LEAP AT THEM within three
                                Tackle(gamePlayerControllerHit, Mathf.Clamp(100f - currentControllerData.distanceToPlayerControlledGamePlayer / 2, 1f, 50f));
                            }
                            else {
                                // PURSUE FASTER
                                Tackle(gamePlayerControllerHit, 3.5f + currentControllerData.distanceToPlayerControlledGamePlayer / 2);
                            }
                        }
                    }

                    // CHECK RANDOM DIE RANGE

                    bool shouldRandomlyDie = false;
                    
                    if (currentControllerData.distanceToPlayerControlledGamePlayer >= currentControllerData.distanceRandomDie) {
                        currentControllerData.isInRandomDieRange = true;
                    }
                    else {
                        currentControllerData.isInRandomDieRange = false;
                    }

                    if (currentControllerData.isInRandomDieRange) {
                        if (currentControllerData.lastRandomDie > UnityEngine.Random.Range(
                            currentControllerData.timeMinimumRandomDie, 
                            currentControllerData.timeMinimumRandomDie + currentControllerData.timeMinimumRandomDie / 2)) {
                            
                            currentControllerData.lastRandomDie = 0;
                            //shouldRandomlyDie = true;
                        }
                        
                        currentControllerData.lastRandomDie += Time.deltaTime;
                    }
                    
                    //public float currentControllerData.distanceRandomDie = 30f;
                    //public float currentControllerData.timeMinimumRandomDie = 5f;
                    
                    if (currentControllerData.lastIsInRandomDieRange != currentControllerData.isInRandomDieRange) {
                        if (currentControllerData.lastIsInRandomDieRange && !currentControllerData.isInRandomDieRange) {
                            // out of range random!
                            //GameController.CurrentGamePlayerController.Score(5);
                            //GamePlayerProgress.SetStatEvaded(1f);
                        }
                        currentControllerData.lastIsWithinEvadeRange = currentControllerData.isInRandomDieRange;
                    }

                    if (shouldRandomlyDie) {
                        runtimeData.hitCount += 10; 
                    }
                }
            }
        }
        else if (controllerState == GamePlayerControllerState.ControllerPlayer
            && GameController.Instance.gameState == GameStateGlobal.GameStarted) {
            float currentSpeed = 0;

            if (currentControllerData.mountData.isMountedVehicle) {
                currentSpeed = currentControllerData.mountData.mountVehicle.driver.currentSpeed;
            }
            else {
                currentSpeed = currentControllerData.thirdPersonController.moveSpeed;
            }
            //LogUtil.Log("currentSpeed:", currentSpeed);
            
            Vector3 pos = Vector3.zero;
            pos.z = Mathf.Clamp(currentSpeed / 3, .3f, 3.5f);
         
            if (gamePlayerEnemyTarget != null) {
                gamePlayerEnemyTarget.transform.localPosition = pos;
            }

            if (gamePlayerModelTarget != null) {
                gamePlayerModelTarget.transform.localPosition = pos;
            }

            if (currentControllerData.playerSpin) {
                // Clamps automatically angles between 0 and 360 degrees.
                float y = 360 * Time.deltaTime;

                gamePlayerModelHolder.transform.localRotation =
                    Quaternion.Euler(0, gamePlayerModelHolder.transform.localRotation.eulerAngles.y + y, 0);

                if (gamePlayerModelHolder.transform.localRotation.eulerAngles.y > 330) {
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

    public virtual void FixedUpdate() {
        
        if(!gameObjectTimer.IsTimerPerf(
            GameObjectTimerKeys.gameFixedUpdateAll, IsPlayerControlled ? 1 : 2)) {
            return;
        }

        if (!controllerReady) {
            return;
        }

        if (!currentControllerData.initialized) {
            return;
        }

        HandlePlayerAliveStateFixed();
    }

    public virtual void LateUpdate() {

        if(!gameObjectTimer.IsTimerPerf(
            GameObjectTimerKeys.gameLateUpdateAll, IsPlayerControlled ? 1 : 2)) {
            return;
        }
        
        if (!controllerReady) {
            return;
        }

        if (controllerData == null) {
            return;
        }

        if (!currentControllerData.initialized) {
            return;
        }

        HandlePlayerAliveStateLate();
    }

    public virtual void UpdateAlways() {        
        HandleCharacterAttachedSounds(); // always run to turn off audio when not playing.
        HandlePlayerInactionState();
    }

    public virtual void UpdateEditorTools() {
        
        
        if (IsPlayerControlled) {
            
            if (Application.isEditor) {
                
                if (Input.GetKey(KeyCode.LeftControl)) {
                    
                    //LogUtil.Log("GamePlayer:moveDirection:" + GameController.CurrentGamePlayerController.currentControllerData.thirdPersonController.movementDirection);
                    //LogUtil.Log("GamePlayer:aimDirection:" + GameController.CurrentGamePlayerController.currentControllerData.thirdPersonController.aimingDirection);
                    //LogUtil.Log("GamePlayer:rotation:" + GameController.CurrentGamePlayerController.transform.rotation);
                    //Vector3 point1 = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
                    //Vector3 point2 = Camera.main.ScreenToWorldPoint(new Vector3(1, 0, 1));
                    
                    //LogUtil.Log("GamePlayer:point1:" + point1);
                    //LogUtil.Log("GamePlayer:point2:" + point2);
                    
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
            
            
            if (Application.isEditor) {
                if (Input.GetKey(KeyCode.LeftControl)) {
                    if (Input.GetKey(KeyCode.RightBracket)) {
                        if (!IsPlayerControlled) {
                            Die();
                        }       
                    }
                    else if (Input.GetKey(KeyCode.V)) {                  
                        LoadWeapon("weapon-machine-gun-1");
                        
                        UINotificationDisplay.QueueTip(
                            "Machine Gun Enabled",
                            "Machine gun simulation trigger and action installed and ready.");
                    }
                    else if (Input.GetKey(KeyCode.B)) {                  
                        LoadWeapon("weapon-flame-thrower-1");
                        
                        UINotificationDisplay.QueueTip(
                            "Flame Thrower Enabled",
                            "Flame thrower simulation trigger and action installed and ready.");
                    }
                    else if (Input.GetKey(KeyCode.N)) {                  
                        LoadWeapon("weapon-shotgun-1");
                        UINotificationDisplay.QueueTip(
                            "Shotgun Enabled",
                            "Shotgun simulation trigger and action installed and ready.");
                    }
                    else if (Input.GetKey(KeyCode.M)) {                  
                        LoadWeapon("weapon-rocket-launcher-1");
                        
                        UINotificationDisplay.QueueTip(
                            "Rocket Launcher Enabled",
                            "Rocket launcher trigger and action installed and ready.");
                    }
                    else if (Input.GetKey(KeyCode.C)) {                  
                        LoadWeapon("weapon-rifle-1");
                        
                        UINotificationDisplay.QueueTip(
                            "Rifle Enabled",
                            "Rifle simulation trigger and action installed and ready.");
                    }
                }
            }
        }


    }
 
    public override void Update() { 
            
        if(!gameObjectTimer.IsTimerPerf(
            GameObjectTimerKeys.gameUpdateAll, IsPlayerControlled ? 1 : 2)) {
            return;
        }
        
        if (!controllerReady) {
            return;
        }

        if (GameConfigs.isUIRunning) {
            return;
        }
     
        if (controllerData != null && !currentControllerData.initialized) {
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

