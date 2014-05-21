using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Events;


public class BaseGamePlayerItem : GameObjectBehavior, IGamePlayerItem {
        
    public float currentTimeBlock = 0.0f;
    public float actionInterval = 3.0f;
    public float downCount = 5f;
    public bool initialized = false;

    public GameObject pointStaticObject;
    public GameObject pointAnimatedObject;
    public GameObject containerPre;
    public GameObject containerRun;
    public GameObject containerPost;
    public GameObject particleSystemPreObject;
    public GameObject particleSystemPostObject;
    public GameObject particleSystemRunObject;

    public float bobSpeed = 10.0f;  //Bob speed
    public float bobHeight = 30.0f; //Bob height
    public float bobOffset = 5f;
    public float PrimaryRot = 25.0f;  //First axies degrees per second
    public float SecondaryRot = 0.0f; //Second axies degrees per second
    public float TertiaryRot = 0.0f;  //Third axies degrees per second
        
    public string uuid = "";
    public string title = "";
    public string description = "";
    //public string gamePlayerItemCode = "item-coin";

    public double pointValue = 1.0;

    public Vector3 positionEnd = Vector3.zero;
    public bool floaty = false;
    public float bottom;
    public Transform cameraTransform;
    public bool allowCollect = false;
    public bool isCollecting = false;
    public float collectRange = 5f;
               
    public virtual void Awake() {
        bobSpeed = Mathf.Clamp(bobSpeed, 0, 100);
        bobHeight = Mathf.Clamp(bobHeight, 0, 100);             
        bottom = transform.position.y;
    }
        
    public virtual void Start() {
        Reset();
    }

    public virtual void Reset() {
        uuid = "";
        title = "";
        description = "";
        pointValue = 1.0;
        //type = GamePlayerItemType.Generic;
        positionEnd = Vector3.zero;
        //floaty = true;
        collectRange = 3f;

        if(cameraTransform == null) {
            if(Camera.main != null) {
                cameraTransform = Camera.main.transform;
            }
        }
        
        allowCollect = false;
        isCollecting = false;
                
        bobSpeed = Mathf.Clamp(bobSpeed, 0, 100);
        bobHeight = Mathf.Clamp(bobHeight, 0, 100);             
        bottom = transform.position.y;
        
        ResetContent();
        RevealCollect(UnityEngine.Random.Range(0f, .01f));
    }
        
    public virtual void RevealCollect(float delay) {
        StartCoroutine(RevealCollectCo(delay));
    }
        
    public virtual IEnumerator RevealCollectCo(float delay) {
        yield return new WaitForSeconds(delay);
        PlayContent();
        allowCollect = true;
    }
        
    public virtual void PlayContent() {
        HideAll(0f);
                
        FadeInObject(containerRun, 3f);
        //FadeInObject(particleSystemRunObject, 2f);
        PlayParticleSystem(particleSystemRunObject);
    }
        
    public virtual void StopContent() {
        HideAll(0f); 
                
        FadeInObject(containerPost, .1f);
        //FadeInObject(particleSystemPostObject, 2f);
        PlayParticleSystem(particleSystemPostObject);
    }
                
    public virtual void CollectContent() {
                
        if(!isCollecting) {
            LogUtil.Log("CollectContent:Collect", true);
                        
            isCollecting = true;         

            string gamePlayerItemCode = transform.name.Replace(" (Clone)", "");
            gamePlayerItemCode = gamePlayerItemCode.Replace("(Clone)", "");

            Debug.Log("CollectContent:" + " gamePlayerItemCode:" + gamePlayerItemCode);

            GameItem gameItem = GameItems.Instance.GetById(gamePlayerItemCode);

            if(gameItem == null) {
                return;
            }

            GameController.CurrentGamePlayerController.HandleItemUse(gameItem);                  
                                
            //UINotificationDisplay.Instance.QueuePoint(title, description, pointValue);
            //}
        }

        RemoveContent();
    }
                
    public virtual void ResetContent() {            
        HideAllNow();
                
        FadeInObject(containerPre, 2f);
        //FadeInObject(particleSystemPreObject, 2f);            
        PlayParticleSystem(particleSystemPreObject);
    }
        
    public virtual void PlayParticleSystem(GameObject go) {
        StartCoroutine(PlayParticleSystemCo(go));
    }
        
    public virtual IEnumerator PlayParticleSystemCo(GameObject go) {
        yield return new WaitForSeconds(.5f);
        go.PlayParticleSystem(true);
    }
        
    public virtual void StopParticleSystem(GameObject go) {
        StartCoroutine(StopParticleSystemCo(go));
    }
        
    public virtual IEnumerator StopParticleSystemCo(GameObject go) {
        yield return new WaitForSeconds(.5f);
        go.StopParticleSystem(true);
    }
        
    public virtual void RemoveContent() {
        StopContent();
        GameObjectHelper.DestroyGameObject(
            gameObject, GameConfigs.usePooledItems);
    }
        
    public virtual void HideAll(float delay) {
        StartCoroutine(HideAllCo(delay));
    }
        
    public virtual IEnumerator HideAllCo(float delay) {
                
        yield return new WaitForSeconds(delay);
                
        FadeOutObject(containerPre);            
        FadeOutObject(containerRun);            
        FadeOutObject(containerPost);
                
        particleSystemPreObject.StopParticleSystem(true);
        particleSystemPostObject.StopParticleSystem(true);
        particleSystemRunObject.StopParticleSystem(true);
    }
        
    public virtual void HideAllNow() {
        FadeOutObjectNow(containerPre);
        FadeOutObjectNow(containerRun);
        FadeOutObjectNow(containerPost);
                
        particleSystemPreObject.StopParticleSystem(true);
        particleSystemPostObject.StopParticleSystem(true);
        particleSystemRunObject.StopParticleSystem(true);
    }
        
    public virtual void FadeInObject(GameObject go, float delay) {
        StartCoroutine(FadeInObjectCo(go, delay));
    }
        
    public virtual IEnumerator FadeInObjectCo(GameObject go, float delay) {
                
        yield return new WaitForSeconds(delay);
                
        if(go != null) {        
            if(!go.activeInHierarchy || !go.activeSelf) {
                ShowObject(go);
            }
            LogUtil.Log("FadeInObject:" + go.name);
            UITweenerUtil.FadeIn(go);  
            //iTween.FadeTo(go, 1f, 2f);
        }
    }
        
    public virtual void FadeOutObject(GameObject go) {
                
        if(go != null) {                        
            LogUtil.Log("FadeOutObject:" + go.name);
            
            UITweenerUtil.FadeOut(go);        
           // iTween.FadeTo(go, 0f, 1f);//(go, iTween.Hash("alpha", 0f, "delay", 0f, "time", 1f));
            HideObjectDelayed(go, 1f);
        }
    }
        
    public virtual void FadeOutObjectNow(GameObject go) {
        if(go != null) {                        
            LogUtil.Log("FadeOutObjectNow:" + go.name);
            UITweenerUtil.FadeOutNow(go);        
            HideObject(go);
        }
    }
        
    public virtual void ShowObject(GameObject go) {
        if(go != null) {
            go.Show();
        }
    }
        
    public virtual void HideObjectDelayed(GameObject go, float delay) {
        StartCoroutine(HideObjectCo(go, delay));
    }

    public virtual IEnumerator HideObjectCo(GameObject go, float delay) {
        yield return new WaitForSeconds(delay);
        if(go != null) {
            go.Hide();
        }
    }
        
    public virtual void HideObject(GameObject go) {
        if(go != null) {
            go.Hide();
        }
    }
        
    // Update is called once per frame
    public virtual void FixedUpdate() {
        if(cameraTransform != null) {
            //transform.LookAt(cameraTransform);
        }
    }
        
    public virtual GamePlayerController GetController(Transform transform) {
        if(transform != null) {
            GamePlayerController gamePlayerController = transform.GetComponentInChildren<GamePlayerController>();
            if(gamePlayerController != null) {
                return gamePlayerController;
            }
        }
        return null;
    }
                
    public virtual void UpdateCollect() {

        GameObject go = GameController.CurrentGamePlayerController.gameObject;

        if(go != null) {
    
            if(Vector3.Distance(
                go.transform.position,
                transform.position)
                    <= collectRange) {
                    //foreach(Collider collide in Physics.OverlapSphere(transform.position, collectRange)) {

                    GamePlayerController gamePlayerController = GameController.GetGamePlayerControllerObject(go, true);
    
                    if(gamePlayerController != null && !gamePlayerController.controllerData.dying) {

                    if(gamePlayerController.IsPlayerControlled) {
                        CollectContent();
                    }
                }
            }
        }
    }
                
    public virtual void UpdateBounds() {
        if(GameController.ShouldUpdateBounds() && !isCollecting) {
            if(!GameController.CheckBounds(transform.position)) {
                RemoveContent();
            }
        }
    }
        
    bool handleClick = false;
        
    public virtual void Update() {        
        
        if(GameConfigs.isUIRunning) {
            return;
        }
        
        if(!GameConfigs.isGameRunning) {
            return;
        }
                
        if(Application.isEditor && Input.GetKeyDown(KeyCode.Space)) {
            //ResetContent();
            StopContent();
        }
                
        /*
        if(true == false) { // floaty) {
            // HANDLE floaty
                        
            transform.Rotate(new Vector3(0, PrimaryRot, 0) * Time.deltaTime, Space.World);
            transform.Rotate(new Vector3(SecondaryRot, 0, 0) * Time.deltaTime, Space.Self);
            transform.Rotate(new Vector3(0, 0, TertiaryRot) * Time.deltaTime, Space.Self);
                        
            float bobY = bottom + (((Mathf.Cos((Time.time + bobOffset) * bobSpeed) + 1) / 2) * bobHeight);
                        
            //if(type == GamePlayerCollectableType.Point) {
            //transform.Translate((positionEnd.WithY(0)) * Time.deltaTime, Space.Self);
            //LogUtil.Log("ARCollectable:positionEnd:", positionEnd);
            transform.position = transform.position.WithY(bobY);
            //}
            //else {                
            //transform.position = transform.position.WithY(bobY);
            //}
        }
        */
                
        UpdateBounds();
        UpdateCollect();
                
        if(downCount > 0f) {
            downCount -= Time.deltaTime;
        }
        else {
            downCount = 0;
        }
                
        handleClick = false;

        if(downCount <= 0
            && (Input.GetMouseButtonDown(0) || Input.touchCount > 0)) {
            ////&& !AppViewerUIController.Instance.uiVisible) {
            handleClick = true;
        }               

        if(handleClick) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 50000000)) {      
                                
                if(hit.collider != null) {
                    //Transform hitTransform = hit.collider.transform;

                    //string linkName = hitTransform.name.Replace(
                    //      GamePlayerItemMessages.gamePlayerItemCoin + "_","");
                    //
                    //LogUtil.Log("HIT!item:" + hitTransform.name);                                                                   
                                        
                    //if(hitTransform.name.ToLower().Contains(
                     //   GamePlayerItemType.itemCoin)) {
                                                
                    //    downCount = 5;

                        //&& state == ARCustomSceneObjectVidariLaunchState.Started
                        //&& playState == ARCustomSceneObjectVidariPlayState.Completed) {                       
                                                
                        //LogUtil.Log("linkName:" + linkName);                                            
                                                
                    //    CollectContent();       
                    //}
                }
            }
        }
    }
                
    //public virtual void OnTriggerEnter(Collider collider) {
    //
    //}
        
    //public virtual void OnTriggerStay(Collider collider) {
    //
    //}
        
    //public virtual void OnTriggerExit(Collider collider) {
    //
    //}
        
    public virtual void OnCollisionEnter(Collision collision) {
        //// foreach (ContactPoint contact in collision.contacts) {
        //Debug.DrawRay(contact.point, contact.normal, Color.white);
        //LogUtil.Log("GamePlayerItem:OnCollisionEnter:", contact.otherCollider.transform.name);

        if(!GameConfigs.isGameRunning) {
            return;
        }

        GameObject go = collision.collider.transform.gameObject;

        if(go == null) {
            return;
        }

        if(GameController.HasGamePlayerControllerObject(go, true)) {
            GamePlayerController gamePlayerController = GameController.GetGamePlayerControllerObject(go, true);
            if(gamePlayerController != null) {
                if(gamePlayerController.controllerState == GamePlayerControllerState.ControllerPlayer) {
                    // If player collect this
                    CollectContent();
                }
            }
        }


        //// }
        //if (collision.relativeVelocity.magnitude > 2)
        //    audio.Play();
    }
        
    //public virtual void OnCollisionStay(Collision collision) {
    //
    //}
                
    //public virtual void OnCollisionExit(Collision collision) {
    //
    //}
        
}


/*
public class GamePlayerCollectable {

        public GameObject containerCollectable;
        public GameObject containerEffects;
        public GameObject containerAudio;

        void Start() {

        }
        
        void FindPlayerCollisionParent() {
                if(gamePlayerControllerObject == null) {
                        gamePlayerControllerObject = gameObject.FindTypeAboveObject<GamePlayerController>();
                }                       
                
                if(gamePlayerController == null 
                && gamePlayerControllerObject != null) {
                        gamePlayerController = gamePlayerControllerObject.GetComponent<GamePlayerController>();
                        
                }
        }
        
        void OnCollisionEnter(Collision collision) {
        if(gamePlayerController != null) {
               /// foreach (ContactPoint contact in collision.contacts) {
                                gamePlayerController.HandleCollision(collision);
                                LogUtil.Log("contact:" + contact);
                        ////}
        }
    }
}

*/