using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Game.App;
using Engine.Game.App.BaseApp;
using Engine.Game.Data;
using UnityEngine;

public class GameObjectQueueItem {
    public string type = "";
    public string code = "";
    public string data_type = "";
    public string display_type = "";
    public Vector3 pos = Vector3.zero;
    public Quaternion rot = Quaternion.identity;
}

public class GameActorDataItem : GameDataObject {

    public bool overrideLoading = false;


    // RPG

    public virtual GameDataItemRPG rpg {
        get {
            return Get<GameDataItemRPG>(BaseDataObjectKeys.rpg, new GameDataItemRPG());
        }

        set {
            Set<GameDataItemRPG>(BaseDataObjectKeys.rpg, value);
        }
    }

    public GameActorDataItem() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

        rpg = new GameDataItemRPG();

        code = "";
        type = BaseDataObjectKeys.character;
        data_type = GameSpawnType.zonedType;
        display_type = GameActorType.enemy;
        rotation_data = new Vector3Data();
        position_data = new Vector3Data(0, 0, 0);
        scale_data = new Vector3Data(1, 1, 1);
    }
}

