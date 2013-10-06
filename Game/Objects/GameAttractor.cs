using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameAttractor : MonoBehaviour {

        public float attractForce = 5000f;
        public float attractRange = 1f;
        
        List<Rigidbody> rbs = new List<Rigidbody>();
        
        void Start() {
                
        }
 
        void FixedUpdate() {
                if(GameDraggableEditor.appEditState == GameDraggableEditEnum.StateNotEditing) {
                        AttractForce<GameProjectile>();
                }
        }
        
        void AttractForce<T>() {
                Collider[] cols  = Physics.OverlapSphere(transform.position, attractRange); 
                
                rbs.Clear();
 
                foreach(Collider c in cols) {   
                        Component[] comps = c.gameObject.GetComponents(typeof(T));
                        if(comps != null) {
                                if(comps.Length > 0) {
                                        Rigidbody rb = c.attachedRigidbody;
                                        if(rb != null && rb != rigidbody && !rbs.Contains(rb)) {
                                                rbs.Add(rb);
                                                Vector3 offset = transform.position - c.transform.position;
                                                rb.AddForce( offset / offset.sqrMagnitude * rb.mass);
                                        }
                                }
                        }
                }
        }
        
        void AddForce(GameObject target, float force) {
                Vector3 dir = target.transform.position - transform.position;
                dir = dir.normalized;
                rigidbody.AddForce(dir * force);
        }
        
        void DestroyMe() {
                LogUtil.Log("Destroying Attractor");
                Destroy(gameObject);
        }
        
        void OnCollisionEnter(Collision collision) {
                if(!GameAppController.shouldRunGame) {
                        return;
                }
                
                GameObject target = collision.collider.gameObject;
                
                if(target != null) {
                        
                }
        }
        
        void OnTriggerEnter(Collider collider) {
                // Check if we hit an actual destroyable sprite
                if(!GameAppController.shouldRunGame) {
                        return;
                }
                
                GameObject target = collider.gameObject;
                
                if(target != null) {
                
                }
        
        }
        
        void Update() {
        
                if(GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
                        //DestroyMe();
                }
        }
        
}