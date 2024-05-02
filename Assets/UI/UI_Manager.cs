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
    public DraftArmiesUIScript draftScreen;
    public AttackUIScript attackScreen;
    public FortifyUIScript fortifyScreen;
    public TroopNumbers_Script troopNumbers;
    
    private bool _clickingActive = false;

    private TurnPhase turnPhase = TurnPhase.Deploy;

    private int originCountry;
    private int destinationCountry;

    // Start is called before the first frame update
    private void Start() {
        map.CountryClick += countryClicked;
        overlay.updateTurnPhaseIndicator(TurnPhase.Deploy);
        gameInterface.TurnPhaseChanged += turnPhaseChanged;
        gameInterface.ResetEvent += ResetEventHandler;
        gameInterface.CountryChanged += updateTroopNum;
        
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
        if (gameInterface.GetTurnPhase() == TurnPhase.Draft || gameInterface.GetTurnPhase() == TurnPhase.Attack || gameInterface.GetTurnPhase() == TurnPhase.Fortify)
        {
            gameInterface.nextPhase();
        }
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
        if (_clickingActive)
        {
            switch (turnPhase)
            {
                case TurnPhase.Deploy:
                    Player player = new Player("Test");
                    _clickingActive = false;
                    gameInterface.deploy(player, country);
                    Debug.Log("test UI");
                    
                    break;
                case TurnPhase.Draft:
                    
                    if (gameInterface.isOwnCountry(country))
                    {
                        _clickingActive = false;
                        draftScreen.Show(country);
                    }
                    break;
                case TurnPhase.Attack:
                    if (originCountry == -1)
                    {
                        if (gameInterface.isOwnCountry(country))
                        {
                            originCountry = country;
                        }
                    }
                    else
                    {
                        if (!gameInterface.isOwnCountry(country))
                        {
                            destinationCountry = country;
                            _clickingActive = false;
                            Debug.Log(originCountry);
                            Debug.Log(destinationCountry);
                            attackScreen.Show(originCountry, destinationCountry);
                        }
                    }

                    break;
                case TurnPhase.Fortify:
                    if (originCountry == -1)
                    {
                        if (gameInterface.isOwnCountry(country))
                        {
                            originCountry = country;
                        }
                    }
                    else
                    {
                        if (gameInterface.isOwnCountry(country))
                        {
                            destinationCountry = country;
                            _clickingActive = false;
                            fortifyScreen.Show(originCountry, destinationCountry);
                        }
                    }
                    break;
            }
        }
    }

    public void popup(string mode, string message, int time = 5)
    {
        switch(mode)
        {
            case "long":
                dialogBox.longMessage(message);
                break;
            default:
                dialogBox.shortMessage(message, time); 
                break;

        }
    }

    void ResetEventHandler(int a)
    {
        _clickingActive = true;
        originCountry = -1;
        destinationCountry = -1;
    }
    
    public void updateTroopNum()
    {
        List<Country> countriesList = gameInterface.getCountries();
        foreach (Country country in countriesList)
        {
            troopNumbers.changeNumber(country.getName(), country.getArmiesCount());
        }
    }
}
