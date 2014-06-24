using UnityEngine;
using System.Collections;

public class GameDamageBase : MonoBehaviour {

	public GameObject Effect;
	[HideInInspector]
    public GameObject Owner;
    public int Damage = 20;
	
	public string[] TargetTag = new string[1]{"Enemy"};
}

public class GameWeaponBase : MonoBehaviour {
	[HideInInspector]
    public GameObject Owner;
	[HideInInspector]
	public GameObject Target;
	
    public string[] TargetTag = new string[1]{"Enemy"};
	public bool RigidbodyProjectile;
	public Vector3 TorqueSpeedAxis;
	public GameObject TorqueObject;
}

