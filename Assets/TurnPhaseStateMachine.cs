using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnPhase
{
    Draft,
    Attack,
    Fortify
}
public class TurnPhaseStateMachine
{
    private TurnPhase currentPhase;
    public event Action phaseChanged;

    private void changePhase(TurnPhase newPhase)
    {
        
        if (currentPhase != null)
        {
            //any code to run on setting initial phase
            
        }

        currentPhase = newPhase;
    }

    public TurnPhase getTurnPhase()
    {
        return currentPhase;
    }
}
