using System;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class SocialNetworks : MonoBehaviour
{
	public GameObject socialNetworkFacebookAndroid;
	public GameObject socialNetworkTwitterAndroid;
	public GameObject socialNetworkiOS;
		
	const string FACEBOOK_APP_ID = "133833223395676";
	const string TWITTER_KEY = "nQuQQSKCPg0uVl8Im1ykMQ";
	const string TWITTER_SECRET = "oIglS7ERBhUk1CVq8Oq4yzLiejNTXt8BCSVEUyKbvE";
	
	// TODO cleanup...
	public void LoadSocialLibs()
	{		
		Invoke("InitFacebook", 1);
		
		Invoke("InitTwitter", 10);
	}
	
	public void InitFacebook()
	{
		LogUtil.Log("LoadSocialLibs FACEBOOK_APP_ID..." + FACEBOOK_APP_ID);
		
		// Social Network Prime31
		if(Application.platform == RuntimePlatform.Android)
		{
			LogUtil.Log("LoadSocialLibs RuntimePlatform.Android..." + Application.platform);			
#if UNITY_ANDROID			
			socialNetworkFacebookAndroid = new GameObject("SocialNetworkingManager");
			socialNetworkFacebookAndroid.AddComponent<FacebookManager>();
			socialNetworkFacebookAndroid.AddComponent<FacebookEventListener>();

			FacebookAndroid.init();
			LogUtil.Log("LoadSocialLibs Facebook init..." + FACEBOOK_APP_ID);
#endif
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
#if UNITY_IPHONE
			socialNetworkiOS = new GameObject("SocialNetworkingManager");
			socialNetworkiOS.AddComponent<FacebookManager>();
			socialNetworkiOS.AddComponent<FacebookEventListener>();
			
			// TODO PLUGIN
			////FacebookBinding.init(FACEBOOK_APP_ID);		
			LogUtil.Log("LoadSocialLibs Facebook init..." + FACEBOOK_APP_ID);
#endif
		}
		else 
		{
			// Web player...
			
#if UNITY_WEBPLAYER
			Application.ExternalCall("if(window.console) window.console.log","web facebook init");
#endif
		}
	}
	
	public void InitTwitter()
	{
		LogUtil.Log("LoadSocialLibs TWITTER_KEY..." + TWITTER_KEY);
		//LogUtil.Log("LoadSocialLibs TWITTER_SECRET..." + TWITTER_SECRET);
		
		// Social Network Prime31
		if(Application.platform == RuntimePlatform.Android)
		{
#if UNITY_ANDROID			
			LogUtil.Log("LoadSocialLibs RuntimePlatform.Android..." + Application.platform);			
			
			socialNetworkTwitterAndroid = new GameObject("TwitterAndroidManager");
			socialNetworkTwitterAndroid.AddComponent<TwitterManager>();
			socialNetworkTwitterAndroid.AddComponent<TwitterEventListener>();
			
			TwitterAndroid.init(TWITTER_KEY, TWITTER_SECRET);
			LogUtil.Log("Twitter init..." + TWITTER_KEY);
#endif			
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
#if UNITY_IPHONE
			TwitterBinding.init(TWITTER_KEY, TWITTER_SECRET);	
			LogUtil.Log("Twitter init..." + TWITTER_KEY);	
#endif
		}
		else 
		{
			// Web player...			
#if UNITY_WEBPLAYER
			Application.ExternalCall("if(window.console) window.console.log","web twitter init");
#endif
		}
	}
	
	public bool CheckTwitterLoggedIn()
	{
		if(Application.platform == RuntimePlatform.Android)
		{
#if UNITY_ANDROID	
			return TwitterAndroid.isLoggedIn();
#endif
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{			
#if UNITY_IPHONE
			return TwitterBinding.isLoggedIn();
#endif
		}
		else 
		{
			// Web player...			
#if UNITY_WEBPLAYER
			return true;//Application.ExternalEval("true");
#endif
		}
		return false;
	}
	
	public void ShowTwitterLogin()
	{
		if(Application.platform == RuntimePlatform.Android)
		{
#if UNITY_ANDROID				
			TwitterAndroid.showLoginDialog();
#endif
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{			
#if UNITY_IPHONE
			TwitterBinding.showLoginDialog();
#endif
		}
		else 
		{
			// Web player...
#if UNITY_WEBPLAYER
			Application.ExternalCall("if(window.console) window.console.log","web show twitter login");
#endif
		}
		
	}
	
	public void PostTwitterMessage(string message)
	{
		if(Application.platform == RuntimePlatform.Android)
		{
#if UNITY_ANDROID				
			//TwitterAndroid.postUpdate(message);
#endif
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{			
#if UNITY_IPHONE
			TwitterBinding.postStatusUpdate(message);
#endif
		}
		else 
		{
			// Web player...
#if UNITY_WEBPLAYER
			Application.ExternalCall("postTwitterMessage", message);			
			LogUtil.Log(String.Format("Twitter posting for web: message:{0}", message));

#endif
		}
	}
	
	public void ShowTwitterLoginOrPostMessage(string message)
	{
		if(CheckTwitterLoggedIn())
		{
			PostTwitterMessage(message);
		}
		else
		{
			ShowTwitterLogin();
		}
	}
	
	public bool CheckFacebookLoggedIn()
	{
		if(Application.platform == RuntimePlatform.Android)
		{
#if UNITY_ANDROID	
			return FacebookAndroid.isSessionValid();
#endif
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{			
#if UNITY_IPHONE
			return FacebookBinding.isSessionValid();
#endif
		}
		else 
		{
			// Web player...
#if UNITY_WEBPLAYER
			return true;//Application.ExternalEval(true);
#endif			
		}
		return false;
	}
	
	public void ShowFacebookLogin()
	{
		if(Application.platform == RuntimePlatform.Android)
		{
#if UNITY_ANDROID				
			FacebookAndroid.login();
#endif
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{		
#if UNITY_IPHONE	
			FacebookBinding.login();
#endif
		}
		else 
		{
			// Web player...
#if UNITY_WEBPLAYER
			//Application.ExternalCall("postFacebookMessage", title, caption, message, url, caption);
#endif			
		}
	}
	
	public void PostFacebookMessage(string message, string url, string title, string linkToImage, string caption)
	{
		if(Application.platform == RuntimePlatform.Android)
		{
#if UNITY_ANDROID				
			//FacebookAndroid.showPostMessageDialogWithOptions(url, title, linkToImage, caption);
#endif
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
#if UNITY_IPHONE
			FacebookBinding.showFacebookComposer(caption, linkToImage, url);//showPostMessageDialogWithOptions(url, title, linkToImage, caption);
#endif
		}
		else 
		{
			// Web player...
#if UNITY_WEBPLAYER
			Application.ExternalCall("postFacebookMessage", title, caption, message, url, caption, linkToImage);			
			LogUtil.Log(String.Format("Facebook posting for web: title:{0} caption:{0} message:{0} url:{0} caption:{0}", title, caption, message, url, caption) );

#endif
		}
	}
	
	public void ShowFacebookLoginOrPostMessage(string message, string url, string title, string linkToImage, string caption)
	{
		if(CheckFacebookLoggedIn() || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			PostFacebookMessage(message, url, title, linkToImage, caption);
		}
		else
		{
			ShowFacebookLogin();
		}
	}
}

