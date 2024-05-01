using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{

    [SerializeField] private string name;
    private Color color;
    private List<RiskCard> riskCardHand;
    private bool isAIPlayer;

    public Player(string name, Color color = new Color())
    {
        if (color.Equals(new Color())){
            color = Random.ColorHSV(0f,1f,1f,1f,0.7f,0.7f,1f,1f);
        }
        this.name = name;
        this.color = color;
        riskCardHand = new();
    }
    
    public void addRiskCardToHand(RiskCard card)
    {
        riskCardHand.Add(card);
    }

    public void removeRiskCardFromHand(RiskCard card)
    {
        riskCardHand.Remove(card);
    }

    public string getName()
    {
        return name;
    }
    
    public int getCardCount()
    {
        return 5;
    }

    public List<RiskCard> getPlayerHand()
    {
        return riskCardHand;
    }

    public Color getColor()
    {
        return color;
    }

    public void setIsAIPlayer(bool isAIPlayer)
    {
        this.isAIPlayer = isAIPlayer;
    }

    public bool getIsAIPlayer()
    {
        return isAIPlayer;
    }
}
