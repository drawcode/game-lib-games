using UnityEngine;
using System.Collections;

public class GameRayShoot : GameDamageBase {

    public int Range = 10000;
    public Vector3 AimPoint;
    public GameObject Explosion;
    public float LifeTime = 1;
    private LineRenderer trail = null;

    // The beam is fired from OnLaunched, NOT Start.
    //
    // The pool re-sends Start from inside CreateGameObject, which is BEFORE the launcher has
    // assigned TargetTag, gamePlayerController, or the spread-adjusted forward for this shot. A
    // beam built in Start therefore cast along the previous life's aim and handed the previous
    // life's TargetTag to the thing that does the damage -- so a ray recycled from an enemy's
    // shot went looking for the player, and hit nothing the player was aiming at.
    //
    // OnLaunched is the hook the launcher calls once all of that is wired for THIS shot. Same
    // reason GameDamage moved its IgnoreCollision pairing here.
    public override void OnLaunched() {

        if (!GameDamageDirector.AllowRayShoot) {
            GameObjectHelper.DestroyGameObject(gameObject);
            return;
        }

        Fire();
    }

    protected virtual void Fire() {

        trail = this.gameObject.GetComponent<LineRenderer>();

        RaycastHit hit;
        GameObject explosion = null;

        bool struck = Physics.Raycast(
            this.transform.position, this.transform.forward, out hit, Range);

        if (struck) {
            AimPoint = hit.point;
        }
        else {
            // The miss point is a POSITION, so it has to start from the muzzle. This read
            // `transform.forward * Range` -- a direction scaled by 10000 -- which put the far end
            // of every missed beam near the world origin instead of out in front of the gun.
            AimPoint = this.transform.position + (this.transform.forward * Range);
        }

        // Guarded on BOTH branches. The miss branch called CreateGameObject unconditionally, so a
        // ray prefab with no Explosion assigned threw here -- after the LineRenderer was fetched
        // but before the beam was drawn or the recycle was scheduled.
        if (Explosion != null) {
            explosion = GameObjectHelper.CreateGameObject(
                Explosion, AimPoint, this.transform.rotation, true);
        }

        if (explosion != null) {

            GameDamageBase dmg = explosion.GetComponent<GameDamageBase>();

            if (dmg != null) {

                // All three, not just the tags. The explosion is what actually applies the
                // damage, so without the owner it cannot tell friendly fire from a hit, and
                // without the ray's Damage it silently used whatever its own prefab authored.
                dmg.TargetTag = TargetTag;
                dmg.gamePlayerController = gamePlayerController;
                dmg.Damage = Damage;

                dmg.OnLaunched();
            }
        }

        if (trail != null) {
            trail.SetPosition(0, this.transform.position);
            trail.SetPosition(1, AimPoint);
        }

        GameObjectHelper.DestroyGameObject(this.gameObject, LifeTime);
    }
}
