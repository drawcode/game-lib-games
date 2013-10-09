using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UIPanelBase : UIAppPanel {
	
	public GameObject panelLeftObject;
	public GameObject panelLeftBottomObject;
	public GameObject panelLeftTopObject;
	public GameObject panelRightObject;
	public GameObject panelRightBottomObject;
	public GameObject panelRightTopObject;
	public GameObject panelTopObject;
	public GameObject panelBottomObject;
	public GameObject panelCenterObject;
	
	public GameObject panelContainer;	

	[NonSerialized]
	public float durationShow = .45f;
	[NonSerialized]
	public float durationHide = .45f;
	[NonSerialized]
	public float durationDelayShow = .5f;
	[NonSerialized]
	public float durationDelayHide = 0f;
	[NonSerialized]
	public float leftOpenX = 0f;
	[NonSerialized]
	public float leftClosedX = -2500f;
	[NonSerialized]
	public float rightOpenX = 0f;
	[NonSerialized]
	public float rightClosedX = 2500f;
	[NonSerialized]
	public float bottomOpenY = 0f;
	[NonSerialized]
	public float bottomClosedY = -2500f;
	[NonSerialized]
	public float topOpenY = 0f;
	[NonSerialized]
	public float topClosedY = 2500f;
			
	public override void Start() {
		base.Start();		
	}
	
	public virtual void AnimateInCenter(float time, float delay) {
		if(panelCenterObject != null) {
        	UITweenerUtil.MoveTo(panelCenterObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(bottomOpenY));			
		}
	}
	
	public virtual void AnimateOutCenter(float time, float delay) {
		if(panelCenterObject != null) {
        	UITweenerUtil.MoveTo(panelCenterObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(bottomClosedY));			
		}
	}
	
	public virtual void AnimateInLeft(float time, float delay) {
		if(panelLeftObject != null) {
        	UITweenerUtil.MoveTo(panelLeftObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(0));			
		}
	}
	
	public virtual void AnimateOutLeft(float time, float delay) {
		if(panelLeftObject != null) {
        	UITweenerUtil.MoveTo(panelLeftObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(leftClosedX));			
		}
	}
	
	public virtual void AnimateInLeftBottom(float time, float delay) {
		if(panelLeftBottomObject != null) {
        	UITweenerUtil.MoveTo(panelLeftBottomObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(0));			
		}
	}
	
	public virtual void AnimateOutLeftBottom(float time, float delay) {
		if(panelLeftBottomObject != null) {
        	UITweenerUtil.MoveTo(panelLeftBottomObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(leftClosedX));			
		}
	}	
	
	public virtual void AnimateInLeftTop(float time, float delay) {
		if(panelLeftTopObject != null) {
        	UITweenerUtil.MoveTo(panelLeftTopObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(0));			
		}
	}
	
	public virtual void AnimateOutLeftTop(float time, float delay) {
		if(panelLeftTopObject != null) {
        	UITweenerUtil.MoveTo(panelLeftTopObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay,  Vector3.zero.WithX(leftClosedX));			
		}
	}
	
	public virtual void AnimateInRight(float time, float delay) {
		if(panelRightObject != null) {
        	UITweenerUtil.MoveTo(panelRightObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(0));			
		}
	}
	
	public virtual void AnimateOutRight(float time, float delay) {
		if(panelRightObject != null) {
        	UITweenerUtil.MoveTo(panelRightObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(rightClosedX));			
		}
	}	
	
	
	public virtual void AnimateInRightBottom(float time, float delay) {
		if(panelRightBottomObject != null) {
        	UITweenerUtil.MoveTo(panelRightBottomObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(0));			
		}
	}
	
	public virtual void AnimateOutRightBottom(float time, float delay) {
		if(panelRightBottomObject != null) {
        	UITweenerUtil.MoveTo(panelRightBottomObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(rightClosedX));			
		}
	}	
	
	public virtual void AnimateInRightTop(float time, float delay) {
		if(panelRightTopObject != null) {
        	UITweenerUtil.MoveTo(panelRightTopObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(0));			
		}
	}
	
	public virtual void AnimateOutRightTop(float time, float delay) {
		if(panelRightTopObject != null) {
        	UITweenerUtil.MoveTo(panelRightTopObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(rightClosedX));			
		}
	}	
	
	public virtual void AnimateInTop(float time, float delay) {
		if(panelTopObject != null) {
        	UITweenerUtil.MoveTo(panelTopObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(0));			
		}
	}
	
	public virtual void AnimateOutTop(float time, float delay) {
		if(panelTopObject != null) {
        	UITweenerUtil.MoveTo(panelTopObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(topClosedY));			
		}
	}
	
	public virtual void AnimateInBottom(float time, float delay) {
		if(panelBottomObject != null) {
        	UITweenerUtil.MoveTo(panelBottomObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(0));			
		}
	}
	
	public virtual void AnimateOutBottom(float time, float delay) {
		if(panelBottomObject != null) {
        	UITweenerUtil.MoveTo(panelBottomObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(bottomClosedY));			
		}
	}
	
	public virtual void AnimateIn() {
		
		//AnimateOut(0f, 0f);
		
		ShowPanel();
		
		float time = durationShow;
		float delay = durationDelayShow;
		
		AnimateIn(time, delay);
	}
	
	public virtual void AnimateIn(float time, float delay) {
				
		AnimateInCenter(time, delay);
		AnimateInLeft(time, delay);
		AnimateInLeftBottom(time, delay);
		AnimateInLeftTop(time, delay);
		AnimateInRight(time, delay);
		AnimateInRightBottom(time, delay);
		AnimateInRightTop(time, delay);
		AnimateInTop(time, delay);
		AnimateInBottom(time, delay);
	}
	
	public virtual void AnimateOut() {
		
		float time = durationHide;
		float delay = durationDelayHide;
		
		AnimateOut(time, delay);
	}
	
	public virtual void AnimateOutNow() {
		
		float time = 0f;
		float delay = 0f;
		
		AnimateOut(time, delay);
	}
	
	public virtual void AnimateOut(float time, float delay) {
		
		AdNetworks.HideAd();
		
		AnimateOutCenter(time, delay);
		AnimateOutLeft(time, delay);
		AnimateOutLeftBottom(time, delay);
		AnimateOutLeftTop(time, delay);
		AnimateOutRight(time, delay);
		AnimateOutRightBottom(time, delay);
		AnimateOutRightTop(time, delay);
		AnimateOutTop(time, delay);
		AnimateOutBottom(time, delay);	

        isVisible = false;

        StartCoroutine(HidePanelCo(delay + .5f));
	}

    public IEnumerator HidePanelCo(float delay) {
        yield return new WaitForSeconds(delay);

        if(!isVisible) {
            HidePanel();
        }
    }
	
	public virtual void HidePanel() {

        if(!isVisible) {

    		if(panelContainer != null) {
            	panelContainer.Hide();
    		}

        }
	}
	
	public virtual void ShowPanel() {

        isVisible = true;

		if(panelContainer != null) {
        	panelContainer.Show();		
		}		
	}
	
	void Update() {
		
	}
}

