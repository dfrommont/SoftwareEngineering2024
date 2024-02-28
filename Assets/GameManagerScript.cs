using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using Random = System.Random;

public enum TurnPhase
{
    Draft,
    Attack,
    Fortify
}

public class GameManagerScript : MonoBehaviour
{
    private Dictionary<int, Country> countries;
    private Dictionary<int, Continent> continents;
    private int availableToDraft = 0;
    private int numberOfPlayers = 3;
    private Player currentPlayer;
    private List<Player> playerList = new();

    private List<string> playerNames = new()
        { "Harold", "Horace", "Henry", "Hermine", "Hetty", "Harriet" };

    private TurnPhase currentPhase;
    
    void Start()
    {
        string text = File.ReadAllText(@"./countries.json");
        JObject data = JObject.Parse(text);
        string countryTokens = data["countries"].ToString();
        countries = JsonConvert.DeserializeObject<Dictionary<int, Country>>(countryTokens);
        Debug.Log(countries.Count);
        string continentTokens = data["continents"].ToString();
        continents = JsonConvert.DeserializeObject<Dictionary<int, Continent>>(continentTokens);
        Debug.Log(continents.Count);
        //generate player names for number of players
        Random rnd = new Random();
        playerNames = playerNames.OrderBy(x => rnd.Next()).Take(numberOfPlayers).ToList();
        //populate playerList
        for (int i = 0; i < numberOfPlayers; i++)
        {
            //pick random player name from list
            string playerName = playerNames[i];
            playerList.Add(new Player(playerName));
        }
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
        foreach (var player in playerList)
        {
            player.setUnallocatedArmies(armiesToAllocate);
        }
        
        //prepare for first turn
        currentPlayer = playerList[0];
        currentPhase = TurnPhase.Draft;
    }

    int getAvailableToDraft(){
        return availableToDraft;
    }

    bool fortify(string player, Country origin, Country destination, int count){
        
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
}
