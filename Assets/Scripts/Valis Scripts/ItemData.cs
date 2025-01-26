using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemRarity { Common, Rare}

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public ItemRarity rarity;
    public bool isArmor;
    
    public List<StatModifiers> statModifiers = new List<StatModifiers>();

    public Dictionary<string, float> GetAttributesDictionary()
    {
        Dictionary<string, float> attributes = new Dictionary<string, float>();
        foreach (StatModifiers statModifier in statModifiers)
        {
            attributes[statModifier.statName] = statModifier.value;
        }
        return attributes;
    }
}
