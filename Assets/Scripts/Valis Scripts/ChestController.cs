using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public List<ItemData> commonItems; // List of common items
    public List<ItemData> rareItems;   // List of rare items

    [Range(0, 1)] public float rareDropChance = 0.2f; // 20% chance for a rare item
    public GameObject itemPrefab;
    public Sprite openChestSprite;

    public ItemData GetRandomItem()
    {
        // Determine if the drop is rare or common
        bool isRare = Random.value < rareDropChance;

        // Select an item from the corresponding pool
        if (isRare && rareItems.Count > 0)
        {
            return rareItems[Random.Range(0, rareItems.Count)];
        }
        else if (commonItems.Count > 0)
        {
            return commonItems[Random.Range(0, commonItems.Count)];
        }

        // Fallback if no items are available
        Debug.LogWarning("No items available in the chest!");
        return null;
    }

    public void OpenChest()
    {
        ItemData droppedItem = GetRandomItem();
        if (droppedItem != null)
        {
            Debug.Log($"You received: {droppedItem.itemName}");
            ItemPickup pickup = Instantiate(itemPrefab, new Vector3(transform.position.x, transform.position.y-0.5f, transform.position.z), Quaternion.identity).GetComponent<ItemPickup>();
            pickup.Initialize(droppedItem);
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = openChestSprite;
        }
        else
        {
            Debug.Log("The chest was empty!");
        }
    }
}
