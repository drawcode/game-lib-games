#pragma warning disable 0108
using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]

public class GameWeaponLauncher : GameWeaponBase {
    public bool OnActive;
    public Transform[] MissileOuter;
    public GameObject Missile;
    public float FireRate = 0.1f;
    public float Spread = 1;
    public float ForceShoot = 8000;
    public int NumBullet = 1;
    public int Ammo = 10;
    public int AmmoMax = 10;
    public bool InfinityAmmo = false;
    public float ReloadTime = 1;
    public bool ShowHUD = true;
    public int MaxAimRange = 10000;
    public bool ShowCrosshair;
    public Texture2D CrosshairTexture;
    public Texture2D TargetLockOnTexture;
    public Texture2D TargetLockedTexture;
    public float DistanceLock = 200;
    public float TimeToLock = 2;
    public float AimDirection = 0.8f;
    public bool Seeker;
    public GameObject Shell;
    public float ShellLifeTime = 2;
    public Transform[] ShellOuter;
    public int ShellOutForce = 300;
    public GameObject Muzzle;
    public float MuzzleLifeTime = 2;
    public AudioClip[] SoundGun;
    public AudioClip SoundReloading;
    public AudioClip SoundReloaded;
    private float timetolockcount = 0;
    private float nextFireTime = 0;

    // GameProfiles.Current.GetAudioEffectsVolume() walks the profile's attribute
    // dictionary and boxes a double. It was being called once per shot -- 25 times a
    // second for the minigun -- purely to set a value that a player changes from a
    // settings slider. Sample it a few times a second instead.

    private float cachedEffectsVolume = -1f;
    private float cachedEffectsVolumeTime = -99999f;
    private const float effectsVolumeRefreshSeconds = 0.5f;
    private GameObject target;
    private Vector3 torqueTemp;
    private float reloadTimeTemp;
    private AudioSource audio;
    //
    //
    [HideInInspector]
    public bool
        Reloading;
    [HideInInspector]
    public float
        ReloadingProcess;

    private void Start() {

        if (!audio) {
            audio = this.GetComponent<AudioSource>();
            if (!audio) {
                // The result was being discarded, so on a weapon without an AudioSource
                // this added a fresh one on every Start and still left audio null, which
                // silenced the gun. [RequireComponent] normally prevents that case.
                audio = this.gameObject.AddComponent<AudioSource>();
            }
        }

    }

    [HideInInspector]
    public Vector3
        AimPoint;
    [HideInInspector]
    public GameObject
        AimObject;

    private void rayAiming() {

        RaycastHit hit;

        if (Physics.Raycast(transform.position, this.transform.forward, out hit, MaxAimRange)) {

            if (Missile != null && hit.collider.tag != Missile.tag) {

                AimPoint = hit.point;
                AimObject = hit.collider.gameObject;
            }
        }
        else {

            AimPoint = this.transform.position + (this.transform.forward * MaxAimRange);
            AimObject = null;
        }

    }

    void FixedUpdate() {

        // Run WHILE the game is running, like every other gameplay gate in the actor
        // layer. These two were the only un-negated ones, which left the aim raycast
        // and the reload timer running only when the game was not being played.

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (OnActive) {
            rayAiming();
        }
    }

    private void Update() {

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (CurrentCamera == null) {

            CurrentCamera = Camera.main;

            if (CurrentCamera == null)
                CurrentCamera = Camera.current;
        }
        if (OnActive) {

            if (TorqueObject) {

                TorqueObject.transform.Rotate(torqueTemp * Time.deltaTime);
                torqueTemp = Vector3.Lerp(torqueTemp, Vector3.zero, Time.deltaTime);
            }
            if (Seeker) {

                for (int t = 0; t < TargetTag.Length; t++) {

                    // One tag sweep per tag per FRAME, shared with every other seeker --
                    // this used to call the allocating FindGameObjectsWithTag twice, per
                    // weapon, per frame.

                    GameObject[] objs = GameWeaponTargets.GetByTag(TargetTag[t]);

                    if (objs.Length > 0) {

                        float distance = int.MaxValue;

                        if (AimObject != null && AimObject.tag == TargetTag[t]) {

                            float dis = Vector3.Distance(AimObject.transform.position, transform.position);

                            if (DistanceLock > dis) {

                                if (distance > dis) {

                                    if (timetolockcount + TimeToLock < Time.time) {

                                        distance = dis;
                                        target = AimObject;
                                    }
                                }
                            }
                        }
                        else {

                            for (int i = 0; i < objs.Length; i++) {

                                if (objs[i]) {

                                    Vector3 dir = (objs[i].transform.position - transform.position).normalized;
                                    float direction = Vector3.Dot(dir, transform.forward);
                                    float dis = Vector3.Distance(objs[i].transform.position, transform.position);

                                    if (direction >= AimDirection) {

                                        if (DistanceLock > dis) {

                                            if (distance > dis) {

                                                if (timetolockcount + TimeToLock < Time.time) {
                                                    distance = dis;
                                                    target = objs[i];
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (target) {

                float targetdistance = Vector3.Distance(transform.position, target.transform.position);
                Vector3 dir = (target.transform.position - transform.position).normalized;
                float direction = Vector3.Dot(dir, transform.forward);

                if (targetdistance > DistanceLock || direction <= AimDirection) {
                    Unlock();
                }
            }

            if (Reloading) {

                ReloadingProcess = ((1 / ReloadTime) * (reloadTimeTemp + ReloadTime - Time.time));

                if (Time.time >= reloadTimeTemp + ReloadTime) {
                    Reloading = false;
                    if (SoundReloaded) {
                        if (audio) {
                            audio.PlayOneShot(SoundReloaded);
                        }
                    }
                    Ammo = AmmoMax;
                }
            }
            else {
                if (Ammo <= 0) {
                    Unlock();
                    Reloading = true;
                    reloadTimeTemp = Time.time;

                    if (SoundReloading) {
                        if (audio) {
                            ApplyEffectsVolume();
                            audio.PlayOneShot(SoundReloading);
                        }
                    }
                }
            }
        }
    }

    public Camera CurrentCamera;

    private void DrawTargetLockon(Transform aimtarget, bool locked) {
        if (!ShowHUD)
            return;

        if (CurrentCamera) {

            Vector3 dir = (aimtarget.position - CurrentCamera.transform.position).normalized;

            float direction = Vector3.Dot(dir, CurrentCamera.transform.forward);

            if (direction > 0.5f) {
                //Vector3 screenPos = CurrentCamera.WorldToScreenPoint(aimtarget.transform.position);
                //float distance = Vector3.Distance(transform.position, aimtarget.transform.position);
                if (locked) {
                    //if (TargetLockedTexture)
                    //GUI.DrawTexture(new Rect(screenPos.x - TargetLockedTexture.width / 2, Screen.height - screenPos.y - TargetLockedTexture.height / 2, TargetLockedTexture.width, TargetLockedTexture.height), TargetLockedTexture);
                    //GUI.Label(new Rect(screenPos.x + 40, Screen.height - screenPos.y, 200, 30), aimtarget.name + " " + Mathf.Floor(distance) + "m.");
                }
                else {
                    //if (TargetLockOnTexture)
                    //    GUI.DrawTexture(new Rect(screenPos.x - TargetLockOnTexture.width / 2, Screen.height - screenPos.y - TargetLockOnTexture.height / 2, TargetLockOnTexture.width, TargetLockOnTexture.height), TargetLockOnTexture);
                }


            }
        }
        else {
            //Debug.Log("Can't Find camera");
        }
    }

    private Vector3 crosshairPos;

    private void DrawCrosshair() {
        if (!ShowCrosshair)
            return;

        if (CurrentCamera) {

            Vector3 screenPosAim = CurrentCamera.WorldToScreenPoint(AimPoint);

            crosshairPos += ((screenPosAim - crosshairPos) / 5);

            if (CrosshairTexture) {
                //GUI.DrawTexture(new Rect(crosshairPos.x - CrosshairTexture.width / 2, Screen.height - crosshairPos.y - CrosshairTexture.height / 2, CrosshairTexture.width, CrosshairTexture.height), CrosshairTexture);

            }
        }
    }
    /*
    private void OnGUI() {
        if (OnActive) {
            if (Seeker) {
           
                if (target) {
                    DrawTargetLockon(target.transform, true);
                }
            
                for (int t=0; t<TargetTag.Length; t++) {
                    if (GameObject.FindGameObjectsWithTag(TargetTag[t]).Length > 0) {
                        GameObject[] objs = GameObject.FindGameObjectsWithTag(TargetTag[t]);
                        for (int i = 0; i < objs.Length; i++) {
                            if (objs[i]) {
                                Vector3 dir = (objs[i].transform.position - transform.position).normalized;
                                float direction = Vector3.Dot(dir, transform.forward);
                                if (direction >= AimDirection) {
                                    float dis = Vector3.Distance(objs[i].transform.position, transform.position);
                                    if (DistanceLock > dis) {
                                        DrawTargetLockon(objs[i].transform, false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            DrawCrosshair();
        }
        
    }
    */

    private void Unlock() {
        timetolockcount = Time.time;
        target = null;
    }

    private string projectileEffectName = null;
    private string projectileEffectCode = null;

    public void NameEffect(GameObject bullet) {

        if (bullet == null) {
            return;
        }

        if (gamePlayerController == null) {
            gamePlayerController = gameObject.FindTypeAboveRecursive<GamePlayerController>();
        }

        if (gamePlayerController == null
            || gamePlayerController.weaponPrimary == null
            || gamePlayerController.weaponPrimary.gameWeaponData == null) {
            return;
        }

        string code = gamePlayerController.weaponPrimary.gameWeaponData.code;

        // Build the name once per weapon rather than concatenating a fresh string on
        // every shot of an auto weapon.

        if (projectileEffectName == null || projectileEffectCode != code) {
            projectileEffectCode = code;
            projectileEffectName = "projectile-" + code;
        }

        // ...and apply it once per pooled bullet rather than once per shot. The old code
        // allocated a GetComponentsInChildren array and wrote a native name on every
        // particle system every time the gun fired -- for an object that is recycled and
        // already carries the name from its last life.

        GameProjectileEffect effect = bullet.GetComponent<GameProjectileEffect>();

        if (effect == null) {
            effect = bullet.AddComponent<GameProjectileEffect>();
        }

        if (effect.HasName(projectileEffectName)) {
            return;
        }

        ParticleSystem[] particles = effect.GetParticles();

        for (int i = 0; i < particles.Length; i++) {

            if (particles[i] != null) {
                particles[i].name = projectileEffectName;
            }
        }

        effect.appliedName = projectileEffectName;
    }

    private int currentOuter = 0;

    private void ApplyEffectsVolume() {

        if (audio == null) {
            return;
        }

        if (cachedEffectsVolume < 0f
            || Time.time - cachedEffectsVolumeTime >= effectsVolumeRefreshSeconds) {

            cachedEffectsVolume = (float)GameProfiles.Current.GetAudioEffectsVolume();
            cachedEffectsVolumeTime = Time.time;
        }

        // AudioSource.volume is a native setter; skip it when nothing moved.

        if (audio.volume != cachedEffectsVolume) {
            audio.volume = cachedEffectsVolume;
        }
    }

    public void Shoot() {

        if (InfinityAmmo) {

            Ammo = 1;
        }

        if (Ammo > 0) {

            // FireRate is the interval between shots. The old gate compared against
            // nextFireTime + FireRate and then also added FireRate to nextFireTime,
            // so every weapon actually fired at half its authored rate.

            if (Time.time >= nextFireTime) {

                nextFireTime = Time.time + FireRate;
                torqueTemp = TorqueSpeedAxis;
                Ammo -= 1;

                Vector3 missileposition = this.transform.position;
                Quaternion missilerotate = this.transform.rotation;

                if (MissileOuter.Length > 0) {

                    missilerotate = MissileOuter[currentOuter].transform.rotation;
                    missileposition = MissileOuter[currentOuter].transform.position;
                }

                if (MissileOuter.Length > 0) {

                    currentOuter += 1;

                    if (currentOuter >= MissileOuter.Length)
                        currentOuter = 0;
                }

                if (Muzzle) {

                    GameObject muzzle = GameObjectHelper.CreateGameObject(
                        Muzzle, missileposition, missilerotate, true);

                    muzzle.transform.parent = this.transform;

                    GameObjectHelper.DestroyGameObject(muzzle, MuzzleLifeTime);

                    if (MissileOuter.Length > 0) {
                        muzzle.transform.parent = MissileOuter[currentOuter].transform;
                    }
                }

                for (int i = 0; i < NumBullet; i++) {

                    if (Missile) {

                        // Spread is a cone around the barrel, built from the weapon's
                        // own right/up axes. The old version offset all three WORLD
                        // axes, so how much a weapon scattered depended on which way
                        // the player happened to be facing, and the world-forward
                        // component only changed the vector's length.

                        Vector2 spread = Random.insideUnitCircle * (Spread / 100f);

                        Vector3 direction = (this.transform.forward
                            + (this.transform.right * spread.x)
                            + (this.transform.up * spread.y)).normalized;

                        GameObject bullet = GameObjectHelper.CreateGameObject(
                            Missile, missileposition, missilerotate, true);

                        NameEffect(bullet);

                        GameDamageBase damageBase = bullet.GetComponent<GameDamageBase>();

                        if (damageBase) {
                            damageBase.gamePlayerController = gamePlayerController;
                            damageBase.TargetTag = TargetTag;
                            damageBase.OnLaunched();
                        }

                        GameWeaponBase weaponBase = bullet.GetComponent<GameWeaponBase>();

                        if (weaponBase) {
                            weaponBase.gamePlayerController = gamePlayerController;
                            weaponBase.Target = target;
                            weaponBase.TargetTag = TargetTag;
                        }

                        bullet.transform.forward = direction;

                        if (RigidbodyProjectile) {

                            if (bullet.Has<Rigidbody>()) {

                                Rigidbody rigid = bullet.Get<Rigidbody>();

                                if (rigid != null) {

                                    // A pooled bullet arrives carrying whatever velocity
                                    // it had when it was recycled. Start it at rest, or
                                    // the impulse below is added to a leftover vector and
                                    // the shot leaves at an arbitrary angle.

                                    rigid.linearVelocity = Vector3.zero;
                                    rigid.angularVelocity = Vector3.zero;

                                    if (gamePlayerController != null
                                        && gamePlayerController.gameObject.GetRigidbody()) {

                                        rigid.linearVelocity = gamePlayerController.gameObject.GetRigidbody().linearVelocity;
                                    }
                                    rigid.AddForce(direction * ForceShoot);
                                }
                            }
                        }
                    }
                }

                if (!FPSDisplay.isUnder25FPS && Shell) {

                    Transform shelloutpos = this.transform;

                    if (ShellOuter.Length > 0) {
                        shelloutpos = ShellOuter[currentOuter];
                    }

                    GameObject shell = GameObjectHelper.CreateGameObject(
                        Shell, shelloutpos.position, Random.rotation, true);

                    GameObjectHelper.DestroyGameObject(shell.gameObject, ShellLifeTime);

                    if (shell.Has<Rigidbody>()) {
                        shell.Get<Rigidbody>().AddForce(shelloutpos.forward * ShellOutForce);
                    }
                }

                if (SoundGun.Length > 0) {
                    if (audio) {
                        ApplyEffectsVolume();
                        audio.PlayOneShot(SoundGun[Random.Range(0, SoundGun.Length)]);
                    }
                }
            }
        }

    }

}
