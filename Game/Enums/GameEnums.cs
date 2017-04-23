using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ENUMS

// CONTROLLER

public enum GameZones {
    left,
    right
}

public enum GameStateGlobal {
    GameNotStarted,
    GameInit,
    GamePrepare,
    GameStarted,
    GameQuit,
    GamePause,
    GameResume,
    GameResults,
    GameContentDisplay, // dialog or in progress choice/content/collection status
    GameOverlay, // external dialog such as sharing/community/over
}

public enum GameControllerType {
    Iso2DSide,
    Iso3D,
    Iso2DTop,
    Perspective3D
}

public enum GameCameraView {
    ViewSide, // tecmo
    ViewSideTop, // tecmo
    ViewBackTilt, // backbreaker cam
    ViewBackTop // john elway cam
}

public enum GameRunningState {
    PAUSED,
    RUNNING,
    STOPPED
}

// PLAYER

public enum GamePlayerControllerState {
    ControllerAgent = 0,
    ControllerPlayer = 1,
    ControllerUI = 2,
    ControllerNetwork = 3,
    ControllerNotSet = 4,
    ControllerSidekick = 5,
    ControllerAsset = 6,
}

public enum GamePlayerActorState {
    ActorMe,
    ActorEnemy,
    ActorFriend
}

public enum GamePlayerContextState {
    ContextInput = 0,
    ContextInputVehicle,
    ContextFollowAgent,
    ContextFollowAgentAttack,
    ContextFollowAgentAttackVehicle,
    ContextFollowInput,
    ContextScript,
    ContextRandom,
    ContextUI,
    ContextNetwork,
    ContextNotSet
}

public enum GameActionTriggerState {
    TriggerEnter,
    TriggerExit,
    ActionTriggerEnter,
    ActionTriggerExit
}