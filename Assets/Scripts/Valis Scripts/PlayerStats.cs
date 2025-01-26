using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int player_id;
    
    // Health optional
    public float maxHealth = 1.0f;
    public float healthRegen = 0.5f;
    public float currentHealth = 50f;
    
    public float moveSpeed = 2f;
    public float damage = 5f;
    public float attackRange = 1.5f;
    public float attackSpeed = 1f;
    public float dashCoolDown = 0.5f;
    public float attackAnimationSpeed = 3;
    public float defense = 20f;
    
    public float fearIncrease= 1f;
    public float fearDecrease = 0.5f;
    public float currentFear = 0f;
    public float maxFear = 100f;

    public float coins = 50f;
    
    private static Dictionary<int, PlayerStats> stats = new Dictionary<int, PlayerStats>();
    public List<ItemData> equippedItems = new List<ItemData>();

    public AnimatorController armorController;
    
    // Recently picked up item for displaying statchanges
    [HideInInspector]
    public ItemData lastItem;

    void Awake()
    {
        player_id = CharacterIdGenerator.GetCharacterId(gameObject, 1);
        stats[player_id] = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
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
        if (newItem != null)
        {
            equippedItems.Add(newItem);
            UpdateStats(newItem);
            lastItem = newItem;
            EventManager.TriggerEvent("ItemPickup");
            Debug.Log("Itempickup event triggered");
            if (newItem.isArmor)
            {
                GameObject.Find("/Main Character").GetComponent<Animator>().runtimeAnimatorController = armorController;
            }
        }
        else
        {
            Debug.Log("Item is null");
        }
        
    }
    
    public void UpdateStats(ItemData newItem)
    {   
        if (equippedItems.Count <= 0) return;
        // percentage based
        var attributes = newItem.GetAttributesDictionary();
        if (attributes.ContainsKey("damage"))
        {
            damage = Math.Max(damage * (attributes["damage"] / 100 + 1), 1);
        }

        if (attributes.ContainsKey("maxFear"))
        {
            maxFear = Math.Max(maxFear * (attributes["maxFear"] / 100 + 1), 1);
        }

        if (attributes.ContainsKey("moveSpeed"))
        {
            moveSpeed = Math.Max(moveSpeed * (attributes["moveSpeed"] / 100 + 1), 1);
        }

        if (attributes.ContainsKey("range"))
        {
            attackRange = Math.Max(attackRange * (attributes["range"] / 100 + 1), 1);
        }

        if (attributes.ContainsKey("fearIncrease"))
        {
            fearIncrease = Math.Max(fearIncrease * (attributes["fearIncrease"] / 100 + 1), 0);
        }

        if (attributes.ContainsKey("fearDecrease"))
        {
            fearDecrease = Math.Max(fearDecrease * (attributes["fearDecrease"] / 100 + 1), 0);
        }

        if (attributes.ContainsKey("attackSpeed"))
        {
            attackSpeed = (float) Math.Max(attackSpeed * (attributes["attackSpeed"] / 100 + 1), 0.5);
        }

        if (attributes.ContainsKey("defense"))
        {
            defense = Math.Max(defense * (attributes["defense"] / 100 + 1), 0);
        }
        
    }

    public void AddCoins(float amount)
    {
        coins += amount;
    }

    public float GetCoins()
    {
        return coins;
    }

    public void RemoveCoins(float amount)
    {
        coins = Math.Max(coins - amount, 0);
    }
    
    // Gambling Heaven

    public String WheelOfFortune(int segmentIndex, ItemData rewardItem)
    {
        
        if (rewardItem != null)
        {
            PickupItem(rewardItem);
        }
        
        switch (segmentIndex)
        {
            case 0:
            case 2:
            case 4:
            case 6:
                return "You won a common Item: " + rewardItem.itemName;
            case 3:
            case 7:
                coins = Mathf.Max(coins - 10, 0);
                if (coins == 0)
                {
                    return "You lose the rest of your lantern oil!";
                }
                return "You lose 10 lantern oil!";
            case 1:
                coins += 20;
                return "You won a bottle of lantern oil!";
            case 5:
                return "You won a rare item: " + rewardItem.itemName;
            default:
                return "MISSING SEGMENT!";
        }

        
    }
    
    
    public void resetStats()
    {
        maxHealth = 100f;
        healthRegen = 0.5f;
        currentHealth = maxHealth;
        moveSpeed = 3f;
        damage = 5f;
        attackRange = 1.3f;
        fearIncrease = 1f;
        fearDecrease = 1f;
        currentFear = 0f;
        maxFear = 100f;
        
        equippedItems.Clear();
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
        set => currentFear = Mathf.Clamp(value, 0, maxFear); // Ensure currentFear is between 0 and maxFear }
    }

    // Getter and Setter for MaxFear
    public float MaxFear
    {
        get => maxFear;
        set => maxFear = Mathf.Max(0, value); // Ensure maxFear is never set below 0
    }
}

