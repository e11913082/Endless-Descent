using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public List<ItemData> commonItems; // List of common items
    public List<ItemData> rareItems;  // List of rare items    
    public ItemData armor;
    [Header("Tick to drop only armor")]
    public bool useArmor = false;
    
    private AudioSource audioSource;
    
    [Range(0, 1)] public float rareDropChance = 0.2f; // 20% chance for a rare item
    public GameObject itemPrefab;
    public Sprite openChestSprite;
    
    public float hoverAmplitude = 0.1f;
    public float hoverFrequency = 0.2f;
    public bool isOpen;
    private bool itemCollectable = false;
    private float itemStartPosition;
    
    private ItemPickup itemPickup;
    private GameObject item;
    void Start()
    {   
        audioSource = GetComponent<AudioSource>();
        isOpen = false;
    }
    
    
    public ItemData GetRandomItem()
    {
        if (useArmor)
        {
            return armor;
        }
        else
        {
            bool isRare = Random.value < rareDropChance;

            if (isRare && rareItems.Count > 0)
            {
                return rareItems[Random.Range(0, rareItems.Count)];
            }
            else if (commonItems.Count > 0)
            {
                return commonItems[Random.Range(0, commonItems.Count)];
            }
             
        }
        // Fallback
        Debug.LogWarning("No items available in the chest!");
        return null;
    }

    public void OpenChest()
    {   
        isOpen = true;
        
        ItemData droppedItem = GetRandomItem();
        if (droppedItem != null)
        {   
            audioSource.Play();
            Debug.Log($"You received: {droppedItem.itemName}");
            item = Instantiate(itemPrefab, new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z), Quaternion.identity);
            itemPickup = item.GetComponent<ItemPickup>();
            itemPickup.Initialize(droppedItem);
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = openChestSprite;
            GetComponent<CircleCollider2D>().radius = 1.75f;
            itemStartPosition = item.transform.position.y;
            itemCollectable = true;
            if (droppedItem.isArmor)
            {
                item.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            }
        }
        else
        {
            Debug.Log("The chest was empty!");
        }
    }

    void Update()
    {
        if (itemCollectable)
        {
            float time = Time.time;
            float hoverOffset = Mathf.Sin(time * hoverFrequency * 2 * Mathf.PI) * hoverAmplitude;
            item.transform.position = new Vector3(item.transform.position.x, itemStartPosition + hoverOffset, item.transform.position.z); ;
        }
    }

    public void ReceiveItem()
    {
        if (isOpen && itemCollectable)
        {
            itemCollectable = false;
            itemPickup.CollectItem();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
    }
}
