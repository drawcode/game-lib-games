using UnityEngine;

/// <summary>
/// Per-bullet cache for the projectile effect naming that GameWeaponLauncher applies.
///
/// The launcher renames a projectile's particle systems so that OnParticleCollision can
/// recognise them (`other.name.Contains("projectile-")`). That rename used to run on
/// EVERY shot: a GetComponentsInChildren array allocation plus a native name write per
/// particle system, 25 times a second for an auto weapon, on an object that is pooled and
/// therefore already carries the name from its previous life.
///
/// This component holds the particle list and the name that was last applied, so the
/// launcher can do one GetComponent and a reference comparison instead.
/// </summary>
public class GameProjectileEffect : GameObjectBehavior {

    /// <summary>
    /// Cached once per pooled instance. Particle systems are authored on the prefab and
    /// do not come and go across lives.
    /// </summary>
    [HideInInspector]
    public ParticleSystem[] particles;

    [HideInInspector]
    public string appliedName;

    public bool HasName(string name) {

        // Reference comparison first -- the launcher hands back the same cached string
        // instance for every shot of a weapon, so this is the common case and it never
        // touches the characters.

        if (ReferenceEquals(appliedName, name)) {
            return true;
        }

        return appliedName != null && appliedName == name;
    }

    public ParticleSystem[] GetParticles() {

        if (particles == null) {
            particles = GetComponentsInChildren<ParticleSystem>(true);
        }

        return particles;
    }
}
