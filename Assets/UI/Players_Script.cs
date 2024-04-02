using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Players_Script : MonoBehaviour
{
    public UI_Manager manager;
    public Players_Script players;

    public GameObject player1;
    public TMP_Text player1_Name;
    public GameObject player2;
    public TMP_Text player2_Name;
    public GameObject player3;
    public TMP_Text player3_Name;
    public GameObject player4;
    public TMP_Text player4_Name;
    public GameObject player5;
    public TMP_Text player5_Name;
    public GameObject player6;
    public TMP_Text player6_Name;

    public Dictionary<string, TMP_Text> playerRecord;

    public void Start()
    {
        playerRecord = new Dictionary<string, TMP_Text>() {
            { "player1", player1_Name },
            { "player2", player2_Name },
            { "player3", player3_Name },
            { "player4", player4_Name },
            { "player5", player5_Name },
            { "player6", player6_Name },
        };
    }

    public void addPLayer(string player, string name)
    {
        playerRecord[player].SetText(name);
    }
}
