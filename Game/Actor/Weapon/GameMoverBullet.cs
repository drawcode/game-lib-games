using UnityEngine;
using System.Collections;

public class GameMoverBullet : GameWeaponBase {

    public int Lifetime;
    public float Speed = 80;
    public float SpeedMax = 80;
    public float SpeedMult = 1;
    private bool hasRigidBody = false;
    private Rigidbody rigbody;

    private float initialSpeed = 80;
    private float initialSpeedMax = 80;
    private float initialSpeedMult = 1;
    private int initialLifeTime = 5;

    private void Start() {        
        
        GameObjectHelper.DestroyGameObject(gameObject, Lifetime);
        rigbody = this.rigbody;
        hasRigidBody = rigbody ? true : false;

        initialSpeed = Speed;
        initialSpeedMax = SpeedMax;
        initialSpeedMult = SpeedMult;
        initialLifeTime = Lifetime;

        Reset();
    }
    
    public void Reset() {
        Speed = initialSpeed;
        SpeedMax = initialSpeedMax;
        SpeedMult = initialSpeedMult;
        Lifetime = initialLifeTime;

    }

    private void FixedUpdate() {
        if (!hasRigidBody)
            return;

        if (!RigidbodyProjectile) {
            rigbody.velocity = transform.forward * Speed;
        }
        else {
            if (rigbody.velocity.normalized != Vector3.zero)
                this.transform.forward = rigbody.velocity.normalized;    
        }
        
		if (Speed < SpeedMax) {
            Speed += SpeedMult * Time.fixedDeltaTime;
        }
    }

}
