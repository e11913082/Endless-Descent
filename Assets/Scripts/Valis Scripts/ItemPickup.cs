using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;
    private TextMeshProUGUI textGUI;
    private GameObject canvas;

    private void Start()
    {   
        canvas = GameObject.Find("/HoverCanvas");
        textGUI = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        
        textGUI.gameObject.transform.parent.gameObject.SetActive(false);
        if (itemData != null)
        {
            textGUI.text = itemData.itemName;
        }
        
    }

    public void Initialize(ItemData itemData)
    {   
        canvas = GameObject.Find("/HoverCanvas");
        textGUI = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        gameObject.layer = LayerMask.NameToLayer("Item");
        
        // setup sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemData.itemIcon;
        spriteRenderer.sortingLayerName = "Ground";
        spriteRenderer.sortingOrder = 5;
        
        // setup circle collider
        
        // setup hover text
        this.itemData = itemData;
        textGUI.text = itemData.itemName;
        textGUI.gameObject.transform.parent.gameObject.SetActive(false);
    }
    public ItemData getItemData()
    {
        return itemData;
    }

    private void OnMouseEnter()
    {   
        Debug.Log("Mouse Enter");
        if (textGUI != null)
        {
            textGUI.gameObject.transform.parent.parent.position = new Vector3(transform.position.x, transform.position.y+1, 0);
            textGUI.gameObject.transform.parent.gameObject.SetActive(true);
            textGUI.text = "Item:\n" + itemData.itemName;
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
