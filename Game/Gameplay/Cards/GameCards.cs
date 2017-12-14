using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

// using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

/*

Card system to add any set of cards and deck amounts to 
card set.  Could also be used for progression systems for 
random behavior.

 */

// stringly typed for server/client comms

// ---------------------------------------------------------------
// GAME CARD TYPES

public class GameCardTypes {
    // other types can be added on server or client so no enums
    public static string cardNormal = "card-normal";
    public static string cardFace = "card-face";
    public static string cardSpecial = "card-special";
}

// ---------------------------------------------------------------
// GAME CARD

public class GameCard {

    public string uid;
    public string type;
    public string val;

    public GameCard() {
        uid = UniqueUtil.CreateUUID4();
        type = GameCardTypes.cardNormal;
        val = "";
    }
}

// ---------------------------------------------------------------
// CARD DECK

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

// ---------------------------------------------------------------
// GAME CARD GENERATOR

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

// ---------------------------------------------------------------
// CARD SETS

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

// ---------------------------------------------------------------
// GAMECARDS

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

// ---------------------------------------------------------------
// GAME CARD TYPE OBJECTS

public class GameCardHand {
    
    public List<GameCard> cards;
    
    public GameCardHand() {
        cards = new List<GameCard>();
    }

    // SET CARDS

    public void SetCard(GameCard gameCard) {

        for (int i = 0; i < cards.Count; i++) {
            if (cards[i].uid == gameCard.uid) {
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
        for (int i = 0; i < cards.Count; i++) {
            if (cards[i].uid == uid) {
                return cards[i];
            }
        }    
        return null;
    }
    
    public GameCard GetCard(int index) {
        
        if (cards.Count > 0
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
        for (int i = 0; i < cards.Count; i++) {
            if (cards[i].uid == uid) {
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

// ---------------------------------------------------------------
// GAMECARD PLAYERS

public class GameCardPlayerType {
    public static string player = "player";
    public static string dealer = "dealer";
    public static string spectator = "spectator";
}

public class GameCardPlayerKeys {
    public static string uid = "uid";
    public static string playerUid = "player-uid";
    public static string dealerUid = "dealer-uid";
}

public class GameCardPlayerStates {

}

public interface IGameCardPlayer {



}

public class GameCardPlayer : IGameCardPlayer {

    public string uid = "";
    public string name = "";
    public string type = "";
    public string typePlayer = "";
    public List<GameCardHand> cardHands;

    public bool isDealer {
        get {
            return type == GameCardPlayerType.dealer;
        }
    }
    
    public bool isPlayer {
        get {
            return type == GameCardPlayerType.player;
        }
    }
    
    public GameCardPlayer() {
        uid = UniqueUtil.CreateUUID4();
        type = GameCardPlayerType.player;
        typePlayer = GameCardPlayerType.player;
        ClearCardHands();
    }
    
    public void ClearCardHands() {        
        cardHands = new List<GameCardHand>();

        InitCardHands();
    }

    public void InitCardHands() {
        
        GameCardHand hand = new GameCardHand();
        cardHands.Add(hand);
    }

    public int CardCountByHand(int idx = 0) {
        for (int i = 0; i < cardHands.Count; i++) {
            if (idx == i) {
                return cardHands[i].cards.Count;
            }
        }
        return 0;
    }
    
    public int CardCountFirstHand() {
        return CardCountByHand(0);
    }

    public void ReceiveCard(GameCard gameCard, int handIndex = 0) {

        if (cardHands.Count <= handIndex) {

            if (handIndex > 0) {
                return;
            }
            else {
                InitCardHands();
            }
        }

        cardHands[handIndex].SetCard(gameCard);
    }

    public void DiscardCard(string uid, int handIndex = 0) {
        
        if (cardHands.Count <= handIndex) {
            return;
        }
        
        GameCard gameCard = cardHands[handIndex].GetCard(uid);

        if (gameCard != null) {
            
        }
    }
}

// ---------------------------------------------------------------
// GAME CARD CONTROLLERS

public class GameCardKeys {
    public static string gameCardChangeState = "game-card-change-state";
    public static string gameCardGameType = "game-card-game-type";

}

public class GameCardGameStates {    
    public static string gameCardNotReady = "game-card-not-ready";
    public static string gameCardStart = "game-card-start";
    public static string gameCardNextRound = "game-card-next-round";
    public static string gameCardNextPlayer = "game-card-next-player";
    public static string gameCardEnd = "game-card-end";
}

public class GameCardGameTypes {
    public static string gameCardTypeDefault = GameCardKeys.gameCardGameType + "-default";
    public static string gameCardTypeBlackjack = GameCardKeys.gameCardGameType + "-blackjack";
    public static string gameCardTypePoker = GameCardKeys.gameCardGameType + "-poker";
}

public class GameCardBase<T> where T : GameCardPlayer, new() {    

    public GameCards gameCards;
    public int cardHandLimit = -1;
    public string gameCardGameType = GameCardGameTypes.gameCardTypeDefault;    
    public string currentGameState = GameCardGameStates.gameCardNotReady;
    List<T> players;

    public GameCardBase() {
        gameCards = new GameCards();
        players = new List<T>();
    }    
    
    // STATES / EVENTS

    public void SetGameType(string gameTypeTo) {
        gameCardGameType = gameTypeTo;
    }

    
    public void SetGameState(string stateTo) {
        if (currentGameState == stateTo) {
            return;
        }
        
        Debug.Log("StateChanged: " + " stateTo:" + stateTo);
        
        currentGameState = stateTo;
    }
    
    public void ChangeState(string stateTo) {

        if (currentGameState == stateTo) {
            return;
        }
        
        Debug.Log("StateChanging: " + " stateTo:" + stateTo);
        
        HandleChangeState(stateTo);
    }
    
    public void HandleChangeState(string stateTo) {

        SetGameState(stateTo);
        
        Messenger<string>.Broadcast(
            GameCardKeys.gameCardChangeState,
            currentGameState
        );
    }

    // DISPLAY

    public virtual void DisplayCards() {

        foreach (T player in players) {
            
            string cards = "";
            cards += " -> " + player.name + " \r\n"; 

            foreach (GameCardHand hand in player.cardHands) {

                foreach (GameCard card in hand.cards) {
                    cards += " | " + card.val + " - " + 
                        " " + card.type + " ";
                }

            }
            
            Debug.Log(cards);
        }
    }

    // GET

    public List<T> GetPlayers() {
        return players;
    }

    public T GetPlayer(string uid) {
        for (int i = 0; i < players.Count; i++) {
            if (players[i].uid == uid) {
                return players[i];
            }
        }    
        return null;
    }

    public T GetPlayer(int playerIndex) {

        if (playerIndex >= players.Count) {
            return null;
        }

        return players[playerIndex];
    }
    
    public T GetPlayer(GameCardPlayer gamePlayer) {
        return GetPlayer(gamePlayer.uid);   
    }

    // ADD / SET

    public void SetPlayer(string uid, string type, string name) {

        T gamePlayer = new T();
        gamePlayer.uid = uid;
        gamePlayer.type = type;
        gamePlayer.name = name;

        SetPlayer(gamePlayer);
    }
    
    public void SetPlayer(T gamePlayer) {
                
        for (int i = 0; i < players.Count; i++) {
            if (players[i].uid == gamePlayer.uid) {
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
    
    public void RemovePlayer(T gamePlayer) {
        
        int playerIdx = -1;
        
        for (int i = 0; i < players.Count; i++) {
            if (players[i].uid == gamePlayer.uid) {
                playerIdx = i;//players.RemoveAt(i);
                break;
            }
        }
        
        if (playerIdx > -1 
            && players.Count > playerIdx) {
            players.RemoveAt(playerIdx);
        }
    }

    //

    public void DealPlayerCard(
        T gamePlayer, GameCard gameCard) {
                
        if (gamePlayer != null && gameCard != null) {
            gamePlayer.ReceiveCard(gameCard);
        }
    }
        
    public void DealPlayerCard(string uid) {
        T gamePlayer = GetPlayer(uid);

        if (gamePlayer == null) {
            return;
        }

        GameCard gameCard = gameCards.DealCard(); 
        DealPlayerCard(gamePlayer, gameCard);
    }

    public void DealPlayerCard(int index) {
        T gamePlayer = GetPlayer(index);

        if (gamePlayer == null) {
            return;
        }
        
        GameCard gameCard = gameCards.DealCard(); 
        DealPlayerCard(gamePlayer, gameCard);
    }

    // DEFAULT PLAYER

    public string GetPlayerId() {

        string uidMe = SystemPrefUtil.GetLocalSettingString(
            GameCardPlayerKeys.playerUid);
        
        if (string.IsNullOrEmpty(uidMe)) {
            SystemPrefUtil.SetLocalSettingString(
                GameCardPlayerKeys.playerUid, System.Guid.NewGuid().ToString());
            
            uidMe = SystemPrefUtil.GetLocalSettingString(
                GameCardPlayerKeys.playerUid);
        }

        return uidMe;
    }

    //
        
    public virtual void DealCards(int handRoundLimit) {
        
        List<T> playerList = GetPlayers();
        
        for (int i = 0; i < handRoundLimit; i++) {
            for (int j = 0; j < playerList.Count; j++) {

                if (playerList[j].CardCountByHand(0) <= i) {
                    DealPlayerCard(playerList[j].uid);
                }
            }
        }
        
        DisplayCards();        
    }
}

// ---------------------------------------------------------------
// BLACKJACK

public class GameCardPlayerBlackjackStates : GameCardPlayerStates {
    //public static string blackjackPlayerWaitForCards = "blackjack-player-wait-for-cards";
    //public static string blackjackPlayerWaitForTurn = "blackjack-player-wait-for-turn";
    public static string blackjackPlayerTurn = "blackjack-player-turn";
    public static string blackjackPlayerTurnDoubleDown = "blackjack-player-turn-double-down"; // handIndex
    public static string blackjackPlayerTurnSplit = "blackjack-player-turn-split"; // handIndex
    public static string blackjackPlayerTurnStand = "blackjack-player-turn-stand"; // handIndex
    public static string blackjackPlayerTurnHit = "blackjack-player-turn-hit"; // uid, card, handIndex
    public static string blackjackPlayerNotStarted = "blackjack-player-not-started";
    public static string blackjackPlayerReady = "blackjack-player-ready";
}

public class GameCardPlayerBlackjack : GameCardPlayer {

    public string currentGamePlayerState = GameCardPlayerBlackjackStates.blackjackPlayerNotStarted;
    
    public GameCardPlayerBlackjack() {
        typePlayer = GameCardPlayerType.player;
        currentGamePlayerState = GameCardPlayerBlackjackStates.blackjackPlayerReady;
    }

    public bool IsReady() {
        return !IsState(GameCardPlayerBlackjackStates.blackjackPlayerNotStarted);
    }

    public bool IsState(string stateTo) {
        return currentGamePlayerState == stateTo; 
    }
}

public class GameCardBlackjackKeys : GameCardKeys {

}

public class GameCardBlackjackStates : GameCardGameStates {
    public static string blackjackWaitForBets = "blackjack-wait-for-bets";
    public static string blackjackDealAll = "blackjack-deal-all"; // card count
    public static string blackjackDealPlayer = "blackjack-deal-player"; // card count, uid
}

public class GameCardBlackjack : GameCardBase<GameCardPlayerBlackjack> {
    public string uidDealer = "11111111-1111-1111-1111-111111111111";

    public List<string> gameLoopStates;

    public GameCardBlackjack() {

        gameLoopStates = new List<string>();

        //gameLoopStates.Add(GameCardBlackjackStates.blackjackWaitForBets);
        //gameLoopStates.Add(GameCardBlackjackStates.blackjackDealAll);
        //gameLoopStates.Add(GameCardBlackjackStates.blackjackDealPlayer);

        SetGameType(GameCardGameTypes.gameCardTypeBlackjack);

        AddEvents();
    }

    ~GameCardBlackjack() {
        
        RemoveEvents();
    }

    public void AddEvents() {

        Messenger<string>.AddListener(GameCardKeys.gameCardChangeState, OnGameChangeState);

        Messenger.AddListener(GameCardBlackjackStates.gameCardStart, OnGameStart);
        Messenger.AddListener(GameCardBlackjackStates.gameCardEnd, OnGameEnd);
        Messenger.AddListener(GameCardBlackjackStates.gameCardNotReady, OnGameNotReady);
        Messenger.AddListener(GameCardBlackjackStates.gameCardNextPlayer, OnGameNextPlayer);
        Messenger.AddListener(GameCardBlackjackStates.gameCardNextRound, OnGameNextRound);

        //
        
        Messenger.AddListener(GameCardBlackjackStates.blackjackWaitForBets, OnGamePlayerWaitForBets);
        Messenger<string,int, int>.AddListener(GameCardBlackjackStates.blackjackDealPlayer, OnGamePlayerDeal);
        Messenger<int, int>.AddListener(GameCardBlackjackStates.blackjackDealAll, OnGamePlayerDealAll);

    }

    public void RemoveEvents() {

        Messenger<string>.RemoveListener(GameCardKeys.gameCardChangeState, OnGameChangeState);
        
        Messenger.RemoveListener(GameCardBlackjackStates.gameCardStart, OnGameStart);
        Messenger.RemoveListener(GameCardBlackjackStates.gameCardEnd, OnGameEnd);
        Messenger.RemoveListener(GameCardBlackjackStates.gameCardNotReady, OnGameNotReady);
        Messenger.RemoveListener(GameCardBlackjackStates.gameCardNextPlayer, OnGameNextPlayer);
        Messenger.RemoveListener(GameCardBlackjackStates.gameCardNextRound, OnGameNextRound);
        
        //
        
        Messenger.RemoveListener(GameCardBlackjackStates.blackjackWaitForBets, OnGamePlayerWaitForBets);
        Messenger<string,int, int>.RemoveListener(GameCardBlackjackStates.blackjackDealPlayer, OnGamePlayerDeal);
        Messenger<int, int>.RemoveListener(GameCardBlackjackStates.blackjackDealAll, OnGamePlayerDealAll);
        
    }

    // GAME EVENT HANDLERS
    
    public void OnGameChangeState(string stateTo) {
        
        Debug.Log("OnGameChangeState" + " stateTo:" + stateTo);
        
        HandleGameState();
    }

    public void OnGameStart() {
    
        Debug.Log("OnGameStart");

        HandleGameState();
    }
    
    public void OnGameEnd() {
        
        Debug.Log("OnGameEnd");
        
        HandleGameState();
    }
    
    public void OnGameNotReady() {
        
        Debug.Log("OnGameNotReady");
        
        HandleGameState();
    }
    
    public void OnGameNextPlayer() {
        
        Debug.Log("OnGameNextPlayer");
        
        HandleGameState();
    }
    
    public void OnGameNextRound() {
        
        Debug.Log("OnGameNextRound");
        
        HandleGameState();
    }

    // GAME PLAYER EVENT HANDLERS
    
    public void OnGamePlayerWaitForBets() {
        
        Debug.Log("OnGamePlayerWaitForBets");
        
        HandleGamePlayerState();
    }
    
    public void OnGamePlayerDeal(string uid, int handIndex, int cardCount) {
        
        Debug.Log("OnGamePlayerDeal");
        
        HandleGamePlayerState();
    }
    
    public void OnGamePlayerDealAll(int handIndex, int cardCount) {
        
        Debug.Log("OnGamePlayerDealAll");
        
        HandleGamePlayerState();
    }

    // STATE HANDLERS
        
    public void HandleGameState() {
        
        if(currentGameState == GameCardBlackjackStates.gameCardStart) {
            
            
            // Check if all players ready
            
            List<GameCardPlayerBlackjack> players = GetPlayers();
            
            bool readyToStart = false;
            
            for(int i = 0; i < players.Count; i++) {
                
                if(!players[i].IsReady()) {
                    readyToStart = false;
                    break;                
                }
                else {
                    readyToStart = true;
                }
            }
            
            Debug.Log("HandleGameState" + "readyToStart:" + readyToStart);
            
            if(readyToStart) {

                GameNextPlayer();
            }
        }
        
    }
    
    public void HandleGamePlayerState() {
        
    }

    // PLAYERS

    public void LoadPlayers() {
           
        string uidMe = GetPlayerId();
        
        SetPlayer(uidDealer, GameCardPlayerType.dealer, "dealer");
        SetPlayer(uidMe, GameCardPlayerType.player, "me");
        
        Debug.Log("players:" + GetPlayers().ToJson());
         
    }

    //

    public void GameStart() {
        ChangeState(GameCardBlackjackStates.gameCardStart);
    }
        
    public void GameEnd() {
        ChangeState(GameCardBlackjackStates.gameCardEnd);
    }
        
    public void GameNextPlayer() {
        ChangeState(GameCardBlackjackStates.gameCardNextPlayer);
    }
    
    public void GameNextRound() {
        ChangeState(GameCardBlackjackStates.gameCardNextRound);
    }

    //

    public void HitDealer() {
        HitPlayer(uidDealer);        
    }

    public void HitMe() {
        HitPlayer(GetPlayerId());
    }

    public void HitPlayer(string uid) {
        DealPlayerCard(uid);

        DisplayCards();
    }

    // DISPLAY

    public int GetCardsValue(List<GameCard> cards) {

        int score = 0;

        foreach (GameCard card in cards) {

            if (card.val == "jack"
                || card.val == "queen"
                || card.val == "king") {

                score += 10;            
            }
            else if (card.val == "ace") {
                // Handle later for multiple values
            }
            else {
                int cardVal = 0;
                int.TryParse(card.val, out cardVal);

                score += cardVal;
            }
        }
                
        foreach (GameCard card in cards) {

            if (card.val == "ace") {

                score += 11;

                if (score > 21) {
                    score -= 10;
                }
            }
        }

        return score;
    }
    
    public override void DisplayCards() {
        
        foreach (GameCardPlayerBlackjack player in GetPlayers()) {
            
            string cards = "";
            cards += " -> " + player.name + " \r\n"; 
            
            foreach (GameCardHand hand in player.cardHands) {
                                                
                foreach (GameCard card in hand.cards) {
                    cards += " | " + card.val + " - " + 
                        " " + card.type + " ";
                }
                int score = GetCardsValue(hand.cards);
                cards += " | score: " + score;

                if (score > 21) {
                    cards += " | bust ";
                } 
                
            }
            
            Debug.Log(cards);
        }
    }
        
    //
    
    public override void DealCards(int handRoundLimit) {
        
        List<GameCardPlayerBlackjack> playerList = GetPlayers();
        
        for (int i = 0; i < handRoundLimit; i++) {
            for (int j = 0; j < playerList.Count; j++) {
                if (playerList[j].CardCountByHand(0) <= i) {
                    if (playerList[i].typePlayer == GameCardPlayerType.player) {
                        DealPlayerCard(playerList[j].uid);
                    }
                }
            }
        }

        
        for (int i = 0; i < handRoundLimit; i++) {
            for (int j = 0; j < playerList.Count; j++) {
                if (playerList[j].CardCountByHand(0) <= i) {
                    if (playerList[i].typePlayer == GameCardPlayerType.dealer) {
                        DealPlayerCard(playerList[j].uid);
                    }
                }
            }
        }
        
        DisplayCards();        
    }

}

// ---------------------------------------------------------------
// POKER

public class GameCardPlayerPoker : GameCardPlayer {
    
    public GameCardPlayerPoker() {
        typePlayer = GameCardPlayerType.player;
    }
}

public class GameCardPoker : GameCardBase<GameCardPlayerPoker> {
    
    public GameCardPoker() {
        
    }
    
}
