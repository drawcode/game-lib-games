using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;
using Engine.Utility;

public class GameObjectItemDisplay : GameObjectBehavior {

    List<GameObjectItemDisplayItem> items;    
    
    void Start() {

        Init();
    }

    void Init() {

        if (items == null) {
            items = new List<GameObjectItemDisplayItem>();
        }

        foreach (GameObjectItemDisplayItem item in
            gameObject.GetList<GameObjectItemDisplayItem>()) {

            items.Add(item);
        }
    }

    void OnEnable() {

        Messenger<string, string, object>.AddListener(
            GameMessages.gameActionItem, OnGameActionItem);// item.type, broadcastVal);
    }

    void OnDisable() {

        Messenger<string, string, object>.RemoveListener(
            GameMessages.gameActionItem, OnGameActionItem);// item.type, broadcastVal);
    }

    void OnGameActionItem(string code, string type, object val) {

        /*
        string itemCode = code;

        if (type == GameDataItemReward.letter) {

            string letter = val.ToString();

            if (!letter.IsNullOrEmpty()) {

                // Update any letter item 

                //itemCode = StringUtil.Dashed(itemCode, letter);
            }
        }
        */

        foreach (GameObjectItemDisplayItem item in items) {
            if (item.name.IsEqualLowercase(code) && !item.collected) {
                item.collected = true;
                break;
            }
        }
    }

    private void Update() {
        if (Application.isEditor) {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                // TEST LETTERS
                if (Input.GetKey(KeyCode.L)) {
                    // TEST U
                    if (Input.GetKeyDown(KeyCode.U)) {
                        Messenger<string, string, object>.Broadcast(
                            GameMessages.gameActionItem, "item-letter-u", "letter", "u");// item.type, broadcastVal);
                    }
                }
            }
        }
    }
}
