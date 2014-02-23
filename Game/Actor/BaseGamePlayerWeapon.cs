using System;
using System.Collections;
using System.Collections.Generic;

using Engine.Animation;
using Engine.Events;
using Engine.Utility;

using UnityEngine;

public class BaseGamePlayerWeapon : GameActor {

    public Vector3 direction = Vector3.left;
    
    public List<GameObject> currentItems = new List<GameObject>();
    
    public bool isLoaded = true;
    public bool isShooting = false;
    public bool isAuto = false;
    public bool useGameObjectProjectile = false;

    public GameObject containerProjectiles;
    public GameObject containerDiscard;
    public GameObject containerEffects;

    public ParticleSystem particleSystemAttackBlast1;
    public ParticleSystem particleSystemAttackBlast2;
    public ParticleSystem particleSystemAttackBlast3;
    
    public ParticleSystem particleSystemAttackProjectile1;
    public ParticleSystem particleSystemAttackProjectile2;
	
	public GamePlayerController gamePlayerController;

    public GameWeapon gameWeaponData;

	public virtual void Awake() {
		
	}
	
	public override void Start() {
		
	}
	
	public override void Init() {
		
		if(gamePlayerController == null) {
			gamePlayerController = gameObject.FindTypeAbove<GamePlayerController>();
		}
				
		Load();
		
		if(currentItems != null) {
			foreach(GameObject go in currentItems) {
				if(go != null) {
					if(go.transform != null) {
						Quaternion resetRotation = go.transform.rotation;
						resetRotation.x = 0;
						resetRotation.y = 0;
						resetRotation.z = 0;
						go.transform.rotation = resetRotation;
					}
				}
			}
		}
	}
	
	public virtual void Load() {
		currentItems = new List<GameObject>();
		GameObject bullet1 = Resources.Load("Prefabs/GameProjectile") as GameObject;
		currentItems.Add(bullet1);		
	}

    public virtual void LoadProjectiles() {

    }
	
	public virtual void PlayParticleSystem(ParticleSystem particles) {
		if(particles != null) {
			//if(!particles.isPlaying) {
				particles.Play(true);
				particles.enableEmission = true;
			//}
		}
	}
	
	public virtual void AttackEffects() {
        		
		PlayParticleSystem(particleSystemAttackBlast1);
		PlayParticleSystem(particleSystemAttackBlast2);
		PlayParticleSystem(particleSystemAttackBlast3);
				
		PlayParticleSystem(particleSystemAttackProjectile1);
		PlayParticleSystem(particleSystemAttackProjectile2);
	}
	
	public virtual void Attack() {

        PlayProjectiles();

		AttackEffects();
		
        PlayAttackSounds();

		// TODO play effect from weapon data


	}

    public virtual void PlayProjectiles() {
        
        if(gameWeaponData.data.HasProjectiles()) {

            foreach(GameDataItemProjectile projectile in gameWeaponData.data.projectiles) {

                GameObject projectilePrefab = AppContentAssets.LoadAsset(projectile.code);

                if(projectilePrefab != null) {

                    //GameObject projectileObject = 
                    GameObjectHelper.CreateGameObject(
                        projectilePrefab, 
                        containerProjectiles.transform.position, 
                        containerProjectiles.transform.rotation, 
                        true);

                    // add components

                }
            }
        }
    }

	public virtual void PlayAttackSounds() {

		GameAudio.PlayEffect(transform, gameWeaponData.data.GetSoundsByTypeShot().code);
		Invoke("PlayAttackPostSounds", .5f);
	}

	public virtual void PlayAttackPrepareSounds() {

	}


	public virtual void PlayAttackPostSounds() {
        GameAudio.PlayEffect(transform, gameWeaponData.data.GetSoundsByTypeLoad().code);
	}
	
	public virtual void AttackPrimary() {
		
		if(!useGameObjectProjectile) {
		
		}
		else {
			
			
			if(currentItems != null) {
				
				LogUtil.Log("GamePlayerWeapon::AttackPrimary:currentItems:" + currentItems);
				
				if(currentItems.Count > 0) {
				// TODO work in other shoot buttons			
				
					GameObject currentItem = currentItems[0];
					
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:currentItem:" + currentItem);
					
					Vector3 projectileStartPosition = transform.position;
					projectileStartPosition.x = projectileStartPosition.x + 2f;

                    GamePlayerProjectile projectile =
                        GameObjectHelper.CreateGameObject(
                            currentItem.gameObject,
                            projectileStartPosition,
                            Quaternion.identity,
                            GameConfigs.usePooledProjectiles
                            ).GetComponent<GamePlayerProjectile>();;

					GameObject gameObject = currentItem;
					if(projectile != null) {
						
						projectile.transform.parent = null;
						
						
					//LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform.localRotation.eulerAngles:" + gameObject.transform.localRotation.eulerAngles);
					//LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform.localRotation:" + gameObject.transform.localRotation);
						
					//LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform.rotation.eulerAngles:" + gameObject.transform.rotation.eulerAngles);
					//LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform.rotation:" + gameObject.transform.rotation);
						
						/*
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform:" + gameObject.transform);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform.name:" + gameObject.name);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform.position:" + gameObject.transform.position);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform.rotation:" + gameObject.transform.rotation);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform.localRotation:" + gameObject.transform.localRotation);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform.rotation.y:" + gameObject.transform.rotation.y);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform.localRotation.y:" + gameObject.transform.localRotation.y);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform.eulerAngles:" + gameObject.transform.eulerAngles);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.transform.localEulerAngles:" + gameObject.transform.localEulerAngles);
						
						
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.parent.transform:" + gameObject.transform.parent.gameObject.transform);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.parent.transform.name:" + gameObject.transform.parent.gameObject.name);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.parent.transform.position:" + gameObject.transform.parent.gameObject.transform.position);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.parent.transform.rotation:" + gameObject.transform.parent.gameObject.transform.rotation);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.parent.transform.localRotation:" + gameObject.transform.parent.gameObject.transform.localRotation);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.parent.transform.eulerAngles:" + gameObject.transform.parent.gameObject.transform.eulerAngles);
					LogUtil.Log("GamePlayerWeapon::AttackPrimary:gameObject.parent.transform.localEulerAngles:" + gameObject.transform.parent.gameObject.transform.localEulerAngles);
						*/
						
						projectile.gameObject.transform.position = gameObject.transform.position;
						projectile.speed = 1f;//currentAcceleration * .1f;
						
						if(InputSystem.Instance != null) {
							//LogUtil.Log("GamePlayerWeapon::AttackPrimary:gInputSystem.Instance.lastTargetDirection:" + InputSystem.Instance.lastTargetDirection);
							//projectile.direction = InputSystem.Instance.lastTargetDirection;
							projectile.transform.position = transform.parent.position;
							projectile.direction = transform.parent.position;
						}
						
						projectile.projectileObject = currentItem;
						projectile.Launch();
					}
				}
			}
		}
	}
	
	public override void Update() {

        if(!GameConfigs.isGameRunning) {
            return;
        }

		if(isShooting && isAuto) {
			AttackPrimary();
		}
	}
		
	public virtual void FixedUpdate () {
	
	}

	public virtual void OnTriggerEnter(Collider collider) {
	
	}
	
	public virtual void OnTriggerStay(Collider collider) {
	
	}
	
	public virtual void OnTriggerExit(Collider collider) {
	
	}

	public virtual void OnCollisionEnter(Collision collision) {
	
	}
	
	public virtual void OnCollisionStay(Collision collision) {
	
	}
		
	public virtual void OnCollisionExit(Collision collision) {
	
	}
	
}

