using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetWheelColliderSettings : GameObjectBehavior {
    public WheelCollider flWheelCollider;
    public WheelCollider frWheelCollider;
    public WheelCollider rlWheelCollider;
    public WheelCollider rrWheelCollider;
    public Transform flWheel;
    public Transform frWheel;
    public Transform rlWheel;
    public Transform rrWheel;
    public float radiusFront = 0.3f;
    public float radiusBack = 0.3f;
    public bool mirrorWheels = true;
    //private Vector3 frontLeftPosition;
    //private Vector3 rearLeftPosition;

    void Awake() {

        UpdateColliders();
    }

    void UpdateColliders() {

        if (flWheelCollider != null) {
            flWheelCollider.transform.position = flWheel.position;
        }

        if (rlWheelCollider != null) {
            rlWheelCollider.transform.position = rlWheel.position;
        }

        if (frWheelCollider != null) {
            frWheelCollider.transform.position = frWheel.position;
        }

        if (rrWheelCollider != null) {
            rrWheelCollider.transform.position = rrWheel.position;
        }


        if (rrWheelCollider != null) {
            rrWheelCollider.radius = radiusFront;
        }

        if (frWheelCollider != null) {
            frWheelCollider.radius = radiusFront;
        }

        if (rlWheelCollider != null) {
            rlWheelCollider.radius = radiusBack;
        }

        if (rrWheelCollider != null) {
            rrWheelCollider.radius = radiusBack;
        }
    }

    // Update is called once per frame
    void Update() {
        if (!Application.isPlaying) {
            if (mirrorWheels) {

                if (frWheel != null) {

                    Vector3 fllp = frWheel.localPosition;
                    fllp.x = fllp.x * (-1);
                    flWheel.localPosition = fllp;
                }

                if (rrWheel != null) {
                    Vector3 rllp = rrWheel.localPosition;
                    rllp.x = rllp.x * (-1);
                    rlWheel.localPosition = rllp;
                }
            }

            UpdateColliders();
        }
    }

    //public void OnDrawGizmosSelected()
    public void OnDrawGizmos() {
        if (!Application.isPlaying) {
            float frontLength = radiusFront * 2;
            float rearLength = radiusBack * 2;
            Vector3 cubeFront = new Vector3(frontLength, frontLength, frontLength);
            Vector3 cubeRear = new Vector3(rearLength, rearLength, rearLength);
            Gizmos.color = Color.white;

            if (frWheel != null) {
                Gizmos.DrawWireCube(frWheel.position, cubeFront);
            }

            if (rrWheel != null) {
               Gizmos.DrawWireCube(rrWheel.position, cubeRear);
            }
            if (mirrorWheels) {
                Gizmos.color = Color.red;
            }

            if (flWheel != null) {
                Gizmos.DrawWireCube(flWheel.position, cubeFront);
            }

            if (rlWheel != null) {
                Gizmos.DrawWireCube(rlWheel.position, cubeRear);
            }
        }
    }
}