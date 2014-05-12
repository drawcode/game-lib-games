using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Engine.Events;

public class GameDraggableLevelItem : GameObjectBehavior {
	
	public GameObject dragColliderObject;
	public GameObject gameLevelItemObject;
	public GameObject dragHolder;
	
	public GameLevelItemAsset gameLevelItemAsset;	

    public NavMeshAgent navAgent;

    bool frozen = false;
    bool visible = true;
			
	void Awake() {
        //navAgent = gameObject.AddComponent<NavMeshAgent>();
        //navAgent.
	}
	
	public virtual void Start() {
		Init();
	}
	
	public virtual void Init() {
		
		if(dragColliderObject != null) {
			dragColliderObject.tag = "drag";
		}
		
		LoadData();
		
		if(gameLevelItemAsset != null) {
			LoadSprite(gameLevelItemAsset.asset_code);
		}
	}
	
	void OnEnable() {		
		Messenger<GameDraggableEditEnum>.AddListener(
            GameDraggableEditorMessages.EditState, OnEditStateHandler);
	}
	
	void OnDisable() {		
		Messenger<GameDraggableEditEnum>.RemoveListener(
            GameDraggableEditorMessages.EditState, OnEditStateHandler);
	}
	
	void OnEditStateHandler(GameDraggableEditEnum state) {
		if(state == GameDraggableEditEnum.StateEditing) {
			
		}
		else if(state == GameDraggableEditEnum.StateNotEditing) {
			
		}
	}
	
	public void ShowAllGameLevelItems() {
		if(gameLevelItemObject != null) {
			foreach(Transform t in gameLevelItemObject.transform) {
				t.gameObject.Show();
			}
		}
	}
	
	public void HideAllGameLevelItems() {
		if(gameLevelItemObject != null) {
			foreach(Transform t in gameLevelItemObject.transform) {
				t.gameObject.Hide();
			}
		}
	}
	
	public void LoadSprite(string spriteCode) {
		
		if(gameLevelItemObject != null) {
			
			RemoveGameLevelItems();
			
			GameObject go = GameDraggableEditor.LoadSprite(
				gameLevelItemObject, spriteCode, Vector3.one);

            //LogUtil.Log("LoadSprite:exists:" + go != null);

			if(go != null) {

                //LogUtil.Log("LoadSprite:" + go.name);

				GameLevelSprite gameLevelSprite = go.GetComponent<GameLevelSprite>();

                if(gameLevelSprite != null) {
					gameLevelSprite.gameDraggableLevelItem = this;

                    //LogUtil.Log("LoadSprite:gameLevelSprite:" + gameLevelSprite.name);
				}
				//go.transform.parent.transform.parent.transform.parent.transform.parent.position = Vector3.zero;
				//go.transform.parent.transform.parent.transform.parent.transform.parent.localPosition = Vector3.zero;
			}
		}
	}
	
	public void RemoveGameLevelItems() {
		if(gameLevelItemObject != null) {
			// clear current or placeholder...
			foreach(Transform t in gameLevelItemObject.transform) {
				Destroy (t.gameObject);
			}
		}
	}
	
	public void LoadSpriteEffect(string spriteEffectCode) {
		
		if(gameLevelItemAsset != null) {

			GameObject go = GameDraggableEditor.LoadSpriteEffect(
				gameLevelItemObject,
                gameLevelItemAsset.destroy_effect_code,
                Vector3.one.WithX(3).WithY(3).WithZ(3) * .1f);

			if(go != null) {
				PackedSprite sprite = go.GetComponent<PackedSprite>();
				if(sprite != null) {
					sprite.PlayAnim(0);
				}
				go.transform.parent = gameLevelItemObject.transform;
			}
		}
	}
		
	public void DestroyGameLevelItemSprite() {
		RemoveGameLevelItems();
		if(gameLevelItemAsset != null) {
			LoadSpriteEffect(gameLevelItemAsset.destroy_effect_code);
		}
		Invoke("DestroyMe",.3f);
	}
	
	public void DestroyMeAnimated() {
		DestroyGameLevelItemSprite();
		Invoke("DestroyMe", .6f);
	}
	
	public void DestroyMe() {
		GameLevelItems.Current.level_items.RemoveAll(item => item.uuid == gameLevelItemAsset.uuid);		

        LogUtil.Log("GameDraggableLevelItem:destroying..." + name);

		//GameController.Instance.ProcessStatDestroy();
		
		Destroy (gameObject);
	}
	
	public void LoadData() {
		if(gameLevelItemAsset != null) {
			LoadSprite(gameLevelItemAsset.asset_code);
			//StartCoroutine(LoadDataCo());			
		}
	}
	
	IEnumerator LoadDataCo() {
		LoadSprite(gameLevelItemAsset.asset_code);
		yield break;		
	}


    public void Freeze() {
        if(!frozen) {
            frozen = true;
            gameObject.FreezeRigidBodies();
        }
    }

    public void UnFreeze() {
        if(frozen) {
            frozen = false;
            gameObject.UnFreezeRigidBodies();
        }
    }
 
    void Update() {

        //if(GameConfigs.isUIRunning) {
        //    return;
        //}

        if(!GameConfigs.isGameRunning) {
            if(!GameDraggableEditor.isEditing) {
                Freeze();
            }
            return;
        }
        else {
            UnFreeze();
        }

		if(dragHolder != null) {
			if(GameDraggableEditor.isEditing) {
				
                if(!visible) {
                    visible = true;
                    dragHolder.Show();
                }
			}
			else if(!GameDraggableEditor.isEditing) {
                
                if(visible) {
                    visible = false;	
				    dragHolder.Hide();
                }
			}
			
		}
	}
	
}
