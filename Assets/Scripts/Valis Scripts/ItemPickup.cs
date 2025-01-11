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

    private void Start()
    {   
        canvas = GameObject.Find("/HoverCanvas");
        textGUI = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        textGUI.gameObject.transform.parent.gameObject.SetActive(false);
        if (itemData != null)
        {
            textGUI.text = itemData.itemName;
        }
        
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

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (chestItem)
            {
                PlayerStats stats = GameObject.Find("/Main Character").GetComponent<PlayerCharacter>().GetStats();
                stats.PickupItem(itemData);
                Cursor.SetCursor(defaultCursor, Vector2.zero, cursorMode);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Item is no chestItem");
            }
        }
    }


    private void OnMouseEnter()
    {   
        Debug.Log("Mouse Enter");
        if (textGUI != null)
        {   
            Cursor.SetCursor(cursor, Vector2.zero, cursorMode);
            
            textGUI.gameObject.transform.parent.parent.position = new Vector3(transform.position.x, transform.position.y+1, 0);
            textGUI.gameObject.transform.parent.gameObject.SetActive(true);
            if (chestItem)
            {
                textGUI.text = "Item:\n" + itemData.itemName + " \n >(Right click to pick up)<";
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
