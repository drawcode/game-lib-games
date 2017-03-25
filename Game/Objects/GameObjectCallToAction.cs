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

        LeanTween.scale(gameObject, Vector3.one * scale, scaleTime)
            .setEase(LeanTweenType.easeInOutQuad)
            .setLoopPingPong()
            .setOnComplete(onScaleComplete);
    }

    void onScaleComplete() {

        float range = UnityEngine.Random.Range(scaleEaseMin, scaleEaseMax);

        scale = Mathf.Clamp(scale + range, scaleMin, scaleMax);
        scaleTime = .2f;

        AnimateScale();
    }

    // rotate

    void AnimateRotate() {
        
        LeanTween.rotateLocal(gameObject, Vector3.zero.WithZ(rotate), rotateTime)
            .setEase(LeanTweenType.easeInOutQuad)
            .setLoopPingPong()
            .setOnComplete(onRotateComplete);
    }

    void onRotateComplete() {
        float range = UnityEngine.Random.Range(-0.5f, .05f);

        rotate = Mathf.Clamp(rotate + range, .95f, 1.05f);
        scaleTime = .2f;

        AnimateRotate();
    }
}

