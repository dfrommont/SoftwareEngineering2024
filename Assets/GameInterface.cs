using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameInterface: MonoBehaviour
{
    private GameEnvironment gameEnvironment = GameEnvironment.Local;
    public GameManager gameManager;
    public event Action<TurnPhase> TurnPhaseChanged;

    void Start(){
        gameManager.TurnPhaseChanged += turnPhaseChanged;
    }

    public bool nextPhase(){
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.nextPhase();
            default:
                return false;
        }
    }

    public bool fortify(Player player, Country origin, Country destination, int count) {
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.fortify(player, origin, destination, count);
            default:
                return false;
        }
    }
    public bool deploy(Player player, int countryID) {
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.deploy(player, countryID);
            default:
                return false;
        }
    }
    public void turnPhaseChanged(TurnPhase a){
        Debug.Log(a);
        TurnPhaseChanged?.Invoke(a);
        Debug.Log("GI");
    }
}

enum GameEnvironment
{
    Local,
    Online
}