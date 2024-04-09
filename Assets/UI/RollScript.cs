using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RollScript : MonoBehaviour
{
    public Button button;
    public TMP_Text text;
    private bool rolled = false;
    private int rolled_number = 0;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(rollDie);
    }
    void rollDie()
    {
        if(!rolled){
            rolled = true;
            int randomNumber = Random.Range(1, 6);
            rolled_number = randomNumber;
            text.text = randomNumber.ToString();
        }
    }

    public int getValue()
    {
        return rolled_number;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
