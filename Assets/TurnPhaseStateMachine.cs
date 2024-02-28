using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPhaseStateMachine
{
    private State currentState;

    private DraftState draftState = new DraftState();
    private AttackState attackState = new AttackState();
    private FortifyState fortifyState = new FortifyState();


    public void changeState(State newState)
    {
        if (currentState != null)
        {
            currentState.onExit();
        }

        currentState = newState;
        currentState.onEnter();
    }
}
