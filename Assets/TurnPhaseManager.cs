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

    public event Action PhaseChanged;
    public event Action EnteredDeploy;
    public event Action EnteredDraft;
    public event Action EnteredAttack;
    public event Action EnteredFortify;
    

    public void changePhase()
    {
        if (currentPhase == TurnPhase.Deploy)
        {
            currentPhase = TurnPhase.Draft;
            EnteredDraft?.Invoke();
        }
        if (currentPhase == TurnPhase.Draft)
        {
            currentPhase = TurnPhase.Attack;
            EnteredAttack?.Invoke();
        }
        if (currentPhase == TurnPhase.Attack)
        {
            currentPhase = TurnPhase.Fortify;
            EnteredFortify?.Invoke();
        }
        if (currentPhase == TurnPhase.Fortify)
        {
            currentPhase = TurnPhase.Draft;
            EnteredDraft?.Invoke();
        }
        PhaseChanged?.Invoke();
    }

    public TurnPhase getTurnPhase()
    {
        return currentPhase;
    }
}
