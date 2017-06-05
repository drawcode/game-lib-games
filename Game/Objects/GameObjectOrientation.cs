using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Utility;
using UnityEngine;

public class GameObjectOrientation : GameObjectBehavior {

    // Apply for objects that change on orientation change

    public Vector3 positionLandscape = Vector3.zero;
    public Vector3 positionPortrait = Vector3.zero;

    public Vector3 scaleLandscape = Vector3.one;
    public Vector3 scalePortrait = Vector3.one;
    
    public DeviceOrientation currentOrientation = DeviceOrientation.Unknown;
    
    void Start() {
        
        HandleOrientation(GetOrientation());
    }

    void HandleOrientation(DeviceOrientation orientationTo) {

        if(orientationTo != currentOrientation) {

            currentOrientation = orientationTo;

            if(orientationTo == DeviceOrientation.LandscapeLeft
            || orientationTo == DeviceOrientation.LandscapeRight) {

                HandleScaleLandscape();
            }
            else if(orientationTo == DeviceOrientation.Portrait
                || orientationTo == DeviceOrientation.PortraitUpsideDown) {

                HandleScalePortrait();
            }
        }
    }

    DeviceOrientation GetOrientation() {

        DeviceOrientation orientation = Input.deviceOrientation;

        if(orientation == DeviceOrientation.Unknown) {

            if(Screen.width >= Screen.height) {
                orientation = DeviceOrientation.LandscapeLeft;
            }
            else if(Screen.height > Screen.width) {
                orientation = DeviceOrientation.Portrait;
            }
        }

        return orientation;
    }

    void HandleScaleLandscape() {
        //TweenUtil.ScaleToObject(gameObject, scaleLandscape);
        gameObject.transform.localScale = scaleLandscape;
    }

    void HandleScalePortrait() {
        //TweenUtil.ScaleToObject(gameObject, scalePortrait);
        gameObject.transform.localScale = scalePortrait;
    }

    void Update() {
        HandleOrientation(GetOrientation());

        if(Input.GetKey(KeyCode.LeftControl)
            || Input.GetKey(KeyCode.LeftControl)) {

            if(Input.GetKey(KeyCode.LeftShift)
                || Input.GetKey(KeyCode.RightShift)) {

                if(Input.GetKeyDown(KeyCode.L)) {
                    HandleOrientation(DeviceOrientation.LandscapeLeft);
                }


                if(Input.GetKeyDown(KeyCode.P)) {
                    HandleOrientation(DeviceOrientation.Portrait);
                }

            }
        }
    }

}

