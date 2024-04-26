using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCardPrefabScript : MonoBehaviour
{
    public TMP_Text text;
    public GameObject box;
    public GameObject inset;
    private Player plyr;

    public void setText(string player_name){
        text.text = player_name;
    }
    public void setColor(Color color){
        box.GetComponent<Image>().color = color;
    }
    public void setPlayer(Player player){
        plyr = player;
    }
    public Player getPlayer(){
        return plyr;
    }
    public void setHighlight(bool highlight){
        /** Enable or disable the highlighting of the prefab card */
        /** This is done by calculating a slightly different S and V from the players color */
        var insetComponent = inset.GetComponent<Image>();
        switch (highlight) {
            case true:
                float H, S, V;
                Color.RGBToHSV(plyr.getColor(), out H, out S, out V);
                insetComponent.color = Color.HSVToRGB(H, 1f, V-0.1f);
                break;
            case false:
                insetComponent.color = new Color(0.8588236f,0.345098f,0.3058824f);
                break;
        }
    }
}
