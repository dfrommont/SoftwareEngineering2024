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
    // Stores the number of armies that a player has left to draft in their draft phase
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
        generatePlayers();
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
        //set unallocated armies for all players

        
        //prepare for first turn
        nextPlayerTurn();
        initCountries();
    }

    int getAvailableToDraft(){
        return availableToDraft;
    }

    public bool fortify(string player, Country origin, Country destination, int count){

        // Check if fortify is a valid move in gamestate. Reliant on StateManager
        if (turnPhaseStateMachine.getTurnPhase() != TurnPhase.Fortify)
        {
            throw new Exception("not in fortify phase");
        }
        
        // Check both countries are owned by the same player
        if(origin.getPlayer() != player){
            return false;
        }
        if(destination.getPlayer() != player){
            return false;
        }

        // Check if destination country is a neigbour to the origin
        if(origin.isNeighbour(destination)){
            return false;
        }

        // Check origin has count + 1 armies
        if(origin.getArmiesCount() > count){
            return false;
        }

        // Passed all checks, so continue with movement
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

    void initCountries(){
        string text = File.ReadAllText(@"./countries.json");
        JObject data = JObject.Parse(text);
        string countryTokens = data["countries"].ToString();
        countries = JsonConvert.DeserializeObject<Dictionary<int, Country>>(countryTokens);
        Debug.Log(countries.Count);

        foreach (KeyValuePair<int, Country> pair in countries)
        {
            pair.Value.initNeighbours(countries);
        }
        string continentTokens = data["continents"].ToString();
        continents = JsonConvert.DeserializeObject<Dictionary<int, Continent>>(continentTokens);
        Debug.Log(continents.Count);
        foreach (KeyValuePair<int, Continent> pair in continents)
        {
            pair.Value.initCountries(countries);
        }
    }
}
