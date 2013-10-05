#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIPanelDialogDisplay : UIPanelBase {

	public static UIPanelDialogDisplay Instance;

    public UILabel labelTitle;
    public UILabel labelDescription;

    public UIImageButton buttonDialogOk;
    public UIImageButton buttonDialogCancel;
    public UIImageButton buttonDialogGo;

	public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }
		
        Instance = this;	
	}
	
	public static bool isInst {
		get {
			if(Instance != null) {
				return true;
			}
			return false;
		}
	}	
	
	public override void Init() {
		base.Init();

        SetTitle("");
        SetDescription("");
        HideButtonCancel();
        HideButtonOk();
        HideButtonGo();
		
		loadData();
	}	
	
	public override void Start() {
		Init();
	}
	
    void OnEnable() {
		Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

    }
    
    void OnDisable() {
		Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

    }
	
    void OnButtonClickEventHandler(string buttonName) {
        if(UIUtil.IsButtonClicked(buttonDialogOk, buttonName)) {
            HideAll();
            GameController.Instance.GameRunningStateRun();
        }
        else if(UIUtil.IsButtonClicked(buttonDialogGo, buttonName)) {
            HideAll();
            GameController.Instance.GameRunningStateRun();
        }
        else if(UIUtil.IsButtonClicked(buttonDialogCancel, buttonName)) {
            HideAll();
            GameController.Instance.GameRunningStateRun();
        }
	}

    public static void ShowDefault() {
        if(isInst) {
            Instance.AnimateIn();
            Instance.loadData();
        }
    }

    public static void HideAll() {
        if(isInst) {
            Instance.AnimateOut();
        }
    }

    public static void ShowButtonOk() {
        if(isInst) {
            Instance.showButtonOk();
        }
    }

    public static void ShowButtonCancel() {
        if(isInst) {
            Instance.showButtonCancel();
        }
    }

    public static void ShowButtonGo() {
        if(isInst) {
            Instance.showButtonGo();
        }
    }

    public static void HideButtonOk() {
        if(isInst) {
            Instance.hideButtonOk();
        }
    }

    public static void HideButtonCancel() {
        if(isInst) {
            Instance.hideButtonCancel();
        }
    }

    public static void HideButtonGo() {
        if(isInst) {
            Instance.hideButtonGo();
        }
    }

    public void showButtonOk() {
        showButton(buttonDialogOk);
    }

    public void showButtonCancel() {
        showButton(buttonDialogCancel);
    }

    public void showButtonGo() {
        showButton(buttonDialogGo);
    }

    public void hideButtonOk() {
        hideButton(buttonDialogOk);
    }

    public void hideButtonCancel() {
        hideButton(buttonDialogCancel);
    }

    public void hideButtonGo() {
        hideButton(buttonDialogGo);
    }

    public static void ShowButton(UIImageButton button) {
        if(isInst) {
            Instance.showButton(button);
        }
    }

    public static void HideButton(UIImageButton button) {
        if(isInst) {
            Instance.hideButton(button);
        }
    }

    public void showButton(UIImageButton button) {
        if(button != null) {
            button.gameObject.Show();
        }
    }

    public void hideButton(UIImageButton button) {
        if(button != null) {
            button.gameObject.Hide();
        }
    }

    public static void SetTitle(string titleTo) {
        if(isInst) {
            Instance.setTitle(titleTo);
        }
    }

    public void setTitle(string titleTo) {
        UIUtil.SetLabelValue(labelTitle, titleTo);
    }

    public static void SetDescription(string descriptionTo) {
        if(isInst) {
            Instance.setDescription(descriptionTo);
        }
    }

    public void setDescription(string descriptionTo) {
        UIUtil.SetLabelValue(labelDescription, descriptionTo);
    }

	public static void LoadData() {
		if(Instance != null) {
			Instance.loadData();
		}
	}
 
    public void loadData() {
        StartCoroutine(loadDataCo());
    }
    
    IEnumerator loadDataCo() {
    
        yield return new WaitForSeconds(1f);

        HideButtonOk();
        HideButtonCancel();
        HideButtonGo();
    }
	
}
