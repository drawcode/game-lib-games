using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameCustomPlayerContainerLoader : MonoBehaviour {
    
    public GameObject containerPlayerDisplay;
    public UnityEngine.Object prefabPlayerDisplay;
    public GameCustomCharacterData customCharacterData; // = new GameCustomCharacterData();

    public bool allowRotator = false;
    public bool zoomAdjust = false;
    public double zoomAdjustAmount = 5f;

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

            playerContainer.allowRotator = allowRotator;
            playerContainer.zoomAdjust = zoomAdjust;
            playerContainer.zoomAdjustAmount = zoomAdjustAmount;

            playerContainer.LoadPlayer(customCharacterData);
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