using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Script : MonoBehaviour
{
    public UI_Manager manager;
    public Button button;
    public GameObject menu;
    public bool defaultView = true;
    public bool onScreen;

    private void Start()
    {
        button.onClick.AddListener(click);
        onScreen = defaultView;
    }

    private void click()
    {
        Console.WriteLine("button clicked");
        manager.menuButtonClicked();
    }

    public void toggle()
    {
        onScreen = !onScreen;
        menu.SetActive(onScreen);
    }

    public bool isOnScreen()
    {
        return onScreen;
    }
}
