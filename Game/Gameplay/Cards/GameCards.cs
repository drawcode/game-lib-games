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
    public string type = GameCardTypes.cardNormal;
    public string val = "";

}

public class GameCardDeck { 
    List<GameCard> cards = new List<GameCard>();
}

public class GameCardSet {
    List<GameCardDeck> decks = new List<GameCardDeck>();
}


public class GameCards {
    
    private static volatile GameCards instance;
    private static System.Object syncRoot = new System.Object();
    
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

    }
}
