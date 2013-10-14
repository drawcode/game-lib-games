using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

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

    public virtual void OnEnable() {
        Messenger<string>.AddListener(UIControllerMessages.uiPanelAnimateIn, OnUIControllerPanelAnimateIn);
        Messenger<string>.AddListener(UIControllerMessages.uiPanelAnimateOut, OnUIControllerPanelAnimateOut);
        Messenger<string, string>.AddListener(UIControllerMessages.uiPanelAnimateType, OnUIControllerPanelAnimateType);
    }

    public virtual void OnDisable() {
        Messenger<string>.RemoveListener(UIControllerMessages.uiPanelAnimateIn, OnUIControllerPanelAnimateIn);
        Messenger<string>.RemoveListener(UIControllerMessages.uiPanelAnimateOut, OnUIControllerPanelAnimateOut);
        Messenger<string, string>.RemoveListener(UIControllerMessages.uiPanelAnimateType, OnUIControllerPanelAnimateType);
    }

    public virtual void OnUIControllerPanelAnimateIn(string classNameTo) {
        if(className == classNameTo) {
            AnimateIn();
        }
    }

    public virtual void OnUIControllerPanelAnimateOut(string classNameTo) {
        if(className == classNameTo) {
            AnimateOut();
        }
    }

    public virtual void OnUIControllerPanelAnimateType(string classNameTo, string code) {
        if(className == classNameTo) {
           //
        }
    }

	public override void Start() {
		base.Start();		
	}

    // CENTER

    public virtual void AnimateInCenter(GameObject go) {
        AnimateInCenter(go, durationShow, durationDelayShow);
    }

    public virtual void AnimateOutCenter(GameObject go) {
        AnimateOutCenter(go, durationHide, durationDelayHide);
    }
	
    public virtual void AnimateInCenter(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(bottomOpenY));
        }
    }

    public virtual void AnimateInCenter(float time, float delay) {
        AnimateInCenter(panelCenterObject, time, delay);
    }

    public virtual void AnimateOutCenter(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(bottomClosedY));
        }
    }

    public virtual void AnimateOutCenter(float time, float delay) {
        AnimateOutCenter(panelCenterObject, time, delay);
    }

    // LEFT

    public virtual void AnimateInLeft(GameObject go) {
        AnimateInLeft(go, durationShow, durationDelayShow);
    }

    public virtual void AnimateOutLeft(GameObject go) {
        AnimateOutLeft(go, durationHide, durationDelayHide);
    }
	
    public virtual void AnimateInLeft(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(0));
        }
    }

    public virtual void AnimateInLeft(float time, float delay) {
        AnimateInLeft(panelLeftObject, time, delay);
    }

    public virtual void AnimateOutLeft(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(leftClosedX));
        }
    }

    public virtual void AnimateOutLeft(float time, float delay) {
        AnimateOutLeft(panelLeftObject, time, delay);
    }

    // LEFT BOTTOM

    public virtual void AnimateInLeftBottom(GameObject go) {
        AnimateInLeftBottom(go, durationShow, durationDelayShow);
    }

    public virtual void AnimateOutLeftBottom(GameObject go) {
        AnimateOutLeftBottom(go, durationHide, durationDelayHide);
    }
	
    public virtual void AnimateInLeftBottom(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(0));
        }
    }

    public virtual void AnimateInLeftBottom(float time, float delay) {
        AnimateInLeftBottom(panelLeftBottomObject, time, delay);
    }

    public virtual void AnimateOutLeftBottom(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(leftClosedX));
        }
    }

    public virtual void AnimateOutLeftBottom(float time, float delay) {
        AnimateOutLeftBottom(panelLeftBottomObject, time, delay);
    }

    // LEFT TOP

    public virtual void AnimateInLeftTop(GameObject go) {
        AnimateInLeftTop(go, durationShow, durationDelayShow);
    }

    public virtual void AnimateOutLeftTop(GameObject go) {
        AnimateOutLeftTop(go, durationHide, durationDelayHide);
    }
	
    public virtual void AnimateInLeftTop(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(0));
        }
    }

    public virtual void AnimateInLeftTop(float time, float delay) {
        AnimateInLeftTop(panelLeftTopObject, time, delay);
    }
    
    public virtual void AnimateOutLeftTop(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay,  Vector3.zero.WithX(leftClosedX));
        }
    }

    public virtual void AnimateOutLeftTop(float time, float delay) {
        AnimateOutLeftTop(panelLeftTopObject, time, delay);
    }

    // RIGHT

    public virtual void AnimateInRight(GameObject go) {
        AnimateInRight(go, durationShow, durationDelayShow);
    }

    public virtual void AnimateOutRight(GameObject go) {
        AnimateOutRight(go, durationHide, durationDelayHide);
    }
	
    public virtual void AnimateInRight(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(0));
        }
    }

    public virtual void AnimateInRight(float time, float delay) {
        AnimateInRight(panelRightObject, time, delay);
    }
    
    public virtual void AnimateOutRight(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(rightClosedX));
        }
    }

    public virtual void AnimateOutRight(float time, float delay) {
        AnimateOutRight(panelRightObject, time, delay);
    }

    // BOTTOM RIGHT

    public virtual void AnimateInRightBottom(GameObject go) {
        AnimateInRightBottom(go, durationShow, durationDelayShow);
    }

    public virtual void AnimateOutRightBottom(GameObject go) {
        AnimateOutRightBottom(go, durationHide, durationDelayHide);
    }
	
    public virtual void AnimateInRightBottom(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(0));
        }
    }

    public virtual void AnimateInRightBottom(float time, float delay) {
        AnimateInRightBottom(panelRightBottomObject, time, delay);
    }
    
    public virtual void AnimateOutRightBottom(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(rightClosedX));
        }
    }

    public virtual void AnimateOutRightBottom(float time, float delay) {
        AnimateOutRightBottom(panelRightBottomObject, time, delay);
    }

    // TOP RIGHT

    public virtual void AnimateInRightTop(GameObject go) {
        AnimateInRightTop(go, durationShow, durationDelayShow);
    }

    public virtual void AnimateOutRightTop(GameObject go) {
        AnimateOutRightTop(go, durationHide, durationDelayHide);
    }
	
    public virtual void AnimateInRightTop(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(0));
        }
    }

    public virtual void AnimateInRightTop(float time, float delay) {
        AnimateInRightTop(panelRightTopObject, time, delay);
    }

    public virtual void AnimateOutRightTop(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(rightClosedX));
        }
    }

    public virtual void AnimateOutRightTop(float time, float delay) {
        AnimateOutRightTop(panelRightTopObject, time, delay);
    }

    // TOP

    public virtual void AnimateInTop(GameObject go) {
        AnimateInTop(go, durationShow, durationDelayShow);
    }

    public virtual void AnimateOutTop(GameObject go) {
        AnimateOutTop(go, durationHide, durationDelayHide);
    }

    public virtual void AnimateInTop(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(0));
        }
    }

    public virtual void AnimateInTop(float time, float delay) {
        AnimateInTop(panelTopObject, time, delay);
    }

    public virtual void AnimateOutTop(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(topClosedY));
        }
    }
	
	public virtual void AnimateOutTop(float time, float delay) {
        AnimateInTop(panelTopObject, time, delay);
	}

    // BOTTOM

    public virtual void AnimateInBottom(GameObject go) {
        AnimateInBottom(go, durationShow, durationDelayShow);
    }

    public virtual void AnimateOutBottom(GameObject go) {
        AnimateInBottom(go, durationHide, durationDelayHide);
    }
	
    public virtual void AnimateInBottom(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(0));
        }
    }

    public virtual void AnimateInBottom(float time, float delay) {
        AnimateInBottom(panelBottomObject, time, delay);
    }

    public virtual void AnimateOutBottom(GameObject go, float time, float delay) {
        if(go != null) {
            UITweenerUtil.MoveTo(go,
            UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(bottomClosedY));
        }
    }

    public virtual void AnimateOutBottom(float time, float delay) {
        AnimateOutBottom(panelBottomObject, time, delay);
    }

    // ANIMATE
	
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

