using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using UnityEngine;

public class CharacterItemPickup : MonoBehaviour
{
    private PlayerStats stats;
    private PlayerCharacter player;


    void Start()
    {
        player = GetComponent<PlayerCharacter>();
        stats = PlayerStats.GetPlayerStats(player.player_id);
    }
    
    // Item pickup logic
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            stats.PickupItem(other.gameObject.GetComponent<ItemPickup>().getItemData());
                
            Destroy(other.gameObject);
        }
    }
}
