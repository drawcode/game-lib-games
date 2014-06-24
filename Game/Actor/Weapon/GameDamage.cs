using UnityEngine;
using System.Collections;

public class GameDamage : GameDamageBase {
    public bool Explosive;
    public float ExplosionRadius = 20;
    public float ExplosionForce = 1000;
    public bool HitedActive = true;
    public float TimeActive = 0;
    private float timetemp = 0;

    private void Start() {
        if (!Owner || !Owner.collider)
            return;
        Physics.IgnoreCollision(collider, Owner.collider);
        
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
    }

    private void ExplosionDamage() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
        for (int i = 0; i < hitColliders.Length; i++) {
            Collider hit = hitColliders[i];
            if (!hit)
                continue;
            
            GameDamageManager damageManage = hit.gameObject.GetComponent<GameDamageManager>();
            if (damageManage) {
                if (damageManage) {
                    damageManage.ApplyDamage(Damage);
                }
            }
            if (hit.rigidbody)
                hit.rigidbody.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 3.0f);
        }

    }

    private void NormalDamage(Collision collision) {
        GameDamageManager damageManage = collision.gameObject.GetComponent<GameDamageManager>();
        if (damageManage) {
            damageManage.ApplyDamage(Damage);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (HitedActive) {
            if (collision.gameObject.tag != "Particle" && collision.gameObject.tag != this.gameObject.tag) {
                if (!Explosive)
                    NormalDamage(collision);
                Active();
            }
        }
    }
}
