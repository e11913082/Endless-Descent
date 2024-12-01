using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinInteraction : MonoBehaviour
{
    private GameObject winBanner;
    void Start()
    {
        winBanner = transform.Find("WinBanner").gameObject;
        winBanner.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //TODO play secret sound
        if (other.CompareTag("Player"))
        {
            Debug.Log("test");
            Time.timeScale = 0f;
            winBanner.SetActive(true);
        }
    }
}
