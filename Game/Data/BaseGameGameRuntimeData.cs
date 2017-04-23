using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class BaseGameGameRuntimeData {
    public double currentLevelTime = 0;
    public double timeRemaining = 90;
    public double coins = 0;
    public string levelCode = "";
    public double score = 0;
    public bool outOfBounds = false;

    // GAMEPLAY TYPE SPECIFIC

    // RUNNER

    // TODO move to runtimeData n 

    public Vector3 rangeStart;
    public Vector3 rangeEnd;
    public Vector4 curve;

    public bool curveEnabled = true;
    public float curveInfiniteDistance = 0f;
    public Vector4 curveInfiniteAmount;     // Determines how much the platform bends (default value (-5,-5,0,0)

    public BaseGameGameRuntimeData() {
        Reset();
    }

    public virtual void Reset() {
        currentLevelTime = 0;
        timeRemaining = 90;
        coins = 0;
        levelCode = "";
        score = 0;
        outOfBounds = false;
        ResetTimeDefault();

        // TYPES

        rangeStart = Vector3.zero.WithX(-16f);
        rangeEnd = Vector3.zero.WithX(16f);
        curve = Vector4.zero;

        curveEnabled = true;
        curveInfiniteDistance = 50f;
        curveInfiniteAmount = Vector4.zero;     // Determines how much the platform bends (default value (-5,-5,0,0)

    }

    public virtual bool timeExpired {
        get {
            if(timeRemaining <= 0) {
                timeRemaining = 0;
                return true;
            }
            return false;
        }
    }

    public virtual bool localPlayerWin {
        get {
            return !timeExpired;
        }
    }

    public virtual void SubtractTime(double delta) {
        if(timeRemaining > 0) {
            timeRemaining -= delta;
        }
    }

    public virtual void ResetTimeDefault() {
        timeRemaining = 90;
    }

    public virtual void ResetTime(double timeTo) {
        timeRemaining = timeTo;
    }

    public virtual void AppendTime(double timeAppend) {
        timeRemaining += timeAppend;
    }


}