using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseGamePlayerCollision : GameObjectBehavior {
    
    public GamePlayerController gamePlayerController;
    private GameDamageManager gameDamageManager;
    public string type = "default";
    float lastCollision = 0f;
    float intervalCollision = .042f;
    
    public virtual void Start() {

    }

    public void UpdateGameObjects() {
        
        gamePlayerController = null;
        gameDamageManager = null;
                
        if (gamePlayerController == null) {
            gamePlayerController = gameObject.FindTypeAboveRecursive<GamePlayerController>();
        }           
        
        if (gameDamageManager == null) {
            gameDamageManager = gameObject.GetOrSet<GameDamageManager>();
            gameDamageManager.UpdateGameObjects();
        }
    }

    //private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];

    public virtual void OnParticleCollision(GameObject other) {

        /*
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        if (lastCollision + intervalCollision < Time.time) {
            lastCollision = Time.time;
        }
        else {
            //return;
        }
        
        if (gamePlayerController != null) {
            
            if (!gamePlayerController.controllerReady) {
                //return;
            }

            /*
            ParticleSystem particleSystem;
            particleSystem = other.GetComponent<ParticleSystem>();
            int safeLength = particleSystem.safeCollisionEventSize;
            if (collisionEvents.Length < safeLength)
                collisionEvents = new ParticleSystem.CollisionEvent[safeLength];
            
            int numCollisionEvents = particleSystem.GetCollisionEvents(gameObject, collisionEvents);
            int i = 0;
            while (i < numCollisionEvents) {
                if (gameObject.rigidbody) {
                    Vector3 pos = collisionEvents[i].intersection;
                    Vector3 force = collisionEvents[i].velocity * 10;
                    gamePlayerController.gameObject.rigidbody.AddForce(force);
                }
                i++;
            }
            */

            //if(gamePlayerController.IsPlayerControlled) {
            //}
            //else {

            /*

            LogUtil.Log("OnParticleCollision:" + other.name);
            
            ParticleSystem particleSystem;
            particleSystem = other.GetComponent<ParticleSystem>();
            
            float power = .1f;
            
            gamePlayerController.runtimeData.health -= power;
            
            //contact.normal.magnitude
            
            gamePlayerController.Hit(power);

            if (particleSystem != null) {

                int safeLength = particleSystem.safeCollisionEventSize;
                if (collisionEvents.Length < safeLength)
                    collisionEvents = new ParticleSystem.CollisionEvent[safeLength];
                
                int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
                
                int i = 0;
                while (i < numCollisionEvents) {
                    if (other.rigidbody) {
                        //Vector3 pos = collisionEvents[i].intersection;
                        Vector3 force = collisionEvents[i].velocity * 10;
                        gamePlayerController.rigidbody.AddForce(force);
                    }
                    i++;
                }
            }
            * /
            //}
        }
    */
    }
    
    public virtual void OnCollisionEnter(Collision collision) {

        if (!GameConfigs.isGameRunning) {
            //return;
        }

        if (lastCollision + intervalCollision < Time.time) {
            lastCollision = Time.time;
        }
        else {
            //return;
        }

        if(gamePlayerController != null) {

            //foreach (ContactPoint contact in collision.contacts) {
            gamePlayerController.HandleCollision(collision);
            //  LogUtil.Log("contact:" + contact);
            //}
        }
    }
}