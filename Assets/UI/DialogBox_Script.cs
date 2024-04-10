using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox_Script : MonoBehaviour
{
    public UI_Manager manager;
    public GameObject dialogBox;
    public ClickerBackground_Script clickerBackground;
    private bool defaultView = false;
    private bool onScreen;
    private string currentMessage = string.Empty;

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
        if (!onScreen) toggle();
        currentMessage = message;
    }

    public void clear()
    {
        if (onScreen) toggle();
        currentMessage = string.Empty;
    }

    public async void shortMessage(string message, int time)
    {
        if (!onScreen) toggle();
        currentMessage = message;
        await Task.Delay(1000 * time);
        clear();
    }
}
