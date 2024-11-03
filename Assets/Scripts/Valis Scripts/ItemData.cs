using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
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
