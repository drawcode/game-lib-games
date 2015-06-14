using System;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class GameCardsGameObject : GameObjectBehavior {

    GameCards gameCards = GameCards.Instance;

    public void Start() {

    }

    public void DealCard() {
        GameCard gameCard = gameCards.DealCard();
        
        //Debug.Log("DealCard:" + gameCard.ToJson());
    }

    public void LoadCards() {
        gameCards.LoadCards();
        
        
        Debug.Log("LoadCards:" + gameCards.cardSet.cardQueue.Count);
    }

    public void HandleInput() {
        
        if(Input.GetKeyDown(KeyCode.P)) {
            LoadCards();
        }

        if(Input.GetKeyDown(KeyCode.U)) {
            DealCard();
        }
    }

    public void Update() {

        HandleInput();
    }

}