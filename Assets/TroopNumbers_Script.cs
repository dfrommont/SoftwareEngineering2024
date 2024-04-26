using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TroopNumbers_Script : MonoBehaviour
{
    public UI_Manager manager;
    public TMP_Text afghanastan;
    public TMP_Text alaska;
    public TMP_Text alberta;
    public TMP_Text argentina;
    public TMP_Text brazil;
    public TMP_Text centralAmerica;
    public TMP_Text china;
    public TMP_Text congo;
    public TMP_Text eastAfrica;
    public TMP_Text easternAustralia;
    public TMP_Text easternUS;
    public TMP_Text egypt;
    public TMP_Text greatBritain;
    public TMP_Text greenland;
    public TMP_Text iceland;
    public TMP_Text india;
    public TMP_Text indonesia;
    public TMP_Text irkutsk;
    public TMP_Text japan;
    public TMP_Text kamchatka;
    public TMP_Text madagascar;
    public TMP_Text middleEast;
    public TMP_Text mongolia;
    public TMP_Text newGuinea;
    public TMP_Text northAfrica;
    public TMP_Text northernEurope;
    public TMP_Text northwestTerritor;
    public TMP_Text ontario;
    public TMP_Text peru;
    public TMP_Text quebec;
    public TMP_Text scandinavia;
    public TMP_Text siam;
    public TMP_Text siberia;
    public TMP_Text southAfrica;
    public TMP_Text southernEurope;
    public TMP_Text ukraine;
    public TMP_Text ural;
    public TMP_Text venezuela;
    public TMP_Text westernAustralia;
    public TMP_Text westernEurope;
    public TMP_Text westernUS;
    public TMP_Text yakutsk;
    private Dictionary<string, TMP_Text> country_dict;

    // Start is called before the first frame update
    void Start()
    {
        country_dict = new Dictionary<string, TMP_Text>() {
            {"Afghanastan", afghanastan},
            {"Alberta", alberta},
            {"Argentina", argentina},
            {"Brazil", brazil},
            {"Central America", centralAmerica},
            {"China", china},
            {"Congo", congo},
            {"East Africa", eastAfrica},
            {"Eastern Australia", easternAustralia},
            {"Eastern US", easternUS},
            {"Egypt", egypt},
            {"Great Britain", greatBritain},
            {"Greenland", greenland},
            {"Iceland", iceland},
            {"India", india},
            {"Indonesia", indonesia},
            {"Irkutsk", irkutsk},
            {"Japan", japan},
            {"Kamchatka", kamchatka},
            {"Madagascar", madagascar},
            {"Middle East", middleEast},
            {"Mongolia", mongolia},
            {"New Guinea", newGuinea},
            {"North Africa", northAfrica},
            {"NorthernEurope", northernEurope},
            {"Northwest Territor", northwestTerritor},
            {"Ontario", ontario},
            {"Peru", peru},
            {"Quebec", quebec},
            {"Scandinavia", scandinavia},
            {"Siam", siam},
            {"Siberia", siberia},
            {"South Africa", southAfrica},
            {"Southern Europe", southernEurope},
            {"Ukraine", ukraine},
            {"Ural", ural},
            {"Venezuela", venezuela},
            {"Western Australia", westernAustralia},
            {"Western Europe", westernEurope},
            {"Western US", westernUS},
            {"Yakutsk", yakutsk},
        };
    }

    public void changeNumber(string name, int number)
    {
        country_dict[name].SetText(""+number);
    }
}
