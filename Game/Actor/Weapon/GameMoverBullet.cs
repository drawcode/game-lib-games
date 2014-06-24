using UnityEngine;
using System.Collections;

public class GameMoverBullet : GameWeaponBase {
    public int Lifetime;
    public float Speed = 80;
    public float SpeedMax = 80;
    public float SpeedMult = 1;

    private void Start() {
        Destroy(gameObject, Lifetime);
    }

    private void FixedUpdate() {
        if (!this.rigidbody)
            return;
        
        if (!RigidbodyProjectile) {
            rigidbody.velocity = transform.forward * Speed;
        }
        else {
            if (this.rigidbody.velocity.normalized != Vector3.zero)
                this.transform.forward = this.rigidbody.velocity.normalized;    
        }
        if (Speed < SpeedMax) {
            Speed += SpeedMult * Time.fixedDeltaTime;
        }
    }

}
