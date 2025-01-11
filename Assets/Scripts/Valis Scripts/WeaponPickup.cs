using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    
    public Weapon weapon;
    private TextMeshProUGUI textGUI;
    public Texture2D defaultCursor;
    public Texture2D cursor;
    private CursorMode cursorMode = CursorMode.Auto;
    private GameObject canvas;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        canvas = GameObject.Find("/HoverCanvas");
        textGUI = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        if (textGUI == null)
        {
            Debug.LogWarning("No text renderer");
        }
        
        if (weapon == null)
        {
            Debug.LogWarning("no weapon assigned");
        }
        else
        {   
            spriteRenderer.sprite = weapon.sprite;
            textGUI.gameObject.transform.parent.gameObject.SetActive(false);
            textGUI.text = weapon.description;
        }
    }
    
    

    
    // called when the weapon is dropped for initialization of text and sprite
    public void InitializeDropped(Weapon weapon)
    {
        this.weapon = weapon;
        canvas = GameObject.Find("/HoverCanvas");
        textGUI = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        gameObject.layer = LayerMask.NameToLayer("Weapon");
        
        
        // initialize sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Ground";
        spriteRenderer.sortingOrder = 5;
        // Set up visuals based on the weapon details
        spriteRenderer.sprite = weapon.sprite;
        textGUI.text = weapon.description;
        textGUI.gameObject.transform.parent.gameObject.SetActive(false);
    }
    
    
    public Weapon GetWeapon()
    {
        return weapon;
    }
    
    private void OnMouseEnter()
    {
        if (textGUI != null)
        {   
            Cursor.SetCursor(cursor, Vector2.zero, cursorMode);
            
            textGUI.gameObject.transform.parent.parent.position = new Vector3(transform.position.x, transform.position.y+1, 0);
            textGUI.gameObject.transform.parent.gameObject.SetActive(true);
            textGUI.text = "Weapon:\n" + weapon.description;
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
