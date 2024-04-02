using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public UI_Manager manager;
    public Background_Script background;
    public ClickerBackground_Script clickerBackground;
    public Menu_Script menu;
    public MainMenu_Script mainMenu;
    public TroopMovement_Script troopMovement;
    public DialogBox_Script dialogBox;
    public CardMenu_script cardMenu;
    public Players_Script players;
    public Overlay_Script overlay;
    public NextStage_Script nextStage;
    public CardHolder_Script cardHolder;

    // Start is called before the first frame update
    private void Start()
    {
        background.setMessage("Our UI is better than Michelle's team's UI");
        overlay.highlight();
        addPlayer("player2", "harry");
        troopMovement.toggle();
    }

    public void addPlayer(string player, string name)
    {
        players.addPLayer(player, name);
    }

    public void menuButtonClicked()
    {
        mainMenu.toggle();
        if (!clickerBackground.isOnScreen())
        {
            clickerBackground.toggle();
        }
    }

    public void setBackgroundMessage(string message)
    {
        background.setMessage(message);
    }

    public void ClickerBackgroundClick()
    {
        if (mainMenu.isOnScreen())
        {
            mainMenu.toggle();
        }
        if (troopMovement.isOnScreen())
        {
            troopMovement.toggle();
        }
        if (dialogBox.isOnScreen())
        {
            dialogBox.toggle();
        }
        if (cardMenu.isOnScreen())
        {
            cardMenu.toggle();
        }
        clickerBackground.toggle();
    }

    public void cardHolderClick()
    {
        cardMenu.toggle();
        if(!clickerBackground.isOnScreen())
        {
            clickerBackground.toggle();
        }
    }
}
