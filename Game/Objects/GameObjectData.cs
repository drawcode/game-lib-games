using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class GameObjectDataItem {
    public string key = "";
    public string val = "";

}

public class GameObjectData : GameObjectBehavior {
    
    // Apply this class to objects needed to be hidden but later found
    // by using GetComponentsInChildren with the inactive flag set without
    // searching recursively through the whole heirarchy of that object.

    public List<GameObjectDataItem> data = new List<GameObjectDataItem>();
        
    void Start() {

    }

    public void Set(string key, object val) {

        if(val == null) {
            return;
        }

        string valString = val.ToString();

        foreach(GameObjectDataItem item in data) {
            if(item.key == key) {
                item.val = valString;
                return;
            }
        }

        GameObjectDataItem itemAdd = new GameObjectDataItem();
        itemAdd.key = key;
        itemAdd.val = valString;

        data.Add(itemAdd);
    }

    public object Get(string key) {
            
        foreach(GameObjectDataItem item in data) {
            if(item.key == key) {
                return item.val;
            }
        }

        return null;
    }

    public Dictionary<string,object> ToDictionary() {
        Dictionary<string,object> dict = new Dictionary<string, object>();

        foreach (GameObjectDataItem dataItem in data) {
            dict.Set(dataItem.key, dataItem.val);
        }

        return dict;
    }
}
