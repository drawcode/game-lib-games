using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameObjectInteractiveType {
    boost,
    freeze,
    beamup,
    item, // action from item...,
    attractor // action from item...
}

public class GameObjectInteractiveBase : MonoBehaviour {
    
    public string uuid = "";
    public string code = "";

    public GameObjectInteractiveType interactiveType = GameObjectInteractiveType.boost;

    // attraction 
    public float attractForce = 5000f;
    public float attractRange = 1f;
    public bool attractProjectiles = false;
    public bool attractGamePlayers = false;
    public List<Rigidbody> rbs = new List<Rigidbody>();

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
        if(GameDraggableEditor.isEditing 
           && GameConfigs.isGameRunning) {

            if(attractProjectiles) {
                AttractForce<GameProjectile>();
            }
            
            if(attractGamePlayers) {
                AttractForce<GamePlayerController>();
            }
        }
    }
        
    public virtual void AttractForce<T>() {

        if(!attractProjectiles && !attractGamePlayers) {
            return;
        }

        Collider[] cols = Physics.OverlapSphere(transform.position, attractRange); 
        
        rbs.Clear();
        
        foreach(Collider c in cols) {   
            Component[] comps = c.gameObject.GetComponents(typeof(T));
            if(comps != null) {
                if(comps.Length > 0) {
                    Rigidbody rb = c.attachedRigidbody;
                    if(rb != null && rb != rigidbody && !rbs.Contains(rb)) {
                        rbs.Add(rb);
                        Vector3 offset = transform.position - c.transform.position;
                        rb.AddForce(offset / offset.sqrMagnitude * rb.mass);
                    }
                }
            }
        }
    }


    public virtual void Boost(GameObject go) {

        LogUtil.Log("Boost:go", go.name);
        LogUtil.Log("Boost:boostGamePlayers", boostGamePlayers);
        LogUtil.Log("Boost:boostProjectiles", boostProjectiles);

        //

        if(!boostGamePlayers && !boostProjectiles) {
            return;
        }
                
        LogUtil.Log("Boost:boostGamePlayers", boostGamePlayers);
        LogUtil.Log("Boost:boostProjectiles", boostProjectiles);

        if(lastBoost + 3f < Time.time) {
            lastBoost = Time.time;
        }
        else {
            return;
        }

        if(boostGamePlayers) {

            GamePlayerController gamePlayerController = GameController.GetGamePlayerControllerObject(go, true);

            if(gamePlayerController != null) {
                if(gamePlayerController.IsPlayerControlled) {
                    
                    LogUtil.Log("Boost:gamePlayerController.IsPlayerControlled", gamePlayerController.IsPlayerControlled);
                    gamePlayerController.Boost(boostForce);
                }
            }
        }

        if(boostProjectiles) {
            
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
        if(!GameConfigs.isGameRunning) {
            return;
        }
        
        GameObject target = collision.collider.gameObject;
        
        if(target != null) {
            Boost(target);            
        }
    }
    
    public virtual void OnTriggerEnter(Collider collider) {
        if(!GameConfigs.isGameRunning) {
            return;
        }
        
        GameObject target = collider.gameObject;
        
        if(target != null) {
            Boost(target);
        }
    }
    
    public virtual void Update() {
        
        if(!GameConfigs.isGameRunning 
           && !GameDraggableEditor.isEditing) {
            //DestroyMe();
        }
    }
}