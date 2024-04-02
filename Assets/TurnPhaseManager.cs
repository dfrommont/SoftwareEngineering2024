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
        EnteredDeploy?.Invoke();
    }

    public event Action<TurnPhase> PhaseChanged;
    public event Action EnteredDeploy;
    public event Action EnteredDraft;
    public event Action EnteredAttack;
    public event Action EnteredFortify;
    

    public void nextTurnPhase()
    {
        switch (currentPhase) {
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
                currentPhase = TurnPhase.Draft;
                EnteredDraft?.Invoke();
                break;
        }
        PhaseChanged?.Invoke(currentPhase);
    }

    public TurnPhase getTurnPhase()
    {
        return currentPhase;
    }
}
