using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
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
    private List<Country> unoccupiedCountries = new();

    private List<string> playerNames = new()
        { "Harold", "Horace", "Henry", "Hermine", "Hetty", "Harriet" };

    private TurnPhaseManager turnPhaseStateMachine = new();
    
    void Start()
    {
        turnPhaseStateMachine.PhaseChanged += _testSubscriber.stateHasChanged;
        initCountries();
        generatePlayers();
        availableToDraft = calculateArmiesToAllocate();
        unoccupiedCountries = countries.Values.ToList();

        //generate deck of risk cards
        
        //prepare for first turn
        nextPlayerTurn();
        
    }

    public void draft(Country draftToCountry, Player player, int amountToDraft)
    {
        if (turnPhaseStateMachine.getTurnPhase()!=TurnPhase.Draft)
        {
            throw new Exception("not in draft phase!");
        }
        if (draftToCountry.getPlayer() != player)
        {
            throw new Exception("can't draft to country not owned by this player!");
        }
        if (amountToDraft > availableToDraft)
        {
            throw new Exception("can't draft more armies than are available to draft");
        }

        draftToCountry.addArmies(amountToDraft);
        availableToDraft -= amountToDraft;

    }

    private int calculateAvailableToDraft()
    {
        if (turnPhaseStateMachine.getTurnPhase()!=TurnPhase.Draft)
        {
            throw new Exception("not in draft phase!");
        }
        int amountAvailableToDraft = 0;
        //calculate territory bonus
        int territoriesOwnedByCurrentPlayer = 0;
        foreach (var country in countries.Values)
        {
            if (country.getPlayer()==currentPlayer)
            {
                territoriesOwnedByCurrentPlayer++;
            }
        }
        amountAvailableToDraft += territoriesOwnedByCurrentPlayer / 3;
        //calculate continent bonus
        foreach (var continent in continents.Values)
        {
            if (continent.isAllOwnedByOnePlayer() && continent.getPlayer() == currentPlayer)
            {
                availableToDraft += continent.getContinentBonus();
            }
        }
        
        //ensure min of 3
        return Math.Max(3, amountAvailableToDraft);

    }


    private void deploy(Country deployToCountry)
    {
        if (turnPhaseStateMachine.getTurnPhase() != TurnPhase.Deploy)
        {
            throw new Exception("not in initial setup phase!");
        }
        

        if (unoccupiedCountries.Any()) //if unoccupiedCountries has elements - i.e. still deploying to unoccupied countries
        {
            if (deployToCountry.getPlayer() != null)
            {
                throw new Exception("can't deploy to occupied country yet, still unoccupied countries remaining!");
            }

            unoccupiedCountries.Remove(deployToCountry);
        }
        else // if unoccupiedCountries has no elements - i.e. finished deploying to unoccupied countries
        {
            if (deployToCountry.getPlayer() != currentPlayer)
            {
                throw new Exception("can't deploy to country occupied by another player!");
            }
        }
        deployToCountry.setPlayer(currentPlayer);
        deployToCountry.addArmies(1);
        availableToDraft--;
        nextPlayerTurn();
        if (availableToDraft == 0)
        {
            turnPhaseStateMachine.changePhase();
        }
    }
    private int calculateArmiesToAllocate()
    {
        //determine number of armies to allocate to players
        //global amount of armies is multiplied by number of players
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

        return armiesToAllocate * numberOfPlayers;
    }
    int getAvailableToDraft(){
        return availableToDraft;
    }

    public bool fortify(Player player, Country origin, Country destination, int count){

        // Check if fortify is a valid move in gamestate.
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
