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
    private RiskCardDeck riskCardDeck;
    private int setsOfRiskCardsTradedIn;
    private bool anyCountryCapturedThisTurn;
    public event Action<TurnPhase> TurnPhaseChanged;
    public event Action<Player> CurrentPlayerChanged;
    public event Action<Player> PlayerAdded;
    public event Action CountryChanged;

    private List<string> playerNames = new()
        { "Harold", "Horace", "Henry", "Hermine", "Hetty", "Harriet" };

    private TurnPhaseManager turnPhaseStateMachine = new();
    
    void Start()
    {
        turnPhaseStateMachine.PhaseChanged += turnPhaseChanged;
        turnPhaseStateMachine.EndedTurn += endOfTurnActions;
        initCountries();
    }

    private void endOfTurnActions()
    {
        if (anyCountryCapturedThisTurn)
        {
            currentPlayer.addRiskCardToHand(riskCardDeck.drawCard());
        }

        anyCountryCapturedThisTurn = false;
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
        setsOfRiskCardsTradedIn = 0;

        //generate deck of risk cards
        riskCardDeck = new RiskCardDeck();
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
        return true;
    }

    public void turnPhaseChanged(TurnPhase turnPhase)
    {
        TurnPhaseChanged?.Invoke(turnPhase);
        Debug.Log("GM");
    }
    public bool tradeInCards(List<RiskCard> cardsToTradeIn)
    {
        if (!isValidCardTradeIn(cardsToTradeIn))
        {
            throw new Exception("invalid trade!");
        }
        availableToDraft += calculateArmiesFromTradedCards(cardsToTradeIn);
        foreach (var card in cardsToTradeIn)
        {
            currentPlayer.removeRiskCardFromHand(card);
        }
        setsOfRiskCardsTradedIn++;
        return true;
    }

    private int calculateArmiesFromTradedCards(List<RiskCard> cardsToTradeIn)
    {
        int amount = 0;
        switch (setsOfRiskCardsTradedIn)
        {
            case <5:
                amount = 2 * setsOfRiskCardsTradedIn + 4;
                break;
            case >=5:
                amount = (setsOfRiskCardsTradedIn - 2) * 5;
                break;
        }

        bool playerOccupiesCountryOnCard = false;
        foreach (var card in cardsToTradeIn)
        {
            int cardCountryID = card.getCountryID();
            if (countries[cardCountryID].getPlayer() == currentPlayer)
            {
                amount += 2;
            }
        }
        //TODO - add check to ensure can only get bonus from occupying territory on card once per turn


        return amount;
    }

    private bool isValidCardTradeIn(List<RiskCard> cards)
    {
        if (cards.Count != 3)
        {
            return false;
        }
        
        List<RiskCardType> cardTypes = cards.Select(item => item.getRiskCardType()).ToList();
        //if all cards are the same type
        if (cardTypes.All(x=>x==cardTypes[0]))
        {
            return true;
        }
        //if all cards are different types
        if (cardTypes.Distinct().Count() == cardTypes.Count())
        {
            return true;
        }
        //if any of the cards are a wild card
        if (cardTypes.Any(x=>x==RiskCardType.Wild))
        {
            return true;
        }
        //otherwise
        return false;
    }
    
    public void countryChanged()
    {
        CountryChanged?.Invoke();
        Debug.Log("GM");
    }

    public bool draft(Player player, int countryID, int amountToDraft)
    {
        Country draftToCountry = countries[countryID];
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

    internal bool fortify(string player, Country origin, Country destination, int count)
    {
        throw new NotImplementedException();
    }
}
