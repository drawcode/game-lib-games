using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectInteractiveCollider : MonoBehaviour {

    public BaseGameObjectInteractive obj;

    public virtual void Awake() { 
        
    }

    public virtual void Start() {
        Init();
    }

    public virtual void Init() {
        FindBase();
    }

    public virtual void FindBase() {
    
        if(obj == null) {
            obj = gameObject.FindTypeAboveRecursive<BaseGameObjectInteractive>();
        }
    }
        
    public virtual void OnCollisionEnter(Collision collision) {

        FindBase();

        if(obj == null) {
            return;
        }

        obj.OnCollisionEnter(collision);
    }
    
    public virtual void OnTriggerEnter(Collider collider) {

        FindBase();
        
        if(obj == null) {
            return;
        }
        
        obj.OnTriggerEnter(collider);
    }

}