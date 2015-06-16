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

public class GameCard {

    public string uid;
    public string type;
    public string val;

    public GameCard() {
        uid = UniqueUtil.Instance.CreateUUID4();
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

        if (cardQueue.Count == 0) {
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

        foreach (GameCard gameCard in cards) {
            cardQueue.Enqueue(gameCard);
        }
    }
}

public class GameCardsGenerator {
        
    static List<string> gameCardTypes;
    static List<string> gameCardValues;
    
    public static List<string> GetStandardCardTypes() {
        
        if (gameCardTypes == null) {
            gameCardTypes = new List<string>();    
            
            gameCardTypes.Add("spade");
            gameCardTypes.Add("heart");
            gameCardTypes.Add("diamond");
            gameCardTypes.Add("clover");
        }
        
        return gameCardTypes;
    }
    
    public static List<string> GetStandardCardValues() {

        if (gameCardValues == null) {
            gameCardValues = new List<string>();    
        }

        gameCardValues.Clear();

        for (int i = 0; i < 9; i++) {
            gameCardValues.Add((i + 1).ToString());            
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

        foreach (string cardType in GetStandardCardTypes()) {
            
            foreach (string cardVal in GetStandardCardValues()) {
                
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

        foreach (GameCardDeck deck in decks) {
            deck.Shuffle();
        }
    }

    public void ClearQueue() {

        cardQueue.Clear();
        
        foreach (GameCardDeck deck in decks) {
            deck.ClearCards();
        }
    }

    public void PrepareCards() {        

        foreach (GameCardDeck deck in decks) {
            deck.PrepareCards();
        }

        foreach (GameCardDeck deck in decks) {

            foreach (GameCard card in deck.GetCards()) {
                cardQueue.Enqueue(card);
            }
        }

        currentCard = null;
    }
    
    public GameCard CurrentCard() {
        return currentCard;
    }
        
    public GameCard DealCard() {

        if (cardQueue.Count == 0) {
            return null;
        }

        currentCard = (GameCard)cardQueue.Dequeue();

        return currentCard;
    }
}

public class GameCards {

    public GameCardSet cardSet;
    public GameCard currentCard;
    
    int deckCount = 1;
    string type = "default";

    public GameCards() {
        cardSet = new GameCardSet();
        LoadCards();
    }
    public void LoadCards(string typeTo = "default", int deckCountTo = 1) {

        currentCard = null;

        type = typeTo;
        deckCount = deckCountTo;

        cardSet.Reset();

        for (int i = 0; i < deckCount; i++) {
            cardSet.LoadDeck(GameCardsGenerator.GetStandardCardDeck());
        }

        cardSet.PrepareCards();
    }

    public GameCard DealCard() {
    
        currentCard = cardSet.DealCard();
        
        if (currentCard == null) {
            LoadCards(type, deckCount);
            Debug.Log("DealCard:reloading cards"); 
            currentCard = cardSet.DealCard();
        }

        Debug.Log("GameCard:DealCard:" + currentCard.ToJson());
        
        Debug.Log("GameCard:CardQueue:" + cardSet.cardQueue.Count);

        return currentCard;
    }
}

// GAME CARD TYPE OBJECTS


public class GameCardHand {
    
    public List<GameCard> cards;
    
    public GameCardHand() {
        cards = new List<GameCard>();
    }

    // SET CARDS

    public void SetCard(GameCard gameCard) {

        for(int i = 0; i < cards.Count; i++) {
            if(cards[i].uid == gameCard.uid) {
                cards[i].uid = gameCard.uid;
                cards[i].type = gameCard.type;
                cards[i].val = gameCard.val;
                return;
            } 
        }

        cards.Add(gameCard);
    }

    // GET CARDS
        
    public GameCard GetCard(string uid) {
        for(int i = 0; i < cards.Count; i++ ){
            if(cards[i].uid == uid) {
                return cards[i];
            }
        }    
        return null;
    }
    
    public GameCard GetCard(int index) {
        
        if(cards.Count > 0
           && index < cards.Count) {
            return null;
        }
        
        return cards[index];
    }
    
    public GameCard GetCard(GameCard card) {
        return GetCard(card.uid);   
    }

    // GET CARD INDEX

    
    public int GetCardIndex(string uid) {
        for(int i = 0; i < cards.Count; i++ ){
            if(cards[i].uid == uid) {
                return i;
            }
        }    
        return -1;
    }
        
    public int GetCardIndex(GameCard card) {
        return GetCardIndex(card.uid);   
    }

    // REMOVE

    public void RemoveCard(string uid) {
        cards.RemoveAll(u => u.uid == uid);
    }

    public void ClearCards() {
        cards.Clear();
    }
    
}

public class GameCardPlayerType {
    public static string player = "player";
    public static string dealer = "dealer";
}

public class GameCardPlayer {

    public string uid = "";
    public string name = "";
    public string type = "";
    public string typePlayer = "";

    public List<GameCardHand> cardHands;
    
    public GameCardPlayer() {
        uid = UniqueUtil.Instance.CreateUUID4();
        type = GameCardPlayerType.player;
        typePlayer = GameCardPlayerType.player;
        ClearCardHands();
    }
    
    public void ClearCardHands() {        
        cardHands = new List<GameCardHand>();
    }

    public int CardCountByHand(int idx = 0) {
        for(int i = 0; i < cardHands.Count; i++) {
            if(idx == i) {
                return cardHands[i].cards.Count;
            }
        }
        return 0;
    }
    
    public int CardCountFirstHand() {
        return CardCountByHand(0);
    }

    public void ReceiveCard(GameCard gameCard, int handIndex = 0) {

        if(cardHands.Count <= handIndex) {
            return;
        }

        cardHands[handIndex].SetCard(gameCard);
    }

    public void DiscardCard(string uid, int handIndex = 0) {
        
        if(cardHands.Count <= handIndex) {
            return;
        }
        
        GameCard gameCard = cardHands[handIndex].GetCard(uid);

        if(gameCard != null) {
            
        }
    }
}

// GAME CARD CONTROLLERS

public class GameCardBase {    

    public GameCards gameCards;
    
    public int cardHandLimit = -1;
    
    List<GameCardPlayer> players;

    public GameCardBase() {
        gameCards = new GameCards();
        players = new List<GameCardPlayer>();
    }

    // GET

    public GameCardPlayer GetPlayer(string uid) {
        for(int i = 0; i < players.Count; i++ ){
            if(players[i].uid == uid) {
                return players[i];
            }
        }    
        return null;
    }

    public GameCardPlayer GetPlayer(int playerIndex) {

        if(playerIndex >= players.Count) {
            return null;
        }

        return players[playerIndex];
    }
    
    public GameCardPlayer GetPlayer(GameCardPlayer gamePlayer) {
        return GetPlayer(gamePlayer.uid);   
    }

    // ADD / SET

    public void SetPlayer(string uid, string type, string name) {

        GameCardPlayer gamePlayer = new GameCardPlayer();
        gamePlayer.uid = uid;
        gamePlayer.type = uid;
        gamePlayer.name = name;

        SetPlayer(gamePlayer);
    }
    
    public void SetPlayer(GameCardPlayer gamePlayer) {
                
        for(int i = 0; i < players.Count; i++ ){
            if(players[i].uid == gamePlayer.uid) {
                players[i].name = gamePlayer.name;
                players[i].uid = gamePlayer.uid;
                players[i].type = gamePlayer.type;
                players[i].cardHands = gamePlayer.cardHands;
                return;
            }
        }
        
        players.Add(gamePlayer);
    }

    // REMOVE
    
    public void RemovePlayer(GameCardPlayer gamePlayer) {
        
        int playerIdx = -1;
        
        for(int i = 0; i < players.Count; i++ ){
            if(players[i].uid == gamePlayer.uid) {
                playerIdx = i;//players.RemoveAt(i);
                break;
            }
        }
        
        if(playerIdx > -1 
           && players.Count > playerIdx) {
            players.RemoveAt(playerIdx);
        }
    }

    //

    public void DealPlayerCard(
        GameCardPlayer gamePlayer, GameCard gameCard) {
                
        if(gamePlayer != null && gameCard != null) {
            gamePlayer.ReceiveCard(gameCard);
        }
    }
        
    public void DealPlayerCard(string uid) {
        GameCardPlayer gamePlayer = GetPlayer(uid);

        if(gamePlayer == null) {
            return;
        }

        GameCard gameCard = gameCards.DealCard(); 
        DealPlayerCard(gamePlayer, gameCard);
    }

    public void DealPlayerCard(int index) {
        GameCardPlayer gamePlayer = GetPlayer(index);
        
        if(gamePlayer == null) {
            return;
        }
        
        GameCard gameCard = gameCards.DealCard(); 
        DealPlayerCard(gamePlayer, gameCard);
    }
}

public class GameCardBlackJack : GameCardBase {

    public GameCardBlackJack() {

    }

}

public class GameCardPoker : GameCardBase {
    
    public GameCardPoker() {
        
    }
    
}
