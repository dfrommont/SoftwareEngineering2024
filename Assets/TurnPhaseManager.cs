using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnPhase
{
    Draft,
    Attack,
    Fortify,
    Deploy
}
public class TurnPhaseManager
{
    private TurnPhase currentPhase;

    public TurnPhaseManager()
    {
        currentPhase = TurnPhase.Deploy;
    }

    public event Action PhaseChanged;

    private void changePhase()
    {
        if (currentPhase == TurnPhase.Deploy)
        {
            currentPhase = TurnPhase.Draft;
        }
        if (currentPhase == TurnPhase.Draft)
        {
            currentPhase = TurnPhase.Attack;
        }
        if (currentPhase == TurnPhase.Attack)
        {
            currentPhase = TurnPhase.Fortify;
        }
        if (currentPhase == TurnPhase.Fortify)
        {
            currentPhase = TurnPhase.Draft;
        }
        PhaseChanged?.Invoke();
    }

    public TurnPhase getTurnPhase()
    {
        return currentPhase;
    }
}
