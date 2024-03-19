using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    public Texture2D indexMap;
    private int width;
    private int height;

    public void Start() {
        width = indexMap.width;
        height = indexMap.height;
    }

    public void Update() {
        var newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0;
        int x = Mathf.RoundToInt((newPosition.x * width / 20) + (width/2));
        int y = Mathf.RoundToInt((newPosition.y * height / 10) + (height/2));
        Debug.Log((x,y));
        // Debug.Log(y);
        
        Debug.Log(indexMap.GetPixel(x, y).r*255);
    }
}
