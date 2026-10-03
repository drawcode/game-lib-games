using System;

using Agnostic.Core;
using Agnostic.Host;
using Agnostic.Input;

using Engine.AgnosticHost;
using Engine.Events;

using UnityEngine;

// Gameplay input as ACTIONS through the agnostic core's ActionMap (plan-agnostic-common-usages
// P3.2, the dasher slice). Callers ask for "move" or "run" instead of reading keys, so a binding is
// data, a pad or a rebind needs no code, and the same map runs on every engine the core does.
//
// A game opts in by shipping Resources/agnostic/input-action-map.json (input-action-map.v1). With
// no asset, or with the switch off, `active` is false and every caller keeps its legacy
// Input read unchanged, so other games on this lib are unaffected.
//
// The map is evaluated once per frame by UnityHostDriver, which runs before default-order
// Updates, so every reader in a frame sees the same evaluate. Gamepads reach it through
// UnityInput's Input System source where that package is installed (left stick → move, right
// stick → aim, which the attack pad sends like its touch stick).
//
// Known differences from the legacy key reads (both deliberate):
//   - A diagonal reads magnitude 0.99, not (0.99, 0.99). Speed was already clamped to 1
//     downstream (min(|dir|, 1)), so only the axis values change, not how fast the actor moves.
//   - Opposite keys cancel, where legacy let right/down win. Mixing a WASD key and an arrow key
//     takes the stronger composite rather than combining them.
public static class GameInputActions {

    public const string resourcePath = "agnostic/input-action-map";

    public const string setGameplay = "gameplay";

    public const string actionMove = "move";
    public const string actionRun = "run";
    public const string actionAim = "aim";

    // The HUD's on-screen sticks as virtual controls (ActionMap.SetVirtualAxis). A map that binds
    // them gets touch through the same actions as keys and pads; one that does not leaves the HUD
    // sending its axes directly, as before.
    public const string controlTouchMove = "touch.stick.move";
    public const string controlTouchAim = "touch.stick.aim";

    // Kill switch. Flip it at this source default (a runtime flip lands after boot); false puts
    // every caller back on its legacy Input read.
    public static bool enabled = true;

    public static UnityHost host;
    public static ActionMap map;
    public static UnityHostDriver driver;

    private static bool touchMoveBound;
    private static bool touchAimBound;

    public static bool active {
        get {
            return enabled && map != null && driver != null;
        }
    }

    // Domain reload is off in this project, so statics outlive a play session while the driver's
    // GameObject does not. Rebuild everything on every boot.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Boot() {

        host = null;
        map = null;
        driver = null;
        touchMoveBound = false;
        touchAimBound = false;

        if (!enabled) {
            return;
        }

        UnityHost h = new UnityHost();
        ActionMapDef def = ActionMapJson.LoadResource(resourcePath, h.log);

        if (def == null) {
            return;
        }

        ActionMap m = new ActionMap(def);
        m.SetSetActive(setGameplay, true);
        h.unityInput.WatchMap(def);

        driver = UnityHostDriver.Create(h, new Loop(m), "_GameInputActions");

        // Create hands the loop in as the sink; the map is the sink here.
        h.input.SetSink(m);

        host = h;
        map = m;
        touchMoveBound = IsBound(def, controlTouchMove);
        touchAimBound = IsBound(def, controlTouchAim);
    }

    private static bool IsBound(ActionMapDef def, string control) {

        foreach (ActionSetDef set in def.actionSets) {
            foreach (ActionDef action in set.actions) {
                foreach (BindingDef b in action.bindings) {
                    if (string.Equals(b.control, control, StringComparison.Ordinal)) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    // An on-screen stick moved or let go. axisName is InputSystemKeys.moveKey or .attackKey; the
    // axis is the stick's offset in axis units. True when the map took it: the caller must NOT
    // send the axis too, because GameTouchInputAxis sends the move/aim action every frame (dead
    // zone, clamp to 1 and the stronger-source rule included). False when inactive or the map has
    // no binding for that stick, and the caller sends the axis itself, as before.
    public static bool SetTouchStick(string axisName, Vector3 axis, bool released) {

        if (!active) {
            return false;
        }

        string control;

        if (string.Equals(axisName, InputSystemKeys.moveKey, StringComparison.Ordinal)) {
            if (!touchMoveBound) {
                return false;
            }
            control = controlTouchMove;
        }
        else if (string.Equals(axisName, InputSystemKeys.attackKey, StringComparison.Ordinal)) {
            if (!touchAimBound) {
                return false;
            }
            control = controlTouchAim;
        }
        else {
            return false;
        }

        if (released) {
            map.ClearVirtual(control);
        }
        else {
            map.SetVirtualAxis(control, new Vec2(axis.x, axis.y));
        }

        return true;
    }

    // The dasher move axis in the shape GameController.SendInputAxisMessage takes. False when
    // inactive, and the caller reads its legacy keys instead.
    public static bool TryGetMove(out Vector3 axis) {

        if (!active) {
            axis = Vector3.zero;
            return false;
        }

        Vec2 v = map.Axis2d(actionMove);
        axis = new Vector3(v.x, v.y, 0f);
        return true;
    }

    // The attack (aim) axis: a pad's right stick, and the HUD's aim stick where the map binds
    // touch.stick.aim. False when inactive; legacy had no key path for attack.
    public static bool TryGetAim(out Vector3 axis) {

        if (!active) {
            axis = Vector3.zero;
            return false;
        }

        Vec2 v = map.Axis2d(actionAim);
        axis = new Vector3(v.x, v.y, 0f);
        return true;
    }

    // Legacy: the Input Manager's Fire3 (left cmd, mouse 2, joystick button 2). A global read --
    // callers that move more than the player gate it on who is asking (see
    // BaseGamePlayerThirdPersonController.ownerController).
    public static bool IsRunHeld() {

        if (!active) {
            return Input.GetButton("Fire3");
        }

        return map.Held(actionRun);
    }

    private class Loop : ICoreLoop {

        private readonly ActionMap map;

        public Loop(ActionMap map) {
            this.map = map;
        }

        public void OnFrame(float unscaledDt) {
            map.Evaluate();
        }

        public void OnFixed(float fixedDt) {
        }

        public void OnLifecycle(AppLifecycle e) {

            // The host already released every watched control; drop derived state too, so no
            // action reads as held when focus returns.
            if (e == AppLifecycle.paused || e == AppLifecycle.focusLost) {
                map.ResetAll();
            }
        }
    }
}
