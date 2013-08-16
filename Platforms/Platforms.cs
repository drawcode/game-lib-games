using System;
using UnityEngine;

public class Platforms
{
	private static volatile Platforms instance;
	private static System.Object syncRoot = new System.Object();
		
	public static Platforms Instance {
		get {
		    if (instance == null) {
		        lock (syncRoot) {
		           if (instance == null) 
		              instance = new Platforms();
		        }
		     }
	    	return instance;
		}
	}
	
	public Platforms() {
		
	}
	
	public bool IsAmazonDevice() {
		
#if !UNITY_FLASH
		if(SystemInfo.deviceModel.IndexOf("Kindle") > -1) {
			return true;
		}
#endif
		return false;
	}
	
	public void ShowWebView(string title, string url) {
		if(Application.platform == RuntimePlatform.Android) {
#if UNITY_ANDROID
			EtceteraAndroid.showWebView(title, url);
#endif
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer) {
#if UNITY_IPHONE		
			EtceteraBinding.showWebPage(url, false);
#endif
		}
		else {
			Application.OpenURL(url);
		}
	}
	
	public void PlayMovie(string url, bool showControls, bool supportLandscape, bool supportPortrait) {
#if UNITY_ANDROID
#elif UNITY_IPHONE
		EtceteraTwoBinding.playMovie(url, showControls, supportLandscape, supportPortrait);
#endif
	}
	
	public void AskForReview(int launchesToWait, string title, string message, string packageId) {
		if(Application.platform == RuntimePlatform.Android) {
#if UNITY_ANDROID
			// TODO android
			EtceteraAndroid.askForReview(3, 0, 3, "Review SupaSupaCross!", "Review SupaSupaCross if you like it.", "com.supasupagames.supasupacross");
#endif
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer) {
			//Etcetera.ask
		}
		else {
		}		
	}
	
	public void ShowEmailView(string to, string subject, string body, bool isHtml) {
#if UNITY_IPHONE
			if(EtceteraBinding.isEmailAvailable()) {
				EtceteraBinding.showMailComposer(to, subject, "iOS: " + body, isHtml); 
			}
			else {
				EtceteraBinding.showWebPage("mailto:" + to, false);
			}
#elif UNITY_ANDROID
			// TODO android
			EtceteraAndroid.showEmailComposer(to, subject, "Android: " + body, isHtml);
#else
		Application.OpenURL("mailto:" + to);
#endif
	}
	
}


