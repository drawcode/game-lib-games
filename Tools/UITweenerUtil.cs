using System;
using System.Collections;

using UnityEngine;

public class UITweenerUtil
{
	
	public static void CameraFade(float amount, float time) {
		iTween.CameraFadeTo(amount, time);
	}
	
	public static void CameraColor(Color color) {
			
		iTween.CameraTexture(color);//(amount, time);

	}
	
	public static void CameraColor(Texture2D texture2d) {
					
		iTween.CameraFadeAdd(texture2d);
	}
	
	/*
	public static TweenColor ColorTo(GameObject go, UITweener.Method method, UITweener.Style style, 
		float duration, float delay, Color colorTo) {
		if(go == null) {
			return null;
		}		
		
		TweenColor comp = UITweenerUtil.Begin<TweenColor>(go, method, style, duration, delay);
		//comp.Reset();
		comp.from = comp.color;
		comp.to = colorTo;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		comp.Reset();
		comp.Play(true);
		return comp;
	}
	*/
	
	
	public static TweenColor ColorTo(GameObject go, UITweener.Method method, UITweener.Style style, 
		float duration, float delay, Color colorTo) {
		if(go == null) {
			return null;
		}		
		
		return ColorTo(false, go, method, style, duration, delay, colorTo);
	}
	
	public static TweenColor ColorTo(bool reset, GameObject go, UITweener.Method method, UITweener.Style style, 
		float duration, float delay, Color colorTo) {
		if(go == null) {
			return null;
		}		
		
		if(reset) {
			//go.RemoveComponent<TweenColor>();
		}
		
		TweenColor comp = UITweenerUtil.Begin<TweenColor>(go, method, style, duration, delay);
		if(reset) {
			comp.Reset();
		}
		comp.from = comp.color;
		comp.to = colorTo;
		comp.duration = duration;
		comp.delay = delay;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		if(!reset) {
			comp.Reset();
		}
		comp.Play(true);
		return comp;
	}
	
	
	public static TweenRotation RotateTo(GameObject go, UITweener.Method method, UITweener.Style style, 
		float duration, float delay, Vector3 rotateFrom, Vector3 rotateTo) {
		if(go == null) {
			return null;
		}		
		
		//go.RemoveComponent<TweenPosition>();
		
		TweenRotation comp = UITweenerUtil.Begin<TweenRotation>(go, method, style, duration, delay);
		//comp.Reset();
		comp.from = rotateFrom;
		comp.to = rotateTo;
		comp.duration = duration;
		comp.delay = delay;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		comp.Reset();
		comp.Play(true);
		return comp;
	}
	
	
	public static TweenRotation RotateTo(GameObject go, UITweener.Method method, UITweener.Style style, 
		float duration, float delay, Vector3 rotateTo) {
		if(go == null) {
			return null;
		}		
		
		return RotateTo(go, method, style, duration, delay, go.transform.rotation.eulerAngles, rotateTo);
	}
	
	public static TweenPosition MoveTo(GameObject go, UITweener.Method method, UITweener.Style style, 
		float duration, float delay, Vector3 posFrom, Vector3 posTo) {
		if(go == null) {
			return null;
		}		
		
		//go.RemoveComponent<TweenPosition>();
		
		TweenPosition comp = UITweenerUtil.Begin<TweenPosition>(go, method, style, duration, delay);
		//comp.Reset();
		comp.from = posFrom;
		comp.to = posTo;
		comp.duration = duration;
		comp.delay = delay;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		comp.Reset();
		comp.Play(true);
		return comp;
	}
	
	public static TweenPosition MoveTo(GameObject go, UITweener.Method method, UITweener.Style style, 
		float duration, float delay, Vector3 pos) {
		if(go == null) {
			return null;
		}		
		
		return MoveTo(false, go, method, style, duration, delay, pos);
	}
	
	public static TweenPosition MoveTo(bool reset, GameObject go, UITweener.Method method, UITweener.Style style, 
		float duration, float delay, Vector3 pos) {
		if(go == null) {
			return null;
		}		
		
		if(reset) {
			//go.RemoveComponent<TweenPosition>();
		}
		
		TweenPosition comp = UITweenerUtil.Begin<TweenPosition>(go, method, style, duration, delay);
		if(reset) {
			comp.Reset();
		}
		comp.from = comp.position;
		comp.to = pos;
		comp.duration = duration;
		comp.delay = delay;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		if(!reset) {
			comp.Reset();
		}
		comp.Play(true);
		return comp;
	}
	
	public static TweenAlpha FadeTo(GameObject go, UITweener.Method method, UITweener.Style style, 
		float duration, float delay, float alpha) {
		if(go == null) {
			return null;
		}		
		
		return FadeTo(false, go, method, style, duration, delay, alpha);
	}
	
	public static TweenAlpha FadeTo(bool reset, GameObject go, UITweener.Method method, UITweener.Style style, 
		float duration, float delay, float alpha) {
		if(go == null) {
			return null;
		}		
		
		if(reset) {
			//go.RemoveComponent<TweenAlpha>();
		}
		
		TweenAlpha comp = UITweenerUtil.Begin<TweenAlpha>(go, method, style, duration, delay);
		if(reset) {
			comp.Reset();
		}
		comp.from = comp.alpha;
		comp.to = alpha;
		comp.duration = duration;
		comp.delay = delay;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		if(!reset) {
			comp.Reset();
		}
		comp.Play(true);
		
		FadeInHandler(go, duration, delay);
		
		return comp;
	}
	
	public static void FadeInHandler(GameObject go, float duration, float delay) {
		foreach(Transform t in go.transform) {
			string toLook = "-a-";
			int alphaMarker = t.name.IndexOf(toLook);
			//string alphaObject = t.name;
			if(alphaMarker > -1) {
				// Fade it immediately
				FadeTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 0f, 0f, 0f);
				// Fade to the correct value after initial fade in
				string val = t.name.Substring(alphaMarker + toLook.Length);
				if(!string.IsNullOrEmpty(val)) {
					float valNumeric = 0f;					
					float.TryParse(val, out valNumeric);
					
					if(valNumeric > 0f) {
						valNumeric = valNumeric/100f;
						
						FadeTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 
							duration + .05f, duration + delay, valNumeric);
					}
				}
			}
			FadeInHandler(t.gameObject, duration, delay);
		}
	}
	
	public static TweenAlpha FadeTo(GameObject go, UITweener.Method method, UITweener.Style style, 
		float duration, float delay, float alphaFrom, float alphaTo) {
		if(go == null) {
			return null;
		}	
		
		//go.RemoveComponent<TweenAlpha>();
		
		TweenAlpha comp = UITweenerUtil.Begin<TweenAlpha>(go, method, style, duration, delay);
		//comp.Reset();
		comp.from = alphaFrom;
		comp.to = alphaTo;
		comp.duration = duration;
		comp.delay = delay;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		comp.Reset();
		comp.Play(true);
		return comp;
	}
	
	public static T Begin<T> (GameObject go, UITweener.Method method, UITweener.Style style, 
		float duration, float delay) where T : UITweener {
		if(go == null) {
			return default(T);
		}	
		
		
		T comp = go.GetComponent<T>();
#if UNITY_FLASH
		if ((object)comp == null) comp = (T)go.AddComponent<T>();
#else
		if (comp == null) comp = go.AddComponent<T>();
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

}
