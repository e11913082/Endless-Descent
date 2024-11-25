using System;
using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    private bool inChestCollider = false;
    private PlayerCharacter playerCharacter;
    private GameObject chest;
    
    private void Start()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
    }

    void Update()
    {
        if (inChestCollider && playerCharacter.GetActionDown())
        {
            ChestController chestController = chest.GetComponent<ChestController>();
            chestController.OpenChest();
            chest.GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Chest"))
        {
            inChestCollider = true;
            chest = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Chest"))
        {
            inChestCollider = false;
            chest = null;
        }
    }
}
