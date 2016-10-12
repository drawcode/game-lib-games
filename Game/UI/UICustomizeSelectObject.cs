using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UICustomizeSelectObject : UICustomizeObject {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UIImageButton buttonCycleLeft;
    public UIImageButton buttonCycleRight;
    public UILabel labelCurrentDisplayName;
    public UILabel labelCurrentType;
    public UILabel labelCurrentStatus;
    public UIInput inputCurrentDisplayName;
#else
    public Button buttonCycleLeft;
    public Button buttonCycleRight;
    public Text labelCurrentDisplayName;
    public Text labelCurrentType;
    public Text labelCurrentStatus;
    public InputField inputCurrentDisplayName;
#endif

    public int currentIndex = -1;
    public GameProfileCustomItem currentProfileCustomItem;
    public GameProfileCustomItem initialProfileCustomItem;

    public override void Start() {

    }

    public override void Load() {

    }
    
    public override void Update() {
       
    }
}
