using UnityEngine;
using System;
using System.Collections;

using Engine.Utility;

public class GameObjectCallToAction : GameObjectBehavior {

    public Vector3 distance;

    public float scale = 1f;
    public float scaleTime = .2f;

    public float scaleEaseMin = -0.5f;
    public float scaleEaseMax = .05f;

    public float scaleMin = .95f;
    public float scaleMax = 1.05f;

    public float rotate = 1.05f;
    public float rotateTime = .2f;

    void Start() {
        Animate();
    }

    public void Animate() {
        StartCoroutine(AnimateCo());
    }

    IEnumerator AnimateCo() {

        yield return new WaitForSeconds(.8f);

        if (gameObject != null) {

            AnimateScale();

            AnimateRotate();
        }
    }

    // sccale

    void AnimateScale() {

        // stopCurrent false: rotate and scale pingPong on this object concurrently.
        // Infinite pingPong never completes, so onScaleComplete stays unwired —
        // LeanTween's setOnComplete never fired on pingPong loops either.
        TweenMeta meta = TweenUtil.GetMetaDefault(
            TweenLib.internalEasing, gameObject, scaleTime, 0f, false,
            TweenCoord.local, TweenEaseType.quadEaseInOut, TweenLoopType.pingPong);

        TweenUtil.ScaleToObject(meta, Vector3.one * scale);
    }

    void onScaleComplete() {

        float range = UnityEngine.Random.Range(scaleEaseMin, scaleEaseMax);

        scale = Mathf.Clamp(scale + range, scaleMin, scaleMax);
        scaleTime = .2f;

        AnimateScale();
    }

    // rotate

    void AnimateRotate() {

        TweenMeta meta = TweenUtil.GetMetaDefault(
            TweenLib.internalEasing, gameObject, rotateTime, 0f, false,
            TweenCoord.local, TweenEaseType.quadEaseInOut, TweenLoopType.pingPong);

        TweenUtil.RotateToObject(meta, Vector3.zero.WithZ(rotate));
    }

    void onRotateComplete() {
        float range = UnityEngine.Random.Range(-0.5f, .05f);

        rotate = Mathf.Clamp(rotate + range, .95f, 1.05f);
        scaleTime = .2f;

        AnimateRotate();
    }
}

