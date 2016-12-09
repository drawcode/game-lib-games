using UnityEngine;
using System.Collections;

public class GameObjectAccess : GameObjectBehavior {
    
    public string code = "";
    public string type = "";

    public GameObject containerLocked;
    public GameObject containerUnlocked;

    void Start() {
        UpdateAccess();
    }

    public bool HasAccessPermission() {

        if (code.IsNullOrEmpty()) {
            return false;
        }

        return GameProfiles.Current.HasAccessPermission(code);
    }
    
    public void UpdateAccess() {

        if (code.IsNullOrEmpty()) {
            return;
        }
        
        if (HasAccessPermission()) {
            containerLocked.Hide();
            containerUnlocked.Show();
        }
        else {
            containerLocked.Show();
            containerUnlocked.Hide();
        }
    }

}
