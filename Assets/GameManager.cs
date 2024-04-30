using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using Random = System.Random;
using NUnit.Framework.Constraints;



public class GameManager : MonoBehaviour
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
    public event Action<TurnPhase> TurnPhaseChanged;
    public event Action<Player> CurrentPlayerChanged;
    public event Action<Player> PlayerAdded;
    public event Action<int> ResetEvent;
    public event Action<int> DraftCountChanged;
    public event Action CountryChanged;

    private List<string> playerNames = new()
        { "Harold", "Horace", "Henry", "Hermine", "Hetty", "Harriet" };

    private TurnPhaseManager turnPhaseStateMachine = new();
    
    void Start()
    {
        turnPhaseStateMachine.PhaseChanged += turnPhaseChanged;
        initCountries();
        Player p1 = new Player("Bob");
        playerList.Enqueue(p1);
        PlayerAdded?.Invoke(p1);
        Player p2 = new Player("Kevin");
        playerList.Enqueue(p2);
        PlayerAdded?.Invoke(p2);
        Player p3 = new Player("Stuart");
        playerList.Enqueue(p3);
        PlayerAdded?.Invoke(p3);
    }

    public bool startGame(){
        // Validate game is in a valid state
        if(playerList.Count<2){
            Debug.Log("Not enough players");
            return false;
        } else if (playerList.Count>6){
            Debug.Log("Too many players");
            return false;
        }
        nextPhase();
        availableToDraft = calculateArmiesToAllocate();
        unoccupiedCountries = countries.Values.ToList();

        //generate deck of risk cards
        
        //prepare for first turn
        Debug.Log("COMPLETE");
        return true;
    }

    public List<Country> getCountries(){
        List<Country> return_countries = new List<Country>();
        foreach(KeyValuePair<int, Country> country in countries)
        {
            return_countries.Add(country.Value);
        }
        return return_countries;
    }

    public bool nextPhase(){
        turnPhaseStateMachine.nextTurnPhase();
        ResetEvent?.Invoke(0);
        if (turnPhaseStateMachine.getTurnPhase()==TurnPhase.Draft)
        {
            nextPlayerTurn();
            availableToDraft = calculateAvailableToDraft();
            DraftCountChanged?.Invoke(availableToDraft);
        }
        return true;
    }

    public void turnPhaseChanged(TurnPhase turnPhase){
        TurnPhaseChanged?.Invoke(turnPhase);
        Debug.Log("GM");
    }
    
    public void countryChanged()
    {
        CountryChanged?.Invoke();
        Debug.Log("GM");
    }

    public bool draft(Player player, int countryID, int amountToDraft)
    {
        ResetEvent?.Invoke(0);
        Country draftToCountry = countries[countryID];
        if (turnPhaseStateMachine.getTurnPhase()!=TurnPhase.Draft)
        {
            throw new Exception("not in draft phase!");
        }
        if (draftToCountry.getPlayer() != currentPlayer)
        {
            throw new Exception("can't draft to country not owned by this player!");
        }
        if (amountToDraft > availableToDraft)
        {
            throw new Exception("can't draft more armies than are available to draft");
        }
        draftToCountry.addArmies(amountToDraft);
        availableToDraft -= amountToDraft;
        if (availableToDraft == 0)
        {
            nextPhase();
        }
        DraftCountChanged?.Invoke(availableToDraft);
        countryChanged();
        return true;
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

    public bool deploy(Player player, int countryID)
    {
        ResetEvent?.Invoke(0);
        Country deployToCountry = countries[countryID];
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
        Debug.Log(currentPlayer.getName());
        Debug.Log(currentPlayer.getColor().ToString());
        deployToCountry.addArmies(1);
        availableToDraft--;
        nextPlayerTurn();
        if (availableToDraft == 0)
        {
            nextPhase();
        }
        countryChanged();
        return true;
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
            case 2:
                armiesToAllocate = 15;
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
        Debug.Log("FORTIFY");
        // Check if fortify is a valid move in gamestate.
        if (turnPhaseStateMachine.getTurnPhase() != TurnPhase.Fortify)
        {
            throw new Exception("not in fortify phase");
        }
        
        // Check both countries are owned by the same player
        if(origin.getPlayer() != currentPlayer){
            // Debug.Log("orig");
            return false;
        }
        if(destination.getPlayer() != currentPlayer){
            // Debug.Log("dest");
            return false;
        }

        // Check if destination country is a neigbour to the origin
        if(origin.isNeighbour(destination)){
            Debug.Log("neighbour");
            return false;
        }

        // Check origin has count + 1 armies
        if(origin.getArmiesCount() > count){
            Debug.Log("count");
            return false;
        }

        // Passed all checks, so continue with movement
        origin.removeArmies(count);
        destination.addArmies(count);
        countryChanged();
        ResetEvent?.Invoke(0);
        return true;
    }

    public bool createPlayer(string name) {
        var pl = new Player(name);
        playerList.Enqueue(pl);
        PlayerAdded?.Invoke(pl);
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
    public bool firstPlayer(int playerIndex){
        for (int i = 0; i < playerIndex+1; i++)
        {
            currentPlayer = playerList.Dequeue();
            playerList.Enqueue(currentPlayer);
        }
        CurrentPlayerChanged?.Invoke(currentPlayer);
        return true;
    }

    private void nextPlayerTurn()
    {
        currentPlayer = playerList.Dequeue();
        playerList.Enqueue(currentPlayer);
        CurrentPlayerChanged?.Invoke(currentPlayer);
    }

    public bool isOwnCountry(int countryID)
    {
        return countries[countryID].getPlayer() == currentPlayer;
    }

    public Country getCountry(int countryID)
    {
        return countries[countryID];
    }

    void initCountries(){
        string text = File.ReadAllText(@"./countries.json");
        JObject data = JObject.Parse(text);
        string countryTokens = data["countries"].ToString();
        countries = JsonConvert.DeserializeObject<Dictionary<int, Country>>(countryTokens);
        Debug.Log(countries.Count);

        foreach (KeyValuePair<int, Country> pair in countries)
        {
            Debug.Log(pair.Key);
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

    public bool battle(Country attacker, int attackRollCount, Country defender, int defendRollCount) {

        Random rnd = new Random();

        List<int> attackRolls = new();
        List<int> defendRolls = new();

        for (int i = 0; i < attackRollCount; i++)
        {
            attackRolls.Add(rnd.Next(1,7));
        }
        for (int i = 0; i < defendRollCount; i++)
        {
            defendRolls.Add(rnd.Next(1,7));
        }
        attackRolls.Sort();
        defendRolls.Sort();
        attackRolls.Reverse();
        defendRolls.Reverse();

        int attackCount = 0;
        int defendCount = 0;
        for (int i = 0; i < Math.Min(attackRollCount, defendRollCount); i++)
        {
            if(attackRolls[i]>defendRolls[i]){
                attackCount++;
            } else {
                defendCount++;
            }
        }

        if(defender.getArmiesCount()==attackCount){
            // Attacker gets country
            defender.setPlayer(currentPlayer);
            defender.zeroArmies();
            defender.addArmies(attackRollCount);
            attacker.removeArmies(attackRollCount);
        } else {
            // Decrement both countries
            attacker.removeArmies(defendCount);
            defender.removeArmies(attackCount);
        }
        countryChanged();
        ResetEvent?.Invoke(0);
        return true;
    }
    internal bool fortify(string player, Country origin, Country destination, int count)
    {
        throw new NotImplementedException();
    }
}
