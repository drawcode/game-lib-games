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

    // PARTICLE WEAPON DAMAGE (the flame thrower is the one that matters).
    //
    // Particle weapons do not spawn a projectile object, so they never reach the launcher's
    // damage path at all -- their only route is GameDamageManager.OnParticleCollision, which
    // applied a hardcoded 3 and carried a "todo lookup projectile and power" note.
    //
    // Dials rather than literals so they can be tuned in one place, the way
    // GameIndicatorConfigs holds the indicator dials.
    public static float particleDamage = 3f;

    // Multiplier at point blank, falling linearly to 1 at particleDamageCloseRange. A flame
    // thrower is meant to be brutal in someone's face and weak at the edge of its plume; with a
    // flat number it was neither.
    public static float particleDamageCloseMultiplier = 4f;
    public static float particleDamageCloseRange = 6f;

    /// <summary>
    /// Damage for one particle-collision frame, scaled up as the emitter gets closer.
    /// </summary>
    public static float ParticleDamageAtDistance(float distance) {

        if (particleDamageCloseRange <= 0f) {
            return particleDamage;
        }

        float t = Mathf.Clamp01(distance / particleDamageCloseRange);

        return particleDamage * Mathf.Lerp(particleDamageCloseMultiplier, 1f, t);
    }

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

    // The pool revives an object by re-sending Start; Awake runs once, in its first life
    // only. HP was never re-armed, so a recycled destructible came back on whatever the
    // previous life spent it down to -- and once it was negative, `if (HP < 0) return;`
    // in ApplyDamage made it permanently UNDAMAGEABLE. Snapshot the authored value where
    // it is still authored, restore it per life.

    private float hpAuthored = -1f;

    private void Awake() {
        hpAuthored = HP;
    }

    private void Start() {

        if (hpAuthored >= 0f) {
            HP = hpAuthored;
        }

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

        if (other == null) {
            return;
        }

        // NameEffect stamps every weapon's particles as "projectile-<code>", which is how a
        // weapon's particles are told apart from ambient ones.
        if (!other.name.Contains("projectile-")) {
            return;
        }

        // Closer hurts more. Measured from the emitter, which is the weapon muzzle.
        float distance = Vector3.Distance(other.transform.position, transform.position);

        // ACTORS TAKE THIS TOO. The actor branch here was an empty `if` with the one call in it
        // commented out, so a particle weapon could only ever damage destructible props -- the
        // flame thrower did literally nothing to an enemy it was pointed at. ApplyDamage already
        // routes an actor hit through gamePlayerController.Hit and applies the friendly-fire
        // reduction, so both cases go through the same door.
        ApplyDamage(GameDamageDirector.ParticleDamageAtDistance(distance));
    }

}
