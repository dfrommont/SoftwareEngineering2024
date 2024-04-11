using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCardPrefabScript : MonoBehaviour
{
    public TMP_Text text;
    public GameObject box;
    // Start is called before the first frame update
    public void setText(string player_name){
        text.text = player_name;
    }
    public void setColor(Color color){
        box.GetComponent<Image>().color = color;
    }
}
