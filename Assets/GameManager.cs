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
using Unity.VisualScripting;


public class GameManager : MonoBehaviour
{
    private Dictionary<int, Country> _countries;
    private Dictionary<int, Continent> _continents;
    
    // Stores the number of armies that a player has left to draft in their draft phase
    private int availableToDraft = 0;
    
    private int numberOfPlayers = 3;
    
    // Currently active player
    private Player currentPlayer;
    
    // Queue of players - Used for the turn system
    private Queue<Player> playerList = new();
    
    // List of unoccupied countries - Used for the initial setup phase
    private List<Country> unoccupiedCountries = new();
    
    // Card deck
    private RiskCardDeck riskCardDeck;
    
    private int setsOfRiskCardsTradedIn;
    private bool anyCountryCapturedThisTurn;
    
    // Used for 2 player risk - Where players can place 2 armies at a time, and 1 neutral army
    private int currentPlayerTurnDeployCount = 0;
    private int currentPlayerTurnDeployMax = 1;
    private bool hasPlacedNeutral = false;
    
    private bool _hasWon = false;
    
    // Neutral player - Acts as a standard player
    private Player neutral = new Player("Neutral", Color.gray);

    private List<Color> _playerColours = new();
    public event Action<TurnPhase> TurnPhaseChanged;
    public event Action<Player> CurrentPlayerChanged;
    public event Action<Player> PlayerAdded;
    public event Action<int> ResetEvent;
    public event Action<int> DraftCountChanged;
    public event Action CountryChanged;

    private TurnPhaseManager _turnPhaseStateMachine = new();

    /// <summary>
    /// Game Manager Start function
    /// - initialises countries
    /// - Attaches event listeners
    /// </summary>
    void Start()
    {
        _turnPhaseStateMachine.PhaseChanged += turnPhaseChanged;
        _turnPhaseStateMachine.EndedTurn += endOfTurnActions;
        CurrentPlayerChanged += onPlayerChangeAutoTradeCardsIn;
        initCountries();
        
        _playerColours.Add(new Color((255 / 255f), (195 / 255f), (10 / 255f)));
        _playerColours.Add(new Color((12 / 255f), (123 / 255f), (220 / 255f)));
        _playerColours.Add(new Color((106 / 255f), (183 / 255f), (106 / 255f)));
        _playerColours.Add(new Color((220 / 255f), (50 / 255f), (32 / 255f)));
        _playerColours.Add(new Color((211 / 255f), (95 / 255f), (183 / 255f)));
        _playerColours.Add(new Color((64 / 255f), (30 / 255f), (4 / 255f)));
    }

    /// <summary>
    /// Starts the game after all players have been added
    /// - Called by UI
    /// - Validates player count
    /// - Initialises cards
    /// - Sets up 2 player risk
    /// </summary>
    /// <returns>true if passes validation</returns>
    public bool startGame()
    {
        numberOfPlayers = playerList.Count;
        // Validate game is in a valid state
        if (playerList.Count < 2)
        {
            //Debug.Log("Not enough players");
            return false;
        }
        else if (playerList.Count > 6)
        {
            //Debug.Log("Too many players");
            return false;
        }

        CompletedPhase();
        
        setsOfRiskCardsTradedIn = 0;

        //generate deck of risk cards
        riskCardDeck = new RiskCardDeck();
        //prepare for first turn
        //Debug.Log("COMPLETE");

        if (playerList.Count == 2)
        {
            // 2 player auto setup

            int player1Count = 0;
            int player2Count = 0;
            int neutralCount = 0;
            List<Player> plist = playerList.ToList();
            Random rnd = new Random();

            currentPlayerTurnDeployMax = 2;
            
            foreach (var country in _countries)
            {
                bool found = false;
                Country c = country.Value;
                do
                {
                    int player = rnd.Next(1, 4);
                    if (player == 1)
                    {
                        if (player1Count < 14)
                        {
                            c.setPlayer(plist[0]);
                            c.addArmies(1);
                            player1Count++;
                            found = true;
                        }
                    } else if (player == 2)
                    {
                        if (player2Count < 14)
                        {
                            c.setPlayer(plist[1]);
                            c.addArmies(1);
                            player2Count++;
                            found = true;
                        }
                    } else if (player == 3)
                    {
                        if (neutralCount < 14)
                        {
                            
                            c.setPlayer(neutral);
                            c.addArmies(1);
                            neutralCount++;
                            found = true;
                        }
                    }
                    //Debug.Log(found);
                    ;
                } while (!found);
                
            }
            CountryChanged?.Invoke();
            unoccupiedCountries.Clear();
        }
        return true;
    }

    /// <summary>
    /// Moves from one phase to another, whether that be from setup to deploy, deploy to draft, or fortify to draft etc.
    /// Sets up variables that are needed for the next phase, such as available to draft.
    /// </summary>
    public void CompletedPhase()
    {
        Debug.Log(availableToDraft);
        // Switch to next phase
        switch (_turnPhaseStateMachine.getTurnPhase())
        {
            case TurnPhase.Setup:
                // Move from setup to Deploy
                
                nextPhase();
                unoccupiedCountries = _countries.Values.ToList();
                availableToDraft = calculateArmiesToAllocate();
                break;
            case TurnPhase.Deploy:
                // Move from Deploy to Draft if there are no more available to draft.
                if (availableToDraft == 0)
                {
                    nextPhase();
                }
                // Move to next player
                nextPlayerTurn();
                break;
            case TurnPhase.Draft:
                // Move from Draft to Attack
                nextPhase();
                break;
            case TurnPhase.Attack:
                // Move from Attack to Fortify
                nextPhase();
                break;
            case TurnPhase.Fortify:
                // Move from Fortify to Draft, and move to next player
                nextPhase();
                nextPlayerTurn();
                break;
        }
        // Setup next phase
        switch (_turnPhaseStateMachine.getTurnPhase())
        {
            case TurnPhase.Draft:
                availableToDraft = calculateAvailableToDraft();
                DraftCountChanged?.Invoke(availableToDraft);
                break;
        }
        // Start next turn, and run Ai code
        switch (_turnPhaseStateMachine.getTurnPhase())
        {
            case TurnPhase.Deploy:
                if (currentPlayer.getIsAIPlayer())
                {
                    doAiDeploy();
                }
                break;
            case TurnPhase.Draft:
                if (currentPlayer.getIsAIPlayer())
                {
                    doAIDraftPhase();
                }
                break;
            case TurnPhase.Attack:
                if (currentPlayer.getIsAIPlayer())
                {
                    doAIAttackPhase();
                }
                break;
            case TurnPhase.Fortify:
                if (currentPlayer.getIsAIPlayer())
                {
                    doAIFortifyPhase();
                }
                break;
        }
    }
    
    /// <summary>
    /// If any country is captured in a round, then a risk card is given to the current player
    /// </summary>
    private void endOfTurnActions()
    {
        if (anyCountryCapturedThisTurn)
        {
            currentPlayer.addRiskCardToHand(riskCardDeck.drawCard());
        }

        anyCountryCapturedThisTurn = false;
    }
    
    /// <summary>
    /// Trade in a collection of cards in return for more troops
    /// </summary>
    /// <param name="cardsToTradeIn">A list of cards to be traded</param>
    /// <returns>true if successful</returns>
    /// <exception cref="Exception">If trade in is not a valid collection</exception>
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

    /// <summary>
    /// Calculate how many armies should be returned from a given set of cards being traded in
    /// </summary>
    /// <param name="cardsToTradeIn">The list of cards to be traded in</param>
    /// <returns>The number of armies to be given in return for cards</returns>
    private int calculateArmiesFromTradedCards(List<RiskCard> cardsToTradeIn)
    {
        int amount = 0;
        switch (setsOfRiskCardsTradedIn)
        {
            case < 5:
                amount = 2 * setsOfRiskCardsTradedIn + 4;
                break;
            case >= 5:
                amount = (setsOfRiskCardsTradedIn - 2) * 5;
                break;
        }

        bool playerOccupiesCountryOnCard = false;
        foreach (var card in cardsToTradeIn)
        {
            int cardCountryID = card.getCountryID();
            if (_countries[cardCountryID].getPlayer() == currentPlayer)
            {
                amount += 2;
            }
        }
        //TODO - add check to ensure can only get bonus from occupying territory on card once per turn


        return amount;
    }

    /// <summary>
    /// Checks if the combination of cards to be traded is valid
    /// </summary>
    /// <param name="cards">List of cards to be traded</param>
    /// <returns>true if valid</returns>
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
    
    /// <summary>
    /// Get a list of all territories
    /// </summary>
    /// <returns>List of all territories</returns>
    public List<Country> getCountries(){
        List<Country> return_countries = new List<Country>();
        foreach(KeyValuePair<int, Country> country in _countries)
        {
            return_countries.Add(country.Value);
        }
        return return_countries;
    }
    
    /// <summary>
    /// Moves to the next phase of play e.g. Draft -> Attack etc.
    /// </summary>
    /// <returns>True if successful</returns>
    public bool nextPhase(){
        _turnPhaseStateMachine.nextTurnPhase();
        ResetEvent?.Invoke(0);
        return true;
    }
    
    /// <summary>
    /// Event forwarding for turnPhaseChanged from turnPhaseManager
    /// - Turn Phase Changed is invoked every time that the turn phase changes
    /// </summary>
    /// <param name="turnPhase">The new turn phase</param>
    public void turnPhaseChanged(TurnPhase turnPhase)
    {
        TurnPhaseChanged?.Invoke(turnPhase);
        //Debug.Log("GM");
    }
    
    /// <summary>
    /// Invokes the CountryChanged event
    /// - Used in UI for re-rendering the owner of a territory
    /// </summary>
    private void countryChanged()
    {
        CountryChanged?.Invoke();
        //Debug.Log("GM");
    }

    /// <summary>
    /// The Draft game mechanic of risk
    /// </summary>
    /// <param name="player">unused (For future networking)</param>
    /// <param name="countryID">The countryID to draft armies to</param>
    /// <param name="amountToDraft">The number of armies to draft</param>
    /// <returns>true if successful</returns>
    /// <exception cref="Exception"></exception>
    public bool Draft(Player player, int countryID, int amountToDraft)
    {
        ResetEvent?.Invoke(0);
        Country draftToCountry = _countries[countryID];
        if (_turnPhaseStateMachine.getTurnPhase()!=TurnPhase.Draft)
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
        DraftCountChanged?.Invoke(availableToDraft);
        countryChanged();
        return true;
    }
    /// <summary>
    /// Calculates the number of armies in which a player can draft
    /// </summary>
    /// <returns>Number of armies that can be drafted</returns>
    /// <exception cref="Exception"></exception>
    private int calculateAvailableToDraft()
    {
        if (_turnPhaseStateMachine.getTurnPhase()!=TurnPhase.Draft)
        {
            throw new Exception("not in draft phase!");
        }
        int amountAvailableToDraft = 0;
        //calculate territory bonus
        int territoriesOwnedByCurrentPlayer = 0;
        foreach (var country in _countries.Values)
        {
            if (country.getPlayer()==currentPlayer)
            {
                territoriesOwnedByCurrentPlayer++;
            }
        }
        amountAvailableToDraft += territoriesOwnedByCurrentPlayer / 3;
        //calculate continent bonus
        foreach (var continent in _continents.Values)
        {
            if (continent.isAllOwnedByOnePlayer() && continent.getPlayer() == currentPlayer)
            {
                availableToDraft += continent.getContinentBonus();
            }
        }
        
        //ensure min of 3
        return Math.Max(3, amountAvailableToDraft);

    }
    
    /// <summary>
    /// The deploy phase of the game, where players initially place their armies
    /// </summary>
    /// <param name="player">unused (For future networking)</param>
    /// <param name="countryID">The countryID to deploy troops to</param>
    /// <returns>True if successful</returns>
    /// <exception cref="Exception"></exception>
    public bool Deploy(Player player, int countryID)
    {
        ResetEvent?.Invoke(0);
        Country deployToCountry = _countries[countryID];
        if (_turnPhaseStateMachine.getTurnPhase() != TurnPhase.Deploy)
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
            if (deployToCountry.getPlayer() == neutral && !hasPlacedNeutral)
            {
            }
            else if (deployToCountry.getPlayer() != currentPlayer)
            {
                throw new Exception("can't deploy to country occupied by another player!");
            }
        }

        // Sets the territory to the current player, unless it's neutral.
        if (deployToCountry.getPlayer() != neutral)
        {
            deployToCountry.setPlayer(currentPlayer);
            currentPlayerTurnDeployCount++;
        }

        // Increments the number of armies on a territory, unless they have reached their max number of deployments
        // Max number of deployments is 1 for 3-6 players, 2 + 1 neutral army for 2 player mode.
        if (currentPlayerTurnDeployCount <= currentPlayerTurnDeployMax || (deployToCountry.getPlayer() == neutral && !hasPlacedNeutral))
        {
            deployToCountry.addArmies(1);
            availableToDraft--;
            DraftCountChanged?.Invoke(availableToDraft);
            countryChanged();
        }
        if (deployToCountry.getPlayer() == neutral && !hasPlacedNeutral)
        {
            hasPlacedNeutral = true;
        }

        // Prevents moving to the next stage unless all deplyments needed are made.
        if (currentPlayerTurnDeployCount < currentPlayerTurnDeployMax || (!hasPlacedNeutral && currentPlayerTurnDeployMax==2))
        {
            return true;
        }
        
        // Reset validation variables
        currentPlayerTurnDeployCount = 0;
        hasPlacedNeutral = false;


        CompletedPhase();
        return true;
    }
    
    /// <summary>
    /// Calculates the number of armies that need to be placed in the initial setup.
    /// Returns number of players * number of armies to distribute
    /// This simplifies other aspects, and is ok to do as every player always has the same number of armies to begin with
    /// </summary>
    /// <returns>The number of armies to distribute</returns>
    /// <exception cref="Exception"></exception>
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

    /// <summary>
    /// The Fortify Game Mechanic in risk
    /// </summary>
    /// <param name="player">unused (For future networking)</param>
    /// <param name="origin">The origin country to move troops from</param>
    /// <param name="destination">The destination country to move troops to</param>
    /// <param name="count">The number of troops to move</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public bool Fortify(Player player, Country origin, Country destination, int count){
        // Check if fortify is a valid move in gamestate.
        if (_turnPhaseStateMachine.getTurnPhase() != TurnPhase.Fortify)
        {
            throw new Exception("not in fortify phase");
        }
        
        // Check both countries are owned by the same player
        if(origin.getPlayer() != currentPlayer){
            return false;
        }
        if(destination.getPlayer() != currentPlayer){
            return false;
        }

        // Check if destination country is a neighbour to the origin
        if(!origin.isNeighbour(destination)){
            return false;
        }

        // Check origin has count + 1 armies
        if(origin.getArmiesCount() < count){
            return false;
        }

        // Passed all checks, so continue with movement
        origin.removeArmies(count);
        destination.addArmies(count);
        countryChanged();
        ResetEvent?.Invoke(0);
        
        return true;
    }
    
    /// <summary>
    /// Creates a new player
    /// </summary>
    /// <param name="name">The name of the player</param>
    /// <returns>true if successful</returns>
    public bool createPlayer(string name) {
        var pl = new Player(name, _playerColours[playerList.Count]);
        playerList.Enqueue(pl);
        PlayerAdded?.Invoke(pl);
        return true;
    }
    
    /// <summary>
    /// Creates a new Ai player
    /// </summary>
    /// <returns>true if successful</returns>
    public bool createAiPlayer() {
        var pl = new Player("Ai", _playerColours[playerList.Count]);
        pl.setIsAIPlayer(true);
        playerList.Enqueue(pl);
        PlayerAdded?.Invoke(pl);
        return true;
    }

    /// <summary>
    /// Cycles through players to the first player in the game as per the dice rolled
    /// </summary>
    /// <param name="playerIndex">The index of the first player in the game</param>
    /// <returns></returns>
    public bool firstPlayer(int playerIndex){
        for (int i = 0; i < playerIndex+1; i++)
        {
            currentPlayer = playerList.Dequeue();
            playerList.Enqueue(currentPlayer);
        }
        CurrentPlayerChanged?.Invoke(currentPlayer);
        return true;
    }

    /// <summary>
    /// Rotates to the next player in the queue
    /// </summary>
    private void nextPlayerTurn()
    {
        checkHasWonOrLost();
        currentPlayer = playerList.Dequeue();
        playerList.Enqueue(currentPlayer);
        
        if (_hasWon)
        {
            //Debug.Log("GAMEWON");
            return;
        }
        if (currentPlayer.IsDead())
        {
            nextPlayerTurn();
            return;
        }
        CurrentPlayerChanged?.Invoke(currentPlayer);
    }
    
    /// <summary>
    /// Checks if the countryID is owned by the current player
    /// </summary>
    /// <param name="countryID">The country ID of country to test</param>
    /// <returns>true if owned by the current player</returns>
    public bool isOwnCountry(int countryID)
    {
        return _countries[countryID].getPlayer() == currentPlayer;
    }

    /// <summary>
    /// Gets a Country object from a countryID
    /// </summary>
    /// <param name="countryID">the countryID of the country to get</param>
    /// <returns>The Country object</returns>
    public Country getCountry(int countryID)
    {
        return _countries[countryID];
    }

    /// <summary>
    /// Gets the current turn phase
    /// </summary>
    /// <returns>The current TurnPhase</returns>
    public TurnPhase GetTurnPhase()
    {
        return _turnPhaseStateMachine.getTurnPhase();
    }
    
    /// <summary>
    /// Initialises the countries from the JSON file
    /// </summary>
    void initCountries(){
        string text = File.ReadAllText(@"./countries.json");
        JObject data = JObject.Parse(text);
        string countryTokens = data["countries"].ToString();
        _countries = JsonConvert.DeserializeObject<Dictionary<int, Country>>(countryTokens);
        //Debug.Log(countries.Count);

        foreach (KeyValuePair<int, Country> pair in _countries)
        {
            //Debug.Log(pair.Key);
            pair.Value.initNeighbours(_countries);
        }
        string continentTokens = data["continents"].ToString();
        _continents = JsonConvert.DeserializeObject<Dictionary<int, Continent>>(continentTokens);
        //Debug.Log(continents.Count);
        foreach (KeyValuePair<int, Continent> pair in _continents)
        {
            pair.Value.initCountries(_countries);
        }
    }

    /// <summary>
    /// The Battle game mechanic in Risk
    /// </summary>
    /// <param name="attacker">The country of the attacker</param>
    /// <param name="attackRollCount">the number of dice to roll for the attacker</param>
    /// <param name="defender">The country of the defender</param>
    /// <param name="defendRollCount">the number of dice to roll for the defender</param>
    /// <returns>true if successful</returns>
    public bool Battle(Country attacker, int attackRollCount, Country defender, int defendRollCount) {

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

    /// <summary>
    /// Ai Deploy functionality
    /// </summary>
    public void doAiDeploy()
    {
        Country deployToCountry;
        if (unoccupiedCountries
            .Any()) //if unoccupiedCountries has elements - i.e. still deploying to unoccupied countries
        {
            //choose an unoccupied country
            Random rnd = new Random();
            deployToCountry = unoccupiedCountries[rnd.Next(unoccupiedCountries.Count)];
        }
        else
        {
            //choose a country occupied by the current player
            List<Country> thisPlayerCountries = getThisPlayerCountries();

            Random rnd = new Random();
            deployToCountry = thisPlayerCountries[rnd.Next(thisPlayerCountries.Count)];
        }

        //deploy to the selected country
        Deploy(currentPlayer, deployToCountry.getID());
    }

    /// <summary>
    /// Gets a list of countries owned by the current player
    /// </summary>
    /// <returns>List of Countries</returns>
    private List<Country> getThisPlayerCountries()
    {
        //find this player's countries
        List<Country> allCountries = getCountries();
        List<Country> thisPlayerCountries = new List<Country>();
        foreach (var country in allCountries)
        {
            if (country.getPlayer() == currentPlayer)
            {
                thisPlayerCountries.Add(country);
            }
        }

        return thisPlayerCountries;
    }
    
    /// <summary>
    /// Gets the number of enemy neighbours for a given country
    /// </summary>
    /// <param name="countries">List of countries to get numbers for</param>
    /// <returns>A Dictionary for looking up number of enemy neighbours</returns>
    private Dictionary<Country, int> getCountriesAndEnemyNeighbours(List<Country> countries)
    {
        Dictionary<Country, int> countriesAndEnemyNeighbours = new Dictionary<Country, int>();

        foreach (var country in countries)
        {
            int numberOfEnemyNeighbours = getEnemyNeighboursCount(country);
            countriesAndEnemyNeighbours.Add(country, numberOfEnemyNeighbours);
        }

        return countriesAndEnemyNeighbours;
    }

    /// <summary>
    /// Get the number of enemy neighbours for a given country
    /// </summary>
    /// <param name="country">The country to get neighbours for</param>
    /// <returns>number of enemy countries</returns>
    private int getEnemyNeighboursCount(Country country)
    {
        int numberOfEnemyNeighbours = 0;

        foreach (var neighbour in country.getNeighbours())
        {
            if (neighbour.getPlayer() != currentPlayer)
            {
                numberOfEnemyNeighbours++;
            }
        }

        return numberOfEnemyNeighbours;
    }
    
    /// <summary>
    /// Ai Draft functionality
    /// </summary>
    private void doAIDraftPhase()
    {
        //Debug.Log("AIDRAFT");
        //get this players countries and how many enemy neighbours they have
        Dictionary<Country, int> thisPlayerCountriesAndEnemyNeighbours =
            getCountriesAndEnemyNeighbours(getThisPlayerCountries());
        //take top 10 (or up to 10) most surrounded countries
        List<Country> candidateDraftCountries = thisPlayerCountriesAndEnemyNeighbours.OrderByDescending(x => x.Value)
            .Select(x => x.Key)
            .Take(10)
            .ToList();
        //Debug.Log("AIDRAFT");
        //Debug.Log(availableToDraft);
        while (availableToDraft > 0)
        {
            //Debug.Log("AIDRAFT");
            //Debug.Log(availableToDraft);
            //get a dict of the candidate draft countries and their troop strengths
            Dictionary<Country, int> candidateDraftCountriesAndTroopStrength = new Dictionary<Country, int>();
            foreach (var country in candidateDraftCountries)
            {
                candidateDraftCountriesAndTroopStrength.Add(country, country.getArmiesCount());
            }

            //choose a country to draft to 
            Country draftToCountry = candidateDraftCountriesAndTroopStrength.OrderBy(kvp => kvp.Value).First().Key;
            Draft(currentPlayer, draftToCountry.getID(), 1);
        }
        CompletedPhase();
    }

    /// <summary>
    /// Ai Attack functionality
    /// </summary>
    private void doAIAttackPhase()
    {
        int maxAttacks = 20;
        int attacksSoFar = 0;
        List<Country> validAttackingCountries = getValidAttackingCountries(getThisPlayerCountries());
        //loop until no valid attacking countries, up to a maximum number of attacks
        while (validAttackingCountries.Count > 0 && attacksSoFar < maxAttacks)
        {
            //choose an attacking country
            Country attackingCountry = getCountryWithMostArmies(validAttackingCountries);
            //choose a country to attack
            Country defendingCountry = getWeakestEnemyNeighbour(attackingCountry);
            //decide how many dice to roll in the attack - this AI always attacks with full strength
            int numDiceToRoll = attackingCountry.getArmiesCount() - 1;
            //make the attack
            Battle(attackingCountry, numDiceToRoll, defendingCountry, 1);

            //ensure loop conditions are up to date
            attacksSoFar++;
            validAttackingCountries = getValidAttackingCountries(getThisPlayerCountries());
        }
        
        CompletedPhase();
    }

    /// <summary>
    /// Gets the weakest enemy neighbour of a country
    /// </summary>
    /// <param name="attackingCountry">The attacking country</param>
    /// <returns>the weakest country</returns>
    private Country getWeakestEnemyNeighbour(Country attackingCountry)
    {
        //get a list of enemy neighbours of the attacking country
        List<Country> enemyNeighbours = getEnemyNeighboursList(attackingCountry);
        //get a dict of the enemy neighbours with their army strengths
        Dictionary<Country,int> enemyNeighboursAndArmiesCount = getCountriesAndArmiesCount(enemyNeighbours);
        //return the enemy neighbour with the weakest army
        return enemyNeighboursAndArmiesCount.OrderBy(kvp => kvp.Value).FirstOrDefault().Key;
    }

    private List<Country> getEnemyNeighboursList(Country attackingCountry)
    {
        List<Country> enemyNeighbours = new List<Country>();
        foreach (var neighbour in attackingCountry.getNeighbours())
        {
            if (neighbour.getPlayer() != currentPlayer)
            {
                enemyNeighbours.Add(neighbour);
            }
        }

        return enemyNeighbours;
    }

    private Country getCountryWithMostArmies(List<Country> countries)
    {
        //gets the country with the greatest army count
        Dictionary<Country,int> attackingCountriesAndArmiesCount = getCountriesAndArmiesCount(countries);
        return attackingCountriesAndArmiesCount.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
    }

    private List<Country> getValidAttackingCountries(List<Country> countries)
    {
        Dictionary<Country, int> thisPlayerCountriesAndArmiesCount =
            getCountriesAndArmiesCount(getThisPlayerCountries());
        List<Country> validAttackingCountries = new List<Country>();
        //add to valid attackers all this player's countries that have more than one armies
        foreach (var kvp in thisPlayerCountriesAndArmiesCount)
        {
            if (kvp.Value>1)
            {
                validAttackingCountries.Add(kvp.Key);
            }
        }
        //remove from valid attackers where there are no enemy neighbours to attack
        var countriesToRemove = new List<Country>();

        foreach (var country in validAttackingCountries.ToList())
        {
            if (getEnemyNeighboursCount(country) < 1)
            {
                countriesToRemove.Add(country);
            }
        }

        foreach (var countryToRemove in countriesToRemove)
        {
            validAttackingCountries.Remove(countryToRemove);
        }
        return validAttackingCountries;
    }

    /// <summary>
    /// Ai Fortify functionality
    /// </summary>
    private void doAIFortifyPhase()
    {
        //choose a country to fortify from
        Country chosenFortifyFromCountry = getThisPlayerLeastSurroundedCountry(getValidFortifyFromCountries());
        //choose an amount of troops to fortify
        int maxFortifyAmount = chosenFortifyFromCountry.getArmiesCount() - 1;
        Random rnd = new Random();
        int amountToFortify = rnd.Next(1, maxFortifyAmount);

        //choose a country to fortify to
        Country chosenFortifyToCountry = getThisPlayerMostSurroundedCountry(getThisPlayerCountries());


        //do fortify if 2 different countries have been chosen as source and dest candidates
        if (chosenFortifyFromCountry != chosenFortifyToCountry)
        {
            Fortify(currentPlayer, chosenFortifyFromCountry, chosenFortifyToCountry, amountToFortify);
        }
        CompletedPhase();
    }

    private List<Country> getValidFortifyFromCountries()
    {
        Dictionary<Country, int> thisPlayerCountriesAndArmiesCount =
            getCountriesAndArmiesCount(getThisPlayerCountries());
        List<Country> validFortifyFromCountries = new List<Country>();
        foreach (var kvp in thisPlayerCountriesAndArmiesCount)
        {
            if (kvp.Value > 1)
            {
                validFortifyFromCountries.Add(kvp.Key);
            }
        }

        return validFortifyFromCountries;
    }

    private Country getThisPlayerLeastSurroundedCountry(List<Country> countries)
    {
        //important note - this method needs to be passed an appropriate list of this player's countries
        Dictionary<Country, int> thisPlayerCountriesAndEnemyNeighbours =
            getCountriesAndEnemyNeighbours(countries);
        //return the country with the lowest number of enemy neighbours
        return thisPlayerCountriesAndEnemyNeighbours.OrderBy(kvp => kvp.Value).FirstOrDefault().Key;
    }

    private Country getThisPlayerMostSurroundedCountry(List<Country> countries)
    {
        //important note - this method needs to be passed an appropriate list of this player's countries
        Dictionary<Country, int> thisPlayerCountriesAndEnemyNeighbours =
            getCountriesAndEnemyNeighbours(countries);
        //return the country with the highest number of enemy neighbours
        return thisPlayerCountriesAndEnemyNeighbours.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
    }

    private Dictionary<Country, int> getCountriesAndArmiesCount(List<Country> countries)
    {
        Dictionary<Country, int> countriesAndArmiesCount = new Dictionary<Country, int>();
        foreach (var country in countries)
        {
            countriesAndArmiesCount.Add(country, country.getArmiesCount());
        }

        return countriesAndArmiesCount;
    }
    public int getDefenceDiceToRoll(Country defendingCountry)
    {
        return Math.Min(2, defendingCountry.getArmiesCount());
    }
    private void onPlayerChangeAutoTradeCardsIn(Player player)
    {
        if (_turnPhaseStateMachine.getTurnPhase()!=TurnPhase.Draft)
        {
            //Debug.Log("can't trade in cards, not in draft phase!");
        }

        List<RiskCard> currentPlayerHand = currentPlayer.getPlayerHand();
        //find a valid trade and make it
        if (currentPlayerHand.Count >2)
        {
            //get the first unique set of 3 cards that is a valid set to trade in
            List<RiskCard> cardsToTradeIn = GetUniqueCombinations(currentPlayerHand).FirstOrDefault(item => isValidCardTradeIn(item));
            if (cardsToTradeIn != null)
            {
                tradeInCards(cardsToTradeIn);
            }
            else
            {
                Console.Out.WriteLine("No valid card sets to trade in this turn.");
            }

        }
        else
        {
            Console.Out.WriteLine("player hand too small to trade in cards");
        }
    }

    private void checkHasWonOrLost()
    {
        List<Player> missingPlayers = playerList.ToList();
        
        foreach (var country in _countries.ToList())
        {
            missingPlayers.Remove(country.Value.getPlayer());
        }

        // We don't want to check if players have lost if we are in the deploy phase
        if (_turnPhaseStateMachine.getTurnPhase() == TurnPhase.Deploy)
        {
            return;
        }
        foreach (var player in missingPlayers)
        {
            player.setDead();
        }
        
        if (missingPlayers.Count == playerList.Count - 1)
        {
            _hasWon = true;
        }
    }

    private List<List<RiskCard>> GetUniqueCombinations(List<RiskCard> playerCards)
    {
        List<List<RiskCard>> combinations = new List<List<RiskCard>>();

        // Generate all unique combinations of three cards
        for (int i = 0; i < playerCards.Count - 2; i++)
        {
            for (int j = i + 1; j < playerCards.Count - 1; j++)
            {
                for (int k = j + 1; k < playerCards.Count; k++)
                {
                    List<RiskCard> combination = new List<RiskCard>
                    {
                        playerCards[i],
                        playerCards[j],
                        playerCards[k]
                    };

                    combinations.Add(combination);
                }
            }
        }

        return combinations;
    }
    

}