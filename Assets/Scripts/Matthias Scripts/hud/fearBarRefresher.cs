using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class FearbarRefresher : MonoBehaviour
{
    public int maximumDecimalPlaces = 1;
    private string stringMask; //used for limiting the decimal places without rounding

    private Slider slider;
    private PlayerStats stats;
    private TextMeshProUGUI maxFearTmp;
    private TextMeshProUGUI curFearTmp;
    private float maxFear;//extra field to avoid trivial object-fieldacceses
    

    void Start()
    {
        slider = GetComponent<Slider>();
        maxFearTmp = GameObject.Find("Hud/FearBar/Text/MaxText").GetComponent<TextMeshProUGUI>();
        curFearTmp = GameObject.Find("Hud/FearBar/Text/CurText").GetComponent<TextMeshProUGUI>();

        stats = PlayerStats.GetPlayerStats(Hud.GetPlayerId());
        stringMask = Hud.CreateStringMask(maximumDecimalPlaces);

        RefreshMaxFear();
        RefreshCurFear();
    }

    void Update()
    {
        RefreshCurFear();
        RefreshMaxFear();
    }

    public void RefreshCurFear()
    {
        slider.value = Math.Clamp(stats.currentFear / stats.maxFear, 0, stats.maxFear);
        curFearTmp.text = stats.currentFear.ToString(stringMask);
    }

    void RefreshMaxFear()
    {
        maxFear = stats.MaxFear;
        maxFearTmp.text = stats.maxFear.ToString(stringMask);
    }

    
}
