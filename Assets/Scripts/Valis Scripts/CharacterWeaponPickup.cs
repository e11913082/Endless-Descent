using System;
using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class CharacterWeaponPickup : MonoBehaviour
{
    private PlayerCharacter character;
    private CharacterWeaponInventory inventory;
    
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<PlayerCharacter>();
        inventory = GetComponent<CharacterWeaponInventory>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            if (character.GetActionDown())
            {   
                Weapon weapon = other.GetComponent<WeaponPickup>().GetWeapon();
                if (inventory.pickupWeapon(weapon))
                {
                    Debug.Log("Picked up weapon: " + weapon.name);
                    Destroy(other.gameObject);
                }
                else
                {
                    Debug.Log("Cannot pick up weapon, inventory full");
                }
            }
        }
    }
}
