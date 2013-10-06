using UnityEngine;
using System.Collections;

public class GameProjectile : MonoBehaviour {

        public int allowedCollisions = 7;
        public int lifeSeconds = 10;
        int currentCollisions = 0;
        
        
        void Start() {
                currentCollisions = allowedCollisions;
                Invoke ("DestroyMe", lifeSeconds);
        }
        
        void DestroyMe() {
                LogUtil.Log("Destroying Projectile: " + currentCollisions);
                Destroy(gameObject);
        }
        
        void OnCollisionEnter(Collision collision) {
                if(!GameAppController.shouldRunGame) {
                        return;
                }
                
                GameObject target = collision.collider.gameObject;
                
                if(target != null) {
                        //foreach(GameLevelSprite gameLevelSprite in target.GetComponentsInChildren<GameLevelSprite>()) {               
                        if(currentCollisions > 0)
                                currentCollisions--;
                        
                        LogUtil.Log("target:" + target);
                
                        foreach(GameLevelSprite gameLevelSprite in target.GetComponentsInChildren<GameLevelSprite>()) {
                                if(gameLevelSprite.gameDraggableLevelItem.gameLevelItemAsset.destructable) {
                                        gameLevelSprite.HandlePhysicsInit();
                                        gameLevelSprite.gameDraggableLevelItem.DestroyMeAnimated();
                                }
                        }
                                
                                //break;
                        //}
                }
                
                        // Check if we should 
                
        }
        
        void OnTriggerEnter(Collider collider) {
                // Check if we hit an actual destroyable sprite
                if(!GameAppController.shouldRunGame) {
                        return;
                }
                
                GameObject target = collider.gameObject;
                
                if(target != null) {
                        foreach(GameLevelSprite gameLevelSprite in target.GetComponentsInChildren<GameLevelSprite>()) {
                                Debug.Log (gameLevelSprite);//.DestroyMe();
                                if(gameLevelSprite.gameDraggableLevelItem.gameLevelItemAsset.destructable) {
                                        DestroyMe();
                                }
                        }
                }
        
        }
        
        void Update() {
        
                if(GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing
                        || GameController.Instance.gameState == GameStateGlobal.GameResults
                        || GameController.Instance.gameState == GameStateGlobal.GameInit
                        || GameController.Instance.gameState == GameStateGlobal.GameQuit
                        || currentCollisions == 0) {
                        DestroyMe();
                }
        }
        
}