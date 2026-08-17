using UnityEngine;
using System.Collections;

public class GameDamage : GameDamageBase {

    public bool Explosive = false;
    public float ExplosionRadius = 3;
    public float ExplosionForce = 300;
    public bool HitedActive = true;
    public float TimeActive = 0;
    private float timetemp = 0;

    private bool initialExplosive = false;
    private float initialExplosiveRadius = 3;
    private float initialExplosiveForce = 300;
    private bool initialHitedActive = true;
    private float initialTimeActive = 0;

    void Awake() {
        initialExplosive = Explosive;
        initialExplosiveRadius = ExplosionRadius;
        initialExplosiveForce = ExplosionForce;
        initialHitedActive = HitedActive;
        initialTimeActive = TimeActive;
    }

    // Set once Active() has run for this life. The object is not recycled immediately
    // (the effect and trail need to finish), so without this a projectile that is still
    // sitting in the world keeps re-triggering on further contacts -- spawning another
    // effect and queueing another delayed recycle each time.

    private bool spent = false;

    private void Reset() {
        Explosive = initialExplosive;
        ExplosionRadius = initialExplosiveRadius;
        ExplosionForce = initialExplosiveForce;
        HitedActive = initialHitedActive;
        TimeActive = initialTimeActive;
        Explosive = initialExplosive;
        spent = false;
    }

    private void Start() {

        Reset();

        timetemp = Time.time;
    }

    private Collider ignoredShooterCollider = null;

    public override void OnLaunched() {

        timetemp = Time.time;

        // Physics.IgnoreCollision is a persistent property of the collider PAIR, and
        // this object is pooled. Setting it in Start was doubly wrong: Start is re-sent
        // before the launcher assigns gamePlayerController, so it paired against the
        // PREVIOUS shooter, and the pairing was never undone -- a bullet accumulated
        // ignores against every actor that had ever fired it and eventually flew
        // straight through them.

        Collider self = collider;

        if (self == null) {
            return;
        }

        if (ignoredShooterCollider != null) {

            if (self.enabled && ignoredShooterCollider.enabled) {
                Physics.IgnoreCollision(self, ignoredShooterCollider, false);
            }

            ignoredShooterCollider = null;
        }

        if (gamePlayerController == null) {
            return;
        }

        Collider shooter = gamePlayerController.collider;

        if (shooter == null || !shooter.enabled || !self.enabled) {
            return;
        }

        Physics.IgnoreCollision(self, shooter, true);

        ignoredShooterCollider = shooter;
    }

    private void Update() {

        if (!HitedActive || TimeActive > 0) {
            if (Time.time >= (timetemp + TimeActive)) {
                Active();
            }
        }
    }

    public void Active() {

        if (spent) {
            return;
        }

        spent = true;

        if (!GameDamageDirector.AllowExplosion) {
            GameObjectHelper.DestroyGameObject(gameObject);
            return;
        }

        if (Effect) {
            GameObject obj = GameObjectHelper.CreateGameObject(
                Effect, transform.position, transform.rotation, true);
            GameObjectHelper.DestroyGameObject(obj, 3, true);
        }

        if (Explosive)
            ExplosionDamage();

        GameObjectHelper.DestroyGameObject(gameObject, 3, true);
    }

    private void ExplosionDamage() {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
        for (int i = 0; i < hitColliders.Length; i++) {
            Collider hit = hitColliders[i];
            if (!hit)
                continue;

            HandleApplyDamage(hit.gameObject);

            if (hit.gameObject.Has<Rigidbody>())
                hit.gameObject.Get<Rigidbody>().AddExplosionForce(
                    ExplosionForce, transform.position, ExplosionRadius, 3.0f);
        }

    }

    private void NormalDamage(GameObject other) {

        HandleApplyDamage(other);
    }

    public void HandleApplyDamage(GameObject go) {

        // Resolve per target. This object is pooled and reused, and an
        // explosion applies damage to many targets in one pass, so the
        // manager must never be cached across calls.

        GameDamageManager damageManage = go.GetComponent<GameDamageManager>();

        if (damageManage != null) {
            if (damageManage.gamePlayerController != null && gamePlayerController != null) {
                if (damageManage.gamePlayerController.uniqueId == gamePlayerController.uniqueId) {
                    return;
                }
            }
            damageManage.ApplyDamage(Damage);
        }
    }

    private void HandleCollisions(GameObject other) {

        if (!HitedActive) {
            return;
        }

        bool doDamage = false;

        // Detect actor hit areas by component, not by object name. Character
        // prefabs shared with other games do not always name them
        // "GamePlayerCollider".

        GamePlayerCollision gamePlayerCollision =
            other.GetComponent<GamePlayerCollision>();

        if (gamePlayerCollision != null) {

            if (gamePlayerController == null) {
                return;
            }

            if (gamePlayerCollision.gamePlayerController == null) {
                return;
            }

            if (gamePlayerCollision.gamePlayerController.uniqueId == gamePlayerController.uniqueId) {
                return;
            }
            else {
                doDamage = true;
            }
        }

        if (other.tag != "Particle" && other.tag != "Player"
            && other.tag != this.gameObject.tag) {

            doDamage = true;
        }

        if (doDamage) {
            if (!Explosive) {
                NormalDamage(other);
            }
            Active();
        }
    }

    private void OnTriggerEnter(Collider collider) {
        HandleCollisions(collider.gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        HandleCollisions(collision.gameObject);
    }
}
