using UnityEngine;
using System.Collections;

public class GameMoverMissile : GameWeaponBase {
    public float Damping = 3;
    public float Speed = 80;
    public float SpeedMax = 80;
    public float SpeedMult = 1;
    public Vector3 Noise = new Vector3(20, 20, 20);
    public float TargetLockDirection = 0.5f;
    public int DistanceLock = 70;
    public int DurationLock = 40;
    public bool Seeker;
    public float LifeTime = 5.0f;
    private bool locked;
    private int timetorock;
    private float timeCount = 0;

    private float initialDamping = 3;
    private float initialSpeed = 80;
    private float initialSpeedMax = 80;
    private float initialSpeedMult = 1;
    private Vector3 initialNoise = new Vector3(20, 20, 20);
    private float initialTargetLockDirection = 0.5f;
    private int initialDistanceLock = 70;
    private int initialDurationLock = 40;
    private bool initialSeeker;
    private float initialLifeTime = 5.0f;
    private bool initiallocked;
    private int initialtimetorock;
    private float initialtimeCount = 0;

    private void Start() {
        
        initialDamping = Damping;
        initialSpeed = Speed;
        initialSpeedMax = SpeedMax;
        initialSpeedMult = SpeedMult;
        initialNoise = Noise;
        initialTargetLockDirection = TargetLockDirection;
        initialDistanceLock = DistanceLock;
        initialDurationLock = DurationLock;
        initialSeeker = Seeker;
        initialLifeTime = LifeTime;
        initiallocked = locked;
        initialtimetorock = timetorock;

        Reset();
    }

    public void Reset() {
        Damping = initialDamping;
        Speed = initialSpeed;
        SpeedMax = initialSpeedMax;
        SpeedMult = initialSpeedMult;
        Noise = initialNoise;
        TargetLockDirection = initialTargetLockDirection;
        DistanceLock = initialDistanceLock;
        DurationLock = initialDurationLock;
        Seeker = initialSeeker;
        LifeTime = initialLifeTime;
        locked = initiallocked;
        timetorock = initialtimetorock;

        timeCount = Time.time;
        GameObjectHelper.DestroyGameObject(gameObject, LifeTime);
    }
    
    private void FixedUpdate() {
        rigidbody.velocity = new Vector3(transform.forward.x * Speed * Time.fixedDeltaTime, transform.forward.y * Speed * Time.fixedDeltaTime, transform.forward.z * Speed * Time.fixedDeltaTime);
        rigidbody.velocity += new Vector3(Random.Range(-Noise.x, Noise.x), Random.Range(-Noise.y, Noise.y), Random.Range(-Noise.z, Noise.z));
        
        if (Speed < SpeedMax) {
            Speed += SpeedMult * Time.fixedDeltaTime;
        }
    }

    GameDamage gameDamageObject = null;

    private void Update() {
        if (Time.time >= (timeCount + LifeTime) - 0.5f) {
            if(gameDamageObject == null) {
                gameDamageObject = GetComponent<GameDamage>();
            }
            if (gameDamageObject != null) {
                gameDamageObject.Active();
            }
        }
        
        if (Target) {
            Quaternion rotation = Quaternion.LookRotation(Target.transform.position - transform.transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * Damping);
            Vector3 dir = (Target.transform.position - transform.position).normalized;
            float direction = Vector3.Dot(dir, transform.forward);
            if (direction < TargetLockDirection) {
                Target = null;
            }
        }
        
        if (Seeker) {
            if (timetorock > DurationLock) {
                if (!locked && !Target) {
                    float distance = int.MaxValue;
                    for (int t=0; t<TargetTag.Length; t++) {
                        if (GameObject.FindGameObjectsWithTag(TargetTag[t]).Length > 0) {
                            GameObject[] objs = GameObject.FindGameObjectsWithTag(TargetTag[t]);

                            for (int i = 0; i < objs.Length; i++) {
                                if (objs[i]) {
                                    Vector3 dir = (objs[i].transform.position - transform.position).normalized;
                                    float direction = Vector3.Dot(dir, transform.forward);
                                    float dis = Vector3.Distance(objs[i].transform.position, transform.position);
                                    if (direction >= TargetLockDirection) {
                                        if (DistanceLock > dis) {
                                            if (distance > dis) {
                                                distance = dis;
                                                Target = objs[i];
                                            }
                                            locked = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else {
                timetorock += 1;
            }

            if (Target) {
                
            }
            else {
                locked = false;

            }
        }
    }

}
