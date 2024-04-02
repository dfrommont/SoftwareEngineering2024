using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;


public class Continent
{
    [SerializeField] private int id;
    [SerializeField] private string display_name;
    [SerializeField] private List<int> countries = new List<int>();
    [JsonIgnore] private List<Country> country_objects = new List<Country>();
    [JsonIgnore] private List<int> country_ids {
        get {
            var list = new List<int>();
            foreach (var country in country_objects) {
                list.Add(country.getID());
            }
            return list;
        }
    }

    private int continentBonus;

    public int getID(){
        return id;
    }
    public void initCountries(Dictionary<int, Country> allCountries) {
        foreach (var id in countries)
        {
            country_objects.Add(allCountries[id]);
        }
    }

    public bool isAllOwnedByOnePlayer()
    {
        //false if there are any nulls for getPlayer(), true if there are no nulls and count of distinct players is one
        return !(country_objects.Any(x => x.getPlayer() == null)) && country_objects.Select(x => x.getPlayer()).Distinct().Count()==1;
    }

    public Player getPlayer()
    {
        if (isAllOwnedByOnePlayer())
        {
            return country_objects[0].getPlayer();
        }
        else
        {
            throw new Exception("can't get player when whole continent not owned by same player");
        }
    }

    public int getContinentBonus()
    {
        return continentBonus;
    }
}
