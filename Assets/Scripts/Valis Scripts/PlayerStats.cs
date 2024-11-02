using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int player_id;
    
    // Health optional
    public float maxHealth = 100f;
    public float healthRegen = 0.5f;
    public float currentHealth = 50f;
    
    public float moveSpeed = 2f;
    public float damage = 20f;
    public float attackRange = 1.5f;
    public float attackSpeed = 1f;
    
    public float fearIncrease= 1f;
    public float fearDecrease = 0.5f;
    public float currentFear = 0f;
    public float maxFear = 100f;
    
    private static Dictionary<int, PlayerStats> stats = new Dictionary<int, PlayerStats>();
    public List<ItemData> equippedItems = new List<ItemData>();

    void Awake()
    {
        stats[player_id] = this;
    }

    private void OnDestroy()
    {
        stats.Remove(player_id);    
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public PlayerStats CreatePlayerStats(int player_id)
    {
        if (!stats.ContainsKey(player_id))
        {
            stats[player_id] = this;
            return this;
        }

        return null;
    }
    
    
    
    public void PickupItem(ItemData newItem)
    {
        equippedItems.Add(newItem);
        UpdateStats(newItem);
    }
    
    public void UpdateStats(ItemData newItem)
    {   
        if (equippedItems.Count <= 0) return;
        
        var attributes = newItem.GetAttributesDictionary();
        if (attributes.ContainsKey("damage"))
        {
            damage += attributes["damage"];
        }

        if (attributes.ContainsKey("maxHealth"))
        {
            maxHealth += attributes["maxHealth"];
        }

        if (attributes.ContainsKey("moveSpeed"))
        {
            moveSpeed += attributes["moveSpeed"];
        }

        if (attributes.ContainsKey("range"))
        {
            attackRange += attributes["range"];
        }

        if (attributes.ContainsKey("healthRegen"))
        {
            healthRegen += attributes["healthRegen"];
        }

        if (attributes.ContainsKey("fearIncrease"))
        {
            fearIncrease += attributes["fearIncrease"];
        }

        if (attributes.ContainsKey("fearDecrease"))
        {
            fearDecrease += attributes["fearDecrease"];
        }
        
    }
    
    public void resetStats()
    {
        maxHealth = 100f;
        healthRegen = 0.5f;
        currentHealth = maxHealth;
        moveSpeed = 2f;
        damage = 1f;
        attackRange = 1.5f;
        fearIncrease = 1f;
        fearDecrease = 0.5f;
        currentFear = 0f;
        maxFear = 100f;
    }
    
    public static PlayerStats GetPlayerStats(int player_id)
    {
        foreach (PlayerStats stat in GetAll())
        {
            if (stat.player_id == player_id)
            {
                return stat;
            }
        }

        return null;
    }

    public static PlayerStats[] GetAll()
    {
        PlayerStats[] list = new PlayerStats[stats.Count];
        stats.Values.CopyTo(list, 0);
        return list;
    }
    
    
    // Getters and Setters
    // Getter and Setter for MaxHealth
    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = Mathf.Max(0, value); // Ensure maxHealth is never set below 0
    }

    // Getter and Setter for HealthRegen
    public float HealthRegen
    {
        get => healthRegen;
        set => healthRegen = Mathf.Max(0, value); // Ensure healthRegen is never set below 0
    }

    // Getter and Setter for CurrentHealth
    public float CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, maxHealth); // Ensure currentHealth is between 0 and maxHealth
    }

    // Getter and Setter for MoveSpeed
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0, value); // Ensure moveSpeed is never set below 0
    }

    // Getter and Setter for Damage
    public float Damage
    {
        get => damage;
        set => damage = Mathf.Max(0, value); // Ensure damage is never set below 0
    }

    // Getter and Setter for AttackRange
    public float AttackRange
    {
        get => attackRange;
        set => attackRange = Mathf.Max(0, value); // Ensure attackRange is never set below 0
    }

    // Getter and Setter for FearIncrease
    public float FearIncrease
    {
        get => fearIncrease;
        set => fearIncrease = Mathf.Max(0, value); // Ensure fearIncrease is never set below 0
    }

    // Getter and Setter for FearDecrease
    public float FearDecrease
    {
        get => fearDecrease;
        set => fearDecrease = Mathf.Max(0, value); // Ensure fearDecrease is never set below 0
    }

    // Getter and Setter for CurrentFear
    public float CurrentFear
    {
        get => currentFear;
        set => currentFear = Mathf.Clamp(value, 0, maxFear); // Ensure currentFear is between 0 and maxFear
    }

    // Getter and Setter for MaxFear
    public float MaxFear
    {
        get => maxFear;
        set => maxFear = Mathf.Max(0, value); // Ensure maxFear is never set below 0
    }
}

