using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using Random = System.Random;



public class GameManagerScript : MonoBehaviour
{
    private Dictionary<int, Country> countries;
    private Dictionary<int, Continent> continents;
    private int availableToDraft = 0;
    private int numberOfPlayers = 3;
    private Player currentPlayer;
    private Queue<Player> playerList = new();
    private TestSubscriber _testSubscriber = new TestSubscriber();

    private List<string> playerNames = new()
        { "Harold", "Horace", "Henry", "Hermine", "Hetty", "Harriet" };

    private TurnPhaseStateMachine turnPhaseStateMachine = new();

    

    void Start()
    {
        turnPhaseStateMachine.PhaseChanged += _testSubscriber.stateHasChanged;
        setupCountries();
        generatePlayers();
        var armiesToAllocate = calculateArmiesToAllocate();
        nextPlayerTurn(); // initialise current player
        allocateArmiesToUnoccupiedCountries();
        
        // allocate armies to occupied territories
        while (armiesToAllocate > 0)
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                // re-use code from player selecting unoccupied territories
            }

            armiesToAllocate--;
        }
        
        //generate deck of risk cards


        //prepare for first turn
        nextPlayerTurn();

    }

    private void allocateArmiesToUnoccupiedCountries()
    {
        // allocate armies to unoccupied countries
        for (int i = 0; i < countries.Count; i++)
        {
            // await player country allocation choice - ?event listener?
            nextPlayerTurn();
        }
    }

    private int calculateArmiesToAllocate()
    {
        //determine number of armies to allocate to players
        int armiesToAllocate;
        switch (numberOfPlayers)
        {
            case 3:
                armiesToAllocate = 35;
                break;
            case 4:
                armiesToAllocate = 30;
                break;
            case 5:
                armiesToAllocate = 25;
                break;
            case >= 6: 
                armiesToAllocate = 20;
                break;
            default:
                throw new Exception("must have at least 3 players! number of players: "+ numberOfPlayers);
        }

        return armiesToAllocate;
    }

    private void setupCountries()
    {
        string text = File.ReadAllText(@"./countries.json");
        JObject data = JObject.Parse(text);
        string countryTokens = data["countries"].ToString();
        countries = JsonConvert.DeserializeObject<Dictionary<int, Country>>(countryTokens);
        Debug.Log(countries.Count);
        string continentTokens = data["continents"].ToString();
        continents = JsonConvert.DeserializeObject<Dictionary<int, Continent>>(continentTokens);
        Debug.Log(continents.Count);
    }

    int getAvailableToDraft(){
        return availableToDraft;
    }

    bool fortify(Player player, Country origin, Country destination, int count){
        //ensure statemachine in fortify phase
        if (turnPhaseStateMachine.getTurnPhase() != TurnPhase.Fortify)
        {
            throw new Exception("not in fortify phase");
        }
        
        // check both countries are owned by the same player
        if(origin.getPlayer() != player){
            return false;
        }
        if(destination.getPlayer() != player){
            return false;
        }

        // check origin has count + 1 armies
        if(origin.getArmiesCount() > count){
            return false;
        }

        origin.removeArmies(count);
        destination.addArmies(count);
        return true;
    }
    void generatePlayers()
    {
        //generate player names for number of players
        Random rnd = new Random();
        playerNames = playerNames.OrderBy(x => rnd.Next()).Take(numberOfPlayers).ToList();
        //populate playerList
        for (int i = 0; i < numberOfPlayers; i++)
        {
            string playerName = playerNames[i];
            playerList.Enqueue(new Player(playerName));
        }
    }

    private void nextPlayerTurn()
    {
        currentPlayer = playerList.Dequeue();
        playerList.Enqueue(currentPlayer);
    }
}
