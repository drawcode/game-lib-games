using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Game.App;
using UnityEngine;

public class BaseGamePlayerAttributeTypes
{
	public static string attack = "att-attack";
	public static string defense = "att-defense";
	public static string health = "att-health";
	public static string energy = "att-energy";
	public static string currency = "att-currency";
	public static string speed = "att-speed";
	public static string recharge = "att-recharge";
	public static string decay = "att-decay";
}

public class BaseGamePlayerAttributes : GameDataObject
{

	public BaseGamePlayerAttributes()
	{
		Reset();
	}

	public override void Reset()
	{
		base.Reset();
	}

	// attack

	public virtual double GetAttack()
	{
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.attack);
	}

	public virtual void SetAttack(double attValue)
	{
		SetAttributeDoubleValue(GamePlayerAttributeTypes.attack, attValue);
	}

	// defense

	public virtual double GetDefense()
	{
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.defense);
	}

	public virtual void SetDefense(double attValue)
	{
		SetAttributeDoubleValue(GamePlayerAttributeTypes.defense, attValue);
	}

	// health

	public virtual double GetHealth()
	{
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.health);
	}

	public virtual void SetHealth(double attValue)
	{
		SetAttributeDoubleValue(GamePlayerAttributeTypes.health, attValue);
	}

	// energy

	public virtual double GetEnergy()
	{
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.energy);
	}

	public virtual void SetEnergy(double attValue)
	{
		SetAttributeDoubleValue(GamePlayerAttributeTypes.energy, attValue);
	}

	// currency

	public virtual double GetCurrency()
	{
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.currency);
	}

	public virtual void SetCurrency(double attValue)
	{
		SetAttributeDoubleValue(GamePlayerAttributeTypes.currency, attValue);
	}

	// speed

	public virtual double GetSpeed()
	{
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.speed);
	}

	public virtual void SetSpeed(double attValue)
	{
		SetAttributeDoubleValue(GamePlayerAttributeTypes.speed, attValue);
	}

	// recharge

	public virtual double GetRecharge()
	{
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.recharge);
	}

	public virtual void SetRecharge(double attValue)
	{
		SetAttributeDoubleValue(GamePlayerAttributeTypes.recharge, attValue);
	}

	// decay

	public virtual double GetDecay()
	{
		return this.GetAttributeDoubleValue(GamePlayerAttributeTypes.decay);
	}

	public virtual void SetDecay(double attValue)
	{
		SetAttributeDoubleValue(GamePlayerAttributeTypes.decay, attValue);
	}
}
