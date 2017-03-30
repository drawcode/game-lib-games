using System;
using System.Collections;

using UnityEngine;
using UnityEngine.AI;

using Engine;
using Engine.Data;
using Engine.Events;
using Engine.Networking;
using Engine.Utility;

public class BaseGamePlayerThirdPersonControllerData {
    
    public bool removing = false;
}

public class BaseGamePlayerThirdPersonController : GameObjectTimerBehavior {

    public GamePlayerThirdPersonControllerData controllerData;

    // The speed when walking
    public float walkSpeed = 3.0f;
    // after trotAfterSeconds of walking we trot with trotSpeed
    public float trotSpeed = 4.0f;
    // when pressing "Fire3" button (cmd) we start running
    public float runSpeed = 6.0f;
    public float inAirControlAcceleration = 3.0f;

    // How high do we jump when pressing jump and letting go immediately
    public float jumpHeight = 0.5f;
    // We add extraJumpHeight meters on top when holding the button down longer while jumping
    public float extraJumpHeight = 2.5f;

    // How high do we slide when pressing slide and letting go immediately
    public float slideLength = 0.5f;
    // We add extraSlideHeight meters on  when holding the button down longer while slidinging
    public float extraSlideHeight = 2.5f;

    // The gravity for the character
    public float gravity = 20.0f;
    // The gravity in cape fly mode
    public float capeFlyGravity = 2.0f;
    public float speedSmoothing = 10.0f;
    public float rotateSpeed = 500.0f;
    public float trotAfterSeconds = 3.0f;
    //
    public bool canJump = true;
    public bool canCapeFly = true;
    public bool canWallJump = false;
    public bool canSlide = true;
    public float jumpRepeatTime = 0.05f;
    public float wallJumpTimeout = 0.15f;
    public float jumpTimeout = 0.15f;

    public float slideRepeatTime = 0.05f;
    public float wallSlideTimeout = 0.15f;
    public float slideTimeout = 0.15f;
    //
    //
    public float groundedTimeout = 0.25f;

    // The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
    public float lockCameraTimer = 0.0f;

    // The current move direction in x-z
    public Vector3 moveDirection = Vector3.zero;
    // The current vertical speed
    public float verticalSpeed = 0.0f;
    // The current x-z move speed
    public float moveSpeed = 0.0f;

    // The last collision flags returned from controller.Move
    public CollisionFlags collisionFlags;

    // Are we jumping? (Initiated with jump button and not grounded yet)
    public bool jumping = false;
    public bool jumpingReachedApex = false;

    //
    public bool sliding = false;

    // Are we moving backwards (This locks the camera to not do a 180 degree spin)
    public bool movingBack = false;
    // Is the user pressing any keys?
    public bool isMoving = false;
    // When did the user start walking (Used for going into trot after a while)
    public float walkTimeStart = 0.0f;
    // Last time the jump button was clicked down
    public float lastJumpButtonTime = -10.0f;
    // Last time the slide button was clicked down
    public float lastSlideButtonTime = -10.0f;
    // Last time we performed a jump
    public float lastJumpTime = -1.0f;
    // Last time we performed a slide
    public float lastSlideTime = -1.0f;
    //public float lastShootTime = -1.0f;
    // Average normal of the last touched geometry
    public Vector3 wallJumpContactNormal;
    //public float wallJumpContactNormalHeight;

    // the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
    public float lastJumpStartHeight = 0.0f;
    // When did we touch the wall the first time during this jump (Used for wall jumping)
    public float touchWallJumpTime = -1.0f;
    public Vector3 inAirVelocity = Vector3.zero;
    public float lastGroundedTime = 0.0f;
    
    //
    public float lastSlideStartHeight = 0.0f;

    //public float lean = 0.0f;

    // The vertical/horizontal input axes and jump button from user input, synchronized over network
    public float verticalInput = 0.0f;
    public float horizontalInput = 0.0f;
    public float verticalInput2 = 0.0f;
    public float horizontalInput2 = 0.0f;
    //
    public bool jumpButton = false;
    public bool slideButton = false;
    public bool getUserInput = false;
    public bool isNetworked = false;
    //
    public Vector3 targetDirection = Vector3.zero;
    public Vector3 movementDirection = Vector3.zero;
    public Vector3 aimingDirection = Vector3.zero;
 
    //Gameverses.GameNetworkAniStates currentNetworkAniState = Gameverses.GameNetworkAniStates.walk;
    //Gameverses.GameNetworkAniStates lastNetworkAniState = Gameverses.GameNetworkAniStates.run;

    public UnityEngine.AI.NavMeshAgent navMeshAgent = null;
 
    public virtual void Awake() {
        moveDirection = transform.TransformDirection(Vector3.forward);
    }
 
    // Update is called once per frame
    public virtual void Start() {
        //base.Start();
    }

    public virtual void Init() {
        controllerData = new GamePlayerThirdPersonControllerData();
    }
 
    public virtual void UpdateSmoothedMovementDirection() {
     
        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(controllerData.removing) {
            return;
        }

        if(Camera.main == null || Camera.main.transform == null) {
            return;
        }
     
        Transform cameraTransform = Camera.main.transform;
        bool grounded = IsGrounded();

        // Forward vector relative to the camera along the x-z plane 
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;

        // Right vector relative to the camera
        // Always orthogonal to the forward vector
        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        if(getUserInput) {
            //verticalInput = Input.GetAxisRaw("Vertical");
            //horizontalInput = Input.GetAxisRaw("Horizontal");
        }
        else {
            if(isNetworked) {
                // TODO check user context before applying
            }
            else {
                // TODO check user context before applying
                //verticalInput = InputSystem.Instance.lastNormalizedTouch.y;
                //horizontalInput = InputSystem.Instance.lastNormalizedTouch.x;
             
                if(verticalInput > 0) {
                    //LogUtil.Log("verticalInput:" + verticalInput);
                }
                if(horizontalInput > 0) {
                    //LogUtil.Log("horizontalInput:" + horizontalInput);
                }
            }
        }

        // Are we moving backwards or looking backwards
        if(verticalInput < -0.2)
            movingBack = true;
        else
            movingBack = false;

        bool wasMoving = isMoving;
        isMoving = Mathf.Abs(horizontalInput) > 0.1 || Mathf.Abs(verticalInput) > 0.1;

        // Target direction relative to the camera
        targetDirection = horizontalInput * right + verticalInput * forward;
        movementDirection = targetDirection;
        aimingDirection = Vector3.zero;
     
        if(horizontalInput2 != 0 || verticalInput2 != 0) {
            aimingDirection = horizontalInput2 * right + verticalInput2 * forward;
        }
        else {
            aimingDirection = targetDirection;
        }
        
        //LogUtil.Log("targetDirection:" + targetDirection);

        // Grounded controls
        if(grounded) {
            // Lock camera for short period when transitioning moving & standing still
            lockCameraTimer += Time.deltaTime;
            if(isMoving != wasMoving)
                lockCameraTimer = 0.0f;

            // We store speed and direction seperately,
            // so that when the character stands still we still have a valid forward direction
            // moveDirection is always normalized, and we only update it if there is user input.
            if(targetDirection != Vector3.zero) {
                // If we are really slow, just snap to the target direction
                //if(moveSpeed < walkSpeed * 0.9 && grounded) {
                //   moveDirection = moveDirection.normalized;
                //}
                // Otherwise smoothly turn towards it
                //else {
                moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);

                moveDirection = moveDirection.normalized;

                //LogUtil.Log("moveDirection:" + moveDirection);
                //}
            }

            // Smooth the speed based on the current target direction
            var curSmooth = speedSmoothing * Time.deltaTime;

            // Choose target speed
            //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
            var targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

            // Pick speed modifier
            if(Input.GetButton("Fire3")) {
                targetSpeed *= runSpeed;
            }
            else if(Time.time - trotAfterSeconds > walkTimeStart) {
                targetSpeed *= trotSpeed;
            }
            else {
                targetSpeed *= walkSpeed;
            }

            moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);

            // Reset walk time start when we slow down
            if(moveSpeed < walkSpeed * 0.3)
                walkTimeStart = Time.time;
        }
     // In air controls
        else {
            // Lock camera while in air
            if(jumping)
                lockCameraTimer = 0.0f;

            if(isMoving)
                inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
        }
    }

    public virtual void ApplyWallJump() {
        // We must actually jump against a wall for this to work
        if(!jumping)
            return;

        // Store when we first touched a wall during this jump
        if(collisionFlags == CollisionFlags.CollidedSides && touchWallJumpTime < 0) {
            touchWallJumpTime = Time.time;
        }

        // The user can trigger a wall jump by hitting the button shortly before or shortly after hitting the wall the first time.
        var mayJump = lastJumpButtonTime > touchWallJumpTime - wallJumpTimeout && lastJumpButtonTime < touchWallJumpTime + wallJumpTimeout;
        if(!mayJump)
            return;

        // Prevent jumping too fast after each other
        if(lastJumpTime + jumpRepeatTime > Time.time)
            return;

        wallJumpContactNormal.y = 0;
        if(wallJumpContactNormal != Vector3.zero) {
            moveDirection = wallJumpContactNormal.normalized;
            // Wall jump gives us at least trotspeed
            moveSpeed = Mathf.Clamp(moveSpeed * 1.5f, trotSpeed, runSpeed);
        }
        else {
            moveSpeed = 0;
        }

        verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
        Jump();
        SendMessage("WallJump", null, SendMessageOptions.DontRequireReceiver);
    }

    public virtual void ApplyJumping() {
        // Prevent jumping too fast after each other
        if(lastJumpTime + jumpRepeatTime > Time.time)
            return;

        if(IsGrounded()) {
            // Jump
            // - Only when pressing the button down
            // - With a timeout so you can press the button slightly before landing      
            if(canJump && Time.time < lastJumpButtonTime + jumpTimeout) {
                verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
                Jump();
            }
        }
    }

    public virtual void ApplySliding() {
        // Prevent sliding too fast after each other
        if (lastSlideTime + slideRepeatTime > Time.time)
            return;

        if (IsGrounded()) {
            // Slide
            // - Only when pressing the button down
            // - With a timeout so you can press the button slightly before landing      
            if (canSlide && Time.time < lastSlideButtonTime + slideTimeout) {
                //verticalSpeed = CalculateJumpVerticalSpeed(slideHeight);
                Slide();
            }
        }
    }

    public virtual void ApplyAttack() {  
        bool doAttack = false;
     
        if(verticalInput2 != 0f || horizontalInput2 != 0f) {
            doAttack = true;
        }
     
        if(doAttack) {           
            //SendMessage("Attack", SendMessageOptions.DontRequireReceiver);
            SendMessage("Attack", SendMessageOptions.DontRequireReceiver);
        }
    }
 
    public virtual void ApplyDie(bool removeMe) {
     
        /*
     bool doAttack = false;
     
     if(getUserInput) {
         if(Input.GetKeyDown(KeyCode.Return)) {
             doAttack = true;
         }               
     }
     */
        controllerData.removing = removeMe;
        SendMessage("Die", SendMessageOptions.DontRequireReceiver);
    }

    public virtual void ApplyGravity() {
        // Apply gravity
        if(getUserInput)
            jumpButton = Input.GetButton("Jump");

        // * When falling down we use capeFlyGravity (only when holding down jump)
        var capeFly = canCapeFly && verticalSpeed <= 0.0 && jumpButton && jumping;

        // When we reach the apex of the jump we send out a message
        if(jumping && !jumpingReachedApex && verticalSpeed <= 0.0) {
            jumpingReachedApex = true;
            SendMessage("JumpReachApex", SendMessageOptions.DontRequireReceiver);
        }

        // * When jumping up we don't apply gravity for some time when the user is holding the jump button
        //   This gives more control over jump height by pressing the button longer
        var extraPowerJump = IsJumping() && verticalSpeed > 0.0 && jumpButton && transform.position.y < lastJumpStartHeight + extraJumpHeight;

        if(capeFly)
            verticalSpeed -= capeFlyGravity * Time.deltaTime;
        else if(extraPowerJump)
            return;
        else if(IsGrounded())
            verticalSpeed = -gravity * 0.2f;
        else
            verticalSpeed -= gravity * Time.deltaTime;
    }

    public virtual float CalculateJumpVerticalSpeed(float targetJumpHeight) {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * targetJumpHeight * gravity);
    }

    // JUMP

    public virtual void Jump() {
        Jump(.5f);
    }

    public virtual void Jump(float duration) {

        if(jumpButton) {
            return;
        }

        jumpButton = true;
        
        if(navMeshAgent == null) {
            navMeshAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        // off while we jump
        navMeshAgent.StopAgent();
        
        jumping = true;
        jumpingReachedApex = false;
        lastJumpTime = Time.time;
        lastJumpStartHeight = transform.position.y;
        touchWallJumpTime = -1;
        lastJumpButtonTime = -10;

        CancelInvoke("JumpStop");
        Invoke("JumpStop", duration);
    }
        
    public virtual void JumpStop() {
        jumpButton = false;
    }
    
    // SLIDE

    public virtual void Slide() {
        Slide(Vector3.zero.WithZ(.15f), .6f);
    }

    public virtual void Slide(Vector3 amount, float time = .5f) {

        if (slideButton) {
            return;
        }

        slideButton = true;

        if (navMeshAgent == null) {
            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        }

        // off while we slide
        navMeshAgent.StopAgent();

        sliding = true;        

        lastSlideTime = Time.time;
        lastSlideStartHeight = transform.position.y;
        lastSlideButtonTime = -10;

        CancelInvoke("SlideStop");
        Invoke("SlideStop", time);
    }

    public virtual void SlideStop() {
        slideButton = false;
    }

    // Update is called once per frame
    public virtual void Update() {
        
        if(!gameObjectTimer.IsTimerPerf(
            GameObjectTimerKeys.gameUpdateAll, 1f)) {
            //return;
        }

        if(!GameConfigs.isGameRunning) {
            return;
        }
     
        //base.Update();
             
        if(controllerData.removing) {
            return;
        }

        if(getUserInput) {
            if(Input.GetButtonDown("Jump"))
                lastJumpButtonTime = Time.time;
            if (Input.GetButtonDown("Slide"))
                lastSlideButtonTime = Time.time;
        }
        else {
            if(jumpButton)
                lastJumpButtonTime = Time.time;
            if (slideButton)
                lastSlideButtonTime = Time.time;
        }

        UpdateSmoothedMovementDirection();

        // Apply gravity
        // - extra power jump modifies gravity
        // - capeFly mode modifies gravity
        ApplyGravity();

        // Perform a wall jump logic
        // - Make sure we are jumping against wall etc.
        // - Then apply jump in the right direction)
        if(canWallJump)
            ApplyWallJump();

        // Apply jumping logic
        ApplyJumping();

        ApplySliding();

        ApplyAttack();

        // Calculate actual motion
        Vector3 movement = moveDirection * (moveSpeed * (1 - verticalInput2 / 10)) + new Vector3(0, verticalSpeed, 0) + inAirVelocity;
        movement *= Time.deltaTime;

        // Move the controller
        CharacterController controller = GetComponent<CharacterController>();
        wallJumpContactNormal = Vector3.zero;
     
        //if(!isNetworked) {
        collisionFlags = controller.Move(movement);
        //}
     
        // Set rotation to the move direction
        if(IsGrounded() && moveDirection != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
        else {
            var xzMove = movement;
            xzMove.y = 0;
            if(xzMove.magnitude > 0.001) {
                transform.rotation = Quaternion.LookRotation(xzMove);
            }
        }

        // We are in jump mode but just became grounded
        if(IsGrounded()) {
            lastGroundedTime = Time.time;
            inAirVelocity = Vector3.zero;

            // turn on agent now we are on the mesh
            navMeshAgent.StartAgent();

            if(jumping) {
                jumping = false;
                SendMessage("land", SendMessageOptions.DontRequireReceiver);
                JumpStop();
            }

            if (sliding) {
                sliding = false;
                SendMessage("land", SendMessageOptions.DontRequireReceiver);
                SlideStop();
            }
        }
     
    }

    public virtual void OnControllerColliderHit(ControllerColliderHit hit) {
        //   Debug.DrawRay(hit.point, hit.normal);
        if(hit.moveDirection.y > 0.01)
            return;
        wallJumpContactNormal = hit.normal;
    }

    public virtual float GetSpeed() {
        return moveSpeed;
    }

    public virtual void SetSpeed(float moveSpeedTo) {
        moveSpeed = moveSpeedTo;
    }

    public virtual bool IsJumping() {
        return jumping;
    }

    public virtual bool IsSliding() {
        return sliding;
    }

    public virtual bool IsGrounded() {
        return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
    }

    public virtual void SuperJump(float height) {
        verticalSpeed = CalculateJumpVerticalSpeed(height);
        collisionFlags = CollisionFlags.None;
        Jump();
    }

    public virtual void SuperJump(float height, Vector3 jumpVelocity) {
        verticalSpeed = CalculateJumpVerticalSpeed(height);
        inAirVelocity = jumpVelocity;

        collisionFlags = CollisionFlags.None;
        Jump();
    }

    public virtual Vector3 GetDirection() {
        return moveDirection;
    }

    public virtual bool IsMovingBackwards() {
        return movingBack;
    }

    public virtual float GetLockCameraTimer() {
        return lockCameraTimer;
    }

    public virtual float GetLean() {
        return 1.0f;
    }

    public virtual bool HasJumpReachedApex() {
        return jumpingReachedApex;
    }

    public virtual bool IsGroundedWithTimeout() {
        return lastGroundedTime + groundedTimeout > Time.time;
    }

    public virtual bool IsCapeFlying() {
        // * When falling down we use capeFlyGravity (only when holding down jump)
        if(getUserInput)
            jumpButton = Input.GetButton("Jump");
        return canCapeFly && verticalSpeed <= 0.0 && jumpButton && jumping;
    }

    public virtual void Reset() {
        gameObject.tag = "Player";
        ResetPlayState();
    }
 
    public virtual void ResetPlayState() {   
        enabled = true;
        controllerData.removing = false;
        jumping = false;
        sliding = false;
        //transform.position = Vector3.zero;
    }

    public virtual void MoveTo(Vector3 move, bool local = true) {

    }
}
// Require a character controller to be attached to the same game object
//[RequireComponent(CharacterController)]
//[AddComponentMenu("Third Person Player/Third Person Controller")]
