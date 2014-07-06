using UnityEngine;
using System.Collections;

public class GameDamage : GameDamageBase {
    public bool Explosive;
    public float ExplosionRadius = 3;
    public float ExplosionForce = 300;
    public bool HitedActive = true;
    public float TimeActive = 0;
    private float timetemp = 0;

    private void Start() {
        if (!gamePlayerController || !gamePlayerController.collider)
            return;
        Physics.IgnoreCollision(collider, gamePlayerController.collider);
        
        timetemp = Time.time;
    }

    private void Update() {
        if (!HitedActive || TimeActive > 0) {
            if (Time.time >= (timetemp + TimeActive)) {
                Active();
            }
        }
    }

    public void Active() {
        if (Effect) {
            GameObject obj = (GameObject)Instantiate(Effect, transform.position, transform.rotation);
            Destroy(obj, 3);
        }

        if (Explosive)
            ExplosionDamage();

        Destroy(gameObject);
        //GameObjectHelper.DestroyGameObject(gameObject, true);
    }

    private void ExplosionDamage() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
        for (int i = 0; i < hitColliders.Length; i++) {
            Collider hit = hitColliders[i];
            if (!hit)
                continue;

            HandleApplyDamage(hit.gameObject);

            if (hit.rigidbody)
                hit.rigidbody.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 3.0f);
        }

    }

    private void NormalDamage(Collision collision) {
        
        HandleApplyDamage(collision.gameObject);
    }

    public void HandleApplyDamage(GameObject go) {
        GameDamageManager damageManage = go.GetComponent<GameDamageManager>();
        if (damageManage) {
            if(damageManage.gamePlayerController != null && gamePlayerController != null) {
                if(damageManage.gamePlayerController.uniqueId == gamePlayerController.uniqueId) {
                    return;
                }
            }
            damageManage.ApplyDamage(Damage);
        }
    }

    private void OnCollisionEnter(Collision collision) {

        if (HitedActive) {

            bool doDamage = false;
            
            if(collision.transform.name == "GamePlayerCollider") {
                GamePlayerCollision gamePlayerCollision = 
                    collision.transform.gameObject.Get<GamePlayerCollision>();
                if(gamePlayerCollision != null) {

                    if(gamePlayerController == null) {
                        return;
                    }
                    
                    if(gamePlayerCollision.gamePlayerController == null) {
                        return;
                    }

                    if(gamePlayerCollision.gamePlayerController.uniqueId == gamePlayerController.uniqueId) {
                        return;
                    }
                    else {
                        doDamage = true;
                    }
                }
            }

            if (collision.gameObject.tag != "Particle" && collision.gameObject.tag != "Player" 
                && collision.gameObject.tag != this.gameObject.tag) {

                doDamage = true;
            }

            if(doDamage) {                
                if (!Explosive)
                    NormalDamage(collision);
                Active();
            }
        }
    }
}
