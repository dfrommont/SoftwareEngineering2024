using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Script : MonoBehaviour
{
    public UI_Manager manager;
    public GameObject mainMenu;
    private bool defaultView = false;
    private bool onScreen;

    private void Start()
    {
        onScreen = defaultView;
        mainMenu.SetActive(defaultView);
    }

    public void toggle()
    {
        onScreen = !onScreen;
        mainMenu.SetActive(onScreen);
    }

    public bool isOnScreen()
    {
        return onScreen;
    }
}
