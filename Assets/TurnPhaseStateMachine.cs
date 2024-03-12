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
public class TurnPhaseStateMachine
{
    private TurnPhase currentPhase;

    public TurnPhaseStateMachine()
    {
        currentPhase = TurnPhase.Deploy;
    }

    public event Action PhaseChanged;

    private void changePhase(TurnPhase newPhase)
    {
        currentPhase = newPhase;
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

    public void changeDraftToAttack()
    {
        if (currentPhase!=TurnPhase.Draft)
        {
            throw new Exception("not in draft phase");
        }
        changePhase(TurnPhase.Attack);
    }
    public void changeAttackToFortify()
    {
        if (currentPhase!=TurnPhase.Attack)
        {
            throw new Exception("not in attack phase");
        }
        changePhase(TurnPhase.Fortify);
    }
    public void changeFortifyToDraft()
    {
        if (currentPhase!=TurnPhase.Fortify)
        {
            throw new Exception("not in fortify phase");
        }
        changePhase(TurnPhase.Draft);
    }
}
