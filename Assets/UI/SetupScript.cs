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
    private List<GameObject> player_cards = new List<GameObject>();
    public GameObject setup_screen;
    // Start is called before the first frame update
    void Start()
    {
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
        gameInterface.startGame();
    }

    void playerAdded(Player player){
        var card = Instantiate(player_card_prefab, player_card_parent);
        var test = card.GetComponent<PlayerCardPrefabScript>();
        test.setText(player.getName());
        test.setColor(player.getColor());
        player_cards.Add(card);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
