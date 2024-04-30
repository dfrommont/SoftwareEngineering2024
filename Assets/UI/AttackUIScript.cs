using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackUIScript : MonoBehaviour
{
    public GameObject root;
    public TMP_InputField attackInputField;
    public TMP_InputField defendInputField;
    public Button submitButton;
    public GameInterface gameInterface;
    private int _originCountryID;
    private int _destinationCountryID;
    private Country _originCountry;
    private Country _destinationCountry;
    public TMP_Text attackCountry;
    public TMP_Text defendCountry;
    // Start is called before the first frame update
    void Start()
    {
        submitButton.onClick.AddListener(submit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Show(int origin, int destination)
    {
        _originCountryID = origin;
        _destinationCountryID = destination;
        _originCountry = gameInterface.getCountry(origin);
        _destinationCountry = gameInterface.getCountry(destination);
        attackCountry.text = _originCountry.getName();
        defendCountry.text = _destinationCountry.getName();
        root.SetActive(true);
    }

    private void submit()
    {
        int attackRolls = int.Parse(attackInputField.text);
        int defendRolls = int.Parse(attackInputField.text);
        if (attackRolls > 3)
        {
            Debug.Log("Attack rolls must not exceed 3");
            return;
        }
        if (attackRolls < 1)
        {
            Debug.Log("Attack rolls must not be below 1");
            return;
        }
        if (attackRolls + 1 > _originCountry.getArmiesCount())
        {
            Debug.Log("Attack rolls must not exceed your army count + 1");
            return;
        }
        if (defendRolls > 2)
        {
            Debug.Log("Defend rolls must not exceed 2");
            return;
        }
        if (defendRolls < 1)
        {
            Debug.Log("Defend rolls must not be below 1");
            return;
        }
        if (defendRolls >= _originCountry.getArmiesCount())
        {
            Debug.Log("Defend rolls must not exceed your army count");
            return;
        }

        if(gameInterface.battle(_originCountry, attackRolls, _destinationCountry, defendRolls))
        {
            root.SetActive(false);
        }
    }
}
