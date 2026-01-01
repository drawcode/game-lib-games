using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;
using Engine.Utility;
using Engine.Game.Data;
using Engine.Game.App;

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

        Messenger<string>.AddListener(
            GameMessages.gameInitLevelStart, OnGameInitLevelStart);
    }

    void OnDisable() {

        Messenger<string, string, object>.RemoveListener(
            GameMessages.gameActionItem, OnGameActionItem);// item.type, broadcastVal);

        Messenger<string>.RemoveListener(
            GameMessages.gameInitLevelStart, OnGameInitLevelStart);
    }

    void OnGameInitLevelStart(string code) {
        Clear();
    }

    void OnGameActionItem(string code, string type, object val) {

        if (type.IsEqualLowercase(BaseDataObjectKeys.item)
            || type.IsEqualLowercase(GameDataItemReward.letter)) {

            CollectSingle(code);
        }
    }

    public void CollectSingle(string code) {

        if (code.IsNullOrEmpty()) {
            return;
        }

        foreach (GameObjectItemDisplayItem item in items) {

            if (item.name.IsEqualLowercase(code) && !item.collected) {
                item.collected = true;
                break;
            }
        }

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
    }

    public void Clear() {
        foreach (GameObjectItemDisplayItem item in items) {
            item.collected = false;
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