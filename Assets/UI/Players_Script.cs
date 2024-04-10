using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Players_Script : MonoBehaviour
{
    public GameInterface gameInterface;
    public GameObject player_card_prefab;
    public Transform player_card_parent;
    private List<GameObject> player_cards = new List<GameObject>();

    public void Start()
    {
        gameInterface.PlayerAdded += playerAdded;
        gameInterface.CurrentPlayerChanged += currentPlayerChanged;
    }

    void playerAdded(Player player){
        var card = Instantiate(player_card_prefab, player_card_parent);
        var test = card.GetComponent<PlayerCardPrefabScript>();
        test.setText(player.getName());
        test.setColor(player.getColor());
        player_cards.Add(card);
    }

    void currentPlayerChanged(Player player) {
        //TODO Highlight current player
    }
}
