using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FortifyUIScript : MonoBehaviour
{
    public GameObject root;
    public TMP_InputField inputField;
    public Button submitButton;
    public GameInterface gameInterface;
    private int _originCountryID;
    private int _destinationCountryID;
    private Country _originCountry;
    private Country _destinationCountry;

    // Start is called before the first frame update
    void Start()
    {
        submitButton.onClick.AddListener(SubmitClicked);
    }

    void SubmitClicked(){
        int count = int.Parse(inputField.text);
        if (gameInterface.fortify(new Player(""), _originCountry, _destinationCountry, count)){
            root.SetActive(false);
            gameInterface.nextPhase();
        }
    }

    public void Show(int origin, int destination)
    {
        _originCountryID = origin;
        _destinationCountryID = destination;
        _originCountry = gameInterface.getCountry(_originCountryID);
        _destinationCountry = gameInterface.getCountry(_destinationCountryID);
        root.SetActive(true);
    }
}
