using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIPanelSettingsProfile : UIPanelBase {
	
	public GameObject listGridRoot;
    public GameObject listItemPrefab;
	
	public UIImageButton buttonClose;
	public UIImageButton buttonProfileFacebook;	
	public UIImageButton buttonProfileTwitter;	
	public UIImageButton buttonProfileGameNetwork;
	
	
	public UIInput inputProfileName;
	
	public static UIPanelSettingsProfile Instance;		

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
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();	
		
		loadData();
	}
	
	public override void OnEnable() {
        base.OnEnable();

		Messenger<string, string>.AddListener(InputEvents.EVENT_ITEM_CHANGE, OnProfileInputChanged);
	}
	
	public override void OnDisable() {
        base.OnDisable();
        
        Messenger<string, string>.AddListener(InputEvents.EVENT_ITEM_CHANGE, OnProfileInputChanged);
	}
	
	void OnProfileInputChanged(string controlName, string data) {
		
		if(inputProfileName != null && controlName == inputProfileName.name) {
			ChangeUsername(data);
		}
	}
	
	public void ChangeUsername(string username) {
		if(inputProfileName == null) {
			return;
		}
		
		UIUtil.SetInputValue(inputProfileName, username);
		GameProfiles.Current.ChangeUser(username);
		GameProfiles.Current.username = username;
		GameState.SaveProfile();
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
		
		ChangeUsername(GameProfiles.Current.username);
	}
	
}
