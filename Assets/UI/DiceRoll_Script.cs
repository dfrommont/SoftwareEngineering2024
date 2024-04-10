using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoll_Script : MonoBehaviour
{

    public UI_Manager manager;
    public GameObject diceRoll;
    public Image dice1;
    public Image dice2;
    public Image dice3;
    public Image dice4;
    public Image dice5;
    public Image dice6;
    private Dictionary<int, Image> images;
    private bool defaultView = false;
    private bool onScreen;
    private int currentDice = 0;

    // Start is called before the first frame update
    void Start()
    {
        images = new Dictionary<int, Image>
        {
            { 1, dice1 },
            { 2, dice2 },
            { 3, dice3 },
            { 4, dice4 },
            { 5, dice5 },
            { 6, dice6 },
        };
        dice1.enabled = false;
        dice2.enabled = false;
        dice3.enabled = false;
        dice4.enabled = false;
        dice5.enabled = false;
        dice6.enabled = false;
        onScreen = defaultView;
    }

    private void changeDice()
    {
        Console.WriteLine("Dice roll loop running");
        images[currentDice].enabled = false;
        currentDice = (currentDice + 1) % 6;
        images[currentDice].enabled = true;

    }

    public void roll()
    {
        InvokeRepeating(nameof(changeDice), 0f, 1f);
    }

    public int stopRoll()
    {
        CancelInvoke(nameof(changeDice));
        return currentDice;
    }

    public void toggle()
    {
        diceRoll.SetActive(onScreen);
        onScreen = !onScreen;
    }
}
