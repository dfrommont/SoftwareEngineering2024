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
        var script = card.GetComponent<PlayerCardPrefabScript>();
        script.setText(player.getName());
        script.setCardCount(0);
        script.setColor(player.getColor());
        script.setPlayer(player);
        player_cards.Add(card);
    }

    void currentPlayerChanged(Player player) {
        foreach (var player_card in player_cards)
        {
            var script = player_card.GetComponent<PlayerCardPrefabScript>();
            script.setHighlight(script.getPlayer().getColor().Equals(player.getColor()));
            script.setCardCount(script.getPlayer().getCardCount());
        }
    }
}
