using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FearbarRefresher : MonoBehaviour
{
    public int maximumDecimalPlaces = 1;
    private string stringMask; //used for limiting the decimal places without rounding

    private Slider slider;
    private PlayerStats stats;
    private TextMeshProUGUI fearIncTmp;
    private TextMeshProUGUI fearDecTmp;
    private float maxFear;//extra field to avoid trivial object-fieldacceses
    private UnityAction onItemPickup;

    private void Awake()
    {
        onItemPickup = new UnityAction(RefreshBuildUpText);
    }

    void Start()
    {
        slider = transform.Find("Bar").GetComponent<Slider>();

        stats = PlayerStats.GetPlayerStats(Hud.GetPlayerId());
        stringMask = Hud.CreateStringMask(maximumDecimalPlaces);

        fearDecTmp = GameObject.Find("Hud V2/FearBar/Arrows/DownArrow/Text").GetComponent<TextMeshProUGUI>();
        fearIncTmp = GameObject.Find("Hud V2/FearBar/Arrows/UpArrow/Text").GetComponent<TextMeshProUGUI>();
        RefreshBuildUpText();
    }

    void Update()
    {
        RefreshCurFear();
    }
    private void OnEnable()
    {
        EventManager.StartListening("ItemPickup", onItemPickup);
    }

    private void OnDisable()
    {
        EventManager.StopListening("ItemPickup", onItemPickup);
    }


    private void RefreshCurFear()
    {
        slider.value = Math.Clamp(stats.currentFear / stats.maxFear, 0, stats.maxFear);
        //curFearTmp.text = stats.currentFear.ToString(stringMask);
    }


    //private void RefreshMaxFear()
    //{
    //    maxFear = stats.MaxFear;
    //    maxFearTmp.text = stats.maxFear.ToString(stringMask);
    //}

    private void RefreshBuildUpText()
    {
        fearDecTmp.text = stats.fearDecrease.ToString(stringMask);
        fearIncTmp.text = stats.fearIncrease.ToString(stringMask);
    }
}
