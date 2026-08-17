using UnityEngine;
using System.Collections;

public class GameDamageBase : GameObjectBehavior {

	public GameObject Effect;
	[HideInInspector]
	public GamePlayerController gamePlayerController;
	public int Damage = 20;

	public string[] TargetTag = new string[1] { "Enemy" };

	/// <summary>
	/// Called by the launcher once this projectile's owner and target tags have been
	/// wired for THIS shot. The pool re-sends Start before any of that is assigned, so
	/// anything that depends on knowing who fired it belongs here, not in Start.
	/// </summary>
	public virtual void OnLaunched() {

	}
}

public class GameWeaponBase : MonoBehaviour {
	[HideInInspector]
	public GamePlayerController gamePlayerController;
	[HideInInspector]
	public GameObject Target;

	public string[] TargetTag = new string[1] { "Enemy" };
	public bool RigidbodyProjectile;
	public Vector3 TorqueSpeedAxis;
	public GameObject TorqueObject;

}

