using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Players_Script : MonoBehaviour
{
    public UI_Manager manager;
    public Players_Script players;

    public  GameObject player1;
    public  TMP_Text player1_Name;
    public  GameObject player2;
    public  TMP_Text player2_Name;
    public  GameObject player3;
    public  TMP_Text player3_Name;
    public  GameObject player4;
    public  TMP_Text player4_Name;
    public  GameObject player5;
    public  TMP_Text player5_Name;
    public  GameObject player6;
    public  TMP_Text player6_Name;

    /*public Dictionary<GameObject, TMP_Text> playerRecord = new Dictionary<GameObject, TMP_Text>() {
        { player1, player1_Name },
        { player2, player2_Name },
        { player3, player3_Name },
        { player4, player4_Name },
        { player5, player5_Name },
        { player6, player6_Name },
    };

    public void replace(GameObject target, GameObject player, TMP_Text player_Name)
    {
        playerRecord.Remove(target);
        playerRecord.Add(player, player_Name);
    }

    public void populate(GameObject player, TMP_Text player_Name) {
        if (playerRecord.Count < 6) {
            playerRecord.Add(player, player_Name);
        }
    }*/
}
