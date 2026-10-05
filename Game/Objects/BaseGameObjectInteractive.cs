using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameObjectInteractiveType {
    mount,
    boost,
    freeze,
    beamup,
    item, // action from item...,
    attractor // action from item...
}

public class GameObjectInteractiveMessages {
    public static string attractForceTrigger = "attract-force-trigger";
}

public class BaseGameObjectInteractive : GameObjectBehavior {

    public string uuid = UniqueUtil.CreateUUID4();
    public string code = "default";
    public GameObjectInteractiveType interactiveType = GameObjectInteractiveType.boost;

    // attraction 
    public float attractForce = 5000f;
    public float attractRange = 1f;
    public bool attractProjectiles = false;
    public bool attractGamePlayers = false;
    public List<Rigidbody> rbs = new List<Rigidbody>();

    // Reused by AttractForce so the physics query does not allocate per step. 64 is a cap, not a
    // count: OverlapSphereNonAlloc simply stops filling past the end of the buffer.
    private readonly Collider[] attractColliders = new Collider[64];

    // Below this separation the pull direction is meaningless and the 1/d^2 term explodes.
    private const float attractMinDistanceSquared = 0.0001f;

    // boost
    public float lastBoost = 0f;
    public float boostForce = 3f;
    public bool boostProjectiles = false;
    public bool boostGamePlayers = false;

    public virtual void Awake() {

    }

    public virtual void Start() {
        Init();
    }

    public virtual void Init() {

    }

    public virtual void FixedUpdate() {
        // `isEditing && isGameRunning` -- which could never both be true. isGameRunning is
        // `GameController.IsGameRunning && !isUIRunning`, and the level editor is a UI, so
        // isEditing implies isUIRunning implies !isGameRunning. Attract has therefore never run
        // for anyone: an object flagged attractProjectiles/attractGamePlayers simply did nothing.
        //
        // Read as intended: attract during play, and NOT while the level is being edited. Both
        // flags default to false, so this only wakes up on objects that were explicitly authored
        // to attract.
        if (!GameDraggableEditor.isEditing
            && GameConfigs.isGameRunning) {

            if (attractProjectiles) {
                AttractForce<GameProjectile>();
            }

            if (attractGamePlayers) {
                AttractForce<GamePlayerController>();
            }
        }
    }

    public virtual void AttractForce<T>() {

        if (!attractProjectiles && !attractGamePlayers) {
            return;
        }

        // OverlapSphereNonAlloc: this runs in FixedUpdate, and the allocating overload handed back
        // a fresh Collider[] every physics step.
        int count = Physics.OverlapSphereNonAlloc(
            transform.position, attractRange, attractColliders);

        rbs.Clear();

        for (int i = 0; i < count; i++) {

            Collider c = attractColliders[i];

            if (c == null) {
                continue;
            }

            // GetComponent<T>, not GetComponents(typeof(T)): the old call allocated a Component[]
            // for every collider in range on every physics step just to ask whether the array was
            // empty.
            if (c.gameObject.GetComponent<T>() == null) {
                continue;
            }

            Rigidbody rb = c.attachedRigidbody;

            if (rb != null && rb != rigidbody && !rbs.Contains(rb)) {

                rbs.Add(rb);

                Vector3 offset = transform.position - c.transform.position;

                // A collider sitting exactly on the attractor makes sqrMagnitude 0, and the
                // division then produces an infinite/NaN force. Unity does not reject that -- the
                // rigidbody's position becomes NaN and the object disappears for good. Skip the
                // degenerate case; there is no meaningful direction to pull in.
                float distanceSquared = offset.sqrMagnitude;

                if (distanceSquared < attractMinDistanceSquared) {
                    continue;
                }

                rb.AddForce(offset / distanceSquared * rb.mass);
            }
        }
    }

    public virtual void Boost(GameObject go) {

        LogUtil.Log("Boost:go", go.name);
        LogUtil.Log("Boost:boostGamePlayers", boostGamePlayers);
        LogUtil.Log("Boost:boostProjectiles", boostProjectiles);

        //

        if (!boostGamePlayers && !boostProjectiles) {
            return;
        }

        LogUtil.Log("Boost:boostGamePlayers", boostGamePlayers);
        LogUtil.Log("Boost:boostProjectiles", boostProjectiles);

        if (lastBoost + 3f < Time.time) {
            lastBoost = Time.time;
        }
        else {
            return;
        }

        if (boostGamePlayers) {

            GamePlayerController gamePlayerController = GameController.GetGamePlayerControllerObject(go, true);

            if (gamePlayerController != null) {
                if (gamePlayerController.IsPlayerControlled) {

                    LogUtil.Log("Boost:gamePlayerController.IsPlayerControlled", gamePlayerController.IsPlayerControlled);
                    gamePlayerController.Boost(boostForce);
                }
            }
        }

        if (boostProjectiles) {

            //GameProjectile projectile = GameController.GetGamePlayerControllerObject(go);

            //if(gamePlayerController != null) {
            //    gamePlayerController.Boost();
            //}
        }

    }

    public virtual void AddForce(GameObject target, float force) {
        Vector3 dir = target.transform.position - transform.position;
        dir = dir.normalized;
        rigidbody.AddForce(dir * force);
    }

    public virtual void DestroyMe() {
        LogUtil.Log("Destroying:" + gameObject.name);
        Destroy(gameObject);
    }

    public virtual void OnCollisionEnter(Collision collision) {
        if (!GameConfigs.isGameRunning) {
            return;
        }

        GameObject target = collision.collider.gameObject;

        if (target != null) {
            Boost(target);
        }
    }

    public virtual void OnTriggerEnter(Collider collider) {
        if (!GameConfigs.isGameRunning) {
            return;
        }

        GameObject target = collider.gameObject;

        if (target != null) {
            Boost(target);
        }
    }

    public virtual void Update() {

        if (!GameConfigs.isGameRunning
            && !GameDraggableEditor.isEditing) {
            //DestroyMe();
        }
    }
}