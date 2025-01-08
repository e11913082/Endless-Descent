using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EndlessDescent;

public class InventoryButtonListener : MonoBehaviour
{
    private static int idCounter = -1;
    public static int lastClicked;

    public int index;
    private Button inventorySlot;

    public void Start()
    {
        if(idCounter == -1) //indices get assigned reversed to be in sync with the weaponinventory of the player
        {
            idCounter = PlayerCharacter.Get(Hud.GetPlayerId()).GetComponent<CharacterWeaponInventory>().maxInventorySize-1;
        }
        index = idCounter;
        idCounter--;
        inventorySlot = GetComponent<Button>();
        inventorySlot.onClick.AddListener(OnButtonClick);
    }
    public void OnButtonClick()
    {
        lastClicked = index;
        EventManager.TriggerEvent("WeaponslotClick");
    }
}
