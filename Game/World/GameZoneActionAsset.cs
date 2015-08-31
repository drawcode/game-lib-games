using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public enum GameZoneActionAssetState {
    none,
    creating,
    destroying,
    created,
    destroyed
}

public class GameZoneActionAsset : GameZoneAction {

    float lastCreateProgress = 0;
    public float currentCreateProgress = 0;
    public GameZoneActionAssetState currentCreateState = GameZoneActionAssetState.none;
    public GameZoneActionAssetState actionGoalState = GameZoneActionAssetState.none;
    public bool actionCompleted = false;
    public bool actionStarted = false;

    public override void Start() {
        base.Start();
        gameZoneType = GameZoneKeys.action_build;
        actionCode = GameZoneActions.action_build;






    }
    
    public override void OnEnable() {
        base.OnEnable();
    }
    
    public override void OnDisable() {
        base.OnDisable();
    }

    public void AssetAnimationIdle() {
        containerAssets.PlayAnimation(assetAnimationNameIdle);
    }

    public void AssetAnimationPlayNormalized(float time) {
        containerAssets.StepAnimationFrame(assetAnimationNamePlay, time);
    }

    public void Update() {

        float power = .1f;

        if (Application.isEditor) {
            if (Input.GetKey(KeyCode.LeftControl)) {

                if (Input.GetKey(KeyCode.KeypadPlus)) {
                    currentCreateProgress += power * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.KeypadMinus)) {

                    currentCreateProgress -= power * Time.deltaTime;
                }
            }
        }        

        Mathf.Clamp(currentCreateProgress, 0f, 1f);

        if (!actionCompleted) {

            if (isActionCodeBuild || isActionCodeRepair) {
                if (currentCreateProgress >= 1) {
                    actionCompleted = true;

                    GameController.CurrentGamePlayerController.ProgressScores(1);
                }
            }
            else if (isActionCodeAttack) {
                if (currentCreateProgress <= 0) {
                    actionCompleted = true;
                    
                    GameController.CurrentGamePlayerController.ProgressScores(1);
                }
            }

            if (currentCreateProgress != lastCreateProgress) {
                lastCreateProgress = currentCreateProgress;
                        
                AssetAnimationPlayNormalized(currentCreateProgress);
            }   
        }

    }

}