using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon/Weapon")]
public class Weapon : ScriptableObject
{
    public string name;
    public string description;
    public float damageBonus;
    public int type; // 0: distance weapon, 1: melee weapon
    public float delay;
    public GameObject projectilePrefab;
    
    // for distance weapons only ATM
    [Header("Only for distance weapons")]
    public float projectileOffsetX;
    public float projectileOffsetY; 
}
