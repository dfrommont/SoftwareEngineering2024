using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{

    [SerializeField] private string name;
    private Color color;

    public Player(string name, Color color = new Color())
    {
        if (color.Equals(new Color())){
            color = Random.ColorHSV(0f,1f,1f,1f,0.7f,0.7f,1f,1f);
        }
        this.name = name;
        this.color = color;
    }

    public string getName()
    {
        return name;
    }

    public Color getColor()
    {
        return color;
    }
}
