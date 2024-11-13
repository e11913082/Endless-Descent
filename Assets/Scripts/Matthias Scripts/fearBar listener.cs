using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EndlessDescent;
using UnityEngine.UI;
using System;

public class fearBarlistener : MonoBehaviour
{
    private int playerId = -1;
    private Slider slider;

    private float maxFear;
    private float curFear;
    

    // Start is called before the first frame update
    void Start()
    {
        playerId = GameObject.FindObjectOfType<PlayerCharacter>().player_id;
        maxFear = PlayerStats.GetPlayerStats(playerId).maxFear;
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        curFear = PlayerStats.GetPlayerStats(playerId).currentFear;
        slider.value = Math.Clamp(curFear / maxFear, 0, maxFear);
    }
}
