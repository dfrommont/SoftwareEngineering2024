using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopMovement_Script : MonoBehaviour
{

    public UI_Manager manager;
    public GameObject troopMovement;
    private bool defaultView = false;
    private bool onScreen;

    // Start is called before the first frame update
    private void Start()
    {
        troopMovement.SetActive(defaultView);
        onScreen = defaultView;
    }

    public void toggle()
    {
        onScreen = !onScreen;
        troopMovement.SetActive(onScreen);
    }

    public bool isOnScreen()
    {
        return onScreen;
    }
}
