using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Engine.Events;

public class BaseGameController : MonoBehaviour {

    public GamePlayerController currentGamePlayerController;

    // ----------------------------------------------------------------------

    public virtual void Start() {

    }

    public virtual void Init() {

    }

    // ---------------------------------------------------------------------

    // GAMEPLAYER CONTROLLER
    
    public GamePlayerController getCurrentPlayerController {
        get {
            return getCurrentController();
        }
    }

    public GamePlayerController getCurrentController() {
        if(GameController.Instance.currentGamePlayerController != null) {
            return GameController.Instance.currentGamePlayerController;
        }
        return null;
    }
 
    // ATTACK
    
    //public static void GamePlayerAttack() {
    //    if(isInst) {
    //        Instance.gamePlayerAttack();
    //    }
    //}
    
    public void gamePlayerAttack() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendAttack();
        }
    }

    //public static void GamePlayerAttackAlt() {
    //    if(isInst) {
    //        Instance.gamePlayerAttackAlt();
    //    }
    //}

    public void gamePlayerAttackAlt() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendAttackAlt();
        }
    }

    //public static void GamePlayerAttackRight() {
    //    if(isInst) {
    //        Instance.gamePlayerAttackRight();
    //    }
    //}

    public void gamePlayerAttackRight() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendAttackRight();
        }
    }

    //public static void GamePlayerAttackLeft() {
    //    if(isInst) {
    //        Instance.gamePlayerAttackLeft();
    //    }
    //}

    public void gamePlayerAttackLeft() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendAttackLeft();
        }
    }

    // DEFEND

    //public static void GamePlayerDefend() {
    //    if(isInst) {
    //        Instance.gamePlayerDefend();
    //    }
    //}

    public void gamePlayerDefend() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendDefend();
        }
    }

    //public static void GamePlayerDefendAlt() {
    //    if(isInst) {
    //        Instance.gamePlayerDefendAlt();
    //    }
    //}

    public void gamePlayerDefendAlt() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendDefendAlt();
        }
    }

    //public static void GamePlayerDefendRight() {
    //    if(isInst) {
    //        Instance.gamePlayerDefendRight();
    //    }
    //}

    public void gamePlayerDefendRight() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendDefendRight();
        }
    }

    //public static void GamePlayerDefendLeft() {
    //    if(isInst) {
    //        Instance.gamePlayerDefendLeft();
    //    }
    //}

    public void gamePlayerDefendLeft() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendDefendLeft();
        }
    }
 
    // JUMP

    //public static void GamePlayerJump() {
    //    if(isInst) {
    //        Instance.gamePlayerJump();
    //    }
    //}
    
    public void gamePlayerJump() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.InputJump();
        }
    }
     
    // USE
    
    //public static void GamePlayerUse() {
    //    if(isInst) {
    //        Instance.gamePlayerUse();
    //    }
    //}
    
    public void gamePlayerUse() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.InputUse();
        }
    }

    // SKILL

    //public static void GamePlayerSkill() {
    //    if(isInst) {
    //        Instance.gamePlayerSkill();
    //    }
    //}

    public void gamePlayerSkill() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.InputSkill();
        }
    }

    // MAGIC

    //public static void GamePlayerMagic() {
    //    if(isInst) {
    //        Instance.gamePlayerMagic();
    //    }
    //}

    public void gamePlayerMagic() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.InputMagic();
        }
    }

    // ----------------------------------------------------------------------

    // ZONES

    public GameZone GetZone(GameObject go) {
        if(go != null) {
            return go.GetComponent<GameZone>();
        }
        return null;
    }

    public GameGoalZone GetGoalZone(GameObject go) {
        if(go != null) {
            return go.GetComponent<GameGoalZone>();
        }
        return null;
    }

    public GameBadZone GetBadZone(GameObject go) {
        if(go != null) {
            return go.GetComponent<GameBadZone>();
        }
        return null;
    }

    // ----------------------------------------------------------------------

    // EXTRA

    public Vector3 CardinalAngles(Vector3 pos1, Vector3 pos2) {
    
        // Adjust both positions to be relative to our origin point (pos1)
        pos2 -= pos1;
        pos1 -= pos1;
    
        Vector3 angles = Vector3.zero;
    
        // Rotation to get from World +Z to pos2, rotated around World X (degrees up from Z axis)
        angles.x = Vector3.Angle( Vector3.forward, pos2 - Vector3.right * pos2.x );
    
        // Rotation to get from World +Z to pos2, rotated around World Y (degrees right? from Z axis)
        angles.y = Vector3.Angle( Vector3.forward, pos2 - Vector3.up * pos2.y );
    
        // Rotation to get from World +X to pos2, rotated around World Z (degrees up from X axis)
        angles.z = Vector3.Angle( Vector3.right, pos2 - Vector3.forward * pos2.z );
    
        return angles;
    }
    
    public float ContAngle(Vector3 fwd, Vector3 targetDir, Vector3 upDir) {
        var angle = Vector3.Angle(fwd, targetDir);
    
        if (AngleDir(fwd, targetDir, upDir) == -1) {
            return 360 - angle;
        }
        else {
            return angle;
        }
    }
    
    //returns -1 when to the left, 1 to the right, and 0 for forward/backward
    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
    
        Vector3 perp = Vector3.Cross(fwd, targetDir);
    
        float dir = Vector3.Dot(perp, up);
    
        if (dir > 0.0) {
            return 1.0f;
        }
        else if (dir < 0.0) {
            return -1.0f;
        }
        else {
            return 0.0f;
        }
    }
}