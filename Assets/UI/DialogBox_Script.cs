using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox_Script : MonoBehaviour
{
    public UI_Manager manager;
    public GameObject dialogBox;
    public TMP_Text text;
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

    public void longMessage(string message)
    {
        toggle();
        text.SetText(message);
    }

    public void clear()
    {
        text.SetText(string.Empty);
    }

    public void shortMessage(string message, int time)
    {
        toggle();
        text.SetText(message);
        System.Threading.Thread.Sleep(time * 1000);
        clear();
        toggle();
    }
}
