using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if USE_UI_NGUI_2_7
#endif
#if USE_UI_NGUI_3

public class UIDraggablePanel : UIDragScrollView {
    
    public void ResetPosition() {
        base.scrollView.ResetPosition();
    }
}

public class UICheckbox : UIToggle {
    
}

#endif

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
        
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public void SetButton(string key, ref UIButton button, UIButtonEventTap tapEvent) {
        SetButton(key, ref button);
        SetTapDelegate(key, tapEvent);
    }
#endif
        
    public void SetTapDelegate(string key, UIButtonEventTap tapEvent) {
        if (buttons.ContainsKey(key)) {  
            //buttons[key].buttonEventTap = tapEvent;
            //SetInputDelegateDefault(key);
        }
    }
        
    public void EventButtonTap(string key) {
        if (buttons.ContainsKey(key)) {  
            UIButtonMetaItem item = buttons[key];                   
            item.button.transform.localPosition = item.buttonPosition;
        }
    }
        
    public void EventButtonPress(string key) {
        if (buttons.ContainsKey(key)) {  
            //UIButtonMetaItem item = buttons[key];
            //if(item.button.UILabel) {
            //      Vector3 temp = item.button.UILabel.gameObject.transform.localPosition;
            //      temp.y = item.buttonPositionCurrentDown.y;
            //      item.button.UILabel.gameObject.transform.localPosition = temp;
            //}
        }
    }
        
    public void EventButtonRelease(string key) {
        if (buttons.ContainsKey(key)) {  
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
        if (buttons.ContainsKey(key)) {                  
            currentButton = buttons[key];                   
            //SetButtonEnable(currentButton.button, enable);
        }
    }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public void SetButtonEnable(UIButton button, bool enable) {
        if(button != null) {
            UIUtil.UIButtonEnable(button, enable);
        }
    }
#else
    public void SetButtonEnable(Button button, bool enable) {
        if (button != null) {
            UIUtil.UIButtonEnable(button, enable);
        }
    }
#endif

    public void SetButtonsDialogState() {
        foreach (KeyValuePair<string, UIButtonMetaItem> buttonItem in buttons) {
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
        foreach (KeyValuePair<string, UIButtonMetaItem> buttonItem in buttons) {
            SetButtonAlertState(buttonItem.Key);    
        }
    }
        
    public void SetButtonAlertState(string key) {
        if (!IsEventReady) {
            SetButtonEnable(key, false);
        }
        else {
            SetButtonEnable(key, true);
        }
    }
        
    public void SetButtonStoreState(string key) {
        if (!IsEventReady) {
            SetButtonEnable(key, false);
        }
        else {
            SetButtonEnable(key, true);
        }
    }
        
    public void ResetButtons() {
        foreach (KeyValuePair<string, UIButtonMetaItem> buttonItem in buttons) {
            ResetButton(buttonItem.Key);    
        }
    }
        
    public void ResetButton(string key) {
        if (buttons.ContainsKey(key)) {                  
            currentButton = buttons[key];                   
            ResetButton(currentButton);
        }
    }
        
    public void ResetButton(UIButtonMetaItem buttonItem) {
        if (buttonItem.button) {
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

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static void ShowInput(UIInput obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }
#endif

    public static void ShowInput(InputField obj) {
        if (obj != null) {
            obj.gameObject.Show();
        }
    }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static void HideInput(UIInput obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
    #endif
    
    public static void HideInput(InputField obj) {
        if (obj != null) {
            obj.gameObject.Hide();
        }
    }
    
    //
    
    #if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static void ShowButton(UIImageButton obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }
    #endif

    public static void ShowButton(Button obj) {
        if (obj != null) {
            obj.gameObject.Show();
        }
    }
    
    //
    
    #if USE_UI_NGUI_2_7 || USE_UI_NGUI_3 
    public static void HideButton(UIImageButton obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
    #endif
        
    public static void HideButton(Image obj) {
        if (obj != null) {
            obj.gameObject.Hide();
        }
    }

    public static void HideButton(Button obj) {
        if (obj != null) {
            obj.gameObject.Hide();
        }
    }

    //

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static void ShowText(UILabel obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }

    public static void ShowLabel(UILabel obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }
#endif

    public static void ShowText(Text obj) {
        if (obj != null) {
            obj.gameObject.Show();
        }
    }
    
    public static void ShowLabel(Text obj) {
        if (obj != null) {
            obj.gameObject.Show();
        }
    }

    //

#if USE_UI_NGUI_2_7
    public static void HideText(UILabel obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }

    public static void HideLabel(UILabel obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
#endif

#if USE_UI_NGUI_3
    public static void HideText(UIToggle obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
    
    public static void HideLabel(UIToggle obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
#endif

    public static void HideLabel(Text obj) {
        if (obj != null) {
            obj.gameObject.Hide();
        }
    }

    //

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static void ShowSlider(UISlider obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }
#endif

    public static void ShowSlider(Slider obj) {
        if (obj != null) {
            obj.gameObject.Show();
        }
    }
    
    //
    
    #if USE_UI_NGUI_2_7 || USE_UI_NGUI_3 
    public static void HideSlider(UISlider obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
    #endif
    
    public static void HideSlider(Slider obj) {
        if (obj != null) {
            obj.gameObject.Hide();
        }
    }
    
    //
    
    #if USE_UI_NGUI_2_7

    public static void ShowToggle(UICheckbox obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }

    public static void ShowCheckbox(UICheckbox obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }
    #endif

    #if USE_UI_NGUI_3
    
    public static void ShowToggle(UIToggle obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }
    
    public static void ShowCheckbox(UIToggle obj) {
        if(obj != null) {
            obj.gameObject.Show();
        }
    }
    #endif


    //
    
    #if USE_UI_NGUI_2_7

    public static void HideToggle(UICheckbox obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }

    public static void HideCheckbox(UICheckbox obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
    #endif

    #if USE_UI_NGUI_3
    
    public static void HideToggle(UIToggle obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
    
    public static void HideCheckbox(UIToggle obj) {
        if(obj != null) {
            obj.gameObject.Hide();
        }
    }
    #endif

    public static void HideToggle(Toggle obj) {
        if (obj != null) {
            obj.gameObject.Hide();
        }
    }

    public static void HideCheckbox(Toggle obj) {
        if (obj != null) {
            obj.gameObject.Hide();
        }
    }

    //

    public static void SetTextValue(GameObject go, string code, string val) {
        
        ////////Debug.Log("SetTextValue:" + " code:" + code + " val:" + val );

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        UILabel[] labels = go.GetComponentsInChildren<UILabel>();
        
        foreach (UILabel label in labels) {
            
            if(label.gameObject.name.Contains(code)) {
                UIUtil.SetLabelValue(label, val);
            }
        }
        
        UIInput[] inputs = go.GetComponentsInChildren<UIInput>();
        
        foreach (UIInput input in inputs) {
            if(input.gameObject.name.Contains(code)) {
                UIUtil.SetInputValue(input, val);
            }
        }
#endif
        
        Text[] labelsNative = go.GetComponentsInChildren<Text>();
        
        foreach (Text label in labelsNative) {
            
            if (label.gameObject.name.Contains(code)) {
                UIUtil.SetLabelValue(label, val);
            }
        }
        
        InputField[] inputsNative = go.GetComponentsInChildren<InputField>();
        
        foreach (InputField input in inputsNative) {
            if (input.gameObject.name.Contains(code)) {
                UIUtil.SetInputValue(input, val);
            }
        }

    }

    //
    
    public static void SetTextColor(GameObject go, string code, Color color) {
        
        //LogUtil.Log("SetMaterialColor name:" + name + " color:" + color );

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3

        UILabel[] labels = go.GetComponentsInChildren<UILabel>();
        
        foreach (UILabel label in labels) {

            if(label.gameObject.name.Contains(code)) {
                SetSpriteColor(label.gameObject, color);
            }
        }

        UIInput[] inputs = go.GetComponentsInChildren<UIInput>();
        
        foreach (UIInput input in inputs) {
            if(input.gameObject.name.Contains(code)) {
                SetSpriteColor(input.gameObject, color);
            }
        }
#endif
        Text[] labelsNative = go.GetComponentsInChildren<Text>();
        
        foreach (Text label in labelsNative) {
            
            if (label.gameObject.name.Contains(code)) {
                SetSpriteColor(label.gameObject, color);
            }
        }
        
        InputField[] inputsNative = go.GetComponentsInChildren<InputField>();
        
        foreach (InputField input in inputsNative) {
            if (input.gameObject.name.Contains(code)) {
                SetSpriteColor(input.gameObject, color);
            }
        }

    }

    //

    public static void UpdateLabelObject(GameObject parentGo, string key, string val) {
        UpdateLabelObject(parentGo.transform, key, val);
    }
 
    public static void UpdateLabelObject(Transform parentTransform, string key, string val) {
        Transform labelObject = parentTransform.FindChild(key);
        if (labelObject != null) {
            
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            UILabel label = labelObject.GetComponent<UILabel>();
            SetLabelValue(label, val);
#endif
            Text txt = labelObject.GetComponent<Text>();
            SetLabelValue(txt, val);
        }
        else {
            foreach (Transform t in parentTransform) {
                UpdateLabelObject(t.gameObject, key, val);
            }
        }
    }

    //

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3        
    public static void SetLabelValue(UILabel obj, string val) {
        if(obj != null) {
            obj.text = val;
        }
    }
#endif
                
    public static void SetLabelValue(Text obj, string val) {
        if (obj != null) {
            obj.text = val;
        }
    }

    //

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static string GetLabelValue(UILabel obj) {
        if(obj != null) {
            return obj.text;
        }
        return "";
    }
#endif
    
    public static string GetLabelValue(Text obj) {
        if (obj != null) {
            return obj.text;
        }
        return "";
    }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3    
    public static void SetInputValue(UIInput obj, string val) {
        if(obj != null) {
            obj.text = val;
        }
    }
#endif
    
    public static void SetInputValue(InputField obj, string val) {
        if (obj != null) {
            obj.text = val;
        }
    }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static string GetInputValue(UIInput obj) {
        if(obj != null) {
            return obj.text;
        }
        return "";
    }
#endif

    public static string GetInputValue(InputField obj) {
        if (obj != null) {
            return obj.text;
        }
        return "";
    }
    
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static void SetSliderValue(UISlider obj, double val) {
        if(obj != null) {
            obj.sliderValue = (float)val;
            obj.ForceUpdate();
        }
    }
#endif

    public static void SetSliderValue(Slider obj, double val) {
        if (obj != null) {
            obj.value = (float)val;
        }
    }
    
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3        
    public static void SetSliderValue(UISlider obj, float val) {
        if(obj != null) {
            obj.sliderValue = val;
            obj.ForceUpdate();
        }
    }
#endif

    public static void SetSliderValue(Slider obj, float val) {
        if (obj != null) {
            obj.value = val;
        }
    }
    
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static float GetSliderValue(UISlider obj) {
        if(obj != null) {
            return obj.sliderValue;
        }
        return 0f;
    }
#endif

    public static float GetSliderValue(Slider obj) {
        if (obj != null) {
            return obj.value;
        }
        return 0f;
    }

#if USE_UI_NGUI_2_7
    public static void SetToggleValue(UICheckbox obj, bool selected) {
        if(obj != null) {
            obj.isChecked = selected;
        }
    }

    public static void SetCheckboxValue(UICheckbox obj, bool selected) {
        if(obj != null) {
            obj.isChecked = selected;
        }
    }
#endif

#if USE_UI_NGUI_3
    public static void SetToggleValue(UIToggle obj, bool selected) {
        if(obj != null) {
            obj.value = selected;
        }
    }
    
    public static void SetCheckboxValue(UIToggle obj, bool selected) {
        if(obj != null) {
            obj.value = selected;
        }
    }
#endif
    
    public static void SetToggleValue(Toggle obj, bool selected) {
        if (obj != null) {
            obj.isOn = selected;
        }
    }

    public static void SetCheckboxValue(Toggle obj, bool selected) {
        if (obj != null) {
            obj.isOn = selected;
        }
    }
    
#if USE_UI_NGUI_2_7
    
    public static bool GetToggleValue(UICheckbox obj) {
        if(obj != null) {
            return obj.isChecked;
        }
        return false;
    }

    public static bool GetCheckboxValue(UICheckbox obj) {
        if(obj != null) {
            return obj.isChecked;
        }
        return false;
    }

#endif

#if USE_UI_NGUI_3
    
    public static bool GetToggleValue(UIToggle obj) {
        if(obj != null) {
            return obj.value;
        }
        return false;
    }
    
    public static bool GetCheckboxValue(UIToggle obj) {
        if(obj != null) {
            return obj.value;
        }
        return false;
    }
#endif
    
    public static bool GetToggleValue(Toggle obj) {
        if (obj != null) {
            return obj.isOn;
        }
        return false;
    }

    public static bool GetCheckboxValue(Toggle obj) {
        if (obj != null) {
            return obj.isOn;
        }
        return false;
    }

    public static bool IsButton(GameObject go) {
        if (go == null)
            return false;
        
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        if(go.Has<UIButton>() || go.Has<UIImageButton>()) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
#endif
                
        if (go.Has<Button>()) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }

        return false;
    }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static bool IsButtonClicked(UIButton button, string buttonClickedName) {
        if(button == null)
            return false;
     
        if(buttonClickedName == button.name) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
        return false;
    }
    
#endif

    public static bool IsButtonClicked(Button button, string buttonClickedName) {
        if (button == null)
            return false;
        
        if (buttonClickedName == button.name) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
        return false;
    }

    public static bool IsButtonClicked(string button, string buttonClickedName) {
        if (button == null)
            return false;
        
        if (buttonClickedName == button) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
        return false;
    }

    public static bool IsButtonClickedLike(string button, string buttonClickedName) {
        if (button == null)
            return false;
        
        if (buttonClickedName.Contains(button)) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
        return false;
    }

#if USE_UI_NGUI_2_7
    public static bool IsToggleOn(UICheckbox toggle,  string toggleName) {
        return IsCheckboxChecked(toggle, toggleName);
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

    public static bool IsCheckboxChecked(UICheckbox toggle) {
        if (toggle == null)
            return false;

        if (toggle.isChecked) {
            return true;
        }
        return false;
    }
#endif

#if USE_UI_NGUI_3
    public static bool IsToggleOn(UIToggle toggle,  string toggleName) {
        return IsCheckboxChecked(toggle, toggleName);
    }

    public static bool IsCheckboxChecked(UIToggle toggle,  string toggleName) {
        if(toggle == null)
            return false;
        
        if(toggleName == toggle.name) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
        return false;
    }    

    public static bool IsCheckboxChecked(UIToggle toggle) {
        if (toggle == null)
            return false;

        if (toggle.isOn) {
            return true;
        }
        return false;
    }
#endif

    public static bool IsToggleOn(Toggle toggle, string toggleName) {

        return IsCheckboxChecked(toggle, toggleName);
    }
        
    public static bool IsCheckboxChecked(Toggle toggle, string toggleName) {
        if (toggle == null)
            return false;
        
        if (toggleName == toggle.name) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            if (toggle.isOn) {
                return true;
            }
        }
        return false;
    }

    public static bool IsCheckboxChecked(Toggle toggle) {
        if (toggle == null)
            return false;

        if (toggle.isOn) {
            return true;
        }
        return false;
    }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static bool IsButtonClicked(UIImageButton button, string buttonClickedName) {         
        if(button == null)
            return false;
     
        if(buttonClickedName.ToString() == button.name.ToString()) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
        return false;
    }
#endif

    public static bool IsButtonClicked(Image button, string buttonClickedName) {         
        if (button == null)
            return false;
        
        if (buttonClickedName.ToString() == button.name.ToString()) {
            //LogUtil.Log("IsButtonClicked: " + buttonClickedName);
            return true;
        }
        return false;
    }

    public static void SetSpriteColor(GameObject go, Color colorTo) {
        if (go == null)
            return;

        if (go != null) {
            UITweenerUtil.ColorTo(go,
                 UITweener.Method.Linear, UITweener.Style.Once, .5f, 0f, colorTo);
        }
    }
    
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static void SetLabelColor(UILabel labelTo, Color colorTo) {            
        if(labelTo == null)
            return;
        
        UITweenerUtil.ColorTo(labelTo.gameObject, 
                              UITweener.Method.Linear, UITweener.Style.Once, .5f, 0f, colorTo);
    }
#endif
        
    public static void SetLabelColor(Text labelTo, Color colorTo) {            
        if (labelTo == null)
            return;
        
        UITweenerUtil.ColorTo(labelTo.gameObject, 
                              UITweener.Method.Linear, UITweener.Style.Once, .5f, 0f, colorTo);
    }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static void SetButtonColor(UIButton buttonTo, Color colorTo) {            
        if(buttonTo == null)
            return;
     
        UITweenerUtil.ColorTo(buttonTo.gameObject, 
             UITweener.Method.Linear, UITweener.Style.Once, .5f, 0f, colorTo);
    }
#endif
    
    public static void SetButtonColor(Button buttonTo, Color colorTo) {            
        if (buttonTo == null)
            return;
        
        UITweenerUtil.ColorTo(buttonTo.gameObject, 
                              UITweener.Method.Linear, UITweener.Style.Once, .5f, 0f, colorTo);
    }
    
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public static void SetButtonColor(UIImageButton buttonTo, Color colorTo) {           
        if(buttonTo == null)
            return;
     
        UITweenerUtil.ColorTo(buttonTo.gameObject, 
             UITweener.Method.Linear, UITweener.Style.Once, .5f, 0f, colorTo);
    }
#endif
    
    public static void SetButtonColor(Image buttonTo, Color colorTo) {           
        if (buttonTo == null)
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

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
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
#endif 
    
    public static void UIButtonEnable(Button buttonObject, bool enabled) {
        if (buttonObject) {
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