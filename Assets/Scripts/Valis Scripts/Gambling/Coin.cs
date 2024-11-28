using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float amount;

    private TextMeshProUGUI text;
    private GameObject canvas;
    
    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("/HoverCanvas");
        text = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Ground";
        spriteRenderer.sortingOrder = 5;
        text.gameObject.transform.parent.gameObject.SetActive(false);

        if (amount > 0)
        {
            text.text = amount.ToString("0.0") + "x \nLantern Oil";
        }
    }

    public void OnMouseEnter()
    {
        if (text != null)
        {
            text.gameObject.transform.parent.parent.position = new Vector3(transform.position.x, transform.position.y+1, 0);
            text.gameObject.transform.parent.gameObject.SetActive(true);
            text.text = amount.ToString("0.0") + "x \nLantern Oil";
        }
        else
        {
            Debug.LogWarning("No TextGUI assigned to Coin");
        }
    }

    public void OnMouseExit()
    {
        if (text != null)
        {
            text.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    public void OnDestroy()
    {
        if (text != null)
        {
            text.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
    
    
    
}
