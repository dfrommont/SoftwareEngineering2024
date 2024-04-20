using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoll_Script : MonoBehaviour
{

    public UI_Manager manager;
    public GameObject diceRoll;
    public Image dice1;
    public Image dice2;
    public Image dice3;
    public Image dice4;
    public Image dice5;
    public Image dice6;
    public Button rollButton;
    protected Dictionary<int, Image> images;
    private bool defaultView = true;
    private bool onScreen;
    private bool rolled = false;

    void Start()
    {
        images = new Dictionary<int, Image>() 
        {
            { 1, dice1 },
            { 2, dice2 },
            { 3, dice3 },
            { 4, dice4 },
            { 5, dice5 },
            { 6, dice6 },
        };
        diceRoll.SetActive(defaultView);
        rollButton.gameObject.SetActive(true);
        onScreen = defaultView;
        rollButton.onClick.AddListener(roll);
    }

    public void roll()
    {
        rollButton.gameObject.SetActive(false);
        if (!rolled)
        {
            rolled = true;
            int randomNumber = Random.Range(1, 6);
            for (int i = 0; i < images.Count; i++)
            {
                images[i].enabled = false;
                if (i == randomNumber-1) { images[i].enabled = true; }
            }
        }
    }

    public void toggle()
    {
        diceRoll.SetActive(onScreen);
        onScreen = !onScreen;
    }
}
