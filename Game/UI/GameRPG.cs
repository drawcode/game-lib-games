using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class GameRPGPoints {
	public static double XP_BUMP = 3;
	public static double XP_LAP = 3;
	public static double XP_RACE = 5;
	public static double XP_OPEN_GAME_TODAY = 10;
	public static double XP_BOOST = 2;
	
	public static double XP_WIN = 10;
	public static double XP_LOSS = -5;	
	
	public static double XP_MUD = -1;
	public static double XP_COLLIDE = -1;
	
	public static double XP_PASS = 3;
	public static double XP_PASSED = 0;
}

public class GameRPGThread {
	public GameRPGThread() {
	
	
	}
}

public class GameRPGMonitor {
	
	private static volatile GameRPGMonitor instance;
	private static System.Object syncRoot = new System.Object();
	
	Thread syncThread;
	
	public static GameRPGMonitor Instance {
	  get {
	     if (instance == null) {
	        lock (syncRoot) {
	           if (instance == null) 
	              instance = new GameRPGMonitor();
	        }
	     }
	
	     return instance;
	  }
	}
		
	public double currentTotalScore = 0;
	public double lastTotalScore = -1;
	
	public bool hasRaceModeEndlessUpdatedData = true;
	public bool hasRaceModeArcadeUpdatedData = true;
	public bool hasRaceModeSeriesUpdatedData = true;
	
	public bool hasBonusXPUpdatedData = true;
	public bool hasFirstPlaceUpdatedData = true;
	public bool hasSecondPlaceUpdatedData = true;
	public bool hasThirdPlaceUpdatedData = true;
	
	public bool hasTotalAchievementPointsUpdatedData = true;
	public bool hasTimesPlayedPointsUpdatedData = true;
	public bool hasTimePlayedPointsUpdatedData = true;
	
	public double currentTotalScoreEndless = 0;
	public double lastTotalScoreEndless = -1;
	
	public double currentTotalScoreArcade = 0;
	public double lastTotalScoreArcade = -1;
	
	public double currentTotalScoreSeries = 0;
	public double lastTotalScoreSeries = -1;
		
	public double currentTotalScoreAchievements = 0;
	public double lastTotalScoreAchievements = -1;
	
	public double currentTotalScoreTimePlayed = 0;
	public double lastTotalScoreTimePlayed = -1;
	
	public double currentTotalScoreTimesPlayed = 0;
	public double lastTotalScoreTimesPlayed = -1;
	
	public double currentTotalXPScore = 0;
	public double lastTotalXPScore = -1;
	
	public double currentTotalScoreBonusXP = 0;
	public double lastTotalScoreBonusXP = -1;
	
	public double currentTotalScoreFirstPlace = 0;
	public double lastTotalScoreFirstPlace = -1;
	
	public double currentTotalScoreSecondPlace = 0;
	public double lastTotalScoreSecondPlace = -1;
	
	public double currentTotalScoreThirdPlace = 0;
	public double lastTotalScoreThirdPlace = -1;
		
	public bool scoreUpdating = false;
	
	public void UpdateScore() {
		UpdateArcadeScore();
		UpdateEndlessScore();
		UpdateSeriesScore();
		SyncCurrentXP();
	}
	
	public void UpdateStatistics() {
		UpdateAchievementPointsScore();
		UpdateBonusXPScore();
		UpdateFirstPlaceScore();
		UpdateSecondPlaceScore();
		UpdateThirdPlaceScore();
		UpdateTimePlayedPointsScore();
		UpdateTimesPlayedPointsScore();
		SyncCurrentXP();
	}
	
	public void UpdateBonusXPScore() {
		hasBonusXPUpdatedData = true;
	}
	
	public void UpdateAchievementPointsScore() {
		hasTotalAchievementPointsUpdatedData = true;
	}
	
	public void UpdateTimePlayedPointsScore() {
		hasTimePlayedPointsUpdatedData = true;
	}
	
	public void UpdateTimesPlayedPointsScore() {
		hasTimesPlayedPointsUpdatedData = true;
	}
	
	public void UpdateFirstPlaceScore() {
		hasFirstPlaceUpdatedData = true;
	}
	
	public void UpdateSecondPlaceScore() {
		hasSecondPlaceUpdatedData = true;
	}
	
	public void UpdateThirdPlaceScore() {
		hasThirdPlaceUpdatedData = true;
	}	
	
	public void UpdateArcadeScore() {
		hasRaceModeArcadeUpdatedData = true;
	}
	
	public void UpdateEndlessScore() {
		hasRaceModeEndlessUpdatedData = true;
	}
	
	public void UpdateSeriesScore() {
		hasRaceModeSeriesUpdatedData = true;
	}
	
	public void SetIncrementXP(double amount) {		
		scoreUpdating = true;
		double currentXP = GameProfileRPGs.Current.GetGamePlayerProgressXP();
		currentXP += amount;
		GameProfileRPGs.Current.SetGamePlayerProgressXP(currentXP);
		UpdateBonusXPScore();
		//LogUtil.Log("GameRPG:SetIncrementXP: currentXP:" + currentXP);
		SyncCurrentXP();
	}
	
	public void InitCurrentXP() {
		scoreUpdating = true;
		currentTotalScore = InitTotalScore();
		lastTotalScore = currentTotalScore;
		//LogUtil.Log("GameRPG:InitCurrentXP: currentTotalScore:" + currentTotalScore);
		//LogUtil.Log("GameRPG:InitCurrentXP: lastTotalScore:" + lastTotalScore);
		scoreUpdating = false;
	}
	
	public void SyncCurrentXP() {
		scoreUpdating = true;
		//LogUtil.Log("GameRPG:SyncCurrentXPCo: scoreUpdating:" + scoreUpdating);
		 SyncTotalScore();
	}
	
	
	public double InitTotalScore() {	
		
		double total = 0;
		
		// tally achievement points
		
		total += GetRPGTotalAchievementPoints();
		
		// tally times played
		
		total += GetRPGTotalTimesPlayedPoints();
			
		// tally time played
		
		total += GetRPGTotalTimePlayedPoints();
		
		// tally total first place wins		
		
		//total += GetRPGFirstPlacePoints();
		
		//total += GetRPGSecondPlacePoints();
		
		//total += GetRPGThirdPlacePoints();	
		
		// tally xperience points bonus from micro actions	
		
		//total += GetRPGBonusXP();
		
		// tally arcade mode points
		
		//total += GetRPGArcadeModePoints();
		
		// tally series mode points
		
		//total += GetRPGSeriesModePoints();
		
		// tally endless mode points
		
		//total += GetRPGEndlessModePoints();
		
		return Math.Floor(total);		
	}
	
	public void SyncTotalScore() {

		//CoroutineUtil.Start(UpdateTotalScoreCo());
		syncThread = new Thread(UpdateTotalScore);
		syncThread.Start();
	}
		
	public void UpdateTotalScore() {	
		
		double total = 0;
		
		// tally achievement points
		
		total += GetRPGTotalAchievementPoints();
				
		// tally times played
		
		total += GetRPGTotalTimesPlayedPoints();
						
		// tally time played
		
		total += GetRPGTotalTimePlayedPoints();
				
		// tally total first place wins		
		
		//total += GetRPGFirstPlacePoints();
				
		//total += GetRPGSecondPlacePoints();
				
		//total += GetRPGThirdPlacePoints();
				
		// tally xperience points bonus from micro actions	
		
		//total += GetRPGBonusXP();
				
		// tally arcade mode points
		
		//total += GetRPGArcadeModePoints();
				
		// tally series mode points
		
		//total += GetRPGSeriesModePoints();
				
		// tally endless mode points
		
		//total += GetRPGEndlessModePoints();
						
		currentTotalScore = total;
		
		scoreUpdating = false;
		//LogUtil.Log("GameRPG:SyncCurrentXPCo: scoreUpdating2:" + scoreUpdating);
}
	
		
	public double GetRPGTotalAchievementPoints() {
		double score = currentTotalScoreAchievements;
		double scoreLast = lastTotalScoreAchievements;
		if(score != scoreLast 
			|| hasTotalAchievementPointsUpdatedData) {
#if !UNITY_FLASH
				score = GamePlayerProgress.Instance.GetTotalAchievementPoints();
#endif
				hasTotalAchievementPointsUpdatedData = false;
				currentTotalScoreAchievements = score;
				lastTotalScoreAchievements = score;
		}
		return Math.Floor(score);
	}
	
	public double GetRPGTotalTimesPlayedPoints() {
		double score = currentTotalScoreTimesPlayed;
		double scoreLast = lastTotalScoreTimesPlayed;
		if(score != scoreLast 
			|| hasTimesPlayedPointsUpdatedData) {
				score = GameProfileStatistics.Current.GetStatisticValue(GameStatistics.STAT_TOTAL_TIMES_PLAYED) * 1;
				hasTimesPlayedPointsUpdatedData = false;
				currentTotalScoreTimesPlayed = score;
				lastTotalScoreTimesPlayed = score;
		}
		return Math.Floor(score);
	}
	
	public double GetRPGTotalTimePlayedPoints() {
		double score = currentTotalScoreTimePlayed;
		double scoreLast = lastTotalScoreTimePlayed;
		if(score != scoreLast 
			|| hasTimePlayedPointsUpdatedData) {
				score = GameProfileStatistics.Current.GetStatisticValue(GameStatistics.STAT_TOTAL_TIME_PLAYED) * .05;
				hasTimePlayedPointsUpdatedData = false;
				currentTotalScoreTimePlayed = score;
				lastTotalScoreTimePlayed = score;
		}
		return Math.Floor(score);
	}
	
	/*
	public double GetRPGFirstPlacePoints() {
		double score = currentTotalScoreFirstPlace;
		double scoreLast = lastTotalScoreFirstPlace;
		if(score != scoreLast 
			|| hasFirstPlaceUpdatedData) {
				score = GameProfileStatistics.Current.GetStatisticValue(GameStatistics.STAT_FINISHES_FIRST) * 10;
				hasFirstPlaceUpdatedData = false;
				currentTotalScoreFirstPlace = score;
				lastTotalScoreFirstPlace = score;
		}
		return Math.Floor(score);
	}
	
	public double GetRPGSecondPlacePoints() {
		double score = currentTotalScoreSecondPlace;
		double scoreLast = lastTotalScoreSecondPlace;
		if(score != scoreLast 
			|| hasSecondPlaceUpdatedData) {
				score = GameProfileStatistics.Current.GetStatisticValue(GameStatistics.STAT_FINISHES_SECOND) * 5;
				hasSecondPlaceUpdatedData = false;
				currentTotalScoreSecondPlace = score;
				lastTotalScoreSecondPlace = score;
		}
		return Math.Floor(score);
	}
	
	public double GetRPGThirdPlacePoints() {
		double score = currentTotalScoreThirdPlace;
		double scoreLast = lastTotalScoreThirdPlace;
		if(score != scoreLast 
			|| hasThirdPlaceUpdatedData) {
				score = GameProfileStatistics.Current.GetStatisticValue(GameStatistics.STAT_FINISHES_THIRD) * 1;
				hasThirdPlaceUpdatedData = false;
				currentTotalScoreThirdPlace = score;
				lastTotalScoreThirdPlace = score;
		}
		return Math.Floor(score);
	}
	
	public double GetRPGBonusXP() {
		double score = currentTotalScoreBonusXP;
		double scoreLast = lastTotalScoreBonusXP;
		if(score != scoreLast 
			|| hasBonusXPUpdatedData) {
				score = GameProfileStatistics.Current.GetProgressXP();
				hasBonusXPUpdatedData = false;
				currentTotalScoreBonusXP = score;
				lastTotalScoreBonusXP = score;
		}
		return Math.Floor(score);		
	}
	
	public double GetRPGArcadeModePoints() {
		double score = currentTotalScoreArcade;
		double scoreLast = lastTotalScoreArcade;
		if(score != scoreLast 
			|| hasRaceModeArcadeUpdatedData) {
			//foreach(GameLevel level in GameLevels.Instance.GetAll()) {
				//score += GamePlayerProgress.Instance.ProcessArcadeScore(level.code);
				hasRaceModeArcadeUpdatedData = false;
				currentTotalScoreArcade = score;
				lastTotalScoreArcade = score;
			//}
		}
		return Math.Floor(score);
	}
	
	public double GetRPGEndlessModePoints() {
		double score = currentTotalScoreEndless;
		double scoreLast = lastTotalScoreEndless;
		if(score != scoreLast 
			|| hasRaceModeEndlessUpdatedData) {
			//score = GameProfiles.Current.GetCurrentGameModeEndlessResultSet().GetTotalScore();
			hasRaceModeEndlessUpdatedData = false;
			currentTotalScoreEndless = score;
			lastTotalScoreEndless = score;
		}
		return Math.Floor(score);
	}
	
	public double GetRPGSeriesModePoints() {
		double score = currentTotalScoreSeries;
		double scoreLast = lastTotalScoreSeries;
		if(score != scoreLast 
			|| hasRaceModeSeriesUpdatedData) {
				
			//score += GameSeriesEvents.Instance.GetTotalScore();
			
			hasRaceModeSeriesUpdatedData = false;
			currentTotalScoreSeries = score;
			lastTotalScoreSeries = score;
		}
		return Math.Floor(score);
	}
	*/

}

public class GameRPG : MonoBehaviour {
	
	public static GameRPG Instance;
	
	public UILabel labelXPValue;
	
	public GameRPGMonitor rpgMonitor = GameRPGMonitor.Instance;
	
	void Awake() {
		if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(this);
            return;
        }
		
        Instance = this;
		
		DontDestroyOnLoad(gameObject);
		
		Init();
	}
	
	public void Init() {
		GameRPGMonitor.Instance.InitCurrentXP();
		SetXPDisplay(GameRPGMonitor.Instance.lastTotalScore);
	}	
	
	public static void IncrementXP(double amount) {
		if(GameRPGMonitor.Instance != null) {
			GameRPGMonitor.Instance.SetIncrementXP(amount);
		}
	}
	
	public void SyncTotalScore() {
		rpgMonitor.SyncTotalScore();
		UpdateXPDisplay();
	}
	
	public void SetXPDisplay(double score) {
		if(labelXPValue) {			
			labelXPValue.text = score.ToString("#,##0");
		}
		
		//LogUtil.Log("GameRPG:SetXPDisplay: score:" + score);
	}
	
	public void UpdateXPDisplay() {
		if(labelXPValue
		   && !rpgMonitor.scoreUpdating) {	
			
			
			//LogUtil.Log("GameRPG:UpdateXPDisplay: scoreUpdating:" + scoreUpdating);
			//LogUtil.Log("GameRPG:UpdateXPDisplay: labelXPValue:" + labelXPValue);
			
			int lastScoreInt = Convert.ToInt32(rpgMonitor.lastTotalScore);
			int currentScoreInt = Convert.ToInt32(rpgMonitor.currentTotalScore);
			
			if((lastScoreInt + 1) == currentScoreInt) {
				rpgMonitor.lastTotalScore += 1;
				SetXPDisplay(rpgMonitor.lastTotalScore);
			}
			else if((lastScoreInt - 1) == currentScoreInt) {
				rpgMonitor.lastTotalScore -= 1;
				SetXPDisplay(rpgMonitor.lastTotalScore);
			}
			else {
				if(currentScoreInt != lastScoreInt
				   && lastScoreInt < currentScoreInt) {				
					rpgMonitor.lastTotalScore += 1;
					SetXPDisplay(rpgMonitor.lastTotalScore);
				}			
				else if(currentScoreInt != lastScoreInt
				   && lastScoreInt > currentScoreInt) {				
					rpgMonitor.lastTotalScore -= 1;
					SetXPDisplay(rpgMonitor.lastTotalScore);
				}
			}
		}
	}
	
	void LateUpdate() {
		UpdateXPDisplay();
	}
		
	
}

