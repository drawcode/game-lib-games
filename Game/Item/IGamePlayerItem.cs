using System;

public interface IGamePlayerItem {
        void PlayContent();
        void StopContent();
        void ResetContent();
        void RemoveContent();
        void CollectContent();
}

/*
public class GamePlayerItemType {
    public static string itemGeneric = "item-generic";
    public static string itemPoint = "item-point";
    public static string itemAction = "item-action";
    public static string itemRandom = "item-random";
    public static string itemHealth = "item-health";
    public static string itemCoin = "item-coin";
    public static string itemDoubleCoin = "item-double-coin";
}
*/

/*
public enum GamePlayerItemType {
        Generic,
        Point,
        RandomPrize,
        Health,
        DoubleCoin,
        Coin,
        Pickup
}
*/
