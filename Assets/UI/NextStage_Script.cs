using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextStage_Script : MonoBehaviour
{
    public UI_Manager manager;
    public Button button;
    public GameObject nextStage;
    private bool defaultView = true;
    private bool onScreen;

    private void Start()
    {
        nextStage.SetActive(defaultView);
        onScreen = defaultView;
        button.onClick.AddListener(click);
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
    private void click()
    {
        manager.nextPhase();
    }
}
