using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInterface
{
    private GameEnvironment gameEnvironment = GameEnvironment.Local;
    private GameManagerScript gameManager;

    bool fortify(string player, Country origin, Country destination, int count){
        switch (gameEnvironment)
        {
            case GameEnvironment.Local:
                return gameManager.fortify(player, origin, destination, count);
            default:
                return false;
        }
    }
}

enum GameEnvironment
{
    Local,
    Online
}