using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UIButtonEventTap();

public class UIButtonMetaItem {
    public string key = "";
    public UIButton button;
    public Vector3 buttonPositionCurrent;
    public Vector3 buttonPositionCurrentDown;
    public Vector3 buttonPosition;
        
    //public EZInputDelegate inputDelegate;
    //public EZValueChangedDelegate changedDelegate;
        
    public UIButtonEventTap buttonEventTap;
}

public class UIButtonMeta {     
        
    public UIButtonMetaItem currentButton;
    public bool isStoreOnly = false;
    public Dictionary<string, UIButtonMetaItem> buttons = new Dictionary<string, UIButtonMetaItem>();
        
    public void SetButton(string key, ref UIButton button) {
                
        //LogUtil.Log("isStoreOnly:" + isStoreOnly);
                
        /*
                if(button != null) {
                        if(buttons.ContainsKey(key)) {
                                //LogUtil.Log("button contains key:" + key);
                                buttons[key].button = button;
                        }
                        else {                  
                                UIButtonMetaItem buttonMetaItem = new UIButtonMetaItem();
                                buttonMetaItem.button = button;
                                buttonMetaItem.buttonPosition = button.transform.localPosition;
                                if(buttonMetaItem.button.UILabel != null) {
                                        buttonMetaItem.buttonPositionCurrent = buttonMetaItem.button.UILabel.gameObject.transform.localPosition;
                                        buttonMetaItem.buttonPositionCurrentDown = buttonMetaItem.button.UILabel.gameObject.transform.localPosition;
                                        buttonMetaItem.buttonPositionCurrentDown.y = buttonMetaItem.buttonPositionCurrentDown.y - .05f;
                                }
                                buttons.Add(key, buttonMetaItem);
                                //LogUtil.Log("button new key:" + key);
                        }
                }
                */
    }
        
    /*
        public void SetInputDelegate(string key, EZInputDelegate inputDelegate) {
                if(buttons.ContainsKey(key)) {  
                        buttons[key].inputDelegate = inputDelegate;
                        buttons[key].button.SetInputDelegate(inputDelegate);
                }
        }
        */
        
    public bool IsEventReady {
        get {
            bool ready = true;
                        
            //if(AlertDialog.IsActive) {
            //      ready = false;
            //}
            //else if(!isStoreOnly && GameStore.IsActive) {                         
            //      ready = false;  
            //}
                        
            return ready;                   
        }
    }
        
    /*
        public void SetInputDelegateDefault(string key) {
                if(buttons.ContainsKey(key)) {  
                        
                        buttons[key].button.SetInputDelegate(delegate (ref POINTER_INFO info) {
                                if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {
                                        
                        //LogUtil.Log("ButtonMeta isStoreOnly:" + isStoreOnly);
                        //LogUtil.Log("ButtonMeta AlertDialog.IsActive:" + AlertDialog.IsActive);
                        //LogUtil.Log("ButtonMeta Tap GameStore.IsActive:" + GameStore.IsActive);
                        //LogUtil.Log("ButtonMeta IsEventReady:" + IsEventReady);
                                        
                                        if(IsEventReady) {      
                                                if(buttons[key].buttonEventTap != null) {                                       
                                                        //GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
                                                        buttons[key].buttonEventTap();
                                                        //LogUtil.Log("button tap key:" + key);
                                                }
                        
                                                EventButtonTap(key);
                                                //LogUtil.Log("button tap ez key:" + key);
                                                //buttons[key].button.SetControlState(UIButton.CONTROL_STATE.activeInHierarchy);
                                        }
                                }                               
                                else if(info.evt == POINTER_INFO.INPUT_EVENT.PRESS) {
                                        if(IsEventReady) {      
                                                EventButtonPress(key);
                                                //LogUtil.Log("button press ez key:" + key);
                                                buttons[key].button.SetControlState(UIButton.CONTROL_STATE.activeInHierarchy);
                                        }
                                }
                                else if(info.evt == POINTER_INFO.INPUT_EVENT.RELEASE
                                        || info.evt == POINTER_INFO.INPUT_EVENT.RELEASE_OFF) {
                                        
                                        if(IsEventReady) {      
                                                EventButtonRelease(key);        
                                                //buttons[key].button.SetControlState(UIButton.CONTROL_STATE.NORMAL);
                                                //LogUtil.Log("button release ez key:" + key);  
                                        }
                                }
                                //else if(info.evt == POINTER_INFO.INPUT_EVENT.DRAG) {
                                //      buttons[key].button.SetControlState(UIButton.CONTROL_STATE.NORMAL);                     
                                //}
                        });
                }
        }
        */
        
    public void SetButton(string key, ref UIButton button, UIButtonEventTap tapEvent) {
        SetButton(key, ref button);
        SetTapDelegate(key, tapEvent);
    }
        
    public void SetTapDelegate(string key, UIButtonEventTap tapEvent) {
        if(buttons.ContainsKey(key)) {  
            //buttons[key].buttonEventTap = tapEvent;
            //SetInputDelegateDefault(key);
        }
    }
        
    public void EventButtonTap(string key) {
        if(buttons.ContainsKey(key)) {  
            UIButtonMetaItem item = buttons[key];                   
            item.button.transform.localPosition = item.buttonPosition;
        }
    }
        
    public void EventButtonPress(string key) {
        if(buttons.ContainsKey(key)) {  
            //UIButtonMetaItem item = buttons[key];
            //if(item.button.UILabel) {
            //      Vector3 temp = item.button.UILabel.gameObject.transform.localPosition;
            //      temp.y = item.buttonPositionCurrentDown.y;
            //      item.button.UILabel.gameObject.transform.localPosition = temp;
            //}
        }
    }
        
    public void EventButtonRelease(string key) {
        if(buttons.ContainsKey(key)) {  
            //UIButtonMetaItem item = buttons[key];
            /*
                        if(item.button.UILabel) {
                                Vector3 temp = item.button.UILabel.gameObject.transform.localPosition;
                                temp.y = item.buttonPositionCurrent.y;
                                item.button.UILabel.gameObject.transform.localPosition = temp;
                        }
                        item.button.transform.localPosition = item.buttonPosition;
                        */
        }
    }
        
    public void SetButtonEnable(string key, bool enable) {
        if(buttons.ContainsKey(key)) {                  
            currentButton = buttons[key];                   
            SetButtonEnable(currentButton.button, enable);
        }
    }
        
    public void SetButtonEnable(UIButton button, bool enable) {
        if(button != null) {
            UIUtil.UIButtonEnable(button, enable);
        }
    }

    public void SetButtonsDialogState() {
        foreach(KeyValuePair<string, UIButtonMetaItem> buttonItem in buttons) {
            SetButtonDialogState(buttonItem.Key);   
        }
    }
        
    public void SetButtonDialogState(string key) {
        //if(AlertDialog.IsActive) {
        SetButtonEnable(key, false);
        //}
        //else {
        SetButtonEnable(key, true);
        //}
    }
        
    public void SetButtonsAlertState() {
        foreach(KeyValuePair<string, UIButtonMetaItem> buttonItem in buttons) {
            SetButtonAlertState(buttonItem.Key);    
        }
    }
        
    public void SetButtonAlertState(string key) {
        if(!IsEventReady) {
            SetButtonEnable(key, false);
        } else {
            SetButtonEnable(key, true);
        }
    }
        
    public void SetButtonStoreState(string key) {
        if(!IsEventReady) {
            SetButtonEnable(key, false);
        } else {
            SetButtonEnable(key, true);
        }
    }
        
    public void ResetButtons() {
        foreach(KeyValuePair<string, UIButtonMetaItem> buttonItem in buttons) {
            ResetButton(buttonItem.Key);    
        }
    }
        
    public void ResetButton(string key) {
        if(buttons.ContainsKey(key)) {                  
            currentButton = buttons[key];                   
            ResetButton(currentButton);
        }
    }
        
    public void ResetButton(UIButtonMetaItem buttonItem) {
        if(buttonItem.button) {
            buttonItem.button.transform.localPosition = buttonItem.buttonPosition;
        }
    }
}

public class UIUtil {
        
    /*
        public static void Show<T>(T obj) {
                if(obj != null) {
                        obj.gameObject.Show();
                }
        }
        
        public static void Hide<T>(T obj) {
                if(obj != null) {
                        obj.gameObject.Hide();
                }
        }
        */
        
    public static void ShowInput(UIInput obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }
        
    public static void HideInput(UIInput obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
        
    public static void ShowButton(UIImageButton obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }
        
    public static void HideButton(UIImageButton obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
        
    public static void ShowLabel(UILabel obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }
        
    public static void HideLabel(UILabel obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
        
    public static void ShowSlider(UISlider obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }
        
    public static void HideSlider(UISlider obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
        
    public static void ShowCheckbox(UICheckbox obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }
        
    public static void HideCheckbox(UICheckbox obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }

    public static void UpdateLabelObject(GameObject parentGo, string key, string val) {
        UpdateLabelObject(parentGo.transform, key, val);
    }
 
    public static void UpdateLabelObject(Transform parentTransform, string key, string val) {
        Transform labelObject = parentTransform.FindChild(key);
        if(labelObject != null) {
            UILabel label = labelObject.GetComponent<UILabel>();
            SetLabelValue(label, val);
        }
        else {
            foreach(Transform t in parentTransform) {
                UpdateLabelObject(t.gameObject, key, val);
            }
        }
    }
        
    public static void SetLabelValue(UILabel obj, string labelValue) {
        if(obj != null) {
            obj.text = labelValue;
        }
    }
        
    public static string GetLabelValue(UILabel obj) {
        if(obj != null) {
            return obj.text;
        }
        return "";
    }
        
    public static void SetInputValue(UIInput obj, string labelValue) {
        if(obj != null) {
            obj.text = labelValue;
        }
    }
        
    public static string GetInputValue(UIInput obj) {
        if(obj != null) {
            return obj.text;
        }
        return "";
    }

    public static void SetSliderValue(UISlider obj, double val) {
        if(obj != null) {
            obj.sliderValue = (float)val;
            obj.ForceUpdate();
        }
    }
                
    public static void SetSliderValue(UISlider obj, float val) {
        if(obj != null) {
            obj.sliderValue = val;
            obj.ForceUpdate();
        }
    }
        
    public static float GetSliderValue(UISlider obj) {
        if(obj != null) {
            return obj.sliderValue;
        }
        return 0f;
    }
        
    public static void SetCheckboxValue(UICheckbox obj, bool selected) {
        if(obj != null) {
            obj.isChecked = selected;
        }
    }
        
    public static bool GetCheckboxValue(UICheckbox obj) {
        if(obj != null) {
            return obj.isChecked;
        }
        return false;
    }
 
    public static bool IsButton(GameObject go) {
        if(go == null)
            return false;
     
        if(go.Has<UIButton>() || go.Has<UIImageButton>()) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
        return false;
    }
 
    public static bool IsButtonClicked(UIButton button, string buttonClickedName) {
        if(button == null)
            return false;
     
        if(buttonClickedName == button.name) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
        return false;
    }

    public static bool IsButtonClicked(string button, string buttonClickedName) {
        if(button == null)
            return false;
        
        if(buttonClickedName == button) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
        return false;
    }

    public static bool IsCheckboxChecked(UICheckbox chk, string chkClickedName) {
        if(chk == null)
            return false;
        
        if(chkClickedName == chk.name) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
        return false;
    }
         
    public static bool IsButtonClicked(UIImageButton button, string buttonClickedName) {         
        if(button == null)
            return false;
     
        if(buttonClickedName.ToString() == button.name.ToString()) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
        return false;
    }

    public static void SetSpriteColor(GameObject go, Color colorTo) {
        if(go == null)
            return;

        if(go != null) {
            UITweenerUtil.ColorTo(go,
                 UITweener.Method.Linear, UITweener.Style.Once, .5f, 0f, colorTo);
        }
    }
 
    public static void SetButtonColor(UIButton buttonTo, Color colorTo) {            
        if(buttonTo == null)
            return;
     
        UITweenerUtil.ColorTo(buttonTo.gameObject, 
             UITweener.Method.Linear, UITweener.Style.Once, .5f, 0f, colorTo);
    }
 
    public static void SetButtonColor(UIImageButton buttonTo, Color colorTo) {           
        if(buttonTo == null)
            return;
     
        UITweenerUtil.ColorTo(buttonTo.gameObject, 
             UITweener.Method.Linear, UITweener.Style.Once, .5f, 0f, colorTo);
    }
        
    public static bool IsEventReady {
        get {
            bool ready = true;
                        
            //if(AlertDialog.IsActive) {
            //      ready = false;
            //}
            //else if(GameStore.IsActive) {                         
            //      ready = false;  
            //}
                        
            return ready;                   
        }
    }
        
    public static void UIButtonEnable(UIButton buttonObject, bool enabled) {
        if(buttonObject) {
            //buttonObject.controlIsEnabled = enabled;
            //if(!enabled) {
            //      buttonObject.SetControlState(UIButton.CONTROL_STATE.DISABLED);
            //}
            //else {
            //      buttonObject.SetControlState(UIButton.CONTROL_STATE.NORMAL);
            //}
        }
    }
 
 
        
}