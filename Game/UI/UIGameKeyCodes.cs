using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UIGameKeyCodes {

    public static KeyCode keyRPGCurrencyAdd = KeyCode.Alpha0;
    public static KeyCode keyRPGCurrencySubtract = KeyCode.Alpha9;
    public static KeyCode keyRPGUpgradesAdd = KeyCode.Alpha8;
    public static KeyCode keyRPGUpgradesSubtract = KeyCode.Alpha7;
    public static KeyCode keyRPGXPAdd = KeyCode.Alpha6;
    public static KeyCode keyRPGXPSubtract = KeyCode.Alpha5;
    public static KeyCode keyRPGEnergyAdd = KeyCode.Alpha4;
    public static KeyCode keyRPGEnergySubtract = KeyCode.Alpha3;
    public static KeyCode keyRPGHealthAdd = KeyCode.Alpha2;
    public static KeyCode keyRPGHealthSubtract = KeyCode.Alpha1;
    public static KeyCode keyRPGLevelAdd = KeyCode.O;
    public static KeyCode keyRPGLevelSubtract = KeyCode.P;

    
    public static KeyCode keyProfileSync = KeyCode.KeypadMultiply;
    public static KeyCode keyProfileSave = KeyCode.KeypadDivide;

    
    public static KeyCode keyRPGPlayerHitAdd = KeyCode.KeypadPlus;
    public static KeyCode keyRPGPlayerHitSubtract = KeyCode.KeypadMinus;


    public static bool KeyAction(KeyCode keyCode) {
        return Input.GetKeyDown(keyCode);
    }

    public static bool KeyActionControl(KeyCode keyCode) {
        return (Application.isEditor &&
            (Input.GetKey(KeyCode.LeftControl)
            || Input.GetKey(KeyCode.RightControl))
            && Input.GetKeyDown(keyCode));
    }

    
    public static bool isActionProfileSync {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyProfileSync);
        }
    }       
    
    public static bool isActionProfileSave {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyProfileSave);
        }
    }

    public static bool isActionCurrencyAdd {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGCurrencyAdd);
        }
    }

    public static bool isActionCurrencySubtract {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGCurrencySubtract);
        }
    }
    
    public static bool isActionPlayerHitAdd {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGPlayerHitAdd);
        }
    }
    
    public static bool isActionPlayerHitSubtract {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGPlayerHitSubtract);
        }
    }

    public static bool isActionXPAdd {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGXPAdd);
        }
    }

    public static bool isActionXPSubtract {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGXPSubtract);
        }
    }

    public static bool isActionEnergyAdd {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGEnergyAdd);
        }
    }

    public static bool isActionEnergySubtract {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGEnergySubtract);
        }
    }

    public static bool isActionHealthAdd {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGHealthAdd);
        }
    }

    public static bool isActionHealthSubtract {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGHealthSubtract);
        }
    }

    public static bool isActionLevelAdd {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGLevelAdd);
        }
    }

    public static bool isActionLevelSubtract {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGLevelSubtract);
        }
    }

    public static bool isActionUpgradesAdd {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGUpgradesAdd);
        }
    }

    public static bool isActionUpgradesSubtract {
        get {
            return UIGameKeyCodes.KeyActionControl(UIGameKeyCodes.keyRPGUpgradesSubtract);
        }
    }
}