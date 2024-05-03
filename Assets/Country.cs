using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UIElements;

[System.Serializable]
public class Country
{
    public int id;
    public string display_name;
    [SerializeField] private int armiesCount = 0;

    [SerializeField] [JsonProperty] List<int> connections = new List<int>();
    [JsonIgnore]
    private List<Country> neighbours = new List<Country>();
    [JsonIgnore] private List<int> neighbour_ids {
        get {
            var list = new List<int>();
            foreach (var country in neighbours) {
                list.Add(country.id);
            }
            return list;
        }
    }
    [JsonIgnore] private Player player;
    public int x;
    public int y;

    public int getID() {
        return id;
    }
    public string getName() {
        return display_name;
    }

    public bool addArmies(int count) {
        armiesCount += count;
        return true;
    }

    public bool zeroArmies() {
        armiesCount = 0;
        return true;
    }

    public bool removeArmies(int count) {
        armiesCount -= count;
        return true;
    }

    public Player getPlayer() {
        return player;
    }

    public void setPlayer(Player newOwnerPlayer)
    {
        this.player = newOwnerPlayer;
    }
    public int getArmiesCount() {
        return armiesCount;
    }

    public bool isNeighbour(Country country) {
        // Debug.Log("isNeigbour");
        // Debug.Log(neighbours);
        // Debug.Log(neighbours.Contains(country));
        return neighbours.Contains(country) ;
    }

    public void initNeighbours(Dictionary<int, Country> countries) {
        Debug.Log(connections);
        foreach (var c_id in connections)
        {
            Debug.Log(getID());
            Debug.Log(c_id);
            neighbours.Add(countries[c_id]);
        }
        Debug.Log(getName());
        Debug.Log(connections.Count);
    }

    public List<Country> getNeighbours()
    {
        return neighbours;
    }
}
