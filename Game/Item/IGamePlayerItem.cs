using System;

public interface IGamePlayerItem {
        void PlayContent();
        void StopContent();
        void ResetContent();
        void RemoveContent();
        void CollectContent();
}

public enum GamePlayerItemType {
        Generic,
        Point,
        RandomPrize,
        Health,
        DoubleCoin,
        Coin,
        Pickup
}
