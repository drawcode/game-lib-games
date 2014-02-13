using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameObjectMountType {
    typeAuto,
    typeTriggered
}

public class GameObjectMount : BaseGameObjectInteractive {

    public List<GameObject> objectsMounted;
    public int allowedMountCount = 1;
        
    public override void Awake() { 
        base.Awake();   
    }
    
    public override void Start() {        
        base.Start();  
        
        objectsMounted = new List<GameObject>();
    }
    
    public override void Init() {
        base.Init();
        interactiveType = GameObjectInteractiveType.mount;
        attractGamePlayers = true;
        attractRange = 5f;
    }
    
    public override void FixedUpdate() {
        base.FixedUpdate();
    }
    
    public override void AttractForce<T>() {
        base.AttractForce<T>();
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

    public virtual void Mount(GameObject go) {
    
    }
        
    public virtual void Unmount(GameObject go) {
        
    }
    
    public override void Update() {
        base.Update();



    }
}