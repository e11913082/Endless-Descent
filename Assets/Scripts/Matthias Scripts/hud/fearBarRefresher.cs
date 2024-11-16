using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class fearBarlistener : MonoBehaviour
{
    public int maximumDecimalPlaces = 1;
    private string stringMask; //used for limiting the decimal places without rounding

    private Slider slider;
    private PlayerStats stats;
    private TextMeshProUGUI maxFearTmp;
    private TextMeshProUGUI curFearTmp;
    private float maxFear;//extra field to avoid trivial object-fieldacceses
    

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        maxFearTmp = GameObject.Find("Hud/FearBar/Text/MaxText").GetComponent<TextMeshProUGUI>();
        curFearTmp = GameObject.Find("Hud/FearBar/Text/CurText").GetComponent<TextMeshProUGUI>();

        hudRefresher hud = transform.parent.GetComponent<hudRefresher>();
        if(hud == null)
        {
            Debug.Log("hud null");
        }
        stats = hud.getStats();
        if(stats == null)
        {
            Debug.Log("stats null");
        }
        stringMask = hudRefresher.createStringMask(maximumDecimalPlaces);

        RefreshMaxFear();
        RefreshCurFear();

        PlayerBuildupManager buildupManager = FindObjectOfType<PlayerBuildupManager>(); //listener for refreshing current fear
        buildupManager.onFearChanged.AddListener(RefreshCurFear);

    }

    // Update is called once per frame
    void Update()
    {
        
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
