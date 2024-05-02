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
    private Dictionary<int, Country> countries;
    private Dictionary<int, Continent> continents;
    // Stores the number of armies that a player has left to draft in their draft phase
    private int availableToDraft = 0;
    private int numberOfPlayers = 3;
    private Player currentPlayer;
    private Queue<Player> _originalPlayerList = new();
    private Queue<Player> playerList = new();
    private TestSubscriber _testSubscriber = new TestSubscriber();
    private List<Country> unoccupiedCountries = new();
    private RiskCardDeck riskCardDeck;
    private int setsOfRiskCardsTradedIn;
    private bool anyCountryCapturedThisTurn;
    private int currentPlayerTurnDeployCount = 0;
    private int currentPlayerTurnDeployMax = 1;
    private bool hasPlacedNeutral = false;
    private bool _hasWon = false;
    private Player neutral = new Player("Neutral", Color.gray);
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
        turnPhaseStateMachine.EndedTurn += endOfTurnActions;
        CurrentPlayerChanged += onPlayerChangeAutoTradeCardsIn;
        initCountries();

        Debug.Log(countries[1].isNeighbour(countries[36]));
        Debug.Log(countries[1].isNeighbour(countries[37]));
        Debug.Log(countries[1].isNeighbour(countries[7]));
        Debug.Log(countries[1].isNeighbour(countries[3]));
        
        // Player p1 = new Player("Bob");
        // playerList.Enqueue(p1);
        // PlayerAdded?.Invoke(p1);
        // Player p2 = new Player("Kevin");
        // playerList.Enqueue(p2);
        // PlayerAdded?.Invoke(p2);
        // Player p3 = new Player("Stuart");
        // playerList.Enqueue(p3);
        // PlayerAdded?.Invoke(p3);
    }

    public bool startGame()
    {
        _originalPlayerList = playerList;
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
            
            foreach (var country in countries)
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

    public void CompletedPhase()
    {
        Debug.Log(availableToDraft);
        // Switch to next phase
        switch (turnPhaseStateMachine.getTurnPhase())
        {
            case TurnPhase.Setup:
                nextPhase();
                unoccupiedCountries = countries.Values.ToList();
                availableToDraft = calculateArmiesToAllocate();
                break;
            case TurnPhase.Deploy:
                if (availableToDraft == 0)
                {
                    nextPhase();
                }
                nextPlayerTurn();
                break;
            case TurnPhase.Draft:
                nextPhase();
                break;
            case TurnPhase.Attack:
                nextPhase();
                break;
            case TurnPhase.Fortify:
                nextPhase();
                nextPlayerTurn();
                break;
        }
        // Setup next phase
        switch (turnPhaseStateMachine.getTurnPhase())
        {
            case TurnPhase.Draft:
                availableToDraft = calculateAvailableToDraft();
                DraftCountChanged?.Invoke(availableToDraft);
                break;
        }
        // Start next turn
        switch (turnPhaseStateMachine.getTurnPhase())
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
    
    private void endOfTurnActions()
    {
        if (anyCountryCapturedThisTurn)
        {
            currentPlayer.addRiskCardToHand(riskCardDeck.drawCard());
        }

        anyCountryCapturedThisTurn = false;
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
        return true;
    }
    
    public void turnPhaseChanged(TurnPhase turnPhase)
    {
        TurnPhaseChanged?.Invoke(turnPhase);
        //Debug.Log("GM");
    }
    public void countryChanged()
    {
        CountryChanged?.Invoke();
        //Debug.Log("GM");
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
            if (deployToCountry.getPlayer() == neutral && !hasPlacedNeutral)
            {
            }
            else if (deployToCountry.getPlayer() != currentPlayer)
            {
                throw new Exception("can't deploy to country occupied by another player!");
            }
        }

        if (deployToCountry.getPlayer() != neutral)
        {
            deployToCountry.setPlayer(currentPlayer);
            currentPlayerTurnDeployCount++;
        }
        //Debug.Log("DEPLOYCOUNTS");
        //Debug.Log(currentPlayerTurnDeployCount);
        //Debug.Log(currentPlayerTurnDeployMax);

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

        if (currentPlayerTurnDeployCount < currentPlayerTurnDeployMax || (!hasPlacedNeutral && currentPlayerTurnDeployMax==2))
        {
            //Debug.Log("F");
            return true;
        }
        
        currentPlayerTurnDeployCount = 0;
        hasPlacedNeutral = false;


        CompletedPhase();
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
        //Debug.Log("FORTIFY");
        // Check if fortify is a valid move in gamestate.
        if (turnPhaseStateMachine.getTurnPhase() != TurnPhase.Fortify)
        {
            throw new Exception("not in fortify phase");
        }
        
        // Check both countries are owned by the same player
        if(origin.getPlayer() != currentPlayer){
            // //Debug.Log("orig");
            return false;
        }
        if(destination.getPlayer() != currentPlayer){
            // //Debug.Log("dest");
            return false;
        }

        // Check if destination country is a neighbour to the origin
        if(!origin.isNeighbour(destination)){
            //Debug.Log("neighbour");
            return false;
        }

        // Check origin has count + 1 armies
        if(origin.getArmiesCount() < count){
            //Debug.Log("count");
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
    public bool createAiPlayer() {
        var pl = new Player("Ai");
        pl.setIsAIPlayer(true);
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
    
    public bool isOwnCountry(int countryID)
    {
        return countries[countryID].getPlayer() == currentPlayer;
    }

    public Country getCountry(int countryID)
    {
        return countries[countryID];
    }

    public TurnPhase GetTurnPhase()
    {
        return turnPhaseStateMachine.getTurnPhase();
    }
    void initCountries(){
        string text = File.ReadAllText(@"./countries.json");
        JObject data = JObject.Parse(text);
        string countryTokens = data["countries"].ToString();
        countries = JsonConvert.DeserializeObject<Dictionary<int, Country>>(countryTokens);
        //Debug.Log(countries.Count);

        foreach (KeyValuePair<int, Country> pair in countries)
        {
            //Debug.Log(pair.Key);
            pair.Value.initNeighbours(countries);
        }
        string continentTokens = data["continents"].ToString();
        continents = JsonConvert.DeserializeObject<Dictionary<int, Continent>>(continentTokens);
        //Debug.Log(continents.Count);
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
            anyCountryCapturedThisTurn = true;
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
        deploy(currentPlayer, deployToCountry.getID());
    }

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
            draft(currentPlayer, draftToCountry.getID(), 1);
        }
        CompletedPhase();
    }

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
            battle(attackingCountry, numDiceToRoll, defendingCountry, 1);

            //ensure loop conditions are up to date
            attacksSoFar++;
            validAttackingCountries = getValidAttackingCountries(getThisPlayerCountries());
        }
        
        CompletedPhase();
    }

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

    private void doAIFortifyPhase()
    {
        //choose a country to fortify from
        Country chosenFortifyFromCountry = getThisPlayerLeastSurroundedCountry(getValidFortifyFromCountries());
        if (chosenFortifyFromCountry !=null)
        {
            //choose an amount of troops to fortify
            int maxFortifyAmount = chosenFortifyFromCountry.getArmiesCount() - 1;
            Random rnd = new Random();
            int amountToFortify = rnd.Next(1, maxFortifyAmount);

            //choose a country to fortify to
            Country chosenFortifyToCountry = getThisPlayerMostSurroundedCountry(getThisPlayerCountries());


            //do fortify if 2 different countries have been chosen as source and dest candidates
            if (chosenFortifyFromCountry != chosenFortifyToCountry)
            {
                fortify(currentPlayer, chosenFortifyFromCountry, chosenFortifyToCountry, amountToFortify);
            }
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
        if (turnPhaseStateMachine.getTurnPhase()!=TurnPhase.Draft)
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
        
        foreach (var country in countries.ToList())
        {
            missingPlayers.Remove(country.Value.getPlayer());
        }

        // We don't want to check if players have lost if we are in the deploy phase
        if (turnPhaseStateMachine.getTurnPhase() == TurnPhase.Deploy)
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