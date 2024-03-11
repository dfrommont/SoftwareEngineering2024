using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{

    [SerializeField] private string playerName;
    [SerializeField] private string playerColour;

    public Player(string playerName)
    {
        this.playerName = playerName;
    }
    
    public string getPlayerName()
    {
        return playerName;
    }
    
    public string getPlayerColour()
    {
        return playerColour;
    }
}
