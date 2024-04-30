using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DraftArmiesUIScript : MonoBehaviour
{
    public GameObject root;
    public TMP_InputField inputField;
    public Button submitButton;
    public GameInterface gameInterface;
    private int _currentCountry;
    public TMP_Text countryID;

    // Start is called before the first frame update
    void Start()
    {
        submitButton.onClick.AddListener(SubmitClicked);
    }

    void SubmitClicked(){
        int count = int.Parse(inputField.text);
        Debug.Log(_currentCountry);
        if (gameInterface.draft(new Player(""), _currentCountry, count)){
            root.SetActive(false);
        }
    }

    public void Show(int country)
    {
        countryID.SetText(country.ToString());
        _currentCountry = country;
        root.SetActive(true);
    }
}
