
// ------------------------------------------------------------------------
// EVENTS


public virtual void OnInputSwipe(InputSystemSwipeDirection direction, Vector3 pos, float velocity) {

    if (!GameConfigs.isGameRunning) {
        return;
    }

    if (controllerData == null) {
        return;
    }

    if (GameController.IsGameplayType(GameplayType.gameRunner)) {

        if (direction == InputSystemSwipeDirection.Up) {
            InputJump();
        }
        else if (direction == InputSystemSwipeDirection.Down) {
            //GameController.GamePlayerSlide(Vector3.zero.WithZ(3f));
            //GameController.GamePlayerAttack();
            InputSlide(Vector3.zero.WithZ(3f));
        }
        else if (direction == InputSystemSwipeDirection.Left
            || direction == InputSystemSwipeDirection.LowerLeftDiagonal
            || direction == InputSystemSwipeDirection.UpperLeftDiagonal) {
            //GameController.GamePlayerMove(Vector3.zero.WithX(-16f), rangeStart, rangeEnd);
            InputMove(GameController.Instance.containerInfinity.SwitchLineLeft(), rangeStart, rangeEnd);
        }
        else if (direction == InputSystemSwipeDirection.Right
            || direction == InputSystemSwipeDirection.LowerRightDiagonal
            || direction == InputSystemSwipeDirection.UpperRightDiagonal) {
            //GameController.GamePlayerMove(Vector3.zero.WithX(16f), rangeStart, rangeEnd);
              InputMove(GameController.Instance.containerInfinity.SwitchLineRight(), rangeStart, rangeEnd);
        }
    }
}

Vector3 rangeStart = Vector3.zero.WithX(-16f);
Vector3 rangeEnd = Vector3.zero.WithX(16f);
float infiniteSpeed = 200f;

internal virtual void handleGameInput() {

if (!IsPlayerControlled) {
    return;
}

if (GameController.IsGameplayType(GameplayType.gameRunner)) {

    if (InputSystem.isUpPressed) {
        InputJump();
    }
    else if (InputSystem.isDownPressed) {
        //GameController.GamePlayerSlide(Vector3.zero.WithZ(3f));
        //GameController.GamePlayerAttack();
        InputSlide(Vector3.zero.WithZ(3f));
    }
    else if (InputSystem.isLeftPressed) {
        //GameController.GamePlayerMove(Vector3.zero.WithX(-16f), rangeStart, rangeEnd);
        InputMove(GameController.Instance.containerInfinity.SwitchLineLeft(), rangeStart, rangeEnd);
    }
    else if (InputSystem.isRightPressed) {
        //GameController.GamePlayerMove(Vector3.zero.WithX(16f), rangeStart, rangeEnd);  
        InputMove(GameController.Instance.containerInfinity.SwitchLineRight(), rangeStart, rangeEnd);
    }
    else {

        //infiniteSpeed += .3f * Time.deltaTime;

        infiniteSpeed = Mathf.Clamp(infiniteSpeed, 0, 500);

        SetSpeed(infiniteSpeed);
        //GameController.SendInputAxisMoveMessage(0, 1);
    }
}
else if (GameController.IsGameplayType(GameplayType.gameDasher)) {
    //DetectSwipe();
    InputSystem.UpdateTouchLaunch();
}
}
















    // ------------------------------------------------------------------------



    
    public virtual void OnInputAxis(string name, Vector3 axisInput) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(controllerData == null) {
            return;
        }

        //float distance = Math.Abs(currentControllerData.effectWarpEnd - currentControllerData.effectWarpCurrent);

        //bool effectWarpOn = distance < 5 ? false : true;
        
        // main
        //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

        if(name == InputSystemEvents.inputAxisMove) {

            // INITIAL D-PAD

            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

            //if(isEntering || isExiting || effectWarpOn) { // || currentControllerData.effectWarpEnabled) {
            //    return;
            //}

            if(currentControllerData.thirdPersonController != null) {

                //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

                if(!axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    axisInput.x = 0f;
                    axisInput.y = 0f;
                }

                //if(axisInput.x != 0 || axisInput.y != 0) {
                Debug.Log("axisInput x:" + axisInput.x + " y:" + axisInput.y);
                //}

                //if(!GameController.isFingerNavigating) {
                HandleThirdPersonControllerAxis(axisInput);
            }
        }
        else if(name == InputSystemEvents.inputAxisAttack) {

            // SECONDARY D-PAD for DUAL STICK

            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

            if(currentControllerData.thirdPersonController != null) {

                //LogUtil.Log("OnInputAxis ATTACK:" + name + "input:" + axisInput);

                if(!axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    axisInput.x = 0f;
                    axisInput.y = 0f;
                }

                currentControllerData.thirdPersonController.horizontalInput2 = axisInput.x;
                currentControllerData.thirdPersonController.verticalInput2 = axisInput.y;

            }
        }
        else if(name == InputSystemEvents.inputAxisMoveHorizontal) {

            // INITIAL D-PAD ONLY FOR HORIZONTAL MOVEMENT

            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

            if(currentControllerData.thirdPersonController != null) {

                //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {

                    currentControllerData.thirdPersonController.horizontalInput = axisInput.x;
                    currentControllerData.thirdPersonController.verticalInput = 0f;//currentControllerData.thirdPersonController.verticalInput;
                }

                if(axisInput.y > .7f) {
                    //LogUtil.Log("axisInput.y:" + axisInput.y);
                    Jump();
                }
                else {
                    JumpStop();
                }

            }
        }
        else if(name == InputSystemEvents.inputAxisMoveVertical) {

            // INITIAL D-PAD ONLY FOR VERTICAL MOVEMENT

            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

            if(currentControllerData.thirdPersonController != null) {

                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
                    currentControllerData.thirdPersonController.horizontalInput = 0f;//axisInput.x;
                    currentControllerData.thirdPersonController.verticalInput = axisInput.y;
                }
            }
        }
        else if(name == InputSystemEvents.inputAxisAttack2DSide2) {

            // INITIAL D-PAD ONLY FOR SIDE SCROLLER ATTACK 2 

            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

            if(currentControllerData.thirdPersonController != null) {

                //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

                //currentControllerData.thirdPersonController.horizontalInput = axisInput.x;
                //currentControllerData.thirdPersonController.verticalInput = 0f;

                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {

                    if(controllerState == GamePlayerControllerState.ControllerPlayer) {
                        gamePlayerModelHolderModel
                                .transform.LookAt(-Vector3.zero.WithX(axisInput.x).WithY(axisInput.y));
                    }
                }
                else {
                    //GameController.CurrentGamePlayerController.gamePlayerModel.transform.rotation
                    //       = Quaternion.Euler(Vector3.zero);
                }


            }
        }
        else if(name == InputSystemEvents.inputAxisAttack2DSide) {

            // INITIAL D-PAD ONLY FOR SIDE SCROLLER ATTACK 1

            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);

            if(currentControllerData.thirdPersonController != null) {

                if(axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    //LogUtil.Log("OnInputAxis ATTACK:" + name + "input:" + axisInput);

                    currentControllerData.thirdPersonController.horizontalInput2 = -axisInput.x;
                    currentControllerData.thirdPersonController.verticalInput2 = 0f;//axisInput.y;

                    //UpdateAim(axisInput.x, axisInput.y);
                }
            }
        }
    }

    
    // ------------------------------------------------------------------------
        
    / *
    public virtual void GamePlayerModelHolderEase(Vector3 posFrom, Vector3 posTo) {
        currentControllerData.easeModelHolderStart = posFrom;
        currentControllerData.easeModelHolderEnd = posTo;
        currentControllerData.easeModelHolderEnabled = true;
    }
    
    public virtual void HandleGamePlayerModelHolderEaseTick() {

        if (currentControllerData.easeModelHolderEnabled && currentControllerData.visible) {

            float fadeSpeed = .5f;

            if (currentControllerData.easeModelHolderCurrent.y < currentControllerData.easeModelHolderEnd.y && currentControllerData.easeModelHolderCurrent.y > 5) {
                currentControllerData.easeModelHolderCurrent.y += (Time.deltaTime * fadeSpeed);

                gamePlayerModelHolder.transform.localPosition = 
                    currentControllerData.easeModelHolderCurrent;
            }
            else if (currentControllerData.easeModelHolderCurrent.y > currentControllerData.easeModelHolderEnd.y && currentControllerData.easeModelHolderCurrent.y > 5) {
                currentControllerData.easeModelHolderCurrent.y -= (Time.deltaTime * fadeSpeed);

                gamePlayerModelHolder.transform.localPosition = 
                    currentControllerData.easeModelHolderCurrent;
            }
            else {
                currentControllerData.easeModelHolderCurrent = currentControllerData.easeModelHolderEnd;
                currentControllerData.easeModelHolderEnabled = false;
                gamePlayerModelHolder.transform.localPosition = 
                    currentControllerData.easeModelHolderCurrent;
            }
        }
    }
    * /

    
    // ------------------------------------------------------------------------


    //GamePlayerController gamePlayerControllerHit;

    //public void OnCollisionEnter(Collision collision) {
    //    if(!GameController.shouldRunGame) {
    //            return;
    //    }
    //
    //    HandleCollision(collision);
    //}

    //GameObject target = collision.collider.gameObject;
    //LogUtil.Log("hit object:" + target);

    //if(target != null) {
    //    gamePlayerControllerHit = target.GetComponent<GamePlayerController>();

    //   if(gamePlayerControllerHit != null) {

    //DeviceUtil.Vibrate();
    //       LogUtil.Log("hit another game player");
    //   }
    //}
    // }
    //private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];


    
    // ------------------------------------------------------------------------



        / *
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
            * /

        / *
            int safeLength = particleSystem.safeCollisionEventSize;
            if (collisionEvents.Length < safeLength)
                collisionEvents = new ParticleSystem.CollisionEvent[safeLength];
            
            int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
            int i = 0;
            while (i < numCollisionEvents) {
                if (other.rigidbody) {
                    Vector3 pos = collisionEvents[i].intersection;
                    Vector3 force = collisionEvents[i].velocity * 10;
                    rigidbody.AddForce(force);
                }
                i++;
            }
            * /

    
    // ------------------------------------------------------------------------
    
    // ACTIONS

    public virtual void HandleZonesActionsController(
        GameActionTriggerState triggerState, GameObject go, string goName) {

        GameZoneAction actionItem =
            go.transform.GetComponentInParent<GameZoneAction>();

        if(actionItem == null) {
            return;
        }

        //Debug.Log("HandleZonesActionsController: actionCode:" + actionItem.actionCode);

        GameZoneActionAsset actionTypeItem =
            go.transform.GetComponentInParent<GameZoneActionAsset>();

        if(actionTypeItem == null) {
            return;
        }

        // CHECK action type

        string actionCode = actionItem.actionCode;

        // GET ACTION CODE

        if(IsPlayerControlled) {

            // ACTIONS
            // TRIGGER ACTION ENTER

            // TRIGGER ENTER

            if(triggerState == GameActionTriggerState.TriggerEnter) {

                // SAVE

                // BUILD

                if(actionTypeItem.isActionCodeBuild) {
                    actionTypeItem.ChangeStateCreating();
                }

                // REPAIR

                if(actionTypeItem.isActionCodeRepair) {
                    actionTypeItem.ChangeStateCreating();
                }

                // ATTACK

                // DEFEND
            }

            // TRIGGER EXIT

            if(triggerState == GameActionTriggerState.TriggerExit) {

                // SAVE

                // BUILD

                if(actionTypeItem.isActionCodeBuild) {
                    actionTypeItem.ChangeStateNone();
                }

                // REPAIR

                if(actionTypeItem.isActionCodeRepair) {
                    actionTypeItem.ChangeStateNone();
                }

                // ATTACK

            }

        }

        if(IsSidekickControlled) {

            // TRIGGER

            if(triggerState == GameActionTriggerState.TriggerEnter) {

                if(actionTypeItem.isActionCodeSave) {
                    SetNavAgentDestination(go);
                }
            }
            else if(triggerState == GameActionTriggerState.TriggerExit) {

                if(actionTypeItem.isActionCodeSave) {
                    SetNavAgentDestination(
                        GameController.CurrentGamePlayerController.gameObject);
                }
            }

            // ACTION TRIGGER

            if(triggerState == GameActionTriggerState.ActionTriggerEnter) {

                if(actionTypeItem.isActionCodeSave) {

                    AppContentCollect appContentCollect =
                        AppContentCollects.GetByTypeAndCode(BaseDataObjectKeys.action, actionCode);

                    if(appContentCollect != null) {

                        //Debug.Log(GameActionKeys.GameZoneActionArea + ":" +
                        //"appContentCollect:" +
                        //appContentCollect.code);

                        ExitPlayer();
                    }
                }
            }
        }

        if(IsAgentControlled) {

        }

    }


    
    // ------------------------------------------------------------------------

            / *
         if(Network.isClient || Network.isServer) {
     
             // call them over the network
         
             if(IsPlayerControlled) {
                 Gameverses.GameNetworkingAction actionEvent = new Gameverses.GameNetworkingAction();
                 actionEvent.uuid = UniqueUtil.CreateUUID4();
                 actionEvent.uuidOwner = uuid;
                 actionEvent.code = animationName;
                 actionEvent.type = Gameverses.GameNetworkingPlayerTypeMessages.PlayerTypeAction;             
             
                 //Gameverses.GameversesGameAPI.Instance.SendActionMessage(actionEvent, Vector3.zero, Vector3.zero);
             }
         }
         else  {
     * /
    
    // ------------------------------------------------------------------------

    
    public virtual void UpdateEditorTools() {

        if(IsPlayerControlled) {

            if(Application.isEditor) {

                if(Input.GetKey(KeyCode.LeftControl)) {

                    //LogUtil.Log("GamePlayer:moveDirection:" + GameController.CurrentGamePlayerController.currentControllerData.thirdPersonController.movementDirection);
                    //LogUtil.Log("GamePlayer:aimDirection:" + GameController.CurrentGamePlayerController.currentControllerData.thirdPersonController.aimingDirection);
                    //LogUtil.Log("GamePlayer:rotation:" + GameController.CurrentGamePlayerController.transform.rotation);
                    //Vector3 point1 = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
                    //Vector3 point2 = Camera.main.ScreenToWorldPoint(new Vector3(1, 0, 1));

                    //LogUtil.Log("GamePlayer:point1:" + point1);
                    //LogUtil.Log("GamePlayer:point2:" + point2);

                    float power = 100f;
                    if(Input.GetKey(KeyCode.V)) {
                        Boost(Vector3.zero.WithZ(1),
                            power);
                    }
                    else if(Input.GetKey(KeyCode.B)) {
                        Boost(Vector3.zero.WithZ(-1),
                            power);
                    }
                    else if(Input.GetKey(KeyCode.N)) {
                        Strafe(Vector3.zero.WithX(-1),
                            power);
                    }
                    else if(Input.GetKey(KeyCode.M)) {
                        Strafe(Vector3.zero.WithX(1),
                            power);
                    }

                    if(Input.GetKey(KeyCode.RightBracket)) {
                        if(!IsPlayerControlled) {
                            Die();
                        }
                    }
                    else if(Input.GetKey(KeyCode.V)) {
                        LoadWeapon("weapon-machine-gun-1");

                        UINotificationDisplay.QueueTip(
                            "Machine Gun Enabled",
                            "Machine gun simulation trigger and action installed and ready.");
                    }
                    else if(Input.GetKey(KeyCode.B)) {
                        LoadWeapon("weapon-flame-thrower-1");

                        UINotificationDisplay.QueueTip(
                            "Flame Thrower Enabled",
                            "Flame thrower simulation trigger and action installed and ready.");
                    }
                    else if(Input.GetKey(KeyCode.N)) {
                        LoadWeapon("weapon-shotgun-1");
                        UINotificationDisplay.QueueTip(
                            "Shotgun Enabled",
                            "Shotgun simulation trigger and action installed and ready.");
                    }
                    else if(Input.GetKey(KeyCode.M)) {
                        LoadWeapon("weapon-rocket-launcher-1");

                        UINotificationDisplay.QueueTip(
                            "Rocket Launcher Enabled",
                            "Rocket launcher trigger and action installed and ready.");
                    }
                    else if(Input.GetKey(KeyCode.C)) {
                        LoadWeapon("weapon-rifle-1");

                        UINotificationDisplay.QueueTip(
                            "Rifle Enabled",
                            "Rifle simulation trigger and action installed and ready.");
                    }
                }
            }
        }
    }
    
    // ------------------------------------------------------------------------



    
    
    // ------------------------------------------------------------------------


    
    
    // ------------------------------------------------------------------------


    
    
    // ------------------------------------------------------------------------


    
    
    // ------------------------------------------------------------------------

*/
