using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class BaseGamePlayerSlots {
    public static string slotPrimary = "primary";
    public static string slotSecondary = "secondary";
    public static string slotExtra = "extra";
}

public class BaseGamePlayerRuntimeData {

    public GamePlayerController currentController = null;
    public double health = 1f;
    public double energy = 1f;
    public double speed = 1f;
    public double scale = 1f;
    public double attack = 1f;
    public double defense = 1f;
    //
    //
    public double scores = 0;
    public double score = 0;
    public double coins = 0;
    public double specials = 0;
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
    public double kills = 0f;
    public double builds = 0f;
    public double attacks = 0f;
    public double repairs = 0f;
    public double defends = 0f;

    public void SetController(GamePlayerController controller) {
        currentController = controller;
    }

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

    public bool IsAlive {
        get {
            return hitHealthRemaining > 0;
        }
    }

    public bool IsCompletedCollections() {
        return IsCompletedCollections(AppContentCollects.Current);
    }

    public bool IsCompletedCollections(AppContentCollect appContentCollect) {

        if(appContentCollect != null) {

            if(currentController == null) {
                if(GameController.CurrentGamePlayerController != null) {
                    currentController = GameController.CurrentGamePlayerController;
                }
            }
            if(currentController != null) {
                if(GameController.isInst) {
                    return appContentCollect.IsCompleted(
                        GameController.Instance.runtimeData,
                        currentController.runtimeData);
                }
            }
        }

        return false;
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
    public float lastExit = 0f;
    public string lastCharacterCode = null;
    public bool actorExiting = false;
    public bool actorEntering = false;

    // controller

    public GamePlayerController gamePlayerController;

    // animation
    public GamePlayerControllerAnimation gamePlayerControllerAnimation;

    // colliders
    public GamePlayerCollision gamePlayerCollision;

    // gameplay
    public float lastAirCheck = 0f;
    public float lastUpdateCommon = 0f;
    public float lastAttackTime = 0;
    public float lastBoostTime = 0;
    public float lastStrafeLeftTime = 0;
    public float lastMoveLeftTime = 0;
    public float lastMoveRightTime = 0;
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
    public bool shouldTackle = true;
    public float incrementScore = 1f;

    // easing

    public bool easeModelHolderEnabled = false;
    public Vector3 easeModelHolderStart = Vector3.zero;
    public Vector3 easeModelHolderEnd = Vector3.zero.WithY(200);
    public Vector3 easeModelHolderCurrent = Vector3.zero;

    // effects

    // effects - warps

    public bool effectWarpEnabled = false;
    public float effectWarpStart = 0f;
    public float effectWarpEnd = 200f;
    public float effectWarpCurrent = 0f;
    public float effectWarpFadeSpeed = 50f;
    public Vector3 effectWarpPosition = Vector3.zero;

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
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
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
    //public Vectrosity.VectorLine lineAim = null;
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

    // rpg

    public GameDataItemRPG characterRPG = null;

    // gameplay type and move/adjustments

    public Vector3 moveGamePlayerPositionTo = Vector3.zero;
    public float speedInfinite = 0f;
    public float speedInfiniteTo = 0f;
    public float speedInfiniteMax = 0f;

    public Vector3 moveGamePlayerPosition = Vector3.zero;
    public Vector3 currentGamePlayerPosition = Vector3.zero;
    public Vector3 overallGamePlayerPosition = Vector3.zero;
    public Vector3 currentGamePlayerPositionBounce = Vector3.zero;

    // Reset runtime values, only reset values that change from life to life
    // some are 

    public void ResetRuntime() {

        moveGamePlayerPositionTo = Vector3.zero;
        speedInfinite = 0f;

        //if(Context.Current.isMobileAndroid) {
        speedInfiniteTo = 72f;
        speedInfiniteMax = 82f;
        //}
        //else {
        //    speedInfiniteTo = 72f;
        //    speedInfiniteMax = 90f;
        //}

        moveGamePlayerPosition = Vector3.zero;
        currentGamePlayerPosition = Vector3.zero;
        overallGamePlayerPosition = Vector3.zero;
        currentGamePlayerPositionBounce = Vector3.zero;
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
            if(isMountedVehicleObject) {
                return true;
            }

            return false;
        }
    }

    public bool isMountedVehicleObject {
        get {
            if(mountVehicle != null) {
                return true;
            }
            return false;
        }
    }

    public void MountVehicle(GameObject go, GameObjectMountVehicle mount) {
        if(!isMountedVehicleObject) {
            mountVehicle = mount;
            mountVehicle.Mount(go);
        }
    }

    public void UnmountVehicle() {
        if(isMountedVehicleObject) {
            mountVehicle.Unmount();
            mountVehicle = null;
        }
    }

    public void SetMountVehicleAxis(float h, float v) {
        if(mountVehicle != null) {
            mountVehicle.SetMountVehicleAxis(h, v);
        }
    }

    public void SetMountWeaponRotator(Vector3 rt) {
        if(mountVehicle != null) {
            mountVehicle.SetMountWeaponRotator(rt);
        }
    }

    public void SetMountWeaponRotator(Quaternion qt) {
        if(mountVehicle != null) {
            mountVehicle.SetMountWeaponRotator(qt);
        }
    }
}