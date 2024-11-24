using EndlessDescent;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class inventoryRefresher : MonoBehaviour
{
    public int paddingBetweenSlots = 20; //padding between slots


    private CharacterWeaponInventory playerInv;
    private GameObject baseInvSlot;
    private List<GameObject> slots;

    private Image curWeaponOverlay;
    private bool initDone = false; //needed to avoid refresh on playercreation

    void Start()
    {
        playerInv = PlayerCharacter.Get(Hud.GetPlayerId()).GetComponent<CharacterWeaponInventory>();
        baseInvSlot = transform.GetChild(0).gameObject;
        slots = new List<GameObject>() {baseInvSlot};

        curWeaponOverlay = gameObject.transform.Find("CurWeaponOverlay").GetComponent<Image>();
        curWeaponOverlay.color = new Color(curWeaponOverlay.color.r, curWeaponOverlay.color.g, curWeaponOverlay.color.b, 0);

        Vector2 curPos = baseInvSlot.transform.position;
        float slotWidth = baseInvSlot.GetComponent<RectTransform>().rect.width;

        for (int i = 1; i < playerInv.maxInventorySize; i++)
        {
            GameObject nextSlot = Instantiate(baseInvSlot);
            nextSlot.transform.SetParent(gameObject.transform, false);
            curPos.x = curPos.x - paddingBetweenSlots - slotWidth/2;
            nextSlot.transform.position = curPos;
            
            slots.Insert(0,nextSlot);
            Refresh();
        }
        initDone = true;
    }

    void Update()
    {
    }

    private void OnEnable()
    {
        EventManager.StartListening("InventoryChange", Refresh);
        EventManager.StartListening("WeaponSwitch", RefreshOverlay);
    }

    private void OnDisable()
    {
        EventManager.StopListening("InventoryChange", Refresh);
        EventManager.StopListening("WeaponSwitch", RefreshOverlay);
    }

    void Refresh()
    {
        if(initDone)
        {
            curWeaponOverlay.color = new Color(curWeaponOverlay.color.r, curWeaponOverlay.color.g, curWeaponOverlay.color.b, 0);//in case no weapon is there anymore the currentweapon-icon gets blended out
            int weaponIndex = 0;

            foreach (GameObject slot in slots)
            {

                Image slotIcon = slot.transform.Find("WeaponIcon").GetComponent<Image>();
                if (playerInv.weapons.Count > weaponIndex) //case weapon in slot
                {
                    slotIcon.sprite = playerInv.weapons[weaponIndex].sprite;
                    slotIcon.color = new Color(slotIcon.color.r, slotIcon.color.g, slotIcon.color.b, 1);
                }
                else
                {
                    slotIcon.color = new Color(slotIcon.color.r, slotIcon.color.g, slotIcon.color.b, 0);
                }
                weaponIndex++;
            }
            RefreshOverlay();
        }
    }

    void RefreshOverlay()
    {
        
        for(int i = 0; i < playerInv.weapons.Count; i++)
        {
            if (playerInv.weapons[i] == playerInv.equippedWeapon)
            {
                curWeaponOverlay.transform.position = slots[i].transform.position;
                curWeaponOverlay.color = new Color(curWeaponOverlay.color.r, curWeaponOverlay.color.g, curWeaponOverlay.color.b, 1);
                return;
            }
        }
    }
}
