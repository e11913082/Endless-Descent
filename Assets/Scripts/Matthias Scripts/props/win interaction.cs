using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinInteraction : MonoBehaviour
{
    private LayerMask playerLayer;
    private GameObject winBanner;
    void Start()
    {
        winBanner = transform.Find("WinBanner").gameObject;
        winBanner.SetActive(false);
        playerLayer = LayerMask.GetMask("Player");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //TODO play secret sound
        if ((playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Time.timeScale = 0f;
            winBanner.SetActive(true);
        }
    }
}
