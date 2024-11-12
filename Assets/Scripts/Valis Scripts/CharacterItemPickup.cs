using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using UnityEngine;

public class CharacterItemPickup : MonoBehaviour
{
    private PlayerStats stats;


    void Start()
    {
        stats = PlayerStats.GetPlayerStats(GetComponent<PlayerCharacter>().player_id);
    }
    
    // Item / Weapon pickup logic
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            stats.PickupItem(other.gameObject.GetComponent<ItemPickup>().getItemData());
                
            Destroy(other.gameObject);
        }
    }
}
