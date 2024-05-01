using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using System.Text;

public class MapScript : MonoBehaviour
{
    public Texture2D indexMap;
    private Material material;
    private Texture2D visualMap;
    private int width;
    private int height;
    private int currentCountryID = -1;

    private Color[] countryColors;
    private float[] countryTimes;
    public GameManager gameManager;

    public event Action<int> CountryClick;

    public void Start()
    {
        material = GetComponent<Renderer>().material;

        width = indexMap.width;
        height = indexMap.height;

        countryColors = new Color[gameManager.getCountries().Count+1];
        countryColors[0] = new Color(0f, 0.3f, 0.8f, 1f);

        countryTimes = new float[gameManager.getCountries().Count+1];
        Debug.Log(countryTimes.Length);
        calculateColors();
        renderMap();
        gameManager.CountryChanged += calculateColors;
        // material.SetTexture("_ColorMapTexture", indexMap);
    }

    private void calculateColors()
    {
        List<Country> all_countries = gameManager.getCountries();
        foreach (var country in all_countries) {
            Player player = country.getPlayer();
            if(player == null) {
                countryColors[country.getID()] = Color.black;
            } else {
                countryColors[country.getID()] = player.getColor();
            }
        }
    }

    private void renderMap()
    {
        material.SetColorArray("_CountryColors", countryColors);
        material.SetFloatArray("_LastVisit", countryTimes);
        material.SetFloat("_CurrentTime", Time.time);

        // Assign other shader properties

        // // Render map with the shader
        // Graphics.DrawProceduralNow(MeshTopology.Quads, 4, countryColors.Length);

        // for (int x = 0; x < indexMap.width; x++)
        // {
        //     for (int y = 0; y < indexMap.height; y++)
        //     {
        //         // visualMap.SetPixel(x,y,Color.red);
        //         Color pixelColor = indexMap.GetPixel(x, y);
        //         int countryID = GetCountryIDFromColor(pixelColor);
        //         Color renderColor = countryColors[countryID];
        //         // Render the pixel with the calculated color
        //         // Example: SetPixel or modify material color
        //         // if(currentCountryID == countryID && currentCountryID != 0){
        //         //     visualMap.SetPixel(x, y, Color.white);
        //         // } else {
        //             visualMap.SetPixel(x, y, renderColor);
        //         // }
        //     }
        // }
        // visualMap.Apply();
        // material.SetTexture("test", visualMap);
    }
    private int GetCountryIDFromColor(Color color)
    {
        return Mathf.RoundToInt(color.r * 255);
    }

    private void Update()
    {
        var newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0;
        int x = Mathf.RoundToInt((newPosition.x * width / 20) + (width/2));
        int y = Mathf.RoundToInt((newPosition.y * height / 10) + (height/2));
        // Debug.Log((x,y));
        
        currentCountryID = Mathf.RoundToInt(indexMap.GetPixel(x, y).r*255);
       
        if(currentCountryID!=0){
            // Debug.Log(currentCountryID);
            countryTimes[currentCountryID] = Time.time;
            // Debug.Log(countryTimes[currentCountryID]);
        }
        renderMap();
    }

    void OnMouseDown() {
        if(currentCountryID!=0){
            Debug.Log(currentCountryID);
            Debug.Log("click");
            CountryClick?.Invoke(currentCountryID);
            calculateColors();
            renderMap();
        }
    }
}
