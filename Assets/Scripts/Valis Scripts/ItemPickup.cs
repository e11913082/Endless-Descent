using System;
using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;
    public TextMeshProUGUI textGUI;
    public Texture2D defaultCursor;
    public Texture2D cursor;
    private CursorMode cursorMode = CursorMode.Auto;
    private GameObject canvas;

    public bool chestItem = false;
    
    
    // pickup "animation"
    private Transform playerTransform;
    private bool isBeingCollected = false;
    private Vector3 originalScale;

    [SerializeField] private float hopHeight = 1.0f;
    [SerializeField] private float animationDuration = 0.5f;
    private void Start()
    {   
        originalScale = transform.localScale;
        canvas = GameObject.Find("/HoverCanvas");
        textGUI = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        textGUI.gameObject.transform.parent.gameObject.SetActive(false);
        if (itemData != null)
        {
            textGUI.text = itemData.itemName;
        }
        
    }

    private void StartPickupAnimation()
    {
        if (!isBeingCollected)
        {
            GameObject player = GameObject.Find("/Main Character");
            if (player != null)
            {
                playerTransform = player.transform;
                StartCoroutine(PickupAnimation());
            }
            else
            {
                Debug.LogWarning("Player is not found");
            }
        }
    }

    private IEnumerator PickupAnimation()
    {
        isBeingCollected = true;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 peakPosition = startPosition + Vector3.up * hopHeight;
        Debug.Log("Pickup Animation");
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            
            float normalizedTime = elapsedTime / animationDuration;

            if (normalizedTime <= 0.5f)
            {
                transform.position = Vector3.Lerp(startPosition, peakPosition, normalizedTime * 2f);
            }
            else
            {
                Vector3 endPosition = playerTransform.position;
                transform.position = Vector3.Lerp(peakPosition, endPosition, (normalizedTime - 0.5f) * 2f);
            }
            
            transform.localScale = Vector3.Lerp(originalScale, originalScale / 2, normalizedTime);
            
            yield return null;
        }
        
        transform.position = playerTransform.position;
        Destroy(gameObject);
    }
    

    public bool isChestItem()
    {
        return chestItem;
    }

    public void Initialize(ItemData itemData)
    {   
        canvas = GameObject.Find("/HoverCanvas");
        textGUI = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        gameObject.layer = LayerMask.NameToLayer("Item");
        
        // setup sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemData.itemIcon;
        AutoOrderLayer orderLayer = gameObject.GetComponent<AutoOrderLayer>();
        orderLayer.offset = -3f;
        // different pickupLogic if item is from chest
        chestItem = true;
        
        // setup hover text
        this.itemData = itemData;
        textGUI.text = itemData.itemName;
        textGUI.gameObject.transform.parent.gameObject.SetActive(false);
    }
    public ItemData getItemData()
    {
        return itemData;
    }

    public void CollectItem()
    {
        StartPickupAnimation();
        GetComponent<CircleCollider2D>().enabled = false;        
        PlayerStats stats = GameObject.Find("/Main Character").GetComponent<PlayerCharacter>().GetStats();
        stats.PickupItem(itemData);
        Cursor.SetCursor(defaultCursor, Vector2.zero, cursorMode);
    }
    

    private void OnMouseEnter()
    {   
        Debug.Log("Mouse Enter");
        if (textGUI != null)
        {   
            Cursor.SetCursor(cursor, Vector2.zero, cursorMode);
            
            textGUI.gameObject.transform.parent.parent.position = new Vector3(transform.position.x, transform.position.y + 1.5f, 0);
            textGUI.gameObject.transform.parent.gameObject.SetActive(true);
            if (chestItem)
            {
                textGUI.text = "Item:\n" + itemData.itemName + " \n\n >(Interact to pick up)<";
            }
            else
            {
               textGUI.text = "Item:\n" + itemData.itemName; 
            }
            
        }
        else
        {
            Debug.LogWarning("No TextGUI assigned to ItemPickup");
        }
        
    }

    private void OnMouseExit()
    {
        if (textGUI != null)
        {   
            Cursor.SetCursor(defaultCursor, Vector2.zero, cursorMode);
            
            textGUI.gameObject.transform.parent.gameObject.SetActive(false);
        }
        
    }
    private void OnDestroy()
    {
        if (textGUI != null)
        {
            textGUI.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}
