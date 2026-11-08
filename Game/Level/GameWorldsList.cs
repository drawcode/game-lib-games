// What the worlds panel's list shows, for a game that puts the planet picker in front of every
// play mode (BaseUIController.useWorldsList; contexts games/shared/context-arcade-vs-missions.md).
//
// A separate value because the content state can't say it: CREATE plays under the arcade state,
// and the arcade state is also the default state.
public enum GameWorldsListKind {
    missions,
    arcade,
    created
}

public static class GameWorldsList {

    public static GameWorldsListKind kind = GameWorldsListKind.missions;

    // Set by a mode-select tile: the next time the worlds panel opens it starts on the picker.
    // Other routes in (Results CONTINUE) leave it alone and come back to the list they left.
    public static bool pickPlanet = false;

    public static void Begin(GameWorldsListKind listKind) {
        kind = listKind;
        pickPlanet = true;
    }
}
