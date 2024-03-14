using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHolder_Script : MonoBehaviour
{
    public UI_Manager manager;
    public GameObject cardHolder;
    public Button cardHolderButton;
    private bool defaultView = true;
    private bool onScreen;

    private void Start()
    {
        cardHolder.SetActive(defaultView);
        onScreen = defaultView;
        cardHolderButton.onClick.AddListener(click);
    }

    private void click()
    {
        manager.cardHolderClick();
    }

    public bool isOnScreen()
    {
        return onScreen;
    }

    public void toggle()
    {
        onScreen = !onScreen;
        cardHolder.SetActive(onScreen);
    }
}
