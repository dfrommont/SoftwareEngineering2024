using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class Country
{
    [SerializeField] private int id;
    [SerializeField] private string display_name;
    [SerializeField] private int armiesCount = 0;

    [SerializeField] private List<int> connections = new List<int>();
    [JsonIgnore] private List<Country> neighbours = new List<Country>();
    [JsonIgnore] private List<int> neighbour_ids {
        get {
            var list = new List<int>();
            foreach (var country in neighbours) {
                list.Add(country.id);
            }
            return list;
        }
    }
    [JsonIgnore] private string player;

    public int getID() {
        return id;
    }

    public bool addArmies(int count) {
        armiesCount += count;
        return true;
    }

    public bool removeArmies(int count) {
        armiesCount -= count;
        return true;
    }

    public string getPlayer() {
        return "";
    }

    public int getArmiesCount() {
        return armiesCount;
    }

    public bool isNeighbour(Country country) {
        return neighbours.Contains(country);
    }

    public void initNeighbours(Dictionary<int, Country> countries) {
        Debug.Log(connections);
        foreach (var c_id in connections)
        {
            Debug.Log(getID());
            Debug.Log(c_id);
            neighbours.Add(countries[c_id]);
        }
    }
}
