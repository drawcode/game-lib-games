using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UICustomizeSelectObject : UICustomizeObject {
    
    public int currentIndex = -1;
    public UIImageButton buttonCycleLeft;
    public UIImageButton buttonCycleRight;
    public UILabel labelCurrentDisplayName;
    public UILabel labelCurrentType;
    public UILabel labelCurrentStatus;
    public UIInput inputCurrentDisplayName;
    public GameProfileCustomItem currentProfileCustomItem;
    public GameProfileCustomItem initialProfileCustomItem;

    public override void Start() {

    }

    public override void Load() {

    }
    
    public override void Update() {
       
    }
}
