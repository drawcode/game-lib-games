using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;

public class BaseGameUIPanelProductCurrencyEarn : GameUIPanelBase {
    
    public static GameUIPanelProductCurrencyEarn Instance;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UIImageButton buttonHelp;
    public UIImageButton buttonEarnLogin;
    public UIImageButton buttonEarnWebsite;
    public UIImageButton buttonEarnTwitter;
    public UIImageButton buttonEarnFacebook;
    public UIImageButton buttonEarnVideoAds;
    public UIImageButton buttonEarnOffers;
    public UIImageButton buttonEarnMoreGames;
    public UIImageButton buttonEarnViewFullscreenAds;
#else
    public Button buttonHelp;
    public Button buttonEarnLogin;
    public Button buttonEarnWebsite;
    public Button buttonEarnTwitter;
    public Button buttonEarnFacebook;
    public Button buttonEarnVideoAds;
    public Button buttonEarnOffers;
    public Button buttonEarnMoreGames;
    public Button buttonEarnViewFullscreenAds;
#endif


    public static bool isInst {
        get {
            if (Instance != null) {
                return true;
            }
            return false;
        }
    }
    
    public virtual void Awake() {
        
    }
    
    public override void Start() {
        Init();
    }
    
    public override void Init() {
        base.Init();
    }

    public override void OnEnable() {

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.AddListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);

        // 

        Messenger<double>.AddListener(AdNetworksMessages.videoAd, OnVideoAdWatched);
    }

    public override void OnDisable() {

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);

        // 
        
        Messenger<double>.RemoveListener(AdNetworksMessages.videoAd, OnVideoAdWatched);
    }

    public override void OnUIControllerPanelAnimateIn(string classNameTo) {
        if (className == classNameTo) {
            AnimateIn();
        }
    }

    public override void OnUIControllerPanelAnimateOut(string classNameTo) {
        if (className == classNameTo) {
            AnimateOut();
        }
    }

    public override void OnUIControllerPanelAnimateType(string classNameTo, string code) {
        if (className == classNameTo) {
            //
        }
    }

    public virtual void OnVideoAdWatched(double amountWatched) {
    
        LogUtil.Log("OnVideoWatched:" + " amountWatched:" + amountWatched);

        if (amountWatched > .9) {
            GameProfileRPGs.Current.AddCurrency(100);
        }

    }
    
    public virtual void OnFacebookLike(string account) {
        
        LogUtil.Log("OnFacebookLike:" + " account:" + account);
        
        //if(url) {
        GameProfileRPGs.Current.AddCurrency(100);
        //}
        
    }
    
    public virtual void OnTwitterFollow(string account) {
        
        LogUtil.Log("OnTwitterFollow:" + " account:" + account);
        
        //if(url) {
        GameProfileRPGs.Current.AddCurrency(100);
        //}
        
    }
        
    public virtual void OnWebsiteViewed(string url) {
        
        LogUtil.Log("OnWebsiteViewed:" + " url:" + url);
        
        //if(url) {
        GameProfileRPGs.Current.AddCurrency(100);
        //}
        
    }

    public override void OnButtonClickEventHandler(string buttonName) {
        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);

        if (UIUtil.IsButtonClicked(buttonEarnVideoAds, buttonName)) {

            LogUtil.Log("buttonEarnVideoAds: " + buttonName);

            AdNetworks.ShowVideoAd();
        }
        else if (UIUtil.IsButtonClicked(buttonEarnOffers, buttonName)) {
            
            LogUtil.Log("buttonEarnOffers: " + buttonName);

            AdNetworks.ShowOfferWall();
        }
        else if (UIUtil.IsButtonClicked(buttonEarnMoreGames, buttonName)) {
            
            LogUtil.Log("buttonEarnMoreGames: " + buttonName);

            AdNetworks.ShowMoreApps();
        }
        else if (UIUtil.IsButtonClicked(buttonEarnFacebook, buttonName)) {
            
            LogUtil.Log("buttonEarnFacebook: " + buttonName);

            OnFacebookLike("default");
            
            GameCommunity.LikeUrl(
                SocialNetworkTypes.facebook, 
                Locos.Get(LocoKeys.app_web_url));
        }
        else if (UIUtil.IsButtonClicked(buttonEarnTwitter, buttonName)) {
            
            LogUtil.Log("buttonEarnTwitter: " + buttonName);

            OnTwitterFollow("default");
            
            Platforms.ShowWebView(
                Locos.Get(LocoKeys.app_display_name), 
                Locos.Get(LocoKeys.app_web_url_twitter));
        }
        else if (UIUtil.IsButtonClicked(buttonEarnWebsite, buttonName)) {
            
            LogUtil.Log("buttonEarnWebsite: " + buttonName);

            OnWebsiteViewed("default");
            
            Platforms.ShowWebView(
                Locos.Get(LocoKeys.app_display_name), 
                Locos.Get(LocoKeys.app_web_url));
        }
        else if (UIUtil.IsButtonClicked(buttonEarnViewFullscreenAds, buttonName)) {
            
            LogUtil.Log("buttonEarnViewFullscreenAds: " + buttonName);
            
            AdNetworks.ShowFullscreenAd();
        }
    }
    
    public static void LoadData() {
        if (GameUIPanelProductCurrencyEarn.Instance != null) {
            GameUIPanelProductCurrencyEarn.Instance.loadData();
        }
    }
    
    public virtual void loadData() {

        StartCoroutine(loadDataCo());
    }
    
    IEnumerator loadDataCo() {
        
        LogUtil.Log("LoadDataCo");

        yield return new WaitForEndOfFrame();

    }
    
    public override void HandleShow() {
        base.HandleShow();
        
        backgroundDisplayState = UIPanelBackgroundDisplayState.PanelBacker;
        adDisplayState = UIPanelAdDisplayState.BannerBottom;
    }

    public override void AnimateIn() {
        
        base.AnimateIn();        

    }

    public override void AnimateOut() {
        
        base.AnimateOut();
    }
}
