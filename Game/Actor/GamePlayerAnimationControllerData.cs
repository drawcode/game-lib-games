using System;
using System.Collections;
using System.Collections.Generic;

using Engine;
using Engine.Data;
using Engine.Game.Controllers;
using Engine.Networking;
using Engine.Utility;

using UnityEngine;

public class GamePlayerAnimationControllerData {
	public string skill = "emo_0";
	public string jump = "jump_0";
	public string run = "run_0";
	public string walk = "walk_0";
	public string attack = "action_0";
	public string idle = "idle_0";
	public string hit = "hit_0";
	public string death = "death_0";
	
	public int skillNum = 0;
	public int jumpNum = 0;
	public int idleNum = 0;
	public int attackNum = 0;
	public int runNum = 0;
	public int walkNum = 0;
	public int hitNum = 0;	
	public int deathNum = 0;
	
	
	// SKILL
	
	public int SkillCount() {
		return 6;
	}
	
	public string Skill() {
		if(skillNum == 0) {
			skillNum = UnityEngine.Random.Range(1, SkillCount());
		}	
		return Skill(skillNum);
	}
	
	public string Skill(int num) {
		return skill + num;
	}
	
	// IDLE
	
	public int JumpCount() {
		return 3;
	}
	
	public string Jump() {
		if(jumpNum == 0) {
			jumpNum = UnityEngine.Random.Range(1, JumpCount());
		}	
		return Jump(jumpNum);
	}
	
	public string Jump(int num) {
		return jump + num;
	}
	
	// IDLE
	
	public int IdleCount() {
		return 5;
	}
	
	public string Idle() {
		if(idleNum == 0) {
			idleNum = UnityEngine.Random.Range(1, IdleCount());
		}
		return Idle(idleNum);
	}
	
	public string Idle(int num) {
		return idle + num;
	}
	
	// ATTACK
	
	public int AttackCount() {
		return 5;
	}
	
	public string Attack() {
		if(attackNum == 0) {
			attackNum = UnityEngine.Random.Range(1, AttackCount());
		}	
		return Attack(attackNum);
	}
	
	public string Attack(int num) {
		return attack + num;
	}
	
	// RUN
	
	public int RunCount() {
		return 3;
	}
	
	public string Run() {
		if(runNum == 0) {
			runNum = UnityEngine.Random.Range(1, RunCount());
		}	
		return Run(runNum);
	}
	
	public string Run(int num) {
		return run + num;
	}
	
	// WALK
	
	public int WalkCount() {
		return 5;
	}
	
	public string Walk() {
		if(walkNum == 0) {
			walkNum = UnityEngine.Random.Range(1, WalkCount());
		}	
		return Walk(walkNum);
	}
	
	public string Walk(int num) {
		return walk + num;
	}
	
	// HIT
	
	public int HitCount() {
		return 3;
	}
	
	public string Hit() {
		if(hitNum == 0) {
			hitNum = UnityEngine.Random.Range(1, HitCount());
		}	
		return Hit(hitNum);
	}
	
	public string Hit(int num) {
		return hit + num;
	}
		
	// DEATH
	
	public int DeathCount() {
		return 2;
	}
	
	public string Death() {
		if(deathNum == 0) {
			deathNum = UnityEngine.Random.Range(1, DeathCount());
		}	
		return Death(deathNum);
	}
	
	public string Death(int num) {
		return death + num;
	}
}