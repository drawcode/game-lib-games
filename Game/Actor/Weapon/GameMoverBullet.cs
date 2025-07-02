#pragma warning disable 1717
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

    private void Awake() {        
        initialSpeed = Speed;
        initialSpeedMax = SpeedMax;
        initialSpeedMult = SpeedMult;
        initialLifeTime = Lifetime;
    }

    private void Start() {        

        Reset();
                
        GameObjectHelper.DestroyGameObject(gameObject, Lifetime);

        rigbody = this.rigbody;
        hasRigidBody = rigbody ? true : false;
    }
    
    public void Reset() {

        gameObject.ResetRigidBodiesVelocity();

        Speed = initialSpeed;
        SpeedMax = initialSpeedMax;
        SpeedMult = initialSpeedMult;
        Lifetime = initialLifeTime;
    }

    private void FixedUpdate() {
        if (!hasRigidBody)
            return;

        if (!RigidbodyProjectile) {
            rigbody.linearVelocity = transform.forward * Speed;
        }
        else {
            if (rigbody.linearVelocity.normalized != Vector3.zero)
                this.transform.forward = rigbody.linearVelocity.normalized;    
        }
        
        if (Speed < SpeedMax) {
            Speed += SpeedMult * Time.fixedDeltaTime;
        }
    }

}
