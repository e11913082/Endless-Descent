using System;
using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterWeaponInventory : MonoBehaviour
{
    public List<Weapon> weapons;
    public Weapon equippedWeapon;
    private int currentIndex;
    private PlayerStats stats;
    public int maxInventorySize;

    private void Awake()
    {
        stats = PlayerStats.GetPlayerStats(GetComponent<PlayerCharacter>().player_id); 
        weapons = new List<Weapon>();
        currentIndex = 0;
    }

    private void Start()
    {
        if (equippedWeapon != null)
        {
            stats.damage += equippedWeapon.damageBonus;
        }
    }

    public bool pickupWeapon(Weapon weapon)
    {
        if (weapons.Count >= maxInventorySize) return false;
        stats = PlayerStats.GetPlayerStats(GetComponent<PlayerCharacter>().player_id);
        weapons.Add(weapon);
        if (weapons.Count == 1)
        {
            stats.Damage += weapon.damageBonus;
            EquipWeapon(0);
        }

        return true;
    }


    public void EquipWeapon(int weaponIndex)
    {   
        equippedWeapon = weapons[weaponIndex];
    }

    public void SwitchWeapon()
    {
        float previousDamage = weapons[currentIndex].damageBonus;
        currentIndex++;
        if (currentIndex == weapons.Count)
        {
            currentIndex = 0;
        }
        EquipWeapon(currentIndex);
        stats.damage = stats.damage - previousDamage + weapons[currentIndex].damageBonus;
    }
    
}
