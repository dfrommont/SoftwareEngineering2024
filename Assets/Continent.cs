using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


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

    public int getID(){
        return id;
    }
    public void initCountries(Dictionary<int, Country> allCountries) {
        foreach (var id in countries)
        {
            country_objects.Add(allCountries[id]);
        }
    }
}
