using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class Continent
{
    [SerializeField] private int id;
    [SerializeField] private string display_name;
    [JsonIgnore] private List<Country> countries;
    [JsonIgnore] private List<int> country_ids {
        get {
            var list = new List<int>();
            foreach (var country in countries) {
                list.Add(country.getID());
            }
            return list;
        }
    }

    public int getID(){
        return id;
    }
}
