using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

using Engine;
using Engine.Data;
using Engine.Events;
using Engine.Utility;

public class BaseGamePlayerProgressRuntimeData {
    
    private static volatile BaseGamePlayerProgressRuntimeData instance;
    private static System.Object syncRoot = new System.Object();
    //private static System.Object syncRootEnd = new System.Object();
    //private static System.Object syncRootPurge = new System.Object();
    
    public string currentPack = "";
    public string currentAppState = "";
    public string currentAppContentState = "";
    public float startTime = 0f;
    public Dictionary<string,float> timesViewed = new Dictionary<string, float>();
    Dictionary<string, float> timesViewedCopy = new Dictionary<string, float>();
    public bool isRunning = false;
    
    public static BaseGamePlayerProgressRuntimeData BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new BaseGamePlayerProgressRuntimeData();
                }
            }
            
            return instance;
        }
        set {
            instance = value;
        }
    }
    
    public BaseGamePlayerProgressRuntimeData() {
        Reset();
    }
    
    public virtual void Reset() {
        currentPack = "";
        currentAppState = "";
        currentAppContentState = "";
        startTime = 0f;
        timesViewed = new Dictionary<string, float>();
    }
    
    public virtual void StartRuntimeDataCollection(string packCode, string app_state, string app_content_state) {
        
        isRunning = true;
        
        currentPack = packCode;
        currentAppState = app_state;
        currentAppContentState = app_content_state;
        startTime = Time.time;
        timesViewed = new Dictionary<string, float>();
    }
    
    public virtual void EndRuntimeDataCollection() {
        // Record all current runtime data to the stats and reset
        
        EndAllTimeViewedCollections();
        
        isRunning = false;
    }
    
    public virtual void StartTimeViewedCollection(string code) {
        if (!timesViewed.ContainsKey(code)) {
            timesViewed.Add(code, Time.time);
        }
    }
    
    public float EndTimeViewedCollection(string code) {
        float totalTime = 0f;
        if (timesViewed.ContainsKey(code)) {
            float startTime = timesViewed[code];
            if (startTime > 0.0f) {
                float endTime = Time.time + .05f;
                totalTime = endTime - startTime;                
                GamePlayerProgress.Instance.SetStatAccumulate(code, totalTime);
            }
            
        }
        return totalTime;
    }
    
    public float PurgeTimeViewedCollection(string code) {
        float totalTime = 0f;
        
        if (timesViewed.ContainsKey(code)) {
            float startTime = timesViewed[code];
            if (startTime > 0.0f) {
                float endTime = Time.time + .05f;
                totalTime = endTime - startTime;                
                GamePlayerProgress.Instance.SetStatAccumulate(code, totalTime);
                timesViewed[code] = endTime;
            }
            
        }
        return totalTime;
    }
    
    public virtual void EndAllTimeViewedCollections() {
        foreach (KeyValuePair<string,float> pair in timesViewed) {
            EndTimeViewedCollection(pair.Key);
        }
        timesViewed.Clear();
    }
    
    public virtual void PurgeAllTimeViewedCollections() {
        
        timesViewedCopy.Clear();
        
        foreach (KeyValuePair<string,float> pair in timesViewed) {
            timesViewedCopy.Add(pair.Key, pair.Value);
        }
        
        foreach (KeyValuePair<string,float> pair in timesViewedCopy) {
            PurgeTimeViewedCollection(pair.Key);
        }
        
        timesViewedCopy.Clear();        
    }
}

public class BaseGameStatisticCodes {
    public static string timePlayed = "time-played";
    public static string timesPlayed = "times-played";
    public static string timePlayedTracker = "time-played-tracker";
    public static string timesPlayedTracker = "times-played-tracker";
    public static string timePlayedAction = "time-played-action";
    public static string timesPlayedAction = "times-played-action";
    public static string timePlayedType = "time-played-type";
    public static string timesPlayedType = "times-played-type";
    public static string timePlayedPack = "time-played-pack";
    public static string timesPlayedPack = "times-played-pack";
    public static string timePlayedTotal = "time-played-total";
    public static string timesPlayedTotal = "times-played-total";
    public static string timePlayedHigh = "time-played-high";
    public static string timesPlayedHigh = "times-played-high";
    public static string timePlayedLow = "time-played-low";
    public static string timesPlayedLow = "times-played-low";
    public static string action = "action";
    public static string total = "total";
    public static string high = "high";
    public static string low = "low";
    public static string swipes = "swipes";
    public static string taps = "taps";
    public static string zooms = "zooms";
    public static string spins = "spins";
    
    /*
    public static string strafeLeft = "strafe-left";
    public static string strafeRight = "strafe-right";
    public static string boosts = "boosts";
    public static string spinBoost = "spin-boosts";
    public static string distance = "distance";

    public static string coins = "coins";
    public static string scores = "scores";
    public static string hits = "hits";
    public static string outOfBounds = "out-of-bounds";
    public static string attacks = "attacks";
    public static string defends = "defends";
    public static string kills = "kills";
    */
    
    /*
    public static string totalTimePlayed = "total-time-played";
    public static string totalTimesPlayed = "total-times-played";
    public static string totalWins = "total-wins";
    public static string totalShots = "total-shots";
    public static string totalDestroyed = "total-destroyed";
    */
    
    public static string genericCodeContentState(string code, string app_content_state) {
        return code + "-" + app_content_state;
    }
    
    public static string genericCodePackContentState(string code, string packCode, string app_content_state) {
        return code + "-" + packCode + "-" + app_content_state;
    }
    
    public static string genericCodePack(string code, string packCode) {
        return code + "-" + packCode;
    }
    
    // action
    
    public static string actionCodeContentState(string code, string app_content_state) {
        return action + "-" + code + "-" + app_content_state;
    }
    
    public static string actionCodePackContentState(string code, string packCode, string app_content_state) {
        return action + "-" + code + "-" + packCode + "-" + app_content_state;
    }
    
    public static string actionCodePack(string code, string packCode) {
        return action + "-" + code + "-" + packCode;
    }
    
    public static string actionCode(string code) {
        return action + "-" + code;
    }
    
    // high
    
    public static string highCodeContentState(string code, string app_content_state) {
        return high + "-" + code + "-" + app_content_state;
    }
    
    public static string highCodePackContentState(string code, string packCode, string app_content_state) {
        return high + "-" + code + "-" + packCode + "-" + app_content_state;
    }
    
    public static string highCodePack(string code, string packCode) {
        return high + "-" + code + "-" + packCode;
    }
    
    public static string highCode(string code) {
        return high + "-" + code;
    }
    
    public static string highCodeLevel(string code, string levelCode) {
        return high + "-" + code + "-" + levelCode;
    }
    
    public static string highCodeLevelCurrent(string code) {
        return high + "-" + code + "-" + GameLevels.Current.code;
    }
    
    // low
    
    public static string lowCodeContentState(string code, string app_content_state) {
        return low + "-" + code + "-" + app_content_state;
    }
    
    public static string lowCodePackContentState(string code, string packCode, string app_content_state) {
        return low + "-" + code + "-" + packCode + "-" + app_content_state;
    }
    
    public static string lowCodePack(string code, string packCode) {
        return low + "-" + code + "-" + packCode;
    }
    
    public static string lowCode(string code) {
        return low + "-" + code;
    }
    
    public static string lowCodeLevel(string code, string levelCode) {
        return low + "-" + code + "-" + levelCode;
    }
    
    public static string lowCodeLevelCurrent(string code) {
        return low + "-" + code + "-" + GameLevels.Current.code;
    }
    
    // total
    
    public static string totalCodeContentState(string code, string app_content_state) {
        return total + "-" + code + "-" + app_content_state;
    }
    
    public static string totalCodePackContentState(string code, string packCode, string app_content_state) {
        return total + "-" + code + "-" + packCode + "-" + app_content_state;
    }
    
    public static string totalCodePack(string code, string packCode) {
        return total + "-" + code + "-" + packCode;
    }
    
    public static string totalCode(string code) {
        return total + "-" + code;
    }
    
    public static string totalCodeLevel(string code, string levelCode) {
        return total + "-" + code + "-" + levelCode;
    }
    
    public static string totalCodeLevelCurrent(string code) {
        return total + "-" + code + "-" + GameLevels.Current.code;
    }
    
    // swipes
    
    public static string swipesContentState(string app_content_state) {
        return swipes + "-" + app_content_state;
    }
    
    public static string swipesPackContentState(string packCode, string app_content_state) {
        return swipes + "-" + packCode + "-" + app_content_state;
    }
    
    public static string swipesPack(string packCode) {
        return swipes + "-" + packCode;
    }
    
    // taps
    
    public static string tapsContentState(string app_content_state) {
        return taps + "-" + app_content_state;
    }
    
    public static string tapsPackContentState(string packCode, string app_content_state) {
        return taps + "-" + packCode + "-" + app_content_state;
    }
    
    public static string tapsPack(string packCode) {
        return taps + "-" + packCode;
    }
    
    // taps
    
    public static string zoomsContentState(string app_content_state) {
        return zooms + "-" + app_content_state;
    }
    
    public static string zoomsPackContentState(string packCode, string app_content_state) {
        return zooms + "-" + packCode + "-" + app_content_state;
    }
    
    public static string zoomsPack(string packCode) {
        return zooms + "-" + packCode;
    }
    
    // spins
    
    public static string spinsContentState(string app_content_state) {
        return spins + "-" + app_content_state;
    }
    
    public static string spinsPackContentState(string packCode, string app_content_state) {
        return spins + "-" + packCode + "-" + app_content_state;
    }
    
    public static string spinsPack(string packCode) {
        return spins + "-" + packCode;
    }
    
    // pack/contentstate
    
    public static string timePlayedCode() {
        return GameStatisticCodes.timePlayed;
    }
    
    public static string timePlayedCode(string code) {
        return GameStatisticCodes.timePlayed + "-" + code;
    }
    
    public static string timePlayedCodeContentState(string code, string app_content_state) {
        return timePlayedCode(code) + "-" + app_content_state;
    }
    
    public static string timePlayedCodePackContentState(string code, string packCode, string app_content_state) {
        return timePlayedCode(code) + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timePlayedPackContentState(string packCode, string app_content_state) {
        return timePlayedCode() + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timePlayedContentState(string app_content_state) {
        return timePlayedCode() + "-" + app_content_state;
    }
    
    public static string timesPlayedCode() {
        return GameStatisticCodes.timesPlayed;
    }
    
    public static string timesPlayedCode(string code) {
        return GameStatisticCodes.timesPlayed + "-" + code;
    }
    
    public static string timesPlayedCodeContentState(string code, string app_content_state) {
        return timesPlayedCode(code) + "-" + app_content_state;
    }
    
    public static string timesPlayedCodePackContentState(string code, string packCode, string app_content_state) {
        return timesPlayedCode(code) + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timesPlayedPackContentState(string packCode, string app_content_state) {
        return timesPlayedCode() + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timesPlayedContentState(string app_content_state) {
        return timesPlayedCode() + "-" + app_content_state;
    }
    
    // tracker 
    public static string timePlayedTrackerCode() {
        return GameStatisticCodes.timePlayedTracker;
    }
    
    public static string timePlayedTrackerCode(string trackerCode) {
        return GameStatisticCodes.timePlayedTracker + "-" + trackerCode;
    }
    
    public static string timePlayedTrackerCodeContentState(string trackerCode, string app_content_state) {
        return timePlayedTrackerCode(trackerCode) + "-" + app_content_state;
    }
    
    public static string timePlayedTrackerCodePackContentState(string trackerCode, string packCode, string app_content_state) {
        return timePlayedTrackerCode(trackerCode) + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timesPlayedTrackerCode() {
        return GameStatisticCodes.timesPlayedTracker;
    }
    
    public static string timesPlayedTrackerCode(string trackerCode) {
        return GameStatisticCodes.timesPlayedTracker + "-" + trackerCode;
    }
    
    public static string timesPlayedTrackerCodeContentState(string trackerCode, string app_content_state) {
        return timesPlayedTrackerCode(trackerCode) + "-" + app_content_state;
    }
    
    public static string timesPlayedTrackerCodePackContentState(string trackerCode, string packCode, string app_content_state) {
        return timesPlayedTrackerCode(trackerCode) + "-" + packCode + "-" + app_content_state;
    }
    
    // action
    public static string timePlayedActionCode() {
        return GameStatisticCodes.timePlayedAction;
    }
    
    public static string timePlayedActionCode(string code) {
        return GameStatisticCodes.timePlayedAction + "-" + code;
    }
    
    public static string timePlayedActionCodeContentState(string code, string app_content_state) {
        return timePlayedActionCode(code) + "-" + app_content_state;
    }
    
    public static string timePlayedActionCodePackContentState(string code, string packCode, string app_content_state) {
        return timePlayedActionCode(code) + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timePlayedActionCodePackContentState(string packCode, string app_content_state) {
        return timePlayedActionCode() + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timesPlayedActionCode() {
        return GameStatisticCodes.timesPlayedAction;
    }
    
    public static string timesPlayedActionCode(string code) {
        return GameStatisticCodes.timesPlayedAction + "-" + code;
    }
    
    public static string timesPlayedActionCodeContentState(string code, string app_content_state) {
        return timesPlayedActionCode(code) + "-" + app_content_state;
    }
    
    public static string timesPlayedActionCodePackContentState(string code, string packCode, string app_content_state) {
        return timesPlayedActionCode(code) + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timesPlayedActionCodePackContentState(string packCode, string app_content_state) {
        return timesPlayedActionCode() + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timesPlayedActionCodeLevel(string code, string levelCode) {
        return timesPlayedActionCode() + "-" + code + "-" + levelCode;
    }
    
    public static string timesPlayedActionCodeLevelCurrent(string code) {
        return timesPlayedActionCode() + "-" + code + "-" + GameLevels.Current.code;
    }
    
    // type
    
    public static string timePlayedTypeCode() {
        return GameStatisticCodes.timePlayedType;
    }
    
    public static string timePlayedTypeCode(string code) {
        return GameStatisticCodes.timePlayedType + "-" + code;
    }
    
    public static string timePlayedTypeCodeContentState(string code, string app_content_state) {
        return timePlayedTypeCode(code) + "-" + app_content_state;
    }
    
    public static string timePlayedTypeCodePackContentState(string code, string packCode, string app_content_state) {
        return timePlayedTypeCode(code) + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timePlayedTypeCodePackContentState(string packCode, string app_content_state) {
        return timePlayedTypeCode() + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timesPlayedTypeCode() {
        return GameStatisticCodes.timesPlayedType;
    }
    
    public static string timesPlayedTypeCode(string code) {
        return GameStatisticCodes.timesPlayedType + "-" + code;
    }
    
    public static string timesPlayedTypeCodeContentState(string code, string app_content_state) {
        return timesPlayedTypeCode(code) + "-" + app_content_state;
    }
    
    public static string timesPlayedTypeCodePackContentState(string code, string packCode, string app_content_state) {
        return timesPlayedTypeCode(code) + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timesPlayedTypeCodePackContentState(string packCode, string app_content_state) {
        return timesPlayedTypeCode() + "-" + packCode + "-" + app_content_state;
    }
    
    // pack
    
    public static string timePlayedPackCode() {
        return GameStatisticCodes.timePlayedPack;
    }
    
    public static string timePlayedPackCode(string code) {
        return GameStatisticCodes.timePlayedPack + "-" + code;
    }
    
    public static string timePlayedPackCodeContentState(string code, string app_content_state) {
        return timePlayedPackCode(code) + "-" + app_content_state;
    }
    
    public static string timePlayedPackCodePackContentState(string code, string packCode, string app_content_state) {
        return timePlayedPackCode(code) + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timePlayedPackCodePackContentState(string packCode, string app_content_state) {
        return timePlayedPackCode() + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timesPlayedPackCode() {
        return GameStatisticCodes.timesPlayedPack;
    }
    
    public static string timesPlayedPackCode(string code) {
        return GameStatisticCodes.timesPlayedPack + "-" + code;
    }
    
    public static string timesPlayedPackCodeContentState(string code, string app_content_state) {
        return timesPlayedPackCode(code) + "-" + app_content_state;
    }
    
    public static string timesPlayedPackCodePackContentState(string code, string packCode, string app_content_state) {
        return timesPlayedPackCode(code) + "-" + packCode + "-" + app_content_state;
    }
    
    public static string timesPlayedPackCodePackContentState(string packCode, string app_content_state) {
        return timesPlayedPackCode() + "-" + packCode + "-" + app_content_state;
    }
    
}

public class BaseGameAchievementCodes {
    /*
    public static string achieve_find_first = "achieve_find_first";
    public static string achieve_find_3 = "achieve_find_3";
    public static string achieve_find_all = "achieve_find_all";
    
    public static string achieve_time_played_action_min_1 = "achieve_time_played_action_min_1";
    
    public static string achieve_time_played_type_funfact_min_1 = "achieve_time_played_type_funfact_min_1";
    public static string achieve_funfact_time_min_1 = "achieve_funfact_time_min_1";
    public static string achieve_find_first = "achieve_find_first";
    public static string achieve_find_first = "achieve_find_first";
    public static string achieve_find_first = "achieve_find_first";
    public static string achieve_find_first = "achieve_find_first";
    public static string achieve_find_first = "achieve_find_first";
    public static string achieve_find_first = "achieve_find_first";
    public static string achieve_find_first = "achieve_find_first";
    */
    
    public static string formatAchievementCode(string code) {       
        code = code.Replace("-", "_");
        return code;
    }
    
    public static string genericCodeContentState(string code, string app_content_state) {
        code = code + "-" + app_content_state;
        code = formatAchievementCode(code);
        return code;
    }
    
    public static string genericCodePackContentState(string code, string packCode, string app_content_state) {
        code = code + "-" + packCode + "-" + app_content_state;
        code = formatAchievementCode(code);
        return code;
    }
    
    public static string genericCodePack(string code, string packCode) {
        code = code + "-" + packCode;
        code = formatAchievementCode(code);
        return code;
    }
}

public class BaseGamePlayerProgress {
    private static volatile BaseGamePlayerProgress instance;
    private static System.Object syncRoot = new System.Object();
    public List<AchievementMeta> achievementMetaList = new List<AchievementMeta>();
    public List<string> gameCenterLeaderboards = new List<string>();
    public float lastTimesPlayed = 0f;
    public float lastTimePlayed = 0f;
    public float lastTimesPlayedPack = 0f;
    public float lastTimePlayedPack = 0f;
    public float lastTimesPlayedTracker = 0f;
    public float lastTimePlayedTracker = 0f;
    public float lastTimesPlayedAction = 0f;
    public float lastTimePlayedAction = 0f;
    public float lastTimesPlayedType = 0f;
    public float lastTimePlayedType = 0f;
    public float lastTimesPlayedTotal = 0f;
    public float lastTimePlayedTotal = 0f;
    public float lastTimesPlayedHigh = 0f;
    public float lastTimePlayedHigh = 0f;
    public float lastSwipes = 0f;
    public float lastTaps = 0f;
    public float lastZooms = 0f;
    public float lastSpins = 0f;
    public float lastOutOfBounds = 0f;
    public float lastScores = 0f;
    public float lastHits = 0f;
    public float lastAttacks = 0f;
    public float lastDefends = 0f;
    public float lastKills = 0f;
    public float lastSpawns = 0f;
    Thread syncThread;
    
    public static BaseGamePlayerProgress BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new BaseGamePlayerProgress();
                }
            }
            
            return instance;
        }
        set {
            instance = value;
        }
    }
    
    public BaseGamePlayerProgress() {
        OnEnable();
    }
    
    void OnEnable() {
        
    }
    
    void OnDisable() {
        
    }
    
    public virtual void Reset() {
    }
    
    public int GetPointsByPlace(int place) {
        int points = 0;
        switch (place) {
        case 1:
            points += 10;
            break;
            
        case 2:
            points += 5;
            break;
            
        case 3:
            points += 3;
            break;
            
        case 4:
            points += 2;
            break;
            
        case 5:
            points += 1;
            break;
        }
        
        return points;
    }
    
    public int GetPlaceByPoints(int points) {
        int place = 5;
        
        switch (points) {
        case 10:
            place = 1;
            break;
            
        case 5:
            place = 2;
            break;
            
        case 3:
            place = 3;
            break;
            
        case 2:
            place = 4;
            break;
            
        case 1:
            place = 5;
            break;
        }
        
        return place;
    }
    
    public string GetPrettyPlace(int place) {
        string prettyPlace = "--";
        switch (place) {
        case 1:
            prettyPlace = place.ToString() + "st";
            break;
        case 2:
            prettyPlace = place.ToString() + "nd";
            break;
        case 3:
            prettyPlace = place.ToString() + "rd";
            break;
        default:
            prettyPlace = place.ToString() + "th";
            break;
        }
        return prettyPlace;
    }   
    
    // Process
    
    // tracker 
    
    public virtual void ProcessProgressTrackerByName(string trackerName) {
        string trackerCode = "";
        
        if (!string.IsNullOrEmpty(trackerCode)) {
            ARDataSetTracker tracker = ARDataSetTrackers.Instance.GetByNameAndPack(
                trackerName,
                GamePacks.Current.code);
            if (tracker != null) {
                trackerCode = tracker.code;
                if (trackerCode != null
                    && trackerCode != "") {
                    ProcessProgressTracker(trackerCode);
                }
            }
        }
    }
    
    public virtual void ProcessProgressTracker(string trackerCode) {
        if (Time.time > lastTimesPlayedTracker + 1f) {
            lastTimesPlayedTracker = Time.time;
            
            ProcessProgressTimesPlayedTracker(trackerCode);
            ProcessProgressTimePlayedTracker(trackerCode);
        }
    }
    
    public virtual void EndProcessProgressTrackerByName(string trackerName) {
        string trackerCode = "";
        
        if (!string.IsNullOrEmpty(trackerCode)) {
            ARDataSetTracker tracker = ARDataSetTrackers.Instance.GetByNameAndPack(
                trackerName,
                GamePacks.Current.code);
            if (tracker != null) {
                trackerCode = tracker.code;
                if (trackerCode != null
                    && trackerCode != "") {
                    EndProcessProgressTracker(trackerCode);
                }
            }
        }
    }
    
    public virtual void EndProcessProgressTracker(string trackerCode) {
        CoroutineUtil.Start(EndProcessProgressTrackerCo(trackerCode));
    }
    
    IEnumerator EndProcessProgressTrackerCo(string trackerCode) {
        yield return new WaitForSeconds(1f);
        EndProcessProgressTimesPlayedTracker(trackerCode);
        EndProcessProgressTimePlayedTracker(trackerCode);       
    }
    
    public virtual void ProcessProgressTimesPlayedTracker(string trackerCode) {
        string packCode = GamePacks.Current.code;
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.timesPlayedTrackerCode();
        string keyContentState = GameStatisticCodes.timesPlayedTrackerCode(app_content_state);
        string keyTracker = GameStatisticCodes.timesPlayedTrackerCode(trackerCode);
        
        string keyTrackerContentState = GameStatisticCodes.timesPlayedTrackerCodeContentState(
            trackerCode,
            app_content_state);
        
        string keyTrackerPackContentState = GameStatisticCodes.timesPlayedTrackerCodePackContentState(
            trackerCode,
            packCode,
            app_content_state);
        
        GamePlayerProgress.Instance.SetStatisticValue(key, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyContentState, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyTracker, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyTrackerContentState, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyTrackerPackContentState, 1);
    }
    
    public virtual void EndProcessProgressTimesPlayedTracker(string trackerCode) {
        
    }
    
    public virtual void ProcessProgressTimePlayedTracker(string trackerCode) {
        string packCode = GamePacks.Current.code;
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.timePlayedTrackerCode();
        string keyContentState = GameStatisticCodes.timePlayedTrackerCode(app_content_state);
        string keyTracker = GameStatisticCodes.timePlayedTrackerCode(trackerCode);      
        
        string keyTrackerContentState = GameStatisticCodes.timePlayedTrackerCodeContentState(
            trackerCode,
            app_content_state);
        
        string keyTrackerPackContentState = GameStatisticCodes.timePlayedTrackerCodePackContentState(
            trackerCode,
            packCode,
            app_content_state);
        
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(key);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyContentState);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyTracker);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyTrackerContentState);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyTrackerPackContentState);
    }
    
    public virtual void EndProcessProgressTimePlayedTracker(string trackerCode) {
        string packCode = GamePacks.Current.code;
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.timePlayedTrackerCode();
        string keyContentState = GameStatisticCodes.timePlayedTrackerCode(app_content_state);
        string keyTracker = GameStatisticCodes.timePlayedTrackerCode(trackerCode);
        
        string keyTrackerContentState = GameStatisticCodes.timePlayedTrackerCodeContentState(
            trackerCode,
            app_content_state);
        
        string keyTrackerPackContentState = GameStatisticCodes.timePlayedTrackerCodePackContentState(
            trackerCode,
            packCode,
            app_content_state);
        
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(key);
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyContentState);
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyTracker);
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyTrackerContentState);
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyTrackerPackContentState);
    }
    
    // action 
    
    public virtual void ProcessProgressAction(string actionCode) {
        if (Time.time > lastTimesPlayedAction + 1f) {
            lastTimesPlayedAction = Time.time;
            
            ProcessProgressTimesPlayedAction(actionCode);
            ProcessProgressTimePlayedAction(actionCode);
        }
    }
    
    public virtual void EndProcessProgressAction(string actionCode) {
        CoroutineUtil.Start(EndProcessProgressActionCo(actionCode));
    }
    
    IEnumerator EndProcessProgressActionCo(string actionCode) {
        yield return new WaitForSeconds(1f);
        EndProcessProgressTimesPlayedAction(actionCode);
        EndProcessProgressTimePlayedAction(actionCode);     
    }
    
    // total 
    string codeProcessProgressTotal = "";
    
    public virtual void ProcessProgressTotal(string code, double val) {
        ProcessProgressTotal(code, (float)val);
    }

    public virtual void ProcessProgressTotal(string code, float val) {
        if (Time.time > lastTimesPlayedTotal + .2f
            || codeProcessProgressTotal != code) {
            lastTimesPlayedTotal = Time.time;
            codeProcessProgressTotal = code;
            ProcessProgressTimesPlayedTotal(code, val);
        }       
    }
    
    public virtual void EndProcessProgressTotal(string code) {
        CoroutineUtil.Start(EndProcessProgressTotalCo(code));
    }
    
    IEnumerator EndProcessProgressTotalCo(string code) {
        yield return new WaitForSeconds(1f);
        EndProcessProgressTimesPlayedTotal(code);       
    }
    
    // high 
    
    public virtual void ProcessProgressHigh(string code, float val) {
        if (Time.time > lastTimesPlayedHigh + 1f) {
            lastTimesPlayedHigh = Time.time;
            
            ProcessProgressTimesPlayedHigh(code, val);
        }       
    }
    
    public virtual void EndProcessProgressHigh(string code) {
        CoroutineUtil.Start(EndProcessProgressHighCo(code));
    }
    
    IEnumerator EndProcessProgressHighCo(string code) {
        yield return new WaitForSeconds(1f);
        EndProcessProgressTimesPlayedHigh(code);    
    }
    
    //
    
    public virtual void SetStatAction(string action) {      
        GamePlayerProgress.Instance.ProcessActionDiscrete(action);
    }
    
    public virtual void SetStatAction(string action, double val) {  
        SetStatAction(action, (float)val);
    }
    
    public virtual void SetStatAction(string action, float val) {   
        GamePlayerProgress.Instance.SetStatisticValue(
            GameStatisticCodes.actionCode(action), 
            val);
    }
    
    public virtual void SetStatTotal(string code, double val) { 
        SetStatTotal(code, (float)val);
    }
    
    public virtual void SetStatTotal(string code, float val) {      
        ProcessProgressTimesPlayedTotal(code, val);
    }
    
    public virtual void SetStatHigh(string code, double val) {  
        SetStatHigh(code, (float)val);
    }
    
    public virtual void SetStatHigh(string code, float val) {   
        ProcessProgressTimesPlayedHigh(code, val);
    }
    
    // total process    
    
    public virtual void ProcessProgressTimesPlayedTotal(string code, float val) {
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.totalCode(code);
        string keyContentState = GameStatisticCodes.totalCodeContentState(code, app_content_state);
        string keyLevel = GameStatisticCodes.totalCodeLevelCurrent(code);
        
        GamePlayerProgress.Instance.SetStatisticValue(key, val);
        GamePlayerProgress.Instance.SetStatisticValue(keyContentState, val);
        GamePlayerProgress.Instance.SetStatisticValue(keyLevel, val);
    }
    
    public virtual void EndProcessProgressTimesPlayedTotal(string actionCode) {
        
    }
    
    public virtual void EndProcessTimesPlayedTotal(string code) {
        
        EndProcessProgressTimesPlayedTotal(code);
    }
    
    // high process
    
    public virtual void ProcessProgressTimesPlayedHigh(string code, float val) {
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.highCode(code);
        string keyContentState = GameStatisticCodes.highCodeContentState(code, app_content_state);
        string keyLevel = GameStatisticCodes.highCodeLevelCurrent(code);
        
        GamePlayerProgress.Instance.SetStatisticValue(key, val);
        GamePlayerProgress.Instance.SetStatisticValue(keyContentState, val);
        GamePlayerProgress.Instance.SetStatisticValue(keyLevel, val);
    }
    
    public virtual void EndProcessProgressTimesPlayedHigh(string actionCode) {
        
    }
    
    public virtual void EndProcessHigh(string code) {
        
        EndProcessProgressTimesPlayedHigh(code);
    }
    
    // action process
    
    public virtual void ProcessActionDiscrete(string actionCode) {
        ProcessProgressActionDiscrete(actionCode);
        EndProcessProgressActionDiscrete(actionCode);
    }
    
    public virtual void ProcessProgressActionDiscrete(string actionCode) {
        ProcessProgressTimesPlayedAction(actionCode);
        ProcessProgressTimePlayedAction(actionCode);
    }
    
    public virtual void EndProcessProgressActionDiscrete(string actionCode) {
        EndProcessProgressTimesPlayedAction(actionCode);
        EndProcessProgressTimePlayedAction(actionCode); 
    }
    
    public virtual void ProcessProgressTimesPlayedAction(string actionCode) {
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.timesPlayedActionCode();
        string keyContentState = GameStatisticCodes.timesPlayedActionCode(app_content_state);
        string keyAction = GameStatisticCodes.timesPlayedActionCode(actionCode);
        
        string keyActionContentState = GameStatisticCodes.timesPlayedActionCodeContentState(
            actionCode,
            app_content_state);
        
        GamePlayerProgress.Instance.SetStatisticValue(key, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyContentState, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyAction, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyActionContentState, 1);
    }
    
    public virtual void EndProcessProgressTimesPlayedAction(string actionCode) {
        
    }
    
    public virtual void ProcessProgressTimePlayedAction(string actionCode) {
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.timePlayedActionCode();
        string keyContentState = GameStatisticCodes.timePlayedActionCode(app_content_state);
        string keyAction = GameStatisticCodes.timePlayedActionCode(actionCode);
        
        string keyActionContentState = GameStatisticCodes.timePlayedActionCodeContentState(
            actionCode,
            app_content_state);
        
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(key);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyContentState);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyAction);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyActionContentState);
    }
    
    public virtual void EndProcessAction(string actionCode) {
        
        EndProcessProgressTimesPlayedAction(actionCode);
        EndProcessProgressTimePlayedAction(actionCode);
    }
    
    public virtual void EndProcessProgressTimePlayedAction(string actionCode) {
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.timePlayedActionCode();
        string keyContentState = GameStatisticCodes.timePlayedActionCode(app_content_state);
        string keyAction = GameStatisticCodes.timePlayedActionCode(actionCode);
        
        string keyActionContentState = GameStatisticCodes.timePlayedActionCodeContentState(
            actionCode,
            app_content_state);
        
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(key);
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyContentState);
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyAction);
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyActionContentState);
    }   
    
    // pack 
    
    public virtual void ProcessProgressPack(string packCode) {
        if (Time.time > lastTimesPlayedPack + 1f) {
            lastTimesPlayedPack = Time.time;
            
            ProcessProgressTimesPlayedPack(packCode);
            ProcessProgressTimePlayedPack(packCode);
        }
    }
    
    public virtual void EndProcessProgressPack(string packCode) {
        EndProcessProgressTimesPlayedPack(packCode);
        EndProcessProgressTimePlayedPack(packCode);     
    }
    
    public virtual void ProcessProgressTimesPlayedPack(string packCode) {
        string app_content_state = AppContentStates.Current.code;
        
        string keyDefault = GameStatisticCodes.timesPlayed;
        string key = GameStatisticCodes.timesPlayedPackCode();
        string keyPack = GameStatisticCodes.timesPlayedPackCode(packCode);
        
        string keyContentState = GameStatisticCodes.timesPlayedContentState(
            app_content_state);       
        
        GamePlayerProgress.Instance.SetStatisticValue(keyDefault, 1);
        GamePlayerProgress.Instance.SetStatisticValue(key, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyPack, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyContentState, 1);
    }
    
    public virtual void EndProcessProgressTimesPlayedPack(string packCode) {
        
    }
    
    public virtual void ProcessProgressTimePlayedPack(string packCode) {
        string app_content_state = AppContentStates.Current.code;
        
        string keyDefault = GameStatisticCodes.timePlayedCode();
        string key = GameStatisticCodes.timePlayedPackCode();
        string keyPack = GameStatisticCodes.timePlayedPackCode(packCode);
        
        string keyContentState = GameStatisticCodes.timePlayedContentState(
            app_content_state);
        
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyDefault);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(key);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyPack);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyContentState);
    }
    
    public virtual void EndProcessProgressTimePlayedPack(string packCode) {
        string app_content_state = AppContentStates.Current.code;
        
        string keyDefault = GameStatisticCodes.timePlayedCode();
        string key = GameStatisticCodes.timePlayedPackCode();
        string keyPack = GameStatisticCodes.timePlayedPackCode(packCode);
        
        string keyContentState = GameStatisticCodes.timePlayedContentState(
            app_content_state);
        
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyDefault);             
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(key);
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyPack);
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyContentState);
    }
    
    
    // type 
    
    public virtual void ProcessProgressType(string typeCode) {
        if (Time.time > lastTimesPlayedType + 1f) {
            lastTimesPlayedType = Time.time;
            
            ProcessProgressTimesPlayedType(typeCode);
            ProcessProgressTimePlayedType(typeCode);
        }
    }
    
    public virtual void EndProcessProgressType(string typeCode) {
        CoroutineUtil.Start(EndProcessProgressTypeCo(typeCode));
    }
    
    IEnumerator EndProcessProgressTypeCo(string typeCode) {
        yield return new WaitForSeconds(1f);
        EndProcessProgressTimesPlayedType(typeCode);
        EndProcessProgressTimePlayedType(typeCode);     
    }
    
    public virtual void ProcessProgressTimesPlayedType(string typeCode) {
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.timesPlayedTypeCode();
        string keyContentState = GameStatisticCodes.timesPlayedTypeCode(
            app_content_state);
        string keyType = GameStatisticCodes.timesPlayedTypeCode(typeCode);
        
        string keyTypeContentState = GameStatisticCodes.timesPlayedTypeCodeContentState(
            typeCode,
            app_content_state);
        
        GamePlayerProgress.Instance.SetStatisticValue(key, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyContentState, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyType, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyTypeContentState, 1);
    }
    
    public virtual void EndProcessProgressTimesPlayedType(string typeCode) {
        
    }
    
    public virtual void ProcessProgressTimePlayedType(string typeCode) {
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.timePlayedTypeCode();
        string keyContentState = GameStatisticCodes.timePlayedTypeCode(
            app_content_state);
        string keyType = GameStatisticCodes.timePlayedTypeCode(typeCode);
        
        string keyTypeContentState = GameStatisticCodes.timePlayedTypeCodeContentState(
            typeCode,
            app_content_state);
        
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(key);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyContentState);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyType);
        GamePlayerProgressRuntimeData.Instance.StartTimeViewedCollection(keyTypeContentState);
    }
    
    public virtual void EndProcessProgressTimePlayedType(string typeCode) {
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.timePlayedTypeCode();
        string keyContentState = GameStatisticCodes.timePlayedTypeCode(
            app_content_state);
        
        string keyType = GameStatisticCodes.timePlayedTypeCode(typeCode);
        string keyTypeContentState = GameStatisticCodes.timePlayedTypeCodeContentState(
            typeCode,
            app_content_state);
        
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(key);
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyContentState);
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyType);
        GamePlayerProgressRuntimeData.Instance.EndTimeViewedCollection(keyTypeContentState);
    }
    
    
    
    
    // actions - swipes 
    
    public virtual void ProcessProgressSwipes() {
        if (Time.time > lastSwipes + .5f) {
            lastSwipes = Time.time;
            
            ProcessProgressSwipesFull();
        }
    }
    
    public virtual void ProcessProgressSwipesFull() {
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.swipes;
        
        string keyContentState = GameStatisticCodes.swipesContentState(
            app_content_state);
        
        GamePlayerProgress.Instance.SetStatisticValue(key, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyContentState, 1);
    }
    
    
    // actions - taps 
    
    public virtual void ProcessProgressTaps() {
        if (Time.time > lastTaps + .5f) {
            lastTaps = Time.time;
            
            ProcessProgressTapsFull();
        }
    }
    
    public virtual void ProcessProgressTapsFull() {
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.taps;
        
        string keyContentState = GameStatisticCodes.tapsContentState(
            app_content_state);
        
        GamePlayerProgress.Instance.SetStatisticValue(key, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyContentState, 1);
    }
    
    
    // actions - zooms 
    
    public virtual void ProcessProgressZooms() {
        if (Time.time > lastZooms + .5f) {
            lastZooms = Time.time;
            
            ProcessProgressZoomsFull();
        }
    }
    
    public virtual void ProcessProgressZoomsFull() {
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.zooms;
        
        string keyContentState = GameStatisticCodes.zoomsContentState(
            app_content_state);
        
        GamePlayerProgress.Instance.SetStatisticValue(key, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyContentState, 1);
    }
    
    // actions - spins 
    
    public virtual void ProcessProgressSpins() {
        if (Time.time > lastSpins + .5f) {
            lastSpins = Time.time;
            
            ProcessProgressSpinsFull();
        }
    }
    
    public virtual void ProcessProgressSpinsFull() {
        string app_content_state = AppContentStates.Current.code;
        
        string key = GameStatisticCodes.spins;
        
        string keyContentState = GameStatisticCodes.spinsContentState(
            app_content_state);
        
        GamePlayerProgress.Instance.SetStatisticValue(key, 1);
        GamePlayerProgress.Instance.SetStatisticValue(keyContentState, 1);
    }

    public virtual void ProcessProgressLeaderboards() {

        //Debug.Log("ProcessProgressLeaderboards");

        // Submit all leaderboards
        foreach (GameLeaderboard board in GameLeaderboards.Instance.GetAll()) {

            string key = board.code;
                        
            //Debug.Log("ProcessProgressLeaderboards:" + " key:" + key);

            long keyValueLong = 0;                

            double keyValueDouble = 
                GameProfileStatistics.Current.GetStatisticValue(key);        

            keyValueLong = (long)keyValueDouble;

            //Debug.Log("ProcessProgressLeaderboards:" + " keyValueLong:" + keyValueLong);

            if (keyValueLong > 0) {
                
                //Debug.Log("ProcessProgressLeaderboards:" + " keyValueLong:" + keyValueLong);

                GameNetworks.SendScore(key, keyValueLong);
            }
        }    
    }
    
    // action code / actions found stats
    
    /*
    public virtual void ProcessStatsActionCode(string packCode, string app_content_state, string actionCode) {
        GamePlayerProgress.Instance.SetStatisticValue(
            GetKeyStatisticCodePackContentStateAction("actions-found", 
                packCode, 
                app_content_state,
                actionCode)
            , 1f);
    }
    
    public virtual void ProcessStatsActionCode(string packCode, string actionCode) {
        GamePlayerProgress.Instance.SetStatisticValue(
            GetKeyStatisticCodePackContentStateAction("actions-found", 
                packCode, 
                AppContentStates.Current.code,
                actionCode)
            , 1f);
    }
    
    public virtual void ProcessStatsActionCode(string actionCode) {
        //string key = GameStatisticCodes.
        //string key = GameStatisticCodes.timGetKeyStatisticCodePackContentStateAction(GameStatisticCodes., 
        //      GamePacks.Current.code, 
        //      AppContentStates.Current.code,
        //      actionCode);
        //GamePlayerProgress.Instance.SetStatisticValue(key, 1f);
    }
    */
    
    
    public virtual void ProcessProgressPackChange() {
        
        GamePlayerProgress.Instance.ProcessProgressPack(GamePacks.Current.code);
        GamePlayerProgressRuntimeData.Instance.PurgeAllTimeViewedCollections();     
        GamePlayerProgress.Instance.ProcessPackRuntimeAchievementsCurrentPack();                
        
        GameState.SaveProfile();
    }
    
    public virtual void ProcessProgressRuntimeAchievements() {
        
        GamePlayerProgressRuntimeData.Instance.PurgeAllTimeViewedCollections();
        GameState.SaveProfile();
        GamePlayerProgress.Instance.ProcessPackRuntimeAchievementsCurrentPack();
    }
    
    public virtual void ProcessPackRuntimeAchievementsCurrentPack() {
        ProcessPackRuntimeAchievements(GamePacks.Current.code);
    }
    
    public virtual void ProcessPackRuntimeAchievements(string packCode) {
        
        LogUtil.Log("ProcessPackRuntimeAchievements:" + packCode);
        
        foreach (GameAchievement achievement in GameAchievements.Instance.GetListByPack(packCode)) {
            if (achievement.data.filters != null) {
                foreach (GameFilter filterItem in achievement.data.filters) {
                    if (filterItem.type == GameFilterType.statisticSingle) {
                        List<GameFilterBase> filterTypeItems = achievement.GetFilterStatisticSingle();
                        foreach (GameFilterBase filter in filterTypeItems) {
                            if (filter != null) {
                                
                                // statistic-single                                                     
                                //
                                // Check each individual stat code separately, award if any are true
                                
                                string filterCode = filter.codes[0];
                                
                                List<string> codes = new List<string>();
                                
                                if (filter.includeKeys.defaultKey != GameFilterIncludeType.none) {
                                    // add explicit code
                                    codes.Add(filterCode);
                                }
                                
                                if (filter.includeKeys.app_content_state != GameFilterIncludeType.none) {
                                    string key = GameStatisticCodes.genericCodeContentState(
                                        filterCode, 
                                        AppContentStates.Current.code);
                                    codes.Add(key);
                                }
                                
                                foreach (string filterCodeItem in codes) {
                                    
                                    CheckStatSetAchievement(
                                        false,
                                        achievement.code, 
                                        filterCodeItem, 
                                        StatEqualityTypeString.GetEnum(filter.compareType),
                                        (float)filter.compareValue
                                    );
                                }
                                
                            }
                        }
                    }
                    else if (filterItem.type == GameFilterType.statisticSet) {
                        List<GameFilterBase> filterTypeItems = achievement.GetFilterStatisticSet();
                        foreach (GameFilterBase filter in filterTypeItems) {
                            if (filter != null) {
                                
                                // statistic-set
                                //
                                // Collect all stat values and if all are true then set the achievement,
                                // if one is false the set is false.
                                
                                bool setAchievement = false;
                                
                                List<string> codes = new List<string>();
                                
                                foreach (string filterCode in filter.codes) {
                                    
                                    
                                    if (filter.includeKeys.defaultKey != GameFilterIncludeType.none) {
                                        // add explicit code
                                        codes.Add(filterCode);
                                    }
                                    
                                    if (filter.includeKeys.app_content_state != GameFilterIncludeType.none) {
                                        string key = GameStatisticCodes.genericCodeContentState(
                                            filterCode, 
                                            AppContentStates.Current.code);
                                        codes.Add(key);
                                    }
                                }
                                
                                foreach (string filterCodeItem in codes) {
                                    
                                    if (CheckStatCondition(
                                        filterCodeItem, 
                                        StatEqualityTypeString.GetEnum(filter.compareType),
                                        (float)filter.compareValue
                                    )) {
                                        setAchievement = true;
                                    }
                                    else {
                                        setAchievement = false;
                                        break;
                                    }
                                }
                                
                                if (setAchievement) {
                                    GamePlayerProgress.Instance.SetAchievementAll(achievement.code);
                                }
                            }
                        }
                    }
                    else if (filterItem.type == GameFilterType.statisticAll) {
                        List<GameFilterBase> filterTypeItems = achievement.GetFilterStatisticAll();
                        foreach (GameFilterBase filter in filterTypeItems) {
                            if (filter != null) {
                                
                                // statistic-all
                                // 
                                // Collect all state values from metdata and if all are true set achievement
                                
                                bool setAchievement = false;
                                
                                List<string> codes = new List<string>();
                                
                                foreach (string filterCode in filter.codes) {                                
                                    
                                    if (filter.includeKeys.defaultKey != GameFilterIncludeType.none) {
                                        // add explicit code
                                        codes.Add(filterCode);
                                    }
                                    
                                    if (filter.includeKeys.action != GameFilterIncludeType.none) {
                                        
                                        if (filter.includeKeys.action == GameFilterIncludeType.all) {
                                            
                                            foreach (AppContentAction action in AppContentActions.Instance.GetListByPackAndState(
                                                GamePacks.Current.code,
                                                AppStates.Current.code)) {
                                                
                                                string key = GameStatisticCodes.genericCodeContentState(
                                                    filterCode,
                                                    action.code + "-" + AppContentStates.Current.code);
                                                
                                                // Add all
                                                codes.Add(key);
                                            }                                           
                                            
                                        }
                                        else if (filter.includeKeys.action == GameFilterIncludeType.current) {
                                            /*
                                            string key = GameStatisticCodes.genericCodeContentState(
                                                    filterCode,
                                                    action.code + "-" + AppContentStates.Current.code);
                                                
                                                // Add all
                                            codes.Add(filterCode);
                                            */
                                        }
                                    }
                                    
                                    /*
                                    if(filter.includeKeys.app_content_state != GameFilterIncludeType.none) {
                                        string key = GameStatisticCodes.genericCodeContentState(
                                            filterCode, 
                                            AppContentStates.Current.code);
                                        codes.Add(key);
                                    }
                                    */
                                }
                                
                                foreach (string filterCodeItem in codes) {
                                    
                                    if (CheckStatCondition(
                                        filterCodeItem, 
                                        StatEqualityTypeString.GetEnum(filter.compareType),
                                        (float)filter.compareValue
                                    )) {
                                        setAchievement = true;
                                    }
                                    else {
                                        setAchievement = false;
                                        break;
                                    }
                                }
                                
                                if (setAchievement) {
                                    GamePlayerProgress.Instance.SetAchievementAll(achievement.code);
                                }
                            }
                        }
                    }
                    else if (filterItem.type == GameFilterType.statisticLike) {
                        List<GameFilterBase> filterTypeItems = achievement.GetFilterStatisticLike();
                        foreach (GameFilterBase filter in filterTypeItems) {
                            if (filter != null) {
                                
                                // statistic-all
                                // 
                                // Collect all state values from metdata and if all are true set achievement
                                
                                bool setAchievement = false;
                                
                                List<string> codes = new List<string>();
                                
                                string likeCode = filter.codeLike;
                                
                                foreach (string filterCode in filter.codes) {                                
                                    
                                    if (filter.includeKeys.action != GameFilterIncludeType.none) {
                                        
                                        if (filter.includeKeys.action == GameFilterIncludeType.all) {
                                            
                                            foreach (AppContentAction action in AppContentActions.Instance.GetListByPackAndState(
                                                GamePacks.Current.code,
                                                AppStates.Current.code)) {
                                                
                                                if (action.code.IndexOf(likeCode) > -1) {
                                                    
                                                    string key = GameStatisticCodes.genericCodeContentState(
                                                        filterCode,
                                                        action.code + "-" + AppContentStates.Current.code);
                                                    
                                                    if (!codes.Contains(key)) {
                                                        // Add all
                                                        codes.Add(key);
                                                    }
                                                }
                                            }                                           
                                            
                                        }
                                    }
                                }
                                
                                foreach (string filterCodeItem in codes) {
                                    
                                    if (CheckStatCondition(
                                        filterCodeItem, 
                                        StatEqualityTypeString.GetEnum(filter.compareType),
                                        (float)filter.compareValue
                                    )) {
                                        setAchievement = true;
                                        break;
                                    }
                                }
                                
                                if (setAchievement) {
                                    GamePlayerProgress.Instance.SetAchievementAll(achievement.code);
                                }
                            }
                        }
                    }
                    else if (filterItem.type == GameFilterType.statisticCompare) {
                        List<GameFilterBase> filterTypeItems = achievement.GetFilterStatisticCompare();
                        foreach (GameFilterBase filter in filterTypeItems) {
                            if (filter != null) {
                                
                                // statistic-compare
                                // 
                                // Compare two stat values
                            }
                        }
                    }
                    else if (filterItem.type == GameFilterType.achievementSet) {
                        List<GameFilterBase> filterTypeItems = achievement.GetFilterAchievementSet();
                        foreach (GameFilterBase filter in filterTypeItems) {
                            if (filter != null) {
                                
                                // achievement-set
                                // 
                                // If multiple other achievements are set, set this achievement
                                
                                bool allSet = false;
                                
                                
                                
                                
                                
                                if (allSet) {
                                    //GamePlayerProgress.Instance.SetAchievement(filter.
                                }
                                
                                
                            }
                        }
                    }
                }
            }
        }
        
        
        GamePlayerProgressRuntimeData.Instance.EndRuntimeDataCollection();
        GameState.SaveProfile();
    }
    
    public virtual void ProcessPackAchievementsCurrentPack() {
        ProcessPackAchievements(GamePacks.Current.code);
    }
    
    public virtual void ProcessPackAchievements(string packCode) {
        
        LogUtil.Log("ProcessPackAchievements:" + packCode);
        
        foreach (GameAchievement achievement in GameAchievements.Instance.GetListByPack(packCode)) {
            if (achievement.data.filters != null) {
                foreach (GameFilter filterItem in achievement.data.filters) {
                    if (filterItem.type == GameFilterType.statisticSingle) {
                        List<GameFilterBase> filterTypeItems = achievement.GetFilterStatisticSingle();
                        foreach (GameFilterBase filter in filterTypeItems) {
                            if (filter != null) {
                                //LogUtil.Log("filter:" + filter.code);
                                
                                //double statisticValue = GameProfileStatistics.Current.GetStatisticValue(filter.code);
                                
                                
                                string filterCode = filter.codes[0];
                                
                                List<string> codes = new List<string>();
                                
                                // add explicit code
                                codes.Add(filterCode);
                                
                                if (filter.includeKeys.app_content_state != GameFilterIncludeType.none) {
                                    string key = GameStatisticCodes.genericCodeContentState(
                                        filterCode, 
                                        AppContentStates.Current.code);
                                    codes.Add(key);
                                }
                                
                                foreach (string filterCodeItem in codes) {
                                    
                                    CheckStatSetAchievement(
                                        false,
                                        achievement.code, 
                                        filterCodeItem, 
                                        StatEqualityTypeString.GetEnum(filter.compareType),
                                        (float)filter.compareValue
                                    );
                                }
                                
                            }
                        }
                    }
                    else if (filterItem.type == GameFilterType.statisticSet) {
                        List<GameFilterBase> filterTypeItems = achievement.GetFilterStatisticSet();
                        foreach (GameFilterBase filter in filterTypeItems) {
                            if (filter != null) {
                                //LogUtil.Log("filter:" + filter.code);
                                //LogUtil.Log("filter2:" + filter.code2);
                            }
                        }
                    }
                    else if (filterItem.type == GameFilterType.statisticAll) {
                        List<GameFilterBase> filterTypeItems = achievement.GetFilterStatisticAll();
                        foreach (GameFilterBase filter in filterTypeItems) {
                            if (filter != null) {
                                //LogUtil.Log("filter:" + filter.code);
                                //LogUtil.Log("filter2:" + filter.code2);
                            }
                        }
                    }
                }
            }
        }
    }
    
    public virtual void CheckStatSetAchievement(bool full, 
                                        string achievementKey, 
                                        string statKey, 
                                        StatEqualityTypeEnum equalType, 
                                        float checkValue) {
        
        double statValue = GameProfileStatistics.Current.GetStatisticValue(statKey);
        
        if (full) {
            
            string packCode = GamePacks.Current.code;
            string app_content_state = AppContentStates.Current.code;
            
            string key = statKey;   
            
            string keyPack = GameStatisticCodes.genericCodePack(
                statKey,
                packCode);      
            
            string keyPackContentState = GameStatisticCodes.genericCodePackContentState(
                statKey,
                packCode,
                app_content_state);
            
            string keyContentState = GameStatisticCodes.genericCodeContentState(
                statKey,
                app_content_state);
            
            LogUtil.Log("Check:key:" + key);
            statValue = GameProfileStatistics.Current.GetStatisticValue(key);           
            CheckStatSetAchievement(achievementKey, statValue, equalType, checkValue);
            
            LogUtil.Log("Check:keyPack:" + keyPack);
            //statValue = GameProfileStatistics.Current.GetStatisticValue(keyPack);         
            //CheckStatSetAchievement(achievementKey, statValue, equalType, checkValue);
            
            LogUtil.Log("Check:keyPackContentState:" + keyPackContentState);
            //statValue = GameProfileStatistics.Current.GetStatisticValue(keyPackContentState);         
            //CheckStatSetAchievement(achievementKey, statValue, equalType, checkValue);
            
            LogUtil.Log("Check:keyContentState:" + keyContentState);
            //statValue = GameProfileStatistics.Current.GetStatisticValue(keyContentState);         
            //CheckStatSetAchievement(achievementKey, statValue, equalType, checkValue);
        }
        else {
            CheckStatSetAchievement(achievementKey, statValue, equalType, checkValue);
        }
    }
    
    public virtual void CheckStatSetAchievement(bool full, 
                                        string achievementKey, 
                                        string statKey,  
                                        string statKey2, 
                                        StatEqualityTypeEnum equalType, 
                                        float checkValue) {
        
        double statValue = GameProfileStatistics.Current.GetStatisticValue(statKey);
        double statValue2 = GameProfileStatistics.Current.GetStatisticValue(statKey2);
        
        if (full) {
            CheckStatSetAchievementFull(achievementKey, statValue + statValue2, equalType, checkValue);
        }
        else {
            CheckStatSetAchievement(achievementKey, statValue + statValue2, equalType, checkValue);
        }
    }
    
    public virtual void CheckStatSetAchievementFull(string achievementKey, 
                                            double statValue, 
                                            StatEqualityTypeEnum equalType, 
                                            float checkValue) {
        // This checks not only the key but also pack, type, tracker and appcontent statekeys
        string app_content_state = AppContentStates.Current.code;
        
        string key = achievementKey;
        string keyContentState = achievementKey + "_" + app_content_state;
        
        CheckStatSetAchievement(key, statValue, equalType, checkValue); 
        CheckStatSetAchievement(keyContentState, statValue, equalType, checkValue);     
        
    }
    
    public virtual bool CheckStatCondition(string statKey, 
                                   StatEqualityTypeEnum equalType, 
                                   float checkValue) {
        double statValue = GameProfileStatistics.Current.GetStatisticValue(statKey);
        return CheckStatCondition(statValue, equalType, checkValue);
    }
    
    public virtual bool CheckStatCondition(double statValue, 
                                   StatEqualityTypeEnum equalType, 
                                   float checkValue) {
        
        if (equalType == StatEqualityTypeEnum.STAT_GREATER_THAN_OR_EQUAL_TO) {
            if (statValue >= checkValue) {
                return true;
            }
        }
        else if (equalType == StatEqualityTypeEnum.STAT_GREATER_THAN) {
            if (statValue > checkValue) {
                return true;
            }           
        }
        else if (equalType == StatEqualityTypeEnum.STAT_LESS_THAN_OR_EQUAL_TO) {
            if (statValue <= checkValue) {
                return true;
            }           
        }
        else if (equalType == StatEqualityTypeEnum.STAT_LESS_THAN) {
            if (statValue < checkValue) {
                return true;
            }           
        }
        else if (equalType == StatEqualityTypeEnum.EQUAL_TO) {
            if (statValue == checkValue) {
                return true;
            }           
        }   
        
        return false;
    }
    
    public virtual void CheckStatSetAchievement(string achievementKey, 
                                        double statValue, 
                                        StatEqualityTypeEnum equalType, 
                                        float checkValue) {
        //SetAchievementAll
        if (CheckStatCondition(statValue, equalType, checkValue)) {
            SetAchievementAll(achievementKey);
        }
    }
    
    public virtual void SetAchievementAll(string key) { 
        SetAchievementAll(key, true);
    }
    
    public virtual void SetAchievementAll(string key, bool completed) { 
        string packCode = GamePacks.Current.code;
        string app_content_state = AppContentStates.Current.code;
        SetAchievementAll(key, packCode, app_content_state, completed);
    }
    
    public virtual void SetAchievementAll(string key, string packCode, string app_content_state, bool completed) {    
        SetAchievement(key, completed);
        //SetAchievementContentState(key, app_content_state, completed);
    }
    
    public virtual void SetAchievementContentState(string key, string contentState, bool completed) {   
        key = GameAchievementCodes.genericCodeContentState(key, contentState);
        key = GameAchievementCodes.formatAchievementCode(key);
        SetAchievement(key, completed, true);
    }
    
    public virtual bool GetAchievement(string key) {
        return GameProfileAchievements.Current.GetAchievementValue(key);
    }
    
    public virtual void SetAchievement(string key, bool completed) {    
        SetAchievement(key, completed, true);
    }
    
    public virtual void SetAchievement(string key, bool completed, bool syncXP) {
        
        bool alreadyCompleted = GameProfileAchievements.Current.GetAchievementValue(key);
        if (!alreadyCompleted && completed) {
            GameProfileAchievements.Current.SetAchievementValue(key, completed);
            
            CoroutineUtil.Start(ReportAchievementCo(key, completed));
            CoroutineUtil.Start(QueueAchievementCo(key));
            if (syncXP) {
                //GameRPGMonitor.Instance.UpdateAchievementPointsScore();
                //GameRPGMonitor.Instance.SyncCurrentXP();
            }
        }
    }
    
    public IEnumerator ReportAchievementCo(string key, bool completed) {
        yield return null;
        GameNetworks.SendAchievement(key, completed);
        yield return null;
    }
    
    public IEnumerator QueueAchievementCo(string key) {
        yield return null;
        // TODO SEND NOTIFICATION
        if (UINotificationDisplay.Instance != null) {
            UINotificationDisplay.Instance.QueueAchievement(key);
        }
        Messenger<string>.Broadcast("queue-achievement", key);
        yield return null;
    }
    
    public virtual void SetStatisticValue(bool sendToGameverses, string key, object keyValue) {
        if (keyValue != null) {
            
            LogUtil.Log("SetStatisticValue:" + key + " :" + keyValue);
            
            GameProfileStatistics.Current.SetStatisticValue(key, keyValue);
            
            //LogUtil.Log("SetStatisticValue gameCenterEnabled:" + GameNetworks.gameCenterEnabled);

            if (GameConfigs.isGameRunning) {
                return;
            }

            long keyValueLong = 0;                
            double keyValueDouble = GameProfileStatistics.Current.GetStatisticValue(key);//Convert.ToDouble(keyValue);
            
            keyValueLong = (long)keyValueDouble;
            
            LogUtil.Log("SetStatisticValue:key:" + key);
            LogUtil.Log("SetStatisticValue:keyValueLong...:" + keyValueLong);
            
            if (keyValueLong > 0) {
                
                LogUtil.Log("SetStatisticValue:keyValueLong:" + keyValueLong);
                
                GameNetworks.SendScore(key, keyValueLong);
            }
        }
    }
    
    public string FilterThirdpartyNetworkLeaderboard(string key) {              
        //LogUtil.Log("GameCenter FilterGameCenterLeaderboard1:" + key);
        key = key.Replace("-", "_"); 
        //LogUtil.Log("GameCenter FilterGameCenterLeaderboard2:" + key);
        if (key == "fastest_lap" || key == "fastest_race") {
            string trackId = GameLevels.Current.code;
            if (!string.IsNullOrEmpty(trackId)) {
                trackId = trackId.Replace("-", "_");
                key = key + "_" + trackId;
                key = key.Replace("-", "_");
                key = key.ToLower();
            }
        }
        if (key.IndexOf("career_events") > -1) {
            // Fix for incorrect leaderboard to state value
            // 'career-events' to 'series'
            key = key.Replace("career_events", "series");
        }
        
        //LogUtil.Log("GameCenter FilterGameCenterLeaderboard3:" + key);
        return key;
    }
    
    public virtual bool IsGameCenterLeaderboard(string key) {
        foreach (GameLeaderboard leaderboard in GameLeaderboards.Instance.GetAll()) {
            if (leaderboard.code.ToLower() == key.ToLower()) {
                return true;
            }
        }
        
        return false;
    }
    
    public virtual void SetStatisticValue(string key, float keyValue) {
        GameStatistic statistic = GameStatistics.Instance.GetById(key);
        if (statistic != null) {
            string order = statistic.order;
            if (order == "ascending") {
                SetStatHighPoint(key, keyValue);
            }
            else if (order == "descending") {
                SetStatLowPoint(key, keyValue);         
            }
            else {
                SetStatAccumulate(key, keyValue);           
            }
        }
        else {
            SetStatAccumulate(key, keyValue);               
        }
    }
    
    public virtual void SetStatAbsolute(string key, float keyValue) {       
        SetStatAbsoluteData(false, key, keyValue);
    }
    
    public virtual void SetStatAccumulate(string key, float keyValue) {     
        SetStatAccumulateData(false, key, keyValue);
    }
    
    public virtual void SetStatHighPoint(string key, float keyValue) {  
        SetStatHighPointData(false, key, keyValue);
    }
    
    public virtual void SetStatLowPoint(string key, float keyValue) {   
        SetStatLowPointData(false, key, keyValue);
    }
    
    public virtual void SetStatAbsoluteData(
        bool sendToGameverses, string key, float keyValue) {
        SetStatisticValue(sendToGameverses, key, keyValue);
    }
    
    public virtual void SetStatAccumulateData(
        bool sendToGameverses, string key, float keyValue) {
        
        if (keyValue > 0.1f) {
            string statCode = key;
            float lastValue = keyValue;
            double currentValue =
                GameProfileStatistics.Current.GetStatisticValue(statCode);
            
            SetStatisticValue(sendToGameverses, statCode, currentValue + lastValue);
        }
    }
    
    public virtual void SetStatHighPointData(
        bool sendToGameverses, string key, float keyValue) {
        
        if (keyValue > 0.1f) {
            string statCode = key;
            float currentValue = keyValue;
            double lastValue =
                GameProfileStatistics.Current.GetStatisticValue(statCode);
            
            if (currentValue >= lastValue) {
                SetStatisticValue(sendToGameverses, statCode, currentValue);
            }   
        }
    }
    
    public virtual void SetStatLowPointData(
        bool sendToGameverses, string key, float keyValue) {
        
        if (keyValue > 0.1f) {           
            string statCode = key;
            float currentValue = keyValue;
            double lastValue =
                GameProfileStatistics.Current.GetStatisticValue(statCode);
            
            if (currentValue <= lastValue
                || lastValue == 0) {
                SetStatisticValue(sendToGameverses, statCode, currentValue);
            }   
        }
    }
    
    public double GetTotalAchievementPoints() {
        double totalScore = 0.0;
        foreach (GameAchievement achievementMeta
                in GameAchievements.Instance.GetAll()) {
            bool achievementValue =
                GamePlayerProgress.Instance.GetAchievement(achievementMeta.code);
            if (achievementValue) {
                totalScore += achievementMeta.data.points;
            }
        }   
        return totalScore;
    }
    
    // --------------------------------------------------------------------
    // HELPERS
    
    // evaded
    
    public static void SetStatEvaded(double val) {
        SetStatEvaded((float)val);
    }
    
    public static void SetStatEvaded(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatEvaded(val);
        }
    }
    
    float lastEvadeTime = 0;
    
    public virtual void setStatEvaded(float val) {
        if (lastEvadeTime + .2f < Time.time) {
            lastEvadeTime = Time.time;
            SetStatTotal(GameStatCodes.evaded, val);
        }
    }
    
    // deaths
    
    public static void SetStatDeaths(double val) {
        SetStatDeaths((float)val);
    }
    
    public static void SetStatDeaths(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatDeaths(val);
        }
    }
    
    float lastDeathsTime = 0;
    
    public virtual void setStatDeaths(float val) {
        if (lastDeathsTime + 2f < Time.time) {
            lastDeathsTime = Time.time;
            SetStatTotal(GameStatCodes.deaths, val);
        }
    }
    
    // hits
    
    public static void SetStatHits(double val) {
        SetStatHits((float)val);
    }
    
    public static void SetStatHits(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatHits(val);
        }
    }
    
    float lastHitsTime = 0;
    
    public virtual void setStatHits(float val) {
        if (lastHitsTime + .2f < Time.time) {
            lastHitsTime = Time.time;
            SetStatTotal(GameStatCodes.hits, val);
        }
    }
    
    // hitsRecieved
    
    public static void SetStatHitsReceived(double val) {
        SetStatHitsReceived((float)val);
    }
    
    public static void SetStatHitsReceived(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatHitsReceived(val);
        }
    }
    
    float lastHitsReceivedTime = 0;
    
    public virtual void setStatHitsReceived(float val) {
        if (lastHitsReceivedTime + .2f < Time.time) {
            lastHitsReceivedTime = Time.time;
            SetStatTotal(GameStatCodes.hitsReceived, val);
        }
    }
    
    // hitsObstacles
    
    public static void SetStatHitsObstacles(double val) {
        SetStatHitsObstacles((float)val);
    }
    
    public static void SetStatHitsObstacles(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatHitsObstacles(val);
        }
    }
    
    float lastHitsObstaclesTime = 0;
    
    public virtual void setStatHitsObstacles(float val) {
        if (lastHitsObstaclesTime + .2f < Time.time) {
            lastHitsObstaclesTime = Time.time;
            SetStatTotal(GameStatCodes.hitsObstacles, val);
        }
    }
    
    // kills
    
    public static void SetStatKills(double val) {
        SetStatKills((float)val);
    }
    
    public static void SetStatKills(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatKills(val);
        }
    }
    
    float lastKillsTime = 0;
    
    public virtual void setStatKills(float val) {
        if (lastKillsTime + .2f < Time.time) {
            lastKillsTime = Time.time;
            SetStatTotal(GameStatCodes.kills, val);
        }
    }
   
    
    // ammo
    
    public static void SetStatAmmo(double val) {
        SetStatAmmo((float)val);
    }
    
    public static void SetStatAmmo(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatAmmo(val);
        }
    }
    
    float lastAmmoTime = 0;
    
    public virtual void setStatAmmo(float val) {
        if (lastAmmoTime + .05f < Time.time) {
            lastAmmoTime = Time.time;
            SetStatTotal(GameStatCodes.ammo, val);
        }
    }

    // assets

    // asset attacks
    
    public static void SetStatAttacks(double val) {
        SetStatAttacks((float)val);
    }
    
    public static void SetStatAttacks(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatAttacks(val);
        }
    }
    
    float lastAttackTime = 0;
    
    public virtual void setStatAttacks(float val) {
        if (lastAttackTime + .05f < Time.time) {
            lastAttackTime = Time.time;
            SetStatTotal(GameStatCodes.attacks, val);
        }
    }

    // asset defends
    
    public static void SetStatDefends(double val) {
        SetStatDefends((float)val);
    }
    
    public static void SetStatDefends(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatDefends(val);
        }
    }
    
    float lastDefendsTime = 0;
    
    public virtual void setStatDefends(float val) {
        if (lastDefendsTime + .05f < Time.time) {
            lastDefendsTime = Time.time;
            SetStatTotal(GameStatCodes.defends, val);
        }
    }

    // asset builds
    
    public static void SetStatBuilds(double val) {
        SetStatBuilds((float)val);
    }
    
    public static void SetStatBuilds(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatBuilds(val);
        }
    }
    
    float lastBuildsTime = 0;
    
    public virtual void setStatBuilds(float val) {
        if (lastBuildsTime + .05f < Time.time) {
            lastBuildsTime = Time.time;
            SetStatTotal(GameStatCodes.builds, val);
        }
    }
    
    // asset builds
    
    public static void SetStatRepairs(double val) {
        SetStatRepairs((float)val);
    }
    
    public static void SetStatRepairs(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatRepairs(val);
        }
    }
    
    float lastRepairsTime = 0;
    
    public virtual void setStatRepairs(float val) {
        if (lastRepairsTime + .05f < Time.time) {
            lastRepairsTime = Time.time;
            SetStatTotal(GameStatCodes.repairs, val);
        }
    }
    
    // score
    
    public static void SetStatScore(double val) {
        SetStatScore((float)val);
    }
    
    public static void SetStatScore(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatScore(val);
        }
    }
    
    float lastScoreTime = 0;
    
    public virtual void setStatScore(float val) {
        if (lastScoreTime + .05f < Time.time) {
            lastScoreTime = Time.time;
            SetStatTotal(GameStatCodes.score, val);
        }
    }
    
    // scores
    
    public static void SetStatScores(double val) {
        SetStatScores((float)val);
    }
    
    public static void SetStatScores(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatScores(val);
        }
    }
    
    float lastScoresTime = 0;
    
    public virtual void setStatScores(float val) {
        if (lastScoresTime + .05f < Time.time) {
            lastScoresTime = Time.time;
            SetStatTotal(GameStatCodes.scores, val);
        }
    }
    
    // xp
    
    public static void SetStatXP(double val) {
        SetStatXP((float)val);
    }
    
    public static void SetStatXP(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatXP(val);
        }
    }
    
    float lastXPTime = 0;
    
    public virtual void setStatXP(float val) {
        if (lastXPTime + .05f < Time.time) {
            lastXPTime = Time.time;
            SetStatTotal(GameStatCodes.xp, val);
        }
    }
    
    // coins
    
    public static void SetStatCoins(double val) {
        SetStatCoins((float)val);
    }
    
    public static void SetStatCoins(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatCoins(val);
        }
    }
    
    float lastCoinsTime = 0;
    
    public virtual void setStatCoins(float val) {
        if (lastCoinsTime + .05f < Time.time) {
            lastCoinsTime = Time.time;
            SetStatTotal(GameStatCodes.coins, val);
        }
    }
    
    // coins pickup
    
    public static void SetStatCoinsPickup(double val) {
        SetStatCoinsPickup((float)val);
    }
    
    public static void SetStatCoinsPickup(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatCoinsPickup(val);
        }
    }
    
    float lastCoinsPickupTime = 0;
    
    public virtual void setStatCoinsPickup(float val) {
        if (lastCoinsPickupTime + .05f < Time.time) {
            lastCoinsPickupTime = Time.time;
            SetStatTotal(GameStatCodes.coinsPickup, val);
        }
    }
    
    // coins earned
    
    public static void SetStatCoinsEarned(double val) {
        SetStatCoinsEarned((float)val);
    }
    
    public static void SetStatCoinsEarned(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatCoinsEarned(val);
        }
    }
    
    float lastCoinsEarnedTime = 0;
    
    public virtual void setStatCoinsEarned(float val) {
        if (lastCoinsEarnedTime + .05f < Time.time) {
            lastCoinsEarnedTime = Time.time;
            SetStatTotal(GameStatCodes.coinsEarned, val);
        }
    }
    
    // boosts
    
    public static void SetStatBoosts(double val) {
        SetStatBoosts((float)val);
    }
    
    public static void SetStatBoosts(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatBoosts(val);
        }
    }
    
    float lastBoostsTime = 0;
    
    public virtual void setStatBoosts(float val) {
        if (lastBoostsTime + .05f < Time.time) {
            lastBoostsTime = Time.time;
            SetStatTotal(GameStatCodes.boosts, val);
        }
    }
    
    // spins
    
    public static void SetStatSpins(double val) {
        SetStatSpins((float)val);
    }
    
    public static void SetStatSpins(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatSpins(val);
        }
    }
    
    float lastSpinsTime = 0;
    
    public virtual void setStatSpins(float val) {
        if (lastSpinsTime + .05f < Time.time) {
            lastSpinsTime = Time.time;
            SetStatTotal(GameStatCodes.spins, val);
        }
    }
    
    // cuts
    
    public static void SetStatCuts(double val) {
        SetStatCuts((float)val);
    }
    
    public static void SetStatCuts(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatCuts(val);
        }
    }
    
    float lastCutsTime = 0;
    
    public virtual void setStatCuts(float val) {
        if (lastCutsTime + .05f < Time.time) {
            lastCutsTime = Time.time;
            SetStatTotal(GameStatCodes.cuts, val);
        }
    }
    
    // cutsRight
    
    public static void SetStatCutsRight(double val) {
        SetStatCutsRight((float)val);
    }
    
    public static void SetStatCutsRight(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatCutsRight(val);
        }
    }
    
    float lastCutsRightTime = 0;
    
    public virtual void setStatCutsRight(float val) {
        if (lastCutsRightTime + .05f < Time.time) {
            lastCutsRightTime = Time.time;
            SetStatTotal(GameStatCodes.cutsRight, val);
        }
    }
    
    // cutsLeft
    
    public static void SetStatCutsLeft(double val) {
        SetStatCutsLeft((float)val);
    }
    
    public static void SetStatCutsLeft(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatCutsLeft(val);
        }
    }
    
    float lastCutsLeftTime = 0;
    
    public virtual void setStatCutsLeft(float val) {
        if (lastCutsLeftTime + .05f < Time.time) {
            lastCutsLeftTime = Time.time;
            SetStatTotal(GameStatCodes.cutsLeft, val);
        }
    }
        
    // item collected
    
    public static void SetStatItems(string itemCode, double val) {
        SetStatItems(itemCode, (float)val);
    }
    
    public static void SetStatItems(string itemCode, float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatItems(itemCode, val);
        }
    }
    
    float lastItemsTime = 0;
    
    public virtual void setStatItems(string code, float val) {
        if (lastItemsTime + .05f < Time.time) {
            lastItemsTime = Time.time;
            SetStatTotal(code, val);
        }
    }
    
    // custom
    
    public static void SetStatCustom(string code, double val) {
        SetStatCustom(code, (float)val);
    }
    
    public static void SetStatCustom(string code, float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatCustom(code, val);
        }
    }
    
    float lastCustomTime = 0;
    
    public virtual void setStatCustom(string code, float val) {
        if (lastCustomTime + .05f < Time.time) {
            lastCustomTime = Time.time;
            SetStatTotal(code, val);
        }
    }    
        
    // highXP
    
    public static void SetStatHighXP(double val) {
        SetStatHighXP((float)val);
    }
    
    public static void SetStatHighXP(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatHighXP(val);
        }
    }
    
    float lastHighXPTime = 0;
    
    public virtual void setStatHighXP(float val) {
        if (lastHighXPTime + .05f < Time.time) {
            lastHighXPTime = Time.time;
            SetStatHigh(GameStatCodes.xp, val);
        }
    }
    
    // highScore
    
    public static void SetStatHighScore(double val) {
        SetStatHighScore((float)val);
    }
    
    public static void SetStatHighScore(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatHighScore(val);
        }
    }
    
    float lastHighScoreTime = 0;
    
    public virtual void setStatHighScore(float val) {
        if (lastHighScoreTime + .05f < Time.time) {
            lastHighScoreTime = Time.time;
            SetStatHigh(GameStatCodes.score, val);
        }
    }
    
    // highScores
    
    public static void SetStatHighScores(double val) {
        SetStatHighScores((float)val);
    }
    
    public static void SetStatHighScores(float val) {
        if (GamePlayerProgress.Instance != null) {
            GamePlayerProgress.Instance.setStatHighScores(val);
        }
    }
    
    float lastHighScoresTime = 0;
    
    public virtual void setStatHighScores(float val) {
        if (lastHighScoresTime + .05f < Time.time) {
            lastHighScoresTime = Time.time;
            SetStatHigh(GameStatCodes.scores, val);
        }
    }
}


