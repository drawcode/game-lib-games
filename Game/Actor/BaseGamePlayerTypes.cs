using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Animation;
using UnityEngine;

public class BaseGamePlayerAttributeTypes {
	public static string attack = "att-attack";
	public static string defense = "att-defense";
	public static string health = "att-health";
	public static string energy = "att-energy";
	public static string currency = "att-currency";
	public static string speed = "att-speed";
	public static string recharge = "att-recharge";
	public static string decay = "att-decay";
}

public class BaseGamePlayerAttributes : GameDataObject {
	
	public BaseGamePlayerAttributes() {
		Reset();
	}
	
	public override void Reset () {
		base.Reset ();
	}	
	
	// attack
	
	public double GetAttack() {
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.attack);
	}
	
	public void SetAttack(double attValue) {
		SetAttributeDoubleValue(GamePlayerAttributeTypes.attack, attValue);
	}
	
	// defense
	
	public double GetDefense() {
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.defense);
	}
	
	public void SetDefense(double attValue) {
		SetAttributeDoubleValue(GamePlayerAttributeTypes.defense, attValue);
	}
	
	// health
	
	public double GetHealth() {
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.health);
	}
	
	public void SetHealth(double attValue) {
		SetAttributeDoubleValue(GamePlayerAttributeTypes.health, attValue);
	}
		
	// energy
	
	public double GetEnergy() {
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.energy);
	}
	
	public void SetEnergy(double attValue) {
		SetAttributeDoubleValue(GamePlayerAttributeTypes.energy, attValue);
	}
			
	// currency
	
	public double GetCurrency() {
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.currency);
	}
	
	public void SetCurrency(double attValue) {
		SetAttributeDoubleValue(GamePlayerAttributeTypes.currency, attValue);
	}
				
	// speed
	
	public double GetSpeed() {
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.speed);
	}
	
	public void SetSpeed(double attValue) {
		SetAttributeDoubleValue(GamePlayerAttributeTypes.speed, attValue);
	}
				
	// recharge
	
	public double GetRecharge() {
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.recharge);
	}
	
	public void SetRecharge(double attValue) {
		SetAttributeDoubleValue(GamePlayerAttributeTypes.recharge, attValue);
	}	
				
	// decay
	
	public double GetDecay() {
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.decay);
	}
	
	public void SetDecay(double attValue) {
		SetAttributeDoubleValue(GamePlayerAttributeTypes.decay, attValue);
	}
}
