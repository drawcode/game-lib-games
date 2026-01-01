using UnityEngine;
using System.Collections;
using Engine.Game.App;

public class GameDamageDirector {

    public static float intervalGameDamageExplosionDamage = 0.2f;
    public static float lastGameDamageExplosionDamage = 0;
    public static float intervalGameDamageChainDamage = 0.2f;
    public static float lastGameDamageChainDamage = 0;
    public static float intervalRayShoot = 0.05f;
    public static float lastRayShoot = 0;

    public static bool AllowExplosion {

        get {

            if (GameDamageDirector.lastGameDamageExplosionDamage +
                GameDamageDirector.intervalGameDamageExplosionDamage < Time.time) {
                GameDamageDirector.lastGameDamageExplosionDamage = Time.time;
                return true;
            }

            return false;
        }
    }

    public static bool AllowChain {

        get {

            if (GameDamageDirector.lastGameDamageChainDamage +
                GameDamageDirector.intervalGameDamageChainDamage < Time.time) {
                GameDamageDirector.lastGameDamageChainDamage = Time.time;
                return true;
            }

            return false;
        }
    }

    public static bool AllowRayShoot {

        get {

            if (GameDamageDirector.lastRayShoot +
                GameDamageDirector.intervalRayShoot < Time.time) {
                GameDamageDirector.lastRayShoot = Time.time;
                return true;
            }

            return false;
        }
    }

}

public class GameDamageManager : MonoBehaviour {
    public string effectDestroy;
    public string audioHit;
    public float HP = 100f;
    public bool enableObjectRemove = true;
    public GamePlayerController gamePlayerController;
    public GameZoneActionAsset gameZoneActionAsset;

    private void Start() {
        UpdateGameObjects();
    }

    public void UpdateGameObjects() {

        if (string.IsNullOrEmpty(effectDestroy)) {
            //effectDestroy = "effect-explosion";
        }
        if (string.IsNullOrEmpty(audioHit)) {
            //audioHit = "attack-hit-1";
        }

        gamePlayerController = null;
        gameZoneActionAsset = null;

        if (gamePlayerController == null) {
            gamePlayerController =
                gameObject.FindTypeAboveRecursive<GamePlayerController>();
        }
        if (gameZoneActionAsset == null) {
            gameZoneActionAsset =
                gameObject.FindTypeAboveRecursive<GameZoneActionAsset>();
        }
    }

    public void ApplyDamage(float damage) {

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (gamePlayerController != null) {

            if (gamePlayerController.IsPlayerControlled
                || gamePlayerController.IsSidekickControlled) {
                // 1/10th power for friendly fire
                damage = damage / 10f;
            }

            if (!gamePlayerController.isDead
                && !gamePlayerController.IsPlayerControlled) {
                gamePlayerController.Hit(damage / 10);
            }
        }
        else {
            if (HP < 0)
                return;

            if (!string.IsNullOrEmpty(audioHit)) {
                GameAudio.PlayEffect(audioHit);
            }

            HP -= damage;
            if (HP <= 0) {
                Dead();
            }
        }
    }

    private void Dead() {

        if (!string.IsNullOrEmpty(effectDestroy)) {
            AppContentAssets.LoadAssetEffects(
                effectDestroy, transform.position, transform.rotation);
        }

        if (gamePlayerController != null) {
            gamePlayerController.Die();
        }
        else if (gameZoneActionAsset != null) {
            gameZoneActionAsset.AssetAnimationPlayNormalized(0f);
        }
        else {
            if (enableObjectRemove) {
                GameObjectHelper.DestroyGameObject(this.gameObject, true);
            }
        }
    }

    public virtual void OnParticleCollision(GameObject other) {

        if (gamePlayerController != null) {

            //gamePlayerController.OnParticleCollision(other);
        }
        else {

            if (other.name.Contains("projectile-")) {
                // todo lookup projectile and power to subtract.                    
                float projectilePower = 3;
                ApplyDamage(projectilePower);
            }
        }
    }

}
