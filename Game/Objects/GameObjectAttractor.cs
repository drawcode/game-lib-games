using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectAttractor : BaseGameObjectInteractive {

    public override void Awake() {
        base.Awake();
    }

    public override void Start() {
        base.Start();
    }

    public override void Init() {
        base.Init();
        interactiveType = GameObjectInteractiveType.attractor;
        attractGamePlayers = true;
    }

    public override void FixedUpdate() {
        base.FixedUpdate();
    }

    public override void AttractForce<T>() {
        base.AttractForce<T>();
    }

    public override void Boost(GameObject go) {
        base.Boost(go);
    }

    public override void AddForce(GameObject target, float force) {
        base.AddForce(target, force);
    }

    public override void OnCollisionEnter(Collision collision) {
        base.OnCollisionEnter(collision);
    }

    public override void OnTriggerEnter(Collider collider) {
        base.OnTriggerEnter(collider);
    }

    public override void Update() {
        base.Update();
    }
}