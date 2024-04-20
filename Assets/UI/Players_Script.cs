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
        Debug.Log("PLAYER ADDED");
        var card = Instantiate(player_card_prefab, player_card_parent);
        var script = card.GetComponent<PlayerCardPrefabScript>();
        script.setText(player.getName());
        script.setColor(player.getColor());
        script.setPlayer(player);
        player_cards.Add(card);
    }

    void currentPlayerChanged(Player player) {
        foreach (var player_card in player_cards)
        {
            var script = player_card.GetComponent<PlayerCardPrefabScript>();
            var scriptPlayer = script.getPlayer();
            var scriptColor = scriptPlayer.getColor();
            var color = player.getColor();
            script.setHighlight(scriptColor.Equals(color));
        }
    }
}
