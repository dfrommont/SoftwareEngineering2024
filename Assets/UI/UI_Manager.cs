using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public GameInterface gameInterface;
    public Background_Script background;
    public ClickerBackground_Script clickerBackground;
    public Menu_Script menu;
    public MainMenu_Script mainMenu;
    public TroopMovement_Script troopMovement;
    public DialogBox_Script dialogBox;
    public CardMenu_script cardMenu;
    public Overlay_Script overlay;
    public NextStage_Script nextStage;
    public CardHolder_Script cardHolder;
    public MapScript map;

    private TurnPhase turnPhase = TurnPhase.Deploy;

    private int originCountry;
    private int destinationCountry;

    // Start is called before the first frame update
    private void Start() {
        map.CountryClick += countryClicked;
        overlay.updateTurnPhaseIndicator(TurnPhase.Deploy);
        gameInterface.TurnPhaseChanged += turnPhaseChanged;
        
        troopMovement.toggle();
    }

    public void menuButtonClicked() {
        mainMenu.toggle();
        if (!clickerBackground.isOnScreen()) {
            clickerBackground.toggle();
        }
    }

    public void setBackgroundMessage(string message) {
        background.setMessage(message);
    }

    public void ClickerBackgroundClick() {
        if (mainMenu.isOnScreen())
        {
            mainMenu.toggle();
        }
        if (troopMovement.isOnScreen())
        {
            troopMovement.toggle();
        }
        if (cardMenu.isOnScreen())
        {
            cardMenu.toggle();
        }
        clickerBackground.toggle();
    }

    public void cardHolderClick() {
        cardMenu.toggle();
        if(!clickerBackground.isOnScreen())
        {
            clickerBackground.toggle();
        }
    }

    public void nextPhase() {
        gameInterface.nextPhase();
    }

    private void turnPhaseChanged(TurnPhase turnPhase) {
        this.turnPhase = turnPhase;
        overlay.updateTurnPhaseIndicator(turnPhase);
        Debug.Log("UI");
        Debug.Log(turnPhase);
    }

    private void playerTurnChanged() {
        originCountry = -1;
        destinationCountry = -1;
        overlay.updateTurnPhaseIndicator(TurnPhase.Draft);
    }

    private void countryClicked(int country) {
        Debug.Log(turnPhase);
        switch (turnPhase) {
            case TurnPhase.Deploy:
                Player player = new Player("Test");
                gameInterface.deploy(player, country);
                Debug.Log("test UI");
                break;
            case TurnPhase.Draft:
            
                break;
            case TurnPhase.Attack:

                break;
            case TurnPhase.Fortify:
                break;
        }
    }
}
