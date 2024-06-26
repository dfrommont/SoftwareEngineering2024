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
    public event Action<int> ResetEvent;
    public event Action<int> DraftCountChanged;
    public event Action CountryChanged;
    void Start(){
        gameManager.TurnPhaseChanged += turnPhaseChanged;
        gameManager.CurrentPlayerChanged += currentPlayerChanged;
        gameManager.PlayerAdded += playerAdded;
        gameManager.ResetEvent += resetEvent;
        gameManager.DraftCountChanged += draftCountChanged;
        gameManager.CountryChanged += countryChanged;
    }

    public bool nextPhase(){
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                gameManager.CompletedPhase();
                return true;
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
    public bool createAiPlayer(){
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.createAiPlayer();
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
                return gameManager.Fortify(player, origin, destination, count);
            default:
                return false;
        }
    }

    public bool battle(Country attacker, int attackRollCount, Country defender, int defendRollCount) {
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.Battle(attacker, attackRollCount, defender, defendRollCount);
            default:
                return false;
        }
    }

    public bool draft(Player player, int countryID, int amountToDraft)
    {
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.Draft(player, countryID, amountToDraft);
            default:
                return false;
        }
    }
    public bool deploy(Player player, int countryID) {
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.Deploy(player, countryID);
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

    public bool isOwnCountry(int countryID)
    {
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.isOwnCountry(countryID);
            default:
                return false;
        }
    }
    public Country getCountry(int countryID)
    {
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.getCountry(countryID);
            default:
                return null;
        }
    }

    public int getDefenceDiceToRoll(Country defendingCountry)
    {
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.getDefenceDiceToRoll(defendingCountry);
            default:
                return -1;
        }
    }

    public TurnPhase GetTurnPhase()
    {
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.GetTurnPhase();
            default:
                return TurnPhase.Setup;
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

    public void resetEvent(int a){
        Debug.Log(a);
        ResetEvent?.Invoke(a);
        Debug.Log("REGI");
    }
    
    public void draftCountChanged(int a){
        Debug.Log(a);
        DraftCountChanged?.Invoke(a);
        Debug.Log("DCCGI");
    }
}

enum GameEnvironment
{
    Local,
    Online
}