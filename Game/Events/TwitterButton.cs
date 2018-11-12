using UnityEngine;
using System.Collections;
using System.IO;

/////**
//// * [CH] Controls the display of the twitter button. The twitter button is enabled
//// * if the user does not have twitter set up.
////**/
////public class TwitterButton : GameObjectBehavior {

////    /**
////     * [CH] The photocontroller lets us know if the user can tweet or not.
////    **/
////    //PhotoUploaderController photoController;

////    private bool _enabled;
////    UIWidget[] childWidgets;

////    void Start() {
////        //GameObject guiController = GameObject.Find("GUIController");
////        //photoController = guiController.GetComponent<PhotoUploaderController>();

////        childWidgets = GetComponentsInChildren<UIWidget>();

////        //_enabled = !photoController.canUserTweet(); // [CH] Set this to the opposite of the setting to ensure the gui is updated at startup.
////        //setButtonEnabled(photoController.canUserTweet());

////        InvokeRepeating("pollForTweetability", 1.0f, 1.0f);
////    }

////    void pollForTweetability() {
////        //setButtonEnabled(photoController.canUserTweet());
////    }

////    void OnClick() {
////        //photoController.startTwitterPhotoUploadProcess();
////    }

////    /**
////     * [CH] This is only a cosmetic difference. Clicking on the button when disabled gives the
////     * user information about setting up their twitterz in the settings menu.
////    **/
////    void setButtonEnabled(bool isEnabled) {
////        if (isEnabled == _enabled) {
////            return;
////        }
////        _enabled = isEnabled;
////        if (_enabled) {
////            foreach (UIWidget widget in childWidgets) {
////                widget.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
////            }
////        }
////        else {
////            foreach (UIWidget widget in childWidgets) {
////                widget.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);
////            }
////        }
////    }

////}