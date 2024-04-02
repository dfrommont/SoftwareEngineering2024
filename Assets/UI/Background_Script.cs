using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Background_Script : MonoBehaviour
{
    public GameObject background;
    public TMP_Text backgroundText;

    private bool defaultView = true; //true if element should be on screen by default
    private bool onScreen; //true if element is currently on screen

    public void toggle()
    {
        onScreen = !onScreen;
        background.SetActive(onScreen);
    }

    public void setMessage(string message)
    {
        backgroundText.SetText(message);
    }

    public bool isOnScreen()
    {
        return onScreen;
    }

    private void Start()
    {
        onScreen = defaultView;
    }
}
