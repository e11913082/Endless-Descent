using System;
using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;

public class CharacterWeaponInventory : MonoBehaviour
{
    private LayerMask playerLayer;

    public List<Weapon> weapons;
    public Weapon equippedWeapon;
    private int currentIndex;
    private PlayerStats stats;
    public int maxInventorySize;

    public GameObject droppedWeaponPrefab;


    private void Awake()
    {
        weapons = new List<Weapon>();
        currentIndex = 0;
    }

    private void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
        stats = PlayerStats.GetPlayerStats(CharacterIdGenerator.GetCharacterId(gameObject, 0));
        if (equippedWeapon != null)
        {
            stats.damage += equippedWeapon.damageBonus;
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening("WeaponslotClick", OnWeaponslotClick);
    }

    private void OnDisable()
    {
        EventManager.StopListening("WeaponslotClick", OnWeaponslotClick);
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
        //EventManager.TriggerEvent("InventoryChange");
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
        EventManager.TriggerEvent("WeaponSwitch");
    }

    public void SwitchWeapon(int slotId)
    {
        if (slotId >= weapons.Count) //case: slot is empty 
        {
            Debug.Log("Clicked weaponslot is empty");
            return;
        }
        if(currentIndex == slotId) //case: slot already equipped
        {
            Debug.Log("Clicked weaponslot is already chosen");
            return;
        }

        float previousDamage = weapons[currentIndex].damageBonus;
        currentIndex = slotId;
        EquipWeapon(currentIndex);
        stats.damage = stats.damage - previousDamage + weapons[currentIndex].damageBonus;
        EventManager.TriggerEvent("WeaponSwitch");
    }

    public void DropWeapon()
    {
        weapons.Remove(equippedWeapon);
        WeaponPickup wp = Instantiate(droppedWeaponPrefab, transform.position, Quaternion.identity).GetComponent<WeaponPickup>();
        wp.InitializeDropped(equippedWeapon);

        if (weapons.Count > 0)
        {
            float previousDamage = equippedWeapon.damageBonus;
            equippedWeapon = weapons[0];
            stats.damage = stats.damage - previousDamage + equippedWeapon.damageBonus;
        }
        else
        {
            stats.damage -= equippedWeapon.damageBonus;
            equippedWeapon = null;
        }
        EventManager.TriggerEvent("InventoryChange");
    }

    public void OnWeaponslotClick()
    {
        if ((playerLayer.value & (1 << gameObject.layer)) == 0) //case: gameobject is not the maincharacter
        {
            return;
        }
        int slotId = InventoryButtonListener.lastClicked;
        SwitchWeapon(slotId);
    }
}
