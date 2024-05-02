using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnPhase
{
    Setup,
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
        currentPhase = TurnPhase.Setup;
    }

    public event Action<TurnPhase> PhaseChanged;
    public event Action EnteredDeploy;
    public event Action EnteredDraft;
    public event Action EnteredAttack;
    public event Action EnteredFortify;
    public event Action EndedTurn;
    

    public void nextTurnPhase()
    {
        switch (currentPhase) {
            case TurnPhase.Setup:
                currentPhase = TurnPhase.Deploy;
                break;
            case TurnPhase.Deploy:
                currentPhase = TurnPhase.Draft;
                EnteredDraft?.Invoke();
                break;
            case TurnPhase.Draft:
                currentPhase = TurnPhase.Attack;
                EnteredAttack?.Invoke();
                break;
            case TurnPhase.Attack:
                currentPhase = TurnPhase.Fortify;
                EnteredFortify?.Invoke();
                break;
            case TurnPhase.Fortify:
                EndedTurn?.Invoke();
                currentPhase = TurnPhase.Draft;
                break;
        }
        PhaseChanged?.Invoke(currentPhase);
    }

    public TurnPhase getNextTurnPhase()
    {
        switch (currentPhase) {
            case TurnPhase.Setup:
                return TurnPhase.Deploy;
            case TurnPhase.Deploy:
                return TurnPhase.Draft;
            case TurnPhase.Draft:
                return TurnPhase.Attack;
            case TurnPhase.Attack:
                return TurnPhase.Fortify;
            default: //case TurnPhase.Fortify:
                return TurnPhase.Draft;
        };
    }

    public TurnPhase getTurnPhase()
    {
        return currentPhase;
    }
}
