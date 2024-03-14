using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickerBackground_Script : MonoBehaviour
{

    public UI_Manager manager;
    public GameObject background;
    public Button clickerButton;
    private bool defaultView = false;
    private bool onScreen = true;

    // Start is called before the first frame update
    private void Start()
    {
        onScreen = defaultView;
        clickerButton.onClick.AddListener(click);
        background.SetActive(defaultView);
    }

    public void click()
    {
        manager.ClickerBackgroundClick();
    }

    public void toggle()
    {
        onScreen = !onScreen;
        background.SetActive(onScreen);
    }

    public bool isOnScreen()
    {
        return onScreen;
    }
}
