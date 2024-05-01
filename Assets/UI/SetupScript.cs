using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SetupScript : MonoBehaviour
{

    public TMP_InputField player_field;
    public GameInterface gameInterface;
    public Button add_player_button;
    public Button start_game_button;
    public GameObject player_card_prefab;
    public Transform player_card_parent;
    public GameObject create_player_parent;
    public GameObject roll_button_prefab;
    public GameObject roll_buttons_grand_parent;
    public Transform roll_buttons_parent;
    private List<GameObject> player_cards = new List<GameObject>();
    private List<GameObject> roll_buttons = new List<GameObject>();
    public GameObject setup_screen;
    private bool create_players_complete = false;
    // Start is called before the first frame update
    void Start()
    {
        roll_buttons_grand_parent.SetActive(false);

        add_player_button.onClick.AddListener(addPlayerClick);
        start_game_button.onClick.AddListener(startGameClick);
        gameInterface.PlayerAdded += playerAdded;
        gameInterface.TurnPhaseChanged += turnPhaseChanged;
    }

    void turnPhaseChanged(TurnPhase turnPhase){
        setup_screen.SetActive(turnPhase == TurnPhase.Setup);
    }

    void addPlayerClick(){
        if(gameInterface.createPlayer(player_field.text)){
            player_field.text = "";
        }
    }

    void startGameClick(){
        switch (create_players_complete){
            case true:
                int currentHighestScore = 0;
                int currentHighestIndex = 0;
                bool playerNotRolled = false;
                for (int i = 0; i < player_cards.Count; i++)
                {
                    // var player_card = player_cards[i];
                    // var pcscript = player_card.GetComponent<PlayerCardPrefabScript>();
                    // var player = pcscript.getPlayer();
                    var roll_button = roll_buttons[i];
                    Debug.Log(roll_button);
                    var rbscript = roll_button.GetComponent<RollScript>();
                    var playerValue = rbscript.getValue();
                    if (playerValue==0){
                        playerNotRolled = true;
                    }
                    if(playerValue>currentHighestScore){
                        currentHighestIndex = i;
                        currentHighestScore = playerValue;
                    }
                }
                if(playerNotRolled){

                } else {
                    gameInterface.firstPlayer(currentHighestIndex);
                    gameInterface.startGame();
                }
                break;
            case false:
                create_players_complete = true;
                create_player_parent.SetActive(false);
                roll_buttons_grand_parent.SetActive(true);
                break;
        }
        
    }

    void playerAdded(Player player){
        var card = Instantiate(player_card_prefab, player_card_parent);
        var test = card.GetComponent<PlayerCardPrefabScript>();
        test.setText(player.getName());
        test.setColor(player.getColor());
        player_cards.Add(card);

        var roll_button = Instantiate(roll_button_prefab, roll_buttons_parent);
        roll_buttons.Add(roll_button);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
