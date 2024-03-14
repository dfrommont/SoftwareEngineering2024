using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMenu_script : MonoBehaviour
{

    public UI_Manager manager;
    public GameObject cardMenu;
    private bool defaultView = false;
    private bool onScreen;

    // Start is called before the first frame update
    private void Start()
    {
        cardMenu.SetActive(defaultView);
        onScreen = defaultView;
    }

    public void toggle()
    {
        onScreen = !onScreen;
        cardMenu.SetActive(onScreen);
    }

    public bool isOnScreen()
    {
        return onScreen;
    }
}
