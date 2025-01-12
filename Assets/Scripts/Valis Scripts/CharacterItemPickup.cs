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
        stats = PlayerStats.GetPlayerStats(CharacterIdGenerator.GetCharacterId(gameObject, 0));
    }
    
    // Item pickup logic
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {   
            if (other.CompareTag("Coin"))
            {
                stats.AddCoins(other.gameObject.GetComponent<Coin>().amount);   
            }
            else if (other.CompareTag("Armor"))
            {
                gameObject.GetComponent<Animator>().runtimeAnimatorController = other.gameObject.GetComponent<Armor>().armorAnimator;
                stats.PickupItem(other.gameObject.GetComponent<ItemPickup>().getItemData());
            }
            else
            {   
                ItemPickup pickup = other.gameObject.GetComponent<ItemPickup>();
                if (!pickup.isChestItem())
                {
                    stats.PickupItem(other.gameObject.GetComponent<ItemPickup>().getItemData());
                }
                else
                {   
                    // Do not destroy the item
                    return;
                }
                
            }
            Destroy(other.gameObject);
        }
    }
}
