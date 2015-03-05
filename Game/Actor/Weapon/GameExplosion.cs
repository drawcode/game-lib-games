using UnityEngine;
using System.Collections;

public class GameExplosion : GameObjectBehavior {
    public int Force;
    public int Radius;
    public AudioClip[] Sounds;

    private void Start() {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, Radius);
        if (Sounds.Length > 0) {
            AudioSource.PlayClipAtPoint(Sounds[Random.Range(0, Sounds.Length)], transform.position,
                                        (float)GameProfiles.Current.GetAudioEffectsVolume());
        }
        foreach (Collider hit in colliders) {
            if (hit.gameObject.Has<Rigidbody>()) {
                hit.gameObject.Get<Rigidbody>().AddExplosionForce(Force, explosionPos, Radius, 3.0f);
            }
        }
    }
}
