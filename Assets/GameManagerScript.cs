using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

public class GameManagerScript : MonoBehaviour
{
    private Dictionary<int, Country> countries;
    private Dictionary<int, Continent> continents;
    private int availableToDraft = 0;
    
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
        destination.addArmies(count);`
        return true;
    }
}
