using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

/*

Card system to add any set of cards and deck amounts to 
card set.  Could also be used for progression systems for 
random behavior.

 */

// stringly typed for server/client comms

public class GameCardTypes {
    // other types can be added on server or client so no enums
    public static string cardNormal = "card-normal";
    public static string cardFace = "card-face";
    public static string cardSpecial = "card-special";
}

public class GameCard : GameDataObject {
    public GameCard() {
        type = GameCardTypes.cardNormal;
        val = "";
    }
}

public class GameCardDeck { 

    // unique deck code
    public string code = "default";

    // deck type i.e. special cards, regular etc per game config
    public string type = "default";

    public List<GameCard> cards;
    public Queue cardQueue;

    public GameCardDeck() {

        cards = new List<GameCard>();
        cardQueue = new Queue();

        PrepareCards();
    }
        
    public void SetCard(string cardType, string cardValue) {
        GameCard gameCard = new GameCard();

        gameCard.type = cardType;
        gameCard.val = cardValue;

        cards.Add(gameCard);
    }

    public void SetCard(GameCard gameCard) {
        cards.Add(gameCard);
    }
    
    public void Shuffle() {
        cards.Shuffle();
    }

    public List<GameCard> GetCards() {
        return cards;
    }
        
    public GameCard DealCard() {

        if(cardQueue.Count == 0) {
            return null;
        }

        return (GameCard)cardQueue.Dequeue();
    }

    public void ClearCards() {
        cards.Clear();
    }
    
    public void ClearQueue() {
        cardQueue.Clear();
    }

    public void LoadCards(List<GameCard> cardsTo) {
        ClearCards();   
        cards.AddRange(cardsTo);
        PrepareCards();
    }
    
    public void PrepareCards() {

        ClearQueue();
        
        Shuffle();   

        foreach(GameCard gameCard in cards) {
            cardQueue.Enqueue(gameCard);
        }

        Debug.Log("GameCards:PrepareCards: cardCount:" + cardQueue.Count);

    }
}

public class GameCardsGenerator {
        
    static List<string> gameCardTypes;    
    static List<string> gameCardValues;
    
    public static List<string> GetStandardCardTypes() {
        
        if(gameCardTypes == null) {
            gameCardTypes = new List<string>();    
            
            gameCardTypes.Add("spade");
            gameCardTypes.Add("heart");
            gameCardTypes.Add("diamond");
            gameCardTypes.Add("clover");
        }
        
        return gameCardTypes;
    }
    
    public static List<string> GetStandardCardValues() {

        if(gameCardValues == null) {
            gameCardValues = new List<string>();    
        }

        gameCardValues.Clear();

        for(int i = 0; i < 9; i++) {
            gameCardValues.Add((i+1).ToString());            
        }
        
        gameCardValues.Add("ace");
        gameCardValues.Add("queen");
        gameCardValues.Add("jack");
        gameCardValues.Add("king");
    
        return gameCardValues;
    }

    public static GameCardDeck GetStandardCardDeck() {
        GameCardDeck deck = new GameCardDeck();

        deck.LoadCards(GetStandardCards());

        return deck;
    }

    public static List<GameCard> GetStandardCards() {

        List<GameCard> cards = new List<GameCard>();

        foreach(string cardType in GetStandardCardTypes()) {
            
            foreach(string cardVal in GetStandardCardValues()) {
                
                cards.Add(CreateCard(cardType, cardVal));
            }
        }

        return cards;
    }
        
    public static GameCard CreateCard(string cardType, string cardValue) {
        GameCard gameCard = new GameCard();
        
        gameCard.type = cardType;
        gameCard.val = cardValue;
        
        return gameCard;
    }

}

public class GameCardSet {

    public List<GameCardDeck> decks;
    public GameCard currentCard = null;

    public Queue cardQueue;

    public GameCardSet() {

        Reset();
    }

    public void Reset() {

        decks = new List<GameCardDeck>();
        cardQueue = new Queue();
        currentCard = null;        
    }
    
    public void LoadDeck(GameCardDeck gameCardDeck) {

        decks.Add(gameCardDeck);
    }
    
    public void Shuffle() {

        foreach(GameCardDeck deck in decks) {
            deck.Shuffle();
        }
    }

    public void ClearQueue() {

        cardQueue.Clear();
        
        foreach(GameCardDeck deck in decks) {
            deck.ClearCards();
        }
    }    

    public void PrepareCards() {        

        foreach(GameCardDeck deck in decks) {
            deck.PrepareCards();
        }

        foreach(GameCardDeck deck in decks) {

            foreach(GameCard card in deck.GetCards()) {
                cardQueue.Enqueue(card);
            }
        }

        currentCard = null;
    }
    
    public GameCard CurrentCard() {
        return currentCard;
    }
        
    public GameCard DealCard() {

        if(cardQueue.Count == 0) {
            return null;
        }

        currentCard = (GameCard)cardQueue.Dequeue();

        return currentCard;
    }
}

public class GameCards {
    
    private static volatile GameCards instance;
    private static System.Object syncRoot = new System.Object();

    public GameCardSet cardSet;
    public GameCard currentCard;
    
    public static GameCards Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new GameCards();
                }
            }   
            return instance;
        }
    }

    public GameCards() {
        cardSet = new GameCardSet();
        LoadCards();
    }

    public void LoadCards(string type = "default", int deckCount = 3) {
        currentCard = null;
        cardSet.Reset();
        for(int i = 0; i < deckCount; i++) {
            cardSet.LoadDeck(GameCardsGenerator.GetStandardCardDeck());
        }
        cardSet.PrepareCards();
    }

    public GameCard DealCard() {
    
        currentCard = cardSet.DealCard();

        Debug.Log("GameCard:DealCard:" + currentCard.ToJson());

        return currentCard;
    }
}
