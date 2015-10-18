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

    private void Reset() {        
        Explosive = initialExplosive;
        ExplosionRadius = initialExplosiveRadius;
        ExplosionForce = initialExplosiveForce;
        HitedActive = initialHitedActive;
        TimeActive = initialTimeActive;
        Explosive = initialExplosive;
    }

    private void Start() {

        Reset();

        timetemp = Time.time;

        if (!gamePlayerController || !gamePlayerController.collider)
            return;

        if (!collider.enabled || !gamePlayerController.collider.enabled) {
            return;
        }

        Physics.IgnoreCollision(collider, gamePlayerController.collider);
    }

    private void Update() {

        if (!HitedActive || TimeActive > 0) {
            if (Time.time >= (timetemp + TimeActive)) {
                Active();
            }
        }
    }

    public void Active() {
                
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
    
    GameDamageManager damageManage = null;

    public void HandleApplyDamage(GameObject go) {
        if (damageManage == null) {
            damageManage = go.GetComponent<GameDamageManager>();        
        }
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
        
        if (other.transform.name == "GamePlayerCollider") {
            GamePlayerCollision gamePlayerCollision = 
                other.Get<GamePlayerCollision>();
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
