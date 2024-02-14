using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private int availableToDraft = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    int getAvailableToDraft(){
        return availableToDraft;
    }
}
