using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Events;

public class FearbarRefresher : MonoBehaviour
{
    public int maximumDecimalPlaces = 1;
    private string stringMask; //used for limiting the decimal places without rounding

    private Slider slider;
    private PlayerStats stats;
    private TextMeshProUGUI fearIncTmp;
    private TextMeshProUGUI fearDecTmp;

    private GameObject upArrow;
    private GameObject downArrow;

    void Start()
    {
        upArrow = GameObject.Find("Hud V2/FearBar/Arrows/UpArrow");
        downArrow = GameObject.Find("Hud V2/FearBar/Arrows/DownArrow");

        slider = transform.Find("Bar").GetComponent<Slider>();

        stats = PlayerStats.GetPlayerStats(Hud.GetPlayerId());
        stringMask = Hud.CreateStringMask(maximumDecimalPlaces);

        fearDecTmp = downArrow.transform.Find("Text").GetComponent<TextMeshProUGUI>(); //GameObject.Find("Hud V2/FearBar/Arrows/DownArrow/Text").GetComponent<TextMeshProUGUI>();
        fearIncTmp = upArrow.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        RefreshBuildUpText();
    }

    private void OnEnable()
    {
        EventManager.StartListening("FearRefresh", RefreshCurFear);
    }

    private void OnDisable()
    {
        EventManager.StopListening("FearRefresh", RefreshCurFear);
    }

    private void RefreshCurFear()
    {
        Debug.Log("FearRefresh");
        float oldVal = slider.value;
        slider.value = Math.Clamp(stats.currentFear / stats.maxFear, 0, stats.maxFear);
        if (slider.value > oldVal)
        {
            upArrow.SetActive(true);
            downArrow.SetActive(false);
        }
        else if (slider.value < oldVal)
        {
            downArrow.SetActive(true);
            upArrow.SetActive(false);
        }
        else
        {
            downArrow.SetActive(false);
            upArrow.SetActive(false);
        }
    }
    private void RefreshBuildUpText()
    {
        fearDecTmp.text = stats.fearDecrease.ToString(stringMask);
        fearIncTmp.text = stats.fearIncrease.ToString(stringMask);
    }
}
