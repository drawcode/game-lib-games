using UnityEngine;
using System.Collections;

public class GameWeaponController : MonoBehaviour {
    public string[] TargetTag = new string[1] { "Enemy" };
    public GameWeaponLauncher[] WeaponLists;
    public int CurrentWeapon = 0;
    public bool ShowCrosshair;

    void Awake() {
        // find all attached weapons.
        if (this.transform.GetComponentsInChildren(typeof(GameWeaponLauncher)).Length > 0) {
            var weas = this.transform.GetComponentsInChildren(typeof(GameWeaponLauncher));
            WeaponLists = new GameWeaponLauncher[weas.Length];
            for (int i = 0; i < weas.Length; i++) {
                WeaponLists[i] = weas[i].GetComponent<GameWeaponLauncher>();
                WeaponLists[i].TargetTag = TargetTag;
            }
        }
    }

    public GameWeaponLauncher GetCurrentWeapon() {
        if (CurrentWeapon < WeaponLists.Length && WeaponLists[CurrentWeapon] != null) {
            return WeaponLists[CurrentWeapon];
        }
        return null;
    }

    private void Start() {
        for (int i = 0; i < WeaponLists.Length; i++) {
            if (WeaponLists[i] != null) {
                WeaponLists[i].TargetTag = TargetTag;
                WeaponLists[i].ShowCrosshair = ShowCrosshair;
            }
        }
    }

    void Update() {
        if (Input.GetButton("Fire1")) {
            LaunchWeapon();
        }

        for (int i = 0; i < WeaponLists.Length; i++) {
            if (WeaponLists[i] != null) {
                WeaponLists[i].OnActive = false;
            }
        }
        if (CurrentWeapon < WeaponLists.Length && WeaponLists[CurrentWeapon] != null) {
            WeaponLists[CurrentWeapon].OnActive = true;
        }

    }

    public void LaunchWeapon(int index) {
        CurrentWeapon = index;
        if (CurrentWeapon < WeaponLists.Length && WeaponLists[index] != null) {
            WeaponLists[index].Shoot();
        }
    }

    public void SwitchWeapon() {
        CurrentWeapon += 1;
        if (CurrentWeapon >= WeaponLists.Length) {
            CurrentWeapon = 0;
        }
    }

    public void LaunchWeapon() {
        if (CurrentWeapon < WeaponLists.Length && WeaponLists[CurrentWeapon] != null) {
            WeaponLists[CurrentWeapon].Shoot();
        }
    }
}
