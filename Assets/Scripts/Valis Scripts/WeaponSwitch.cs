using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using EndlessDescent;
using TMPro;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{   
    private PlayerCharacter character;
    private CharacterWeaponInventory inventory;
    
    private DistanceWeapon distanceWeapon;
    private MeleeWeapon meleeWeapon;
    private PlayerStats stats;
    private CharacterHoldItem holdItem;
    
    
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<PlayerCharacter>();
        inventory = GetComponent<CharacterWeaponInventory>();
        
        distanceWeapon = GetComponent<DistanceWeapon>();
        meleeWeapon = GetComponent<MeleeWeapon>();
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
        }
    }

    // Update is called once per frame
  /*  void Update()
    {
        if (character.GetWeaponSwitch())
        {
            if (distanceWeapon.IsSelected() && !meleeWeapon.IsSelected())
            {
                if (meleeWeapon.IsAvailable())
                {
                    distanceWeapon.Deselect();
                    meleeWeapon.Select();
                    stats.damage = stats.damage - distanceWeapon.GetEquippedWeapon().damageBonus + meleeWeapon.GetEquippedWeapon().damageBonus;
                } else 
                {
                    Debug.Log("Melee Weapon not available");
                }
                
            } else if (!distanceWeapon.IsSelected() && meleeWeapon.IsSelected())
            {
                if (distanceWeapon.IsAvailable())
                {
                    meleeWeapon.Deselect();
                    distanceWeapon.Select();
                    stats.damage = stats.damage - meleeWeapon.GetEquippedWeapon().damageBonus + distanceWeapon.GetEquippedWeapon().damageBonus;
                } else 
                {
                    Debug.Log("Distance Weapon not available");
                }
                
            }
            else
            {
                Debug.LogError("Selected Mismatch at Weapons: Distance: " + distanceWeapon.IsSelected() + " Melee: " + meleeWeapon.IsSelected());
            }
        } else if (character.GetActionDown())
        {
            //holdItem.DropItem();
        }
    }*/
    
    
}
