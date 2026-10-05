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

        // The pool re-sends Start on every revive, so everything a fresh bullet needs
        // has to happen here and not in Awake.

        if (rigbody == null) {
            rigbody = GetComponent<Rigidbody>();
            hasRigidBody = rigbody != null;
        }

        Reset();

        GameObjectHelper.DestroyGameObject(gameObject, Lifetime);
    }

    public void Reset() {

        // Clear LINEAR velocity too, not just angular. A recycled bullet otherwise
        // carries the velocity it died with -- after a ricochet or a mid-flight
        // recycle that is a large vector in an arbitrary direction, and the launcher's
        // impulse adds to it, sending the next shot off at a wild angle.

        gameObject.ResetRigidBodiesMotion();

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
            if (rigbody.linearVelocity.sqrMagnitude > 0.0001f)
                this.transform.forward = rigbody.linearVelocity.normalized;
        }

        if (Speed < SpeedMax) {
            Speed += SpeedMult * Time.fixedDeltaTime;
        }
    }

}
