using System;
using System.Collections.Generic;
using UnityEngine;

// using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class GameCardsBlackjackGameObject : GameObjectBehavior {

    GameCardBlackjack gameCards;

    public void Start() {
        gameCards = new GameCardBlackjack();

        Debug.Log("gameCards.LoadPlayers:KeyCode.L");
        Debug.Log("gameCards.DealCards(2):KeyCode.D");
        Debug.Log("gameCards.HitDealer():KeyCode.Alpha1");
        Debug.Log("gameCards.HitMe():KeyCode.Alpha2");
        Debug.Log("gameCards.GameStart():KeyCode.S");
        Debug.Log("gameCards.GameEnd():KeyCode.E");
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

        
        if(Input.GetKeyDown(KeyCode.S)) {
            gameCards.GameStart();
        }
        
        if(Input.GetKeyDown(KeyCode.E)) {
            gameCards.GameEnd();
        }


    }

    public void Update() {

        HandleInput();
    }

}