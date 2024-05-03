using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{

    /// <summary>
    /// Class <c>UI_Manager</c> manages incoming communications from the UI components and GameInterface.
    /// </summary>

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

    /// <summary>
    /// Method  <c>Start</c> is called before the first frame.
    /// <para>
    /// It initialises the following event handlers: countryClicked, turnPhaseChanged, RssetEventHandler, updateTroopNum, countriesInitialised.
    /// </para>
    /// </summary>
    private void Start() {

        map.CountryClick += countryClicked;
        overlay.updateTurnPhaseIndicator(TurnPhase.Deploy);
        gameInterface.TurnPhaseChanged += turnPhaseChanged;
        gameInterface.ResetEvent += ResetEventHandler;
        gameInterface.CountryChanged += updateTroopNum;
        gameInterface.CountriesInitialised += countriesInitialised;
        
        troopMovement.toggle();
    }

    /// <summary>
    /// Method <c>countriesInitialised()</c> is an event handler that calls a method in the TroopNumber_Script to load the troop numbers onto the screen in position.
    /// </summary>

    public void countriesInitialised()
    {
        troopNumbers.load(); //Call the load() method to position the UI troop numbers once the countries have been initialised
    }

    /// <summary>
    /// Method <c>menuButtonClicked()</c> is called when the menu Button is clicked, it opens the menu on screen and loads the clicker background if it is needed (this allows the user to click off the menu).
    /// </summary>

    public void menuButtonClicked() {
        mainMenu.toggle(); //open the main menu
        if (!clickerBackground.isOnScreen()) {
            clickerBackground.toggle(); //If the clicker background (a hidden background used to click off the menu) isn't on screen, load it on screen
        }
    }

    /// <summary>
    /// Method <c>setBackgroundMessage()</c> calls a method in the Background_Script to load the given message on to the background.
    /// </summary>
    /// <param name="message"></param>

    public void setBackgroundMessage(string message) {
        background.setMessage(message); //Pass a message to the background componenet to display
    }

    /// <summary>
    /// Method <c>ClickerBackgroundClick()</c> loops through a number of popup UIs (main menu, troop movement and card menu) and closes them if they are on screen. This method is called the clicker background if it is clicked.
    /// </summary>

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

    /// <summary>
    /// Method <c>cardHolderClick()</c> is called when the card holder button is clicked and loads the card menu on screen and the clicker background is needed.
    /// </summary>

    public void cardHolderClick() {
        cardMenu.toggle(); //Open the card menu
        if(!clickerBackground.isOnScreen())
        {
            clickerBackground.toggle(); //Load in the clicker menu so the user can click off the card menu if it isn't already loaded
        }
    }

    /// <summary>
    /// Method <c>nextPhase()</c> checks if the current phase is a phase we can move from and then calls a method in the gameInterface to move to the next phase.
    /// </summary>

    public void nextPhase() {
        if (gameInterface.GetTurnPhase() == TurnPhase.Draft || gameInterface.GetTurnPhase() == TurnPhase.Attack || gameInterface.GetTurnPhase() == TurnPhase.Fortify)
        {
            gameInterface.nextPhase(); //Switch to the next game phase if the above conditons are met when the nextPhase button componenet tells the UI it has been clicked
        }
    }

    /// <summary>
    /// Method <c>trunPhaseChanged()</c> is an event handler for turnphaseChanged and updtes the overlay for the new phase which is passed as a parameter.
    /// </summary>
    /// <param name="turnPhase"></param>

    private void turnPhaseChanged(TurnPhase turnPhase) {
        this.turnPhase = turnPhase;
        overlay.updateTurnPhaseIndicator(turnPhase);
        Debug.Log("UI");
        Debug.Log(turnPhase); //Update the overlay component for a new phase when this event is triggered
    }

    /// <summary>
    /// Method <c>countryClicked()</c> manages countries being clicked and passing this information to the gameInterface
    /// </summary>
    /// <para>
    /// If the countries are 'clickable' then we check the current phase. If it is the deploy phase then we call the deploy method of the gameInterface and pass the player and country. On thr draft phase we check that the country being clicked isn't already owned by the user and then call the draft method in the gameInterface. On the attack phase, we store the first country being clicked as the origin country and on the second call store the country as the target country and then call the attack method with both of these countries. The process for the fortify phase is very similar to the attack phase but we call the fortify pahse of the gameInterface.
    /// </para>
    /// <param name="country"></param>

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

    /// <summary>
    /// Method <c>popup()</c> checks if the mode is "long", "clear" and any other value. If the mode is "long" we call the longMessage() method of the dialogBox and pass in the message. If the mode is clear then we call the clear method and if the mode is anything else we call the shortMessage() method passing in the string and time which is 5 unless the user passes in their own value.
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="message"></param>
    /// <param name="time">, default value 5</param>

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

    /// <summary>
    /// Method <c>ResetEventHandler()</c> is an event handler for ResetEvent that resets the clicking toggle and origin/destination value to their defaults.
    /// </summary>
    /// <param name="a"></param>

    public void ResetEventHandler(int a)
    {
        _clickingActive = true;
        originCountry = -1;
        destinationCountry = -1; //Reset event handler to default values
    }

    /// <summary>
    /// method <c>updatetroopnum()</c> is the event handler for countryChanged that works for the list of all countries and calls the troopNumbers.changeNumber() nmethod is update the country passing in the country as a parameter.
    /// </summary>
    
    public void updateTroopNum()
    {
        List<Country> countriesList = gameInterface.getCountries();
        foreach (Country country in countriesList)
        {
            troopNumbers.changeNumber(country.getName()); //When a country's troop number changes, this method is called which updates the troopNumbers component
        }
    }
}
