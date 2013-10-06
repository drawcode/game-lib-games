using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class GameObjectLevelBase : MonoBehaviour {

    public string code = "question-1";

    public virtual void Start() {
        LoadData();
    }

    public virtual void LoadData() {

    }
}
