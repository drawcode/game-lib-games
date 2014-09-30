using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameCustomPlayerContainerLoader : MonoBehaviour {
    
    public GameObject containerPlayerDisplay;
    public UnityEngine.Object prefabPlayerDisplay;
    public GameCustomCharacterData customCharacteData = new GameCustomCharacterData();

    public void Awake() {
    }
    
    public void Start() {
        Init();
    }
    
    public void Init() {

        gameObject.layer = transform.parent.gameObject.layer;

        Load();
    }

    public void UpdatePlayers() {
        foreach (GameCustomPlayerContainer playerContainer in 
                gameObject.GetList<GameCustomPlayerContainer>()) {

            playerContainer.LoadPlayer(customCharacteData);
        }
    }

    public void Load() {
        
        if (prefabPlayerDisplay != null
            && containerPlayerDisplay != null) {
            
            containerPlayerDisplay.DestroyChildren();
            
            GameObject go = PrefabsPool.Instantiate(prefabPlayerDisplay) as GameObject;
            
            if (go != null) {
                
                go.transform.parent = containerPlayerDisplay.transform;
                
                go.ResetObject();
                
                go.SetLayerRecursively(gameObject.layer);

                UpdatePlayers();
            }
        }
    }

}