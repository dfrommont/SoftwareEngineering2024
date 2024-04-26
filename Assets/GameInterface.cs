using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameInterface: MonoBehaviour
{
    private GameEnvironment gameEnvironment = GameEnvironment.Local;
    public GameManager gameManager;
    public event Action<TurnPhase> TurnPhaseChanged;
    public event Action<Player> CurrentPlayerChanged;
    public event Action<Player> PlayerAdded;
    public event Action CountryChanged;

    void Start(){
        gameManager.TurnPhaseChanged += turnPhaseChanged;
        gameManager.CurrentPlayerChanged += currentPlayerChanged;
        gameManager.PlayerAdded += playerAdded;
        gameManager.CountryChanged += countryChanged;
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

    public bool createPlayer(string name){
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.createPlayer(name);
            default:
                return false;
        }
    }

    public bool startGame(){
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.startGame();
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

    public bool draft(Player player, int countryID, int amountToDraft)
    {
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.draft(player, countryID, amountToDraft);
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
    public bool firstPlayer(int playerIndex) {
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.firstPlayer(playerIndex);
            default:
                return false;
        }
    }

    public List<Country> getCountries()
    {
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.getCountries();
            default:
                return null;
        }
    }
    public void turnPhaseChanged(TurnPhase a){
        Debug.Log(a);
        TurnPhaseChanged?.Invoke(a);
        Debug.Log("GI");
    }
    public void currentPlayerChanged(Player a){
        Debug.Log(a);
        CurrentPlayerChanged?.Invoke(a);
        Debug.Log("PCGI");
    }
    public void playerAdded(Player a)
    {
        Debug.Log(a);
        PlayerAdded?.Invoke(a);
        Debug.Log("PAGI");
    }
    public void countryChanged()
    {
        CountryChanged?.Invoke();
        Debug.Log("CCGI");
    }
}

enum GameEnvironment
{
    Local,
    Online
}