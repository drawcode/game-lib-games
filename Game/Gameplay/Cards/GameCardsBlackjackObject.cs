using System;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class GameCardsBlackjackGameObject : GameObjectBehavior {

    GameCardBlackJack gameCards;

    public void Start() {
        gameCards = new GameCardBlackJack();
    }

    public void DealCard() {
        
        //Debug.Log("DealCard:" + gameCard.ToJson());
    }

    public void LoadCards() {
        

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