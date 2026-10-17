#if UNITY_EDITOR
using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Stops a mobile build that ships the Google Mobile Ads SDK without a valid AdMob App ID.
//
// The SDK aborts at startup when the App ID is missing from AndroidManifest / Info.plist,
// and LevelPlay's own post-processor only WARNS (or silently skips when its network
// settings asset doesn't exist). The SDK arrives two ways, both checked:
//   - LevelPlay AdMob adapter (ISAdMobAdapterDependencies.xml) - App ID from
//     Ads Mediation > Network Settings (LevelPlayMediatedNetworkSettings.asset)
//   - Google Mobile Ads plugin directly - App ID from Assets > Google Mobile Ads > Settings
//
// Release builds fail; development builds log an error and continue.
// The settings assets are read through SerializedObject so this compiles without
// referencing either SDK's editor assembly.
public class AdNetworksBuildValidator : IPreprocessBuildWithReport {

    const string levelPlayNetworkSettingsPath =
        "Assets/LevelPlay/Resources/LevelPlayMediatedNetworkSettings.asset";

    public int callbackOrder {
        get {
            return 0;
        }
    }

    public void OnPreprocessBuild(BuildReport report) {

        BuildTarget target = report.summary.platform;

        if (target != BuildTarget.Android && target != BuildTarget.iOS) {
            return;
        }

        List<string> problems = Validate(target);

        if (problems.Count == 0) {
            return;
        }

        string message = "AdNetworks: AdMob App ID problems - the app would crash at launch:\n- " +
            string.Join("\n- ", problems);

        bool development = (report.summary.options & BuildOptions.Development) != 0;

        if (development) {
            Debug.LogError(message + "\n(Development build - continuing. Release builds will fail.)");
            return;
        }

        throw new BuildFailedException(message);
    }

    [MenuItem("Tools/Ad Networks/Validate AdMob App IDs")]
    static void ValidateMenu() {

        foreach (BuildTarget target in new[] { BuildTarget.Android, BuildTarget.iOS }) {

            List<string> problems = Validate(target);

            if (problems.Count == 0) {
                Debug.Log("AdNetworks: " + target + " AdMob App ID OK (or AdMob not included).");
            }
            else {
                Debug.LogError("AdNetworks: " + target + ":\n- " + string.Join("\n- ", problems));
            }
        }
    }

    public static List<string> Validate(BuildTarget target) {

        List<string> problems = new List<string>();
        bool ios = target == BuildTarget.iOS;

        // LevelPlay-mediated AdMob
        if (AssetDatabase.FindAssets("ISAdMobAdapterDependencies").Length > 0) {

            ScriptableObject settings =
                AssetDatabase.LoadAssetAtPath<ScriptableObject>(levelPlayNetworkSettingsPath);

            if (settings == null) {
                problems.Add("LevelPlay AdMob adapter is installed but there are no Network Settings " +
                    "(Ads Mediation > Network Settings). Enable AdMob there and enter the App IDs, " +
                    "or remove the AdMob adapter in the LevelPlay Network Manager.");
            }
            else {
                SerializedObject so = new SerializedObject(settings);
                SerializedProperty enabled = so.FindProperty("EnableAdmob");
                SerializedProperty appId = so.FindProperty(ios ? "AdmobIOSAppId" : "AdmobAndroidAppId");

                if (enabled == null || !enabled.boolValue) {
                    problems.Add("LevelPlay AdMob adapter is installed but 'AdMob Integration' is off in " +
                        "Ads Mediation > Network Settings, so no App ID reaches the " +
                        (ios ? "Info.plist" : "AndroidManifest") + ".");
                }
                else {
                    appIdCheck(problems, "LevelPlay Network Settings", ios,
                        appId != null ? appId.stringValue : null);
                }
            }
        }

        // Direct Google Mobile Ads plugin
        if (typeExists("GoogleMobileAds.Api.MobileAds")) {

            ScriptableObject settings = gmaSettingsFind();

            if (settings == null) {
                problems.Add("Google Mobile Ads plugin is installed but has no settings asset " +
                    "(Assets > Google Mobile Ads > Settings).");
            }
            else {
                SerializedObject so = new SerializedObject(settings);
                SerializedProperty appId = so.FindProperty(ios ? "adMobIOSAppId" : "adMobAndroidAppId");

                if (appId == null) {
                    Debug.LogWarning("AdNetworks: couldn't read the App ID field from GoogleMobileAdsSettings " +
                        "(plugin version changed?) - check it by hand.");
                }
                else {
                    appIdCheck(problems, "Google Mobile Ads Settings", ios, appId.stringValue);
                }
            }
        }

        return problems;
    }

    static void appIdCheck(List<string> problems, string source, bool ios, string appId) {

        string platform = ios ? "iOS" : "Android";

        if (string.IsNullOrEmpty(appId)) {
            problems.Add(source + ": " + platform + " AdMob App ID is empty.");
        }
        else if (AdNetworksConfig.IsAdmobPublisherId(appId)) {
            problems.Add(source + ": " + platform + " AdMob App ID '" + appId + "' is a publisher id " +
                "(pub-...), not an App ID. Use the App ID from AdMob > Apps > App settings " +
                "(ca-app-pub-XXXXXXXXXXXXXXXX~NNNNNNNNNN).");
        }
        else if (!AdNetworksConfig.IsValidAdmobAppId(appId)) {
            problems.Add(source + ": " + platform + " AdMob App ID '" + appId + "' is not in the " +
                "ca-app-pub-XXXXXXXXXXXXXXXX~NNNNNNNNNN format.");
        }
    }

    static ScriptableObject gmaSettingsFind() {

        foreach (string guid in AssetDatabase.FindAssets("GoogleMobileAdsSettings")) {

            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase)) {
                ScriptableObject settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (settings != null) {
                    return settings;
                }
            }
        }

        return null;
    }

    static bool typeExists(string fullName) {

        foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            if (assembly.GetType(fullName, false) != null) {
                return true;
            }
        }

        return false;
    }
}
#endif
