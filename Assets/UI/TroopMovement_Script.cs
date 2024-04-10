using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TroopMovement_Script : MonoBehaviour
{

    public UI_Manager manager;
    public GameObject troopMovement;
    public Button doneButton;
    public Button leftButton;
    public Button rightButton;
    public TMP_Text defendingTroopsNumber;
    public TMP_Text attackingTroopsNumber;
    private bool defaultView = false;
    private bool onScreen;
    private int defendingTroops;
    private int attackingTroops;

    // Start is called before the first frame update
    private void Start()
    {
        troopMovement.SetActive(defaultView);
        onScreen = defaultView;
        defendingTroops = 0;
        attackingTroops = 0;
        Update();
        doneButton.onClick.AddListener(done);
        leftButton.onClick.AddListener(moveRight);
        rightButton.onClick.AddListener(moveLeft);
    }

    private void done()
    {
        Console.Write("Player has moved");
        Console.Write(attackingTroops-defendingTroops);
        Console.Write("to attack");
        toggle();
    }

    private void moveRight()
    {
        defendingTroops += 1;
        attackingTroops -= 1;
    }

    private void moveLeft()
    {
        defendingTroops -= 1;
        attackingTroops += 1;
    }

    private void Update()
    {
        defendingTroopsNumber.SetText("" + defendingTroops);
        attackingTroopsNumber.SetText("" + attackingTroops);
    }

    public void toggle()
    {
        onScreen = !onScreen;
        troopMovement.SetActive(onScreen);
    }

    public bool isOnScreen()
    {
        return onScreen;
    }
}
