using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class MainMenu_Script : MonoBehaviour
{
    public UI_Manager manager;
    public GameObject mainMenu;
    public Button closeButton;
    public ClickerBackground_Script clickerBackground;
    public Overlay_Script overlay;
    private bool defaultView = false;
    private bool onScreen;

    private void Start()
    {
        onScreen = defaultView;
        mainMenu.SetActive(defaultView);
        closeButton.onClick.AddListener(click);
    }

    private void click()
    {
        if (clickerBackground.isOnScreen())
        {
            clickerBackground.toggle();
        }
        toggle();
    }

    public void toggle()
    {
        onScreen = !onScreen;
        mainMenu.SetActive(onScreen);
        overlay.pauseTimer();
    }

    public bool isOnScreen()
    {
        return onScreen;
    }
}
