using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameZoneGoal : GameZone {

    public string gameEndZoneType = GameZoneKeys.goal_left;
    
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    UILabel labelEndZone;
#else
    GameObject labelEndZone;
#endif

    public GameObject spriteEndZone;

    public override void Start() {
        base.Start();
        gameZoneType = GameZoneKeys.goal;
        HandleCustomizations();
    }

    public override void OnEnable() {
        base.OnEnable();
    }

    public override void OnDisable() {
        base.OnDisable();
    }

    public void HandleCustomizations() {

        // TODO set end zone name 

        if (gameZoneType.StartsWith(GameZoneKeys.goal)) {

            /*
            bool isLeft = gameEndZoneType == GameZoneKeys.goal_left;

            string colorNameHighlight = isLeft ?
                GameCustomItemNames.jerseyHighlight :
                    GameCustomItemNames.jersey;

            string colorNameBackground = isLeft ?
                GameCustomItemNames.jersey :
                    GameCustomItemNames.jerseyHighlight;


            if (gameEndZoneType == GameZoneKeys.goal_left
               || gameEndZoneType == GameZoneKeys.goal_right) {

                if (labelEndZone != null) {

                    UIColorCustomObject objectText =
                        labelEndZone.gameObject.AddComponent<UIColorCustomObject>();
                    objectText.SetColorByKey(colorNameHighlight, .8f);
                }

                if (spriteEndZone != null) {

                    UIColorCustomObject objectSprite =
                        spriteEndZone.gameObject.AddComponent<UIColorCustomObject>();
                    objectSprite.SetColorByKey(colorNameBackground, .8f);
                }
            }
            */
        }
    }
}