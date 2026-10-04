using System;
using System.Collections;

using UnityEngine;

#if !USE_EASING_NGUI
using Engine.Animation;
using Engine.Utility;
#endif

#if !USE_UI_NGUI_2_7 && !USE_UI_NGUI_3 && !USE_EASING_NGUI
// B12 (2026-10-03): the agnostic UITweenerUtil below keeps NGUI's signatures, which name
// UITweener.Method / UITweener.Style. With NGUI compiled in, those are NGUI's own nested enums
// (Assets/NGUI/Scripts/Tweening/UITweener.cs); this stand-in exists ONLY when no NGUI define is
// set, so the NGUI build never sees two UITweener types. Members and order mirror NGUI 2.6.3.
// Removing NGUI means removing the define AND Assets/NGUI together (what nongui_compile.py
// models): dropping only the define while the NGUI sources still compile would clash here.
public abstract class UITweener {

    public enum Method {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
        BounceIn,
        BounceOut,
    }

    public enum Style {
        Once,
        Loop,
        PingPong,
    }
}
#endif

// USE_EASING_NGUI (undefined in this project): the original NGUI UITweener-component API, untouched.
// Otherwise (B12, 2026-10-03): the same public calls run on the engine's agnostic tween backend
// (TweenUtil.backend = EasingTweenBackend over AnimationEasing), so a game calling UITweenerUtil.X
// gets a working tween without NGUI. Differences from the NGUI branch, by design:
//   - Return type: NGUI returned the TweenX component. With no component there, calls return the
//     Engine.Animation.ITweenTarget they drive (null when go is null). Every call site in the libs
//     ignores the return value, so source compatibility holds for them.
//   - ResetTween(TweenX) and Begin<T>() operate ON NGUI components and have no agnostic meaning;
//     they stay NGUI-branch only.
//   - Like NGUI's per-component tweeners, a call replaces only the same channel's in-flight tween
//     (a fade never kills a move) and never changes GameObject active state.
//   - duration <= 0 with no delay applies the end value immediately (NGUI's Sample(1f, true)).
public class UITweenerUtil {


#if USE_EASING_NGUI

    public static void CameraFade(float amount, float time) {
        //iTween.CameraFadeTo(amount, time);
    }

    public static void CameraColor(Color color) {

        //iTween.CameraTexture(color);//(amount, time);

    }

    public static void CameraColor(Texture2D texture2d) {

        //iTween.CameraFadeAdd(texture2d);
    }

    /*
    public static TweenColor ColorTo(GameObject go, UITweener.Method method, UITweener.Style style, 
        float duration, float delay, Color colorTo) {
        if(go == null) {
            return null;
        }       
        
        TweenColor comp = UITweenerUtil.Begin<TweenColor>(go, method, style, duration, delay);
        //comp.ResetToBeginning();
        comp.from = comp.color;
        comp.to = colorTo;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        comp.ResetToBeginning();
        comp.Play(true);
        return comp;
    }
    */

    public static void ColorToHandler<T>(
        GameObject go, Color colorTo, float duration, float delay) where T : Component {

        foreach (Transform t in go.transform) {

            if (go.Has<T>()) {
                string toLook = "-a-";
                int alphaMarker = t.name.IndexOf(toLook);

                if (alphaMarker > -1) {
                    string val = t.name.Substring(alphaMarker + toLook.Length);
                    if (!string.IsNullOrEmpty(val)) {
                        float valNumeric = 0f;
                        float.TryParse(val, out valNumeric);

                        if (valNumeric > 0f) {
                            valNumeric = valNumeric / 100f;
                            colorTo.a = valNumeric;
                        }
                    }
                }

                ColorTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 0f, 0f, colorTo);
            }

            ColorToHandler(t.gameObject, colorTo, duration, delay);
        }
    }

    public static void ColorToHandler(GameObject go, Color colorTo, float duration, float delay) {
        foreach (Transform t in go.transform) {
            string toLook = "-a-";
            int alphaMarker = t.name.IndexOf(toLook);

            if (alphaMarker > -1) {
                string val = t.name.Substring(alphaMarker + toLook.Length);
                if (!string.IsNullOrEmpty(val)) {
                    float valNumeric = 0f;
                    float.TryParse(val, out valNumeric);

                    if (valNumeric > 0f) {
                        valNumeric = valNumeric / 100f;

                        colorTo.a = valNumeric;
                    }
                }
            }

            ColorTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 0f, 0f, colorTo);

            ColorToHandler(t.gameObject, colorTo, duration, delay);
        }
    }

    //

    public static TweenPosition ResetTween(TweenPosition twn) {

#if USE_UI_NGUI_3
        twn.ResetToBeginning();
#else
        twn.Reset();
#endif

        return twn;
    }

    public static TweenAlpha ResetTween(TweenAlpha twn) {

#if USE_UI_NGUI_3
        twn.ResetToBeginning();
#else
        twn.Reset();
#endif

        return twn;
    }

    public static TweenRotation ResetTween(TweenRotation twn) {

#if USE_UI_NGUI_3
        twn.ResetToBeginning();
#else
        twn.Reset();
#endif

        return twn;
    }

    public static TweenScale ResetTween(TweenScale twn) {

#if USE_UI_NGUI_3
        twn.ResetToBeginning();
#else
        twn.Reset();
#endif

        return twn;
    }

    public static TweenColor ResetTween(TweenColor twn) {

#if USE_UI_NGUI_3
        twn.ResetToBeginning();
#else
        twn.Reset();
#endif

        return twn;
    }

    //

    public static TweenColor ColorTo(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Color colorTo) {
        if (go == null) {
            return null;
        }

        return ColorTo(false, go, method, style, duration, delay, colorTo);
    }

    public static TweenColor ColorTo(bool reset, GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Color colorTo) {
        if (go == null) {
            return null;
        }

        if (reset) {
            //go.RemoveComponent<TweenColor>();
        }

        TweenColor comp = UITweenerUtil.Begin<TweenColor>(go, method, style, duration, delay);
        if (reset) {
            comp = ResetTween(comp);
        }
        comp.from = comp.color;
        comp.to = colorTo;
        comp.duration = duration;
        comp.delay = delay;

        if (duration <= 0f) {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        if (!reset) {
            comp = ResetTween(comp);
        }
        comp.Play(true);
        return comp;
    }

    public static TweenRotation RotateTo(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Vector3 rotateFrom, Vector3 rotateTo) {
        if (go == null) {
            return null;
        }

        //go.RemoveComponent<TweenPosition>();

        TweenRotation comp = UITweenerUtil.Begin<TweenRotation>(go, method, style, duration, delay);
        //comp.ResetToBeginning();
        comp.from = rotateFrom;
        comp.to = rotateTo;
        comp.duration = duration;
        comp.delay = delay;

        if (duration <= 0f) {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        comp = ResetTween(comp);
        comp.Play(true);
        return comp;
    }

    public static TweenRotation RotateTo(
            GameObject go, UITweener.Method method, UITweener.Style style,
            float duration, float delay, Vector3 rotateTo) {

        if (go == null) {
            return null;
        }

        return RotateTo(go, method, style, duration, delay, go.transform.rotation.eulerAngles, rotateTo);
    }

    public static TweenPosition MoveTo(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Vector3 posFrom, Vector3 posTo) {
        if (go == null) {
            return null;
        }

        //go.RemoveComponent<TweenPosition>();

        TweenPosition comp = UITweenerUtil.Begin<TweenPosition>(go, method, style, duration, delay);
        //comp.ResetToBeginning();
        comp.from = posFrom;
        comp.to = posTo;
        comp.duration = duration;
        comp.delay = delay;

        if (duration <= 0f) {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        comp = ResetTween(comp);
        comp.Play(true);
        return comp;
    }

    public static TweenPosition MoveTo(GameObject go,
                                       float duration, float delay, Vector3 pos) {
        return MoveTo(go, UITweener.Method.EaseIn, UITweener.Style.Once, duration, delay, pos);
    }

    public static TweenPosition MoveTo(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Vector3 pos) {
        if (go == null) {
            return null;
        }

        return MoveTo(false, go, method, style, duration, delay, pos);
    }

    public static TweenPosition MoveTo(bool reset, GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Vector3 pos) {
        if (go == null) {
            return null;
        }

        if (reset) {
            //go.RemoveComponent<TweenPosition>();
        }

        TweenPosition comp = UITweenerUtil.Begin<TweenPosition>(go, method, style, duration, delay);
        if (reset) {
            comp = ResetTween(comp);
        }
        comp.from = comp.position;
        comp.to = pos;
        comp.duration = duration;
        comp.delay = delay;

        if (duration <= 0f) {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        if (!reset) {
            comp = ResetTween(comp);
        }
        comp.Play(true);
        return comp;
    }

    public static TweenAlpha FadeIn(
        GameObject go,
        float duration = 1f, float delay = 1f) {

        return FadeTo(go,
                      UITweener.Method.EaseIn,
                      UITweener.Style.Once, duration, delay, 1);
    }

    public static TweenAlpha FadeOut(
        GameObject go,
        float duration = 1f, float delay = 0f) {

        return FadeTo(go,
                      UITweener.Method.EaseIn,
                      UITweener.Style.Once, duration, delay, 0f);
    }

    public static TweenAlpha FadeOutNow(
        GameObject go) {

        return FadeTo(go,
                      UITweener.Method.EaseIn,
                      UITweener.Style.Once, 0f, 0f, 0f);
    }

    public static TweenAlpha FadeTo(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, float alpha) {
        if (go == null) {
            return null;
        }

        return FadeTo(false, go, method, style, duration, delay, alpha);
    }

    public static TweenAlpha FadeTo(bool reset, GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, float alpha) {
        if (go == null) {
            return null;
        }

        if (reset) {
            //go.RemoveComponent<TweenAlpha>();
        }

        iTween.Stop(go);

        TweenAlpha comp = UITweenerUtil.Begin<TweenAlpha>(go, method, style, duration, delay);
        if (reset) {
            comp = ResetTween(comp);
        }
        comp.from = comp.alpha;
        comp.to = alpha;
        comp.duration = duration;
        comp.delay = delay;

        if (duration <= 0f) {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        if (!reset) {
            comp = ResetTween(comp);
        }
        comp.Play(true);

        FadeInHandler(go, duration, delay);

        return comp;
    }

    public static void FadeInHandler(GameObject go, float duration, float delay) {
        foreach (Transform t in go.transform) {
            string toLook = "-a-";
            int alphaMarker = t.name.IndexOf(toLook);
            //string alphaObject = t.name;
            if (alphaMarker > -1) {
                // Fade it immediately
                FadeTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 0f, 0f, 0f);
                // Fade to the correct value after initial fade in
                string val = t.name.Substring(alphaMarker + toLook.Length);
                if (!string.IsNullOrEmpty(val)) {
                    float valNumeric = 0f;
                    float.TryParse(val, out valNumeric);

                    if (valNumeric > 0f) {
                        valNumeric = valNumeric / 100f;

                        float currentDuration = duration + .05f;
                        float currentDelay = duration + delay;

                        FadeTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once,
                           currentDuration, currentDelay, valNumeric);
                    }
                }
            }
            FadeInHandler(t.gameObject, duration, delay);
        }
    }

    public static TweenAlpha FadeTo(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, float alphaFrom, float alphaTo) {

        if (go == null) {
            return null;
        }

        //go.RemoveComponent<TweenAlpha>();

        TweenAlpha comp = UITweenerUtil.Begin<TweenAlpha>(go, method, style, duration, delay);
        //comp.ResetToBeginning();
        comp.from = alphaFrom;
        comp.to = alphaTo;
        comp.duration = duration;
        comp.delay = delay;

        if (duration <= 0f) {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        comp = ResetTween(comp);
        comp.Play(true);
        return comp;
    }

    public static T Begin<T>(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay) where T : UITweener {

        if (go == null) {
            return default(T);
        }

        T comp = go.GetComponent<T>();
#if UNITY_FLASH
        if ((object)comp == null) comp = (T)go.AddComponent<T>();
#else
        if (comp == null)
            comp = go.AddComponent<T>();
#endif
        comp.delay = delay;
        comp.duration = duration;
        comp.method = method;
        comp.style = style;
        comp.eventReceiver = null;
        comp.callWhenFinished = null;
        comp.onFinished = null;
        comp.enabled = true;
        return comp;
    }
#else
    public static void CameraFade(float amount, float time) {
        //iTween.CameraFadeTo(amount, time);
    }

    public static void CameraColor(Color color) {

        //iTween.CameraTexture(color);//(amount, time);

    }

    public static void CameraColor(Texture2D texture2d) {

        //iTween.CameraFadeAdd(texture2d);
    }

    // ------------------------------------------------------------------------
    // Mapping NGUI's curve/loop names onto the agnostic presets.

    // NGUI 2.6.3 UITweener.Sample: EaseIn = 1 - sin(pi/2 (1 - t)) and EaseOut = sin(pi/2 t), which
    // ARE Penner's sine in/out. EaseInOut = t - sin(2 pi t) / 2 pi has zero slope at both ends and
    // slope 2 at the middle, the same profile as quad in/out. TweenEaseType exposes no bounce, so
    // the two bounce curves take the nearest exposed shape: NGUI's BounceIn settles at the END
    // (backEaseOut overshoots and settles there); its BounceOut bounces at the START and then
    // accelerates in (quadEaseIn).
    public static TweenEaseType ToEaseType(UITweener.Method method) {
        switch (method) {
            case UITweener.Method.EaseIn: return TweenEaseType.sineEaseIn;
            case UITweener.Method.EaseOut: return TweenEaseType.sineEaseOut;
            case UITweener.Method.EaseInOut: return TweenEaseType.quadEaseInOut;
            case UITweener.Method.BounceIn: return TweenEaseType.backEaseOut;
            case UITweener.Method.BounceOut: return TweenEaseType.quadEaseIn;
            default: return TweenEaseType.linear;
        }
    }

    public static TweenLoopType ToLoopType(UITweener.Style style) {
        switch (style) {
            case UITweener.Style.Loop: return TweenLoopType.loop;
            case UITweener.Style.PingPong: return TweenLoopType.pingPong;
            default: return TweenLoopType.once;
        }
    }

    // NGUI's TweenPosition/TweenRotation animate LOCAL transforms; so does this.
    static TweenMeta BuildMeta(object native, UITweener.Method method, UITweener.Style style,
        float duration, float delay) {

        TweenMeta meta = TweenUtil.GetMetaDefault(
            TweenLib.internalEasing,
            native as GameObject,
            Mathf.Max(0f, duration), Mathf.Max(0f, delay),
            true,
            TweenCoord.local,
            ToEaseType(method),
            ToLoopType(style));

        return meta;
    }

    static bool IsImmediate(UITweener.Style style, float duration, float delay) {
        return duration <= 0f && delay <= 0f && style == UITweener.Style.Once;
    }

    // ------------------------------------------------------------------------
    // Shared drivers: one per channel, over any ITweenTarget (Transform or VisualElement).

    static ITweenTarget DriveAlpha(ITweenTarget t, UITweener.Method method, UITweener.Style style,
        float duration, float delay, float alpha) {

        if (t == null) {
            return null;
        }

        if (IsImmediate(style, duration, delay)) {
            TweenUtil.backend.Cancel(t, TweenChannel.alpha);
            t.SetAlpha(alpha);
            return t;
        }

        TweenUtil.backend.Fade(t, alpha, BuildMeta(t.native, method, style, duration, delay));
        return t;
    }

    static ITweenTarget DrivePosition(ITweenTarget t, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Vector3 pos) {

        if (t == null) {
            return null;
        }

        if (IsImmediate(style, duration, delay)) {
            TweenUtil.backend.Cancel(t, TweenChannel.position);
            t.SetPosition(pos, TweenCoord.local);
            return t;
        }

        TweenUtil.backend.Move(t, pos, BuildMeta(t.native, method, style, duration, delay));
        return t;
    }

    static ITweenTarget DriveRotation(ITweenTarget t, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Vector3 rotateTo) {

        if (t == null) {
            return null;
        }

        if (IsImmediate(style, duration, delay)) {
            TweenUtil.backend.Cancel(t, TweenChannel.rotation);
            t.SetRotation(rotateTo, TweenCoord.local);
            return t;
        }

        TweenUtil.backend.Rotate(t, rotateTo, BuildMeta(t.native, method, style, duration, delay));
        return t;
    }

    static ITweenTarget DriveColor(ITweenTarget t, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Color colorTo) {

        if (t == null) {
            return null;
        }

        if (IsImmediate(style, duration, delay)) {
            TweenUtil.backend.Cancel(t, TweenChannel.color);
            t.SetColor(colorTo);
            return t;
        }

        TweenUtil.backend.ColorTo(t, colorTo, BuildMeta(t.native, method, style, duration, delay));
        return t;
    }

    // ------------------------------------------------------------------------
    // COLOR

    public static void ColorToHandler<T>(
        GameObject go, Color colorTo, float duration, float delay) where T : Component {

        if (go == null) {
            return;
        }

        foreach (Transform t in go.transform) {

            if (go.Has<T>()) {
                string toLook = "-a-";
                int alphaMarker = t.name.IndexOf(toLook);

                if (alphaMarker > -1) {
                    string val = t.name.Substring(alphaMarker + toLook.Length);
                    if (!string.IsNullOrEmpty(val)) {
                        float valNumeric = 0f;
                        float.TryParse(val, out valNumeric);

                        if (valNumeric > 0f) {
                            valNumeric = valNumeric / 100f;
                            colorTo.a = valNumeric;
                        }
                    }
                }

                ColorTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 0f, 0f, colorTo);
            }

            ColorToHandler<T>(t.gameObject, colorTo, duration, delay);
        }
    }

    public static void ColorToHandler(GameObject go, Color colorTo, float duration, float delay) {

        if (go == null) {
            return;
        }

        foreach (Transform t in go.transform) {
            string toLook = "-a-";
            int alphaMarker = t.name.IndexOf(toLook);

            if (alphaMarker > -1) {
                string val = t.name.Substring(alphaMarker + toLook.Length);
                if (!string.IsNullOrEmpty(val)) {
                    float valNumeric = 0f;
                    float.TryParse(val, out valNumeric);

                    if (valNumeric > 0f) {
                        valNumeric = valNumeric / 100f;

                        colorTo.a = valNumeric;
                    }
                }
            }

            ColorTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 0f, 0f, colorTo);

            ColorToHandler(t.gameObject, colorTo, duration, delay);
        }
    }

    public static ITweenTarget ColorTo(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Color colorTo) {
        if (go == null) {
            return null;
        }

        return ColorTo(false, go, method, style, duration, delay, colorTo);
    }

    // reset: NGUI restarted the existing component from its start; every call here starts a fresh
    // tween from the current value, so reset has nothing left to do.
    public static ITweenTarget ColorTo(bool reset, GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Color colorTo) {
        if (go == null) {
            return null;
        }

        return DriveColor(TweenUtil.ResolveTarget(go), method, style, duration, delay, colorTo);
    }

    // ------------------------------------------------------------------------
    // ROTATE

    public static ITweenTarget RotateTo(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Vector3 rotateFrom, Vector3 rotateTo) {
        if (go == null) {
            return null;
        }

        ITweenTarget t = TweenUtil.ResolveTarget(go);
        TweenUtil.backend.Cancel(t, TweenChannel.rotation);
        t.SetRotation(rotateFrom, TweenCoord.local);

        return DriveRotation(t, method, style, duration, delay, rotateTo);
    }

    public static ITweenTarget RotateTo(
            GameObject go, UITweener.Method method, UITweener.Style style,
            float duration, float delay, Vector3 rotateTo) {

        if (go == null) {
            return null;
        }

        return DriveRotation(TweenUtil.ResolveTarget(go), method, style, duration, delay, rotateTo);
    }

    // ------------------------------------------------------------------------
    // MOVE

    public static ITweenTarget MoveTo(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Vector3 posFrom, Vector3 posTo) {
        if (go == null) {
            return null;
        }

        ITweenTarget t = TweenUtil.ResolveTarget(go);
        TweenUtil.backend.Cancel(t, TweenChannel.position);
        t.SetPosition(posFrom, TweenCoord.local);

        return DrivePosition(t, method, style, duration, delay, posTo);
    }

    public static ITweenTarget MoveTo(GameObject go,
                                       float duration, float delay, Vector3 pos) {
        return MoveTo(go, UITweener.Method.EaseIn, UITweener.Style.Once, duration, delay, pos);
    }

    public static ITweenTarget MoveTo(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Vector3 pos) {
        if (go == null) {
            return null;
        }

        return MoveTo(false, go, method, style, duration, delay, pos);
    }

    public static ITweenTarget MoveTo(bool reset, GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Vector3 pos) {
        if (go == null) {
            return null;
        }

        return DrivePosition(TweenUtil.ResolveTarget(go), method, style, duration, delay, pos);
    }

    // ------------------------------------------------------------------------
    // FADE

    public static ITweenTarget FadeIn(
        GameObject go,
        float duration = 1f, float delay = 1f) {

        return FadeTo(go,
                      UITweener.Method.EaseIn,
                      UITweener.Style.Once, duration, delay, 1);
    }

    public static ITweenTarget FadeOut(
        GameObject go,
        float duration = 1f, float delay = 0f) {

        return FadeTo(go,
                      UITweener.Method.EaseIn,
                      UITweener.Style.Once, duration, delay, 0f);
    }

    public static ITweenTarget FadeOutNow(
        GameObject go) {

        return FadeTo(go,
                      UITweener.Method.EaseIn,
                      UITweener.Style.Once, 0f, 0f, 0f);
    }

    public static ITweenTarget FadeTo(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, float alpha) {
        if (go == null) {
            return null;
        }

        return FadeTo(false, go, method, style, duration, delay, alpha);
    }

    // The NGUI branch stopped every iTween on the object first (iTween.Stop); iTween is gone, and
    // the agnostic backend replaces only the alpha channel, matching NGUI's per-component tweeners.
    // FadeInHandler keeps the "-a-NN" child convention exactly as the NGUI branch ran it.
    public static ITweenTarget FadeTo(bool reset, GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, float alpha) {
        if (go == null) {
            return null;
        }

        ITweenTarget t = DriveAlpha(TweenUtil.ResolveTarget(go), method, style, duration, delay, alpha);

        FadeInHandler(go, duration, delay);

        return t;
    }

    public static void FadeInHandler(GameObject go, float duration, float delay) {

        if (go == null) {
            return;
        }

        foreach (Transform t in go.transform) {
            string toLook = "-a-";
            int alphaMarker = t.name.IndexOf(toLook);
            //string alphaObject = t.name;
            if (alphaMarker > -1) {
                // Fade it immediately
                FadeTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 0f, 0f, 0f);
                // Fade to the correct value after initial fade in
                string val = t.name.Substring(alphaMarker + toLook.Length);
                if (!string.IsNullOrEmpty(val)) {
                    float valNumeric = 0f;
                    float.TryParse(val, out valNumeric);

                    if (valNumeric > 0f) {
                        valNumeric = valNumeric / 100f;

                        float currentDuration = duration + .05f;
                        float currentDelay = duration + delay;

                        FadeTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once,
                           currentDuration, currentDelay, valNumeric);
                    }
                }
            }
            FadeInHandler(t.gameObject, duration, delay);
        }
    }

    public static ITweenTarget FadeTo(GameObject go, UITweener.Method method, UITweener.Style style,
        float duration, float delay, float alphaFrom, float alphaTo) {

        if (go == null) {
            return null;
        }

        ITweenTarget t = TweenUtil.ResolveTarget(go);
        TweenUtil.backend.Cancel(t, TweenChannel.alpha);
        t.SetAlpha(alphaFrom);

        return DriveAlpha(t, method, style, duration, delay, alphaTo);
    }
#endif

#if !USE_EASING_NGUI
    // ------------------------------------------------------------------------
    // UIRef overloads (B12, additive): the same calls on a backend-agnostic handle, so a toolkit
    // element (VisualElement) or a UGUI/NGUI GameObject behind a UIRef tweens through one API.
    // A dead or unresolvable ref returns null and does nothing.

    static ITweenTarget ResolveRef(Engine.UI.UIRef r) {
        if (r == null || !r.alive) {
            return null;
        }
        return TweenUtil.ResolveTarget(r.native);
    }

    public static ITweenTarget FadeTo(Engine.UI.UIRef r, UITweener.Method method, UITweener.Style style,
        float duration, float delay, float alpha) {
        return DriveAlpha(ResolveRef(r), method, style, duration, delay, alpha);
    }

    public static ITweenTarget FadeTo(Engine.UI.UIRef r, UITweener.Method method, UITweener.Style style,
        float duration, float delay, float alphaFrom, float alphaTo) {

        ITweenTarget t = ResolveRef(r);

        if (t == null) {
            return null;
        }

        TweenUtil.backend.Cancel(t, TweenChannel.alpha);
        t.SetAlpha(alphaFrom);

        return DriveAlpha(t, method, style, duration, delay, alphaTo);
    }

    public static ITweenTarget MoveTo(Engine.UI.UIRef r, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Vector3 pos) {
        return DrivePosition(ResolveRef(r), method, style, duration, delay, pos);
    }

    public static ITweenTarget RotateTo(Engine.UI.UIRef r, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Vector3 rotateTo) {
        return DriveRotation(ResolveRef(r), method, style, duration, delay, rotateTo);
    }

    public static ITweenTarget ColorTo(Engine.UI.UIRef r, UITweener.Method method, UITweener.Style style,
        float duration, float delay, Color colorTo) {
        return DriveColor(ResolveRef(r), method, style, duration, delay, colorTo);
    }
#endif
}
