using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{

    /**
     * Importing all the scripts that control the componenets of the UI
     */

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
        /**
         * Adding the handlers for all of the events that can be passed from the GameManager
         */
        map.CountryClick += countryClicked;
        overlay.updateTurnPhaseIndicator(TurnPhase.Deploy);
        gameInterface.TurnPhaseChanged += turnPhaseChanged;
        gameInterface.ResetEvent += ResetEventHandler;
        gameInterface.CountryChanged += updateTroopNum;
        gameInterface.CountriesInitialised += countriesInitialised;
        
        troopMovement.toggle();
    }

    public void countriesInitialised()
    {
        troopNumbers.load(); //Call the load() method to position the UI troop numbers once the countries have been initialised
    }

    public void menuButtonClicked() {
        mainMenu.toggle(); //open the main menu
        if (!clickerBackground.isOnScreen()) {
            clickerBackground.toggle(); //If the clicker background (a hidden background used to click off the menu) isn't on screen, load it on screen
        }
    }

    public void setBackgroundMessage(string message) {
        background.setMessage(message); //Pass a message to the background componenet to display
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
        clickerBackground.toggle(); //We check if any of the on-screen menus are on the screen, close them if they are, then remove the clicker background
    }

    public void cardHolderClick() {
        cardMenu.toggle(); //Open the card menu
        if(!clickerBackground.isOnScreen())
        {
            clickerBackground.toggle(); //Load in the clicker menu so the user can click off the card menu if it isn't already loaded
        }
    }

    public void nextPhase() {
        if (gameInterface.GetTurnPhase() == TurnPhase.Draft || gameInterface.GetTurnPhase() == TurnPhase.Attack || gameInterface.GetTurnPhase() == TurnPhase.Fortify)
        {
            gameInterface.nextPhase(); //Switch to the next game phase if the above conditons are met when the nextPhase button componenet tells the UI it has been clicked
        }
    }

    private void turnPhaseChanged(TurnPhase turnPhase) {
        this.turnPhase = turnPhase;
        overlay.updateTurnPhaseIndicator(turnPhase);
        Debug.Log("UI");
        Debug.Log(turnPhase); //Update the overlay component for a new phase when this event is triggered
    }

    /**
     * When the screen is clicked, this method is called
     * Starting by differentiatign between the phases, it completes a number of checks per phase for the country that was clicked and if the correct conditions for a move are met, then the move is passed to the gameInterface, otherwise the user can carry on clicking
     */

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

    public void popup(string mode, string message, int time = 5) //Default popup time: 5 seconds
    {
        switch(mode)
        {
            case "long":
                dialogBox.longMessage(message); //popup on screen till user clears it
                break;
            case "clear":
                dialogBox.clear(); //Clear the message when requested
                break;
            default:
                dialogBox.shortMessage(message, time);  //Temporary popup on screen for time number of second
                break;

        }
    }

    void ResetEventHandler(int a)
    {
        _clickingActive = true;
        originCountry = -1;
        destinationCountry = -1; //Reset event handler to default values
    }
    
    public void updateTroopNum()
    {
        List<Country> countriesList = gameInterface.getCountries();
        foreach (Country country in countriesList)
        {
            troopNumbers.changeNumber(country.getName()); //When a country's troop number changes, this method is called which updates the troopNumbers component
        }
    }
}
