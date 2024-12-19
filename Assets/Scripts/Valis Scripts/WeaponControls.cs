using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using EndlessDescent;
using TMPro;
using UnityEngine;

public class WeaponControls : MonoBehaviour
{   
    private PlayerCharacter character;
    private CharacterWeaponInventory inventory;
    
    private PlayerStats stats;
    private CharacterHoldItem holdItem;
    
    
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<PlayerCharacter>();
        inventory = GetComponent<CharacterWeaponInventory>();
        
        holdItem = GetComponent<CharacterHoldItem>();
        stats = PlayerStats.GetPlayerStats(character.player_id);
    }

    private void Update()
    {
        if (character.GetWeaponSwitch())
        {
            if (inventory.weapons.Count > 1 && inventory.equippedWeapon != null)
            {
                inventory.SwitchWeapon();
            }
            else
            {
                Debug.Log("NO other weapon available or nothing at all");
            }
        } else if (character.GetWeaponDrop())
        {
            if (inventory.weapons.Count > 0 && inventory.equippedWeapon != null)
            {
                inventory.DropWeapon();
            }
            else
            {
                Debug.Log("Cannot drop weapon");
            }
        }
    }

    
    
    
}
