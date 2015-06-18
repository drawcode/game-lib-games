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

    public void HandleInput() {

        if(Input.GetKeyDown(KeyCode.L)) {
            gameCards.LoadPlayers();
        }

        if(Input.GetKeyDown(KeyCode.D)) {
            gameCards.DealCards(2);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            gameCards.HitDealer();
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            gameCards.HitMe();
        }


    }

    public void Update() {

        HandleInput();
    }

}