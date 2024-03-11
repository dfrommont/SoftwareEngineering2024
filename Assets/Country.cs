using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class Country
{
    [SerializeField] private int id;
    [SerializeField] private string display_name;
    [SerializeField] private int armiesCount = 0;

    [SerializeField] private List<int> connections;
    [JsonIgnore] private List<Country> neighbours;
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
}
