using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    
    public Weapon weapon;
    public TextMeshProUGUI textGUI;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = weapon.sprite;
        textGUI.gameObject.transform.parent.parent.gameObject.SetActive(false);
        textGUI.text = weapon.description;

        if (textGUI == null)
        {
            Debug.LogWarning("No text renderer");
        }

        
    }
    
    

    
    
    public void Initialize(Weapon weapon, TextMeshProUGUI textGUI)
    {
        this.weapon = weapon;
        this.textGUI = textGUI;

        // Set up visuals based on the weapon details
        spriteRenderer.sprite = weapon.sprite;
        textGUI.text = weapon.description;
        textGUI.gameObject.transform.parent.parent.gameObject.SetActive(false);
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
