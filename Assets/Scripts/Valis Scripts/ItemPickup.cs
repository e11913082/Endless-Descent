using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;
    public TextMeshProUGUI textGUI;

    private void Start()
    {
        textGUI.gameObject.transform.parent.parent.gameObject.SetActive(false);
        textGUI.text = itemData.itemDescription;
    }

    public ItemData getItemData()
    {
        return itemData;
    }

    private void OnMouseEnter()
    {
        textGUI.gameObject.transform.parent.parent.position = new Vector3(transform.position.x, transform.position.y+1, 0);
        textGUI.gameObject.transform.parent.parent.gameObject.SetActive(true);
        textGUI.text = "Item:\n" + itemData.itemDescription;
    }

    private void OnMouseExit()
    {
        textGUI.gameObject.transform.parent.parent.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        textGUI.gameObject.transform.parent.parent.gameObject.SetActive(false);
    }
}
