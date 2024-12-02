using System;
using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using UnityEngine;

public class CharacterGamblingTrader : MonoBehaviour
{
    public GameObject canvas;
    private bool inTrigger = false;
    private PlayerCharacter playerCharacter;

    void Start()
    {
        canvas = GameObject.Find("/GamblingCanvas");
        canvas.gameObject.SetActive(false);
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GamblingTrader"))
        {
            inTrigger = true;
            playerCharacter = GetComponent<PlayerCharacter>();
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("GamblingTrader"))
        {
            inTrigger = true;
            playerCharacter = GetComponent<PlayerCharacter>();
        }
    }
    private void Update()
    {
        if (inTrigger && playerCharacter.GetActionDown())
        {
           // canvas = GameObject.Find("/GamblingCanvas");
            
            canvas.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
