using System;
using Engine.Events;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameNetworkUnityMessages {

    public static string gameNetworkUnityLoggedIn = "game-network-unity-logged-in";
    public static string gameNetworkUnityAchievements = "game-network-unity-achievements";
    public static string gameNetworkUnityAchievementDescriptions = "game-network-unity-achievement-descriptions";

    public static string gameNetworkUnityLeaderboardScores = "game-network-unity-leaderboard-scores";
}

public class GameNetworkUnity : MonoBehaviour {

    private static GameNetworkUnity _instance = null;

    public static GameNetworkUnity instance {
        get {
            if (!_instance) {

                // check if an ObjectPoolManager is already available in the scene graph
                _instance = FindObjectOfType(typeof(GameNetworkUnity)) as GameNetworkUnity;

                // nope, create a new one
                if (!_instance) {
                    var obj = new GameObject("_GameNetworkUnity");
                    _instance = obj.AddComponent<GameNetworkUnity>();
                }
            }

            return _instance;
        }
    }

    void Start() {

    }

    // ------------------------------------------------------------------------
    // AUTHENTICATION

    public static bool IsAuthenticated() {
        if (instance != null) {

            return instance.isAuthenticated();
        }
        return false;
    }

    public bool isAuthenticated() {
        return currentUser.authenticated;
    }

    //

    public static void Authenticate() {
        if (instance != null) {

            instance.authenticate();
        }
    }

    public void authenticate(Action<ILocalUser> callback = null) {

        Social.localUser.Authenticate(success => {
            if (success) {
                Debug.Log("Authentication successful");
                string userInfo = "Username: " + currentUser.userName +
                    "\nUser ID: " + currentUser.id +
                    "\nIsUnderage: " + currentUser.underage;
                Debug.Log(userInfo);

                if (callback != null) {
                    callback(Social.localUser);
                }

                Messenger<ILocalUser>.Broadcast(
                    GameNetworkUnityMessages.gameNetworkUnityLoggedIn, Social.localUser);

                // Request loaded achievements, and register a callback for processing them
                //Social.LoadAchievements(processLoadedAchievements);
            }
            else {
                Debug.Log("Authentication failed");
            }
        });
    }

    //

    public static ILocalUser CurrentUser() {
        if (instance != null) {

            return instance.currentUser;
        }
        return null;
    }

    public ILocalUser currentUser {
        get {

            return Social.localUser;
        }
    }

    // ------------------------------------------------------------------------
    // ACHIEVEMENTS

    public void showAchievements() {

        if (!isAuthenticated()) {
            return;
        }

        Social.ShowAchievementsUI();
    }

    //

    public void loadAchievements() {

        if (!isAuthenticated()) {
            return;
        }
        // Request loaded achievements, and register a callback for processing them
        Social.LoadAchievements(processLoadedAchievements);
    }

    // This function gets called when the LoadAchievement call completes
    void processLoadedAchievements(IAchievement[] achievements) {

        Messenger<IAchievement[]>.Broadcast(
            GameNetworkUnityMessages.gameNetworkUnityAchievements, achievements);


        /*
        if(achievements.Length == 0)
            Debug.Log("Error: no achievements found");
        else
            Debug.Log("Got " + achievements.Length + " achievements");

        // You can also call into the functions like this
        Social.ReportProgress("Achievement01", 100.0, result => {
            if(result)
                Debug.Log("Successfully reported achievement progress");
            else
                Debug.Log("Failed to report achievement");
        });
        */
    }

    //

    public void loadAchievementDescriptions() {

        if (!isAuthenticated()) {
            return;
        }

        // Request loaded achievements, and register a callback for processing them
        Social.LoadAchievementDescriptions(processLoadedAchievementDescriptions);
    }

    // This function gets called when the LoadAchievement call completes
    void processLoadedAchievementDescriptions(IAchievementDescription[] achievementDescriptions) {

        Messenger<IAchievementDescription[]>.Broadcast(
            GameNetworkUnityMessages.gameNetworkUnityAchievementDescriptions, achievementDescriptions);


        /*
        if(achievements.Length == 0)
            Debug.Log("Error: no achievements found");
        else
            Debug.Log("Got " + achievements.Length + " achievements");

        // You can also call into the functions like this
        Social.ReportProgress("Achievement01", 100.0, result => {
            if(result)
                Debug.Log("Successfully reported achievement progress");
            else
                Debug.Log("Failed to report achievement");
        });
        */
    }

    //

    public void reportProgress(string achievementId, double progress, Action<bool> callback) {

        if (!isAuthenticated()) {
            return;
        }

        Social.ReportProgress(achievementId, progress, callback);
    }

    // ------------------------------------------------------------------------
    // LEADERBOARDS

    public void showLeaderboards() {
        if (!isAuthenticated()) {
            return;
        }

        Social.ShowLeaderboardUI();
    }

    //

    public void reportScore(long score, string board) {
        reportScore(score, board, onScoreReport);
    }

    public void reportScore(long score, string board, Action<bool> callback) {

        if (!isAuthenticated()) {
            return;

        }
        Social.ReportScore(score, board, callback);
    }

    public void onScoreReport(bool success) {

    }

    //

    public void loadScores(string leaderboardId) {
        loadScores(leaderboardId, onScoresLoad);
    }

    public void loadScores(string leaderboardId, Action<IScore[]> callback) {

        if (!isAuthenticated()) {
            return;

        }
        Social.LoadScores(leaderboardId, callback);
    }

    public void onScoresLoad(IScore[] scores) {
        Debug.Log("onScoresLoad:" + scores.Length);
    }
}