using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    
    public Weapon weapon;
    public TextMeshProUGUI textGUI;

    private void Start()
    {
        textGUI.gameObject.transform.parent.parent.gameObject.SetActive(false);
        textGUI.text = weapon.description;
    }
    
    
    public Weapon GetWeapon()
    {
        return weapon;
    }
    
    private void OnMouseEnter()
    {
        textGUI.gameObject.transform.parent.parent.position = new Vector3(transform.position.x, transform.position.y+1, 0);
        textGUI.gameObject.transform.parent.parent.gameObject.SetActive(true);
        textGUI.text = "Weapon:\n" + weapon.description;
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
