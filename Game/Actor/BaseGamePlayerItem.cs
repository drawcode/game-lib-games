using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Events;
using Engine.Utility;

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
    public float collectRange = 8f;

    // How far above or below the player an item may sit and still be collectable.
    // The collect test is a CYLINDER, not a sphere -- see UpdateCollect.
    public float collectHeightRange = 6f;

    // Slack between the player's capsule surface and the item's collider surface at
    // which the item is taken. Both bodies are solid, so a centre-to-centre range
    // smaller than the sum of their radii can never be satisfied by walking.
    public float collectPadding = 1f;
    //
    string gamePlayerItemCode = "";
    GameItem gameItem = null;

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
        //collectRange = 8f;

        if (cameraTransform == null) {
            if (Camera.main != null) {
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

        InitItem();
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

    public virtual void InitItem() {

        if (string.IsNullOrEmpty(gamePlayerItemCode)) {
            gamePlayerItemCode = transform.name.Replace(" (Clone)", "");
            gamePlayerItemCode = gamePlayerItemCode.Replace("(Clone)", "");
        }

        //Debug.Log("InitItem:" + " gamePlayerItemCode:" + gamePlayerItemCode);

        if (gameItem == null) {
            gameItem = GameItems.Instance.GetById(gamePlayerItemCode);
        }
    }

    public virtual GameItem GetGameItem() {
        return gameItem;
    }

    public virtual void CollectContent() {

        // allowCollect is the arming gate RevealCollectCo sets once the item has actually
        // revealed itself. It was written by Reset/RevealCollectCo and read by NOBODY, so an
        // item could be taken in the frame it spawned, before its reveal ran. Both collect
        // paths -- the UpdateCollect cylinder test and OnCollisionEnter -- land here.

        if (!allowCollect) {
            return;
        }

        if (!isCollecting) {

            //Debug.Log("CollectContent:Collect" + true);

            isCollecting = true;

            if (gameItem == null) {
                return;
            }

            // add state for item

            GamePlayerProgress.SetStatItems(gameItem.code, 1);

            GameController.CurrentGamePlayerController.HandleItemUse(gameItem);

            //Debug.Log("CollectContent:" + gameItem.ToJson());

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

        if (go != null) {
            if (!go.activeInHierarchy || !go.activeSelf) {
                go.Show();
            }
            TweenUtil.FadeToObject(go, 1f);
        }
    }

    public virtual void FadeOutObject(GameObject go) {

        if (go != null) {
            TweenUtil.FadeToObject(go, 0f);
            go.HideObjectDelayed(1f);
        }
    }

    public virtual void FadeOutObjectNow(GameObject go) {
        if (go != null) {
            TweenUtil.FadeToObject(go, 0f, 0f, 0f);
            go.Hide();
        }
    }

    // Update is called once per frame
    public virtual void FixedUpdate() {
        if (cameraTransform != null) {
            //transform.LookAt(cameraTransform);
        }
    }

    /// <summary>
    /// Horizontal half-extent of this item's own collider, in world units, so an
    /// oversized pickup collider does not push the player out of its own collect range.
    /// </summary>
    public virtual float GetCollectItemRadius() {

        Collider itemCollider = collider;

        if (itemCollider == null) {
            itemCollider = GetComponentInChildren<Collider>();
        }

        if (itemCollider == null) {
            return 0f;
        }

        Vector3 extents = itemCollider.bounds.extents;

        return Mathf.Max(extents.x, extents.z);
    }

    /// <summary>
    /// How far apart the player's and the item's CENTRES may be horizontally.
    ///
    /// Both bodies are solid: the player capsule is radius 1.88 and the coin/health
    /// spheres are radius 2 and 3, so physics stops the player 3.88 and 4.88 units from
    /// the item's centre respectively. An authored collectRange of 3 was therefore
    /// unreachable on foot -- which is why these had to be jumped on to be picked up.
    /// Never let the usable range fall below "surfaces touching, plus a little".
    /// </summary>
    public virtual float GetCollectReach(GamePlayerController playerController) {

        float playerRadius = 0f;

        if (playerController != null) {

            // Strictly on the actor root -- that is where the controller sets it up, and
            // Get<T> would otherwise descend into children and find somebody else's.
            CharacterController characterController
                = playerController.gameObject.GetComponent<CharacterController>();

            playerRadius = characterController != null
                ? characterController.radius
                : playerController.characterRadius;
        }

        return Mathf.Max(collectRange, playerRadius + GetCollectItemRadius() + collectPadding);
    }

    public virtual void UpdateCollect() {

        if (isCollecting) {
            return;
        }

        GamePlayerController currentPlayerController = GameController.CurrentGamePlayerController;

        if (currentPlayerController == null) {
            return;
        }

        GameObject go = currentPlayerController.gameObject;

        if (go != null) {

            Vector3 playerPosition = go.transform.position;
            Vector3 itemPosition = transform.position;

            // Cylinder, not sphere. The old test was a 3D distance, so the item's height
            // above the player was charged against the same budget as the horizontal
            // gap. An item resting on the ground sits a full collider-radius up (2 for
            // the coin, 3 for health) while the player's origin is at their feet, so for
            // health the vertical offset alone consumed the entire authored range of 3
            // before any horizontal distance was counted. Jumping was the only way to
            // shrink that vertical term -- exactly the reported symptom.

            float horizontalX = playerPosition.x - itemPosition.x;
            float horizontalZ = playerPosition.z - itemPosition.z;
            float horizontalDistanceSqr = (horizontalX * horizontalX) + (horizontalZ * horizontalZ);

            float reach = GetCollectReach(currentPlayerController);

            if (horizontalDistanceSqr > reach * reach) {
                return;
            }

            if (Mathf.Abs(playerPosition.y - itemPosition.y) > collectHeightRange) {
                return;
            }

            GamePlayerController gamePlayerController = GameController.GetGamePlayerControllerObject(go, true);

            if (gamePlayerController != null && !gamePlayerController.controllerData.dying) {

                if (gamePlayerController.IsPlayerControlled) {
                    CollectContent();
                }
            }
        }
    }

    public virtual void UpdateBounds() {
        if (GameController.ShouldUpdateBounds() && !isCollecting) {
            if (!GameController.CheckBounds(transform.position)) {
                RemoveContent();
            }
        }
    }

    //bool handleClick = false;

    public virtual void Update() {

        if (GameConfigs.isUIRunning) {
            return;
        }

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (Application.isEditor && Input.GetKeyDown(KeyCode.Space)) {
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

        if (downCount > 0f) {
            downCount -= Time.deltaTime;
        }
        else {
            downCount = 0;
        }

        // TODO tap to collect

        /*

        handleClick = false;

        if (downCount <= 0
            && (Input.GetMouseButtonDown(0) || Input.touchCount > 0)) {
            ////&& !AppViewerUIController.Instance.uiVisible) {
            handleClick = true;
        }               

        if (handleClick) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50000000)) {      
                                
                if (hit.collider != null) {
                    //Transform hitTransform = hit.collider.transform;

                    //string linkName = hitTransform.name.Replace(
                    //      GamePlayerItemMessages.gamePlayerItemCoin + "_","");
                    //
                    //LogUtil.Log("HIT!item:" + hitTransform.name);                                                                   
                                        
                    //if(hitTransform.name.ToLower().Contains(
                    //   GamePlayerItemType.itemCoin)) {
                                                
                    //    downCount = 5;

                    //&& state == ARCustomSceneObjectLaunchState.Started
                    //&& playState == ARCustomSceneObjectPlayState.Completed) {                       
                                                
                    //LogUtil.Log("linkName:" + linkName);                                            
                                                
                    //    CollectContent();       
                    //}
                }
            }
        }
        */
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

        //return;

        if (!GameConfigs.isGameRunning) {
            return;
        }

        foreach (ContactPoint contact in collision.contacts) {

            GameObject go = contact.otherCollider.transform.gameObject;

            if (go == null) {
                return;
            }

            if (GameController.HasGamePlayerControllerObject(go, true)) {
                GamePlayerController gamePlayerController = GameController.GetGamePlayerControllerObject(go, true);
                if (gamePlayerController != null) {
                    if (gamePlayerController.controllerState == GamePlayerControllerState.ControllerPlayer) {
                        // If player collect this
                        CollectContent();
                    }
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