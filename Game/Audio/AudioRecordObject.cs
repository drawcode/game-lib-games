//#define AUDIO_RECORDER_USE_PLUGIN
#define AUDIO_RECORDER_USE_UNITY

using System;
using UnityEngine;

using Engine.Utility;

public class AudioRecordObject : GameObjectBehavior
{	
	public static AudioRecordObject Instance;
	
	public GameObject audioSystem;
	public GameObject audioManagerObject;
	public GameObject audioEventListenerObject;	
	public bool audioSystemAdded = false;
	
	public bool enableAudioRecording = false;

#if AUDIO_RECORDER_USE_UNITY


#elif AUDIO_RECORDER_USE_PRIME31
	
#if UNITY_IPHONE

	public AudioRecorderManager audioManager;
	public AudioRecorderEventListener audioEventListener;
#elif UNITY_ANDROID
	public AudioRecorderAndroidManager audioManager;
	public AudioRecorderAndroidEventListener audioEventListener;
#else
	// Web/PC
	public GameObject audioManager;
	public GameObject audioEventListener;
#endif
#endif

    public bool EnableAudioRecording {
		get {
			return enableAudioRecording;
		}
		set {
			enableAudioRecording = value;
		}
	}
	
	
	void Awake() {        
		if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
		
        Instance = this;
	}
	
	void Start() {
		InitAudioSystem();
		DontDestroyOnLoad(gameObject);
	}
	
	void InitAudioSystem() {
		// If iOS add the plugin
		// else do nothing for now
		
		if(!audioSystemAdded) {
			LogUtil.Log("AudioObject::InitAudioSystem ");
			
			audioSystem = new GameObject("AudioObjectSystem");
			DontDestroyOnLoad(audioSystem);

#if AUDIO_RECORDER_USE_UNITY
            
#elif AUDIO_RECORDER_USE_PRIME31
#if UNITY_IPHONE
			audioManager = audioSystem.AddComponent<AudioRecorderManager>();				
			audioEventListener = audioSystem.AddComponent<AudioRecorderEventListener>();		
			
			LogUtil.Log("AudioObject::InitAudioSystem iOS added...");
#elif UNITY_ANDROID
		
			audioManager = audioSystem.AddComponent<AudioRecorderAndroidManager>();				
			audioEventListener = audioSystem.AddComponent<AudioRecorderAndroidEventListener>();
		
			LogUtil.Log("AudioObject::InitAudioSystem IAB/Android added...");	
#elif UNITY_FLASH
#elif UNITY_WEBPLAYER
#else
			// Web/PC - storekit stub for now...
			audioManager = audioSystem.AddComponent<AudioRecorderManager>();				
			audioEventListener = audioSystem.AddComponent<AudioRecorderEventListener>();		
			
			LogUtil.Log("AudioObject::InitAudioSystem default added...");
#endif
#endif
            if (audioManagerObject != null)
				DontDestroyOnLoad(audioManagerObject);	
			
			if(audioEventListenerObject != null)
				DontDestroyOnLoad(audioEventListenerObject);	
			
			audioSystemAdded = true;
		}		
	}	
}

