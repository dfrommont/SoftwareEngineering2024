using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{

    [SerializeField] private string playerName;
    private int unallocatedArmies = 0;

    public Player(string playerName)
    {
        this.playerName = playerName;
    }

    public string getPlayerName()
    {
        return playerName;
    }

    public void setUnallocatedArmies(int armiesAmount)
    {
        unallocatedArmies = armiesAmount;
    }
}
