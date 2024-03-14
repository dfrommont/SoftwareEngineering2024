using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStage_Script : MonoBehaviour
{
    public UI_Manager manager;
    public GameObject nextStage;
    private bool defaultView = false;
    private bool onScreen;

    private void Start()
    {
        nextStage.SetActive(defaultView);
        onScreen = defaultView;
    }

    public bool isOnScreen()
    {
        return onScreen;
    }

    public void toggle()
    {
        nextStage.SetActive(onScreen);
        onScreen = !onScreen;
    }
}
