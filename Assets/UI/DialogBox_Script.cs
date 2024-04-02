using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox_Script : MonoBehaviour
{
    public UI_Manager manager;
    public GameObject dialogBox;
    public ClickerBackground_Script clickerBackground;
    private bool defaultView = false;
    private bool onScreen;

    private void Start()
    {
        dialogBox.SetActive(defaultView);
        onScreen = defaultView;
    }

    public void toggle()
    {
        onScreen = !onScreen;
        dialogBox.SetActive(onScreen);
    }

    public bool isOnScreen()
    {
        return onScreen;
    }
}
