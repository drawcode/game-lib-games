using UnityEngine;
using System.Collections;

public class GrapplingHook : MonoBehaviour {
    
    public float speed = 25f;
    public float maxVelocity = 10f;
    public float swingForwardSpeed = 10f;
    public float swingStrafeSpeed = 5f;
    public float initialForce = 15f;
    float distToGround;
    Collider col;
    
    //This should be left as 0 for now.
    //Rope length is set by the inital raycast hit
    //This makes sure that player is always on the outer most part of his swing.
    public float ropeLength = 0f;
    
    //An empty GameObject should be assigned here.
    //Make sure to not child it to anything in the Hierarchy. 
    public Transform hookAnchor;
    
    //Line renderer for a simple representation of a rope.
    public LineRenderer line;
    bool canGrapple = false;
    Rigidbody rb;
    bool Swinging;
    float dist;
    Vector3 hitPoint;
    Vector3 newVel;
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Start() {
        distToGround = col.bounds.extents.y;
    }
    
    void Update() {
        
        RaycastHit hit;
        //RaycastHit lastSight;
        
        //Check to make sure we actually hit something that can be grappled.
        //The Raycast distance can be change to another value to set a certain distance a player can grapple to.
        if (Input.GetButtonDown("Fire1")) {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 5000)) {
                if (hit.collider) {
                    hitPoint = hit.point;
                    canGrapple = true;
                    hookAnchor.position = hitPoint;
                    dist = Vector3.Distance(transform.position, hookAnchor.position);
                    ropeLength = dist;
                    canGrapple = true;
                    Swinging = true;
                }
            }
        }
        
        //If the mouse button is still held after hitting something, continue to swing.
        if (Input.GetButton("Fire1")) {
            if (canGrapple) {
                line.enabled = true;
                Vector3 v = transform.position - hookAnchor.position;
                float distance = v.magnitude;
                newVel = v;
                //Vector3 myUp = (transform.position - hookAnchor.position).normalized;
                //Draws an imaginary sphere around the player, simulating the rope length.
                //If the player is past the maximum distance of the rope, we move it back in
                //This allows the player to move toward the anchor point, but keeps him from going further out.
                if (distance > ropeLength) {
                    newVel.Normalize();
                    v = Vector3.ClampMagnitude(v, ropeLength);
                    transform.position = hookAnchor.position + v;
                    float x = Vector3.Dot(newVel, rb.linearVelocity);
                    newVel *= x;
                    rb.linearVelocity -= newVel;
                }
                //Makes the rope longer
                if (Input.GetKey(KeyCode.Q)) {
                    ropeLength += .15f;
                }
                //Makes the rope shorter
                if (Input.GetKey(KeyCode.E)) {
                    ropeLength -= .15f;
                }
                //Checks the player's distance to the ground
                //If he swings to close to the ground, it shortens the rope by one meter
                //This stops the player from stopping when he hits the ground.
                if (Physics.Raycast(transform.position, Vector3.down, distToGround + 1)) {
                    ropeLength -= .1f;
                }
            }
        }
        //When the mouse is let go, the player stops moving
        //
        if (Input.GetButtonUp("Fire1")) {
            line.enabled = false;
            canGrapple = false;
            Swinging = false;
        }
    }
    
    void FixedUpdate() {
        
        //All the code below is just movement code for the player.
        //The change in drag allows the control of how fast the player stops, while still allowing the use AddForce
        //Movement Speed is Halved in the air for a more realistic feel.
        float x = Input.GetAxis("Horizontal") * 1;
        float z = Input.GetAxis("Vertical") * 1;
        
        if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0 || !IsGrounded()) {
            rb.linearDamping = 0;
        }
        else {
            rb.linearDamping = 5;
        }
        //Keeps the player's speed under a maximum
        if (rb.linearVelocity.magnitude < maxVelocity && IsGrounded()) {
            rb.AddForce(transform.forward * z * speed);
            rb.AddForce(transform.right * x * speed);
        }
        //This controls when the player can add more force to his swing.
        //If the player is above the grapple point, he can no longer add more force
        //On the way down however, he can add more, this stops the player from doing a full circle over the grappling anchor without any prior momentum.
        else if (!IsGrounded() && Swinging && transform.position.y <= hookAnchor.position.y) {
            if (z > 0 && rb.linearVelocity.y < 0f) {
                rb.AddForce(-transform.up * z * swingForwardSpeed);
            }
            rb.AddForce(transform.right * x * swingStrafeSpeed);
        }
        else if (!IsGrounded()) {
            rb.AddForce(transform.forward * z * speed / 2);
            rb.AddForce(transform.right * x * speed / 2);
        }
        
        //Jump
        if (Input.GetButtonDown("Jump") && IsGrounded()) {
            rb.AddForce(Vector3.up * 250);
        }
    }
    
    //The isGrounded Check for the player
    bool IsGrounded() {
        return Physics.Raycast(transform.position, -transform.up, distToGround + .05f);
    }
}