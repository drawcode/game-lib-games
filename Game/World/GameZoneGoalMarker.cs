using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameZoneGoalMarker : GameObjectBehavior {

    public GameObject containerEffects;
    public GameObject containerAssets;

    public float currentTimeBlock = 0.0f;
    public float actionInterval = 7.0f;

    public GameZoneGoal gameZoneGoalLeft;
    public GameZoneGoal gameZoneGoalRight;

    bool initialized = false;

    public void Start() {
        Init();
    }

    public void OnEnable() {

    }

    public void OnDisable() {

    }

    public void CheckZones() {
        if (gameZoneGoalLeft == null) {
            gameZoneGoalLeft = GetZoneObject(GameZoneKeys.goal_left);
        }
        if (gameZoneGoalRight == null) {
            gameZoneGoalRight = GetZoneObject(GameZoneKeys.goal_right);
        }
    }

    public GameZoneGoal GetZoneObject(string goalZoneType) {
        foreach (GameZoneGoal zoneItem in
            GameController.Instance.levelZonesContainerObject
            .GetComponentsInChildren<GameZoneGoal>(true)) {
            if (zoneItem.gameEndZoneType == goalZoneType) {
                return zoneItem;
            }
        }
        return null;
    }

    void Init() {
        CheckZones();
        initialized = true;
        UpdateZoneMarker();
    }

    void Update() {

        if (!initialized) {
            return;
        }

        if (!GameConfigs.isGameRunning) {
            return;
        }

        currentTimeBlock += Time.deltaTime;

        if (currentTimeBlock > actionInterval) {
            currentTimeBlock = 0.0f;
            UpdateZoneMarker();
        }
    }

    public static GameZoneGoalMarker GetMarker() {
        foreach (GameZoneGoalMarker marker in
            GameController.Instance.levelMarkersContainerObject
            .GetComponentsInChildren<GameZoneGoalMarker>(true)) {
            return marker;
        }
        return null;
    }

    public void UpdateIndicator() {
        if (!AppModes.Instance.isAppModeGameTraining) {
            GamePlayerIndicator.AddIndicator(
                gameObject, GamePlayerIndicatorType.goal);
        }
    }

    void UpdateZoneMarker() {
        // Check for marker location and move it

        if (!AppModes.Instance.isAppModeGameTraining) {
            if (GameController.Instance.currentGameZone == GameZoneKeys.goal_left) {
                transform.position = gameZoneGoalLeft.gameObject.transform.position;
            }
            else if (GameController.Instance.currentGameZone == GameZoneKeys.goal_right) {
                transform.position = gameZoneGoalRight.gameObject.transform.position;
            }
        }
    }

    public virtual void PlayEffects() {
        if (containerEffects != null) {
            containerEffects.Show();
            containerEffects.PlayParticleSystem(true);
        }
    }

    public virtual void StopEffects() {
        if (containerEffects != null) {
            containerEffects.StopParticleSystem(true);
        }
    }
}