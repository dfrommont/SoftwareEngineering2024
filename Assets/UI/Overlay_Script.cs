using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Overlay_Script : MonoBehaviour
{

    public UI_Manager manager;
    public GameObject overlay;
    public TMP_Text timerText;
    private float start;
    public TMP_Text draftSign;
    public TMP_Text attackSign;
    public TMP_Text fortifySign;

    private void Start()
    {
        start = Time.time;
        InvokeRepeating("Timer", 0f, 1f);
    }

    private void Timer()
    {
        float elapsedTime = Time.time - start;
        timerText.SetText(FormatTime(elapsedTime));
    }

    private string FormatTime(float timeInSeconds)
    {
        float hours = Mathf.Floor(timeInSeconds / 3600);
        float minutes = Mathf.Floor(timeInSeconds / 60) % 60;
        float seconds = timeInSeconds % 60;

        string timeString = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        return timeString;
    }

    public void updateTurnPhaseIndicator(TurnPhase currentPhase)
    {
        draftSign.fontSize = 18;
        attackSign.fontSize = 18;
        fortifySign.fontSize = 18;
        switch (currentPhase) {
            case TurnPhase.Draft:
                draftSign.fontSize = 25;
                break;
            case TurnPhase.Attack:
                attackSign.fontSize = 25;
                break;
            case TurnPhase.Fortify:
                fortifySign.fontSize = 25;
                break;
            default:
                break;
        };
    }
}
