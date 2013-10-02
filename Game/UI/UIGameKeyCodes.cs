using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;


public class UIGameKeyCodes {

    public static KeyCode keyRPGCurrencyAdd = KeyCode.Alpha9;
    public static KeyCode keyRPGCurrencySubtract = KeyCode.Alpha8;
    public static KeyCode keyRPGUpgradesAdd = KeyCode.Alpha7;
    public static KeyCode keyRPGUpgradesSubtract = KeyCode.Alpha6;
    public static KeyCode keyRPGXPAdd = KeyCode.Alpha5;
    public static KeyCode keyRPGXPSubtract = KeyCode.Alpha4;

    public static bool KeyAction(KeyCode keyCode) {
        return Input.GetKeyDown(keyCode);
    }

    public static bool KeyActionControl(KeyCode keyCode) {
        return (
            (Input.GetKey(KeyCode.LeftControl)
            || Input.GetKey(KeyCode.RightControl))
            && Input.GetKeyDown(keyCode));
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