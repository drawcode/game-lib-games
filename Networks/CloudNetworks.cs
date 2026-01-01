#define CLOUD_GAMEVERSES
#define CLOUD_SYNCHRONO

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

// using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public enum CloudNetworkType {
    Gameverses,
    Synchrono,
    Google,
    Amazon,
    Apple
}

public class CloudMessages {
    public static string cloudNetworksStart = "cloud-networks-start";
    public static string cloudNetworksFileSync = "cloud-networks-file-sync";
}

public class CloudNetworks : GameObjectBehavior {

#if USE_CONFIG_APP
    public static bool cloudNetworksEnabled = AppConfigs.cloudNetworksEnabled;
    public static bool cloudNetworksTestingEnabled = AppConfigs.cloudNetworksTestingEnabled;

#else
    public static bool cloudNetworksEnabled = false;
    public static bool cloudNetworksTestingEnabled = false;
#endif

    // Only one CloudNetworks can exist. We use a singleton pattern to enforce this.
    private static CloudNetworks _instance = null;

    public static CloudNetworks Instance {
        get {
            if (!_instance) {

                // check if an ObjectPoolManager is already available in the scene graph
                _instance = FindObjectOfType(typeof(CloudNetworks)) as CloudNetworks;

                // nope, create a new one
                if (!_instance) {
                    var obj = new GameObject("_CloudNetworks");
                    _instance = obj.AddComponent<CloudNetworks>();
                }
            }

            return _instance;
        }
    }

    void Start() {
        Init();
    }

    void OnEnable() {

    }

    void OnDisable() {

    }

    public void Init() {

#if CLOUD_SYNCHRONO
        Invoke("synchronoInit", 1);
#endif
    }

#if CLOUD_SYNCHRONO
    // ----------------------------------------------------------------------
    // GA - http://support.gameanalytics.com/hc/en-us/articles/200841396-Tips#customArea

    public void synchronoInit() {

        //if(analyticsGameAnalytics == null) {

        //	analyticsGameAnalytics = FindObjectOfType(typeof(GA_SystemTracker)) as GA_SystemTracker;

        //	if (!analyticsGameAnalytics) {
        //		var obj = new GameObject("_analyticsGameAnalytics");
        //		analyticsGameAnalytics = obj.AddComponent<GA_SystemTracker>();
        //		DontDestroyOnLoad(analyticsGameAnalytics);
        //	}
        //}

        //analyticsEveryplay.clientId = AppConfigs.analyticsEveryplayClientId;
        //analyticsEveryplay.clientSecret = AppConfigs.analyticsEveryplayClientSecret;
        //analyticsEveryplay.redirectURI = AppConfigs.analyticsEveryplayAuthUrl;

    }

#endif


    // ----------------------------------------------------------------------

    // GENERIC CALLS


    // ----------------------------------------------------------------------

    // cloud Synchrono

    // IS READY

    public static bool IsReady() {
        if (Instance != null) {
            return Instance.isReady();
        }
        return false;
    }

    public bool isReady() {

#if CLOUD_SYNCHRONO
        return true;
#else
		
		return false;
#endif
    }

    // UPLOAD

    public void Upload(string uuid, string url, Dictionary<string, object> args, byte[] data) {

    }

    public void OnUploadSuccess() {

    }

    public void OnUploadError() {

    }

    // DOWNLOAD

    public void Download(string uuid, string url, Dictionary<string, object> args) {

    }

    public void OnDownloadSuccess() {

    }

    public void OnDownloadError() {

    }
}