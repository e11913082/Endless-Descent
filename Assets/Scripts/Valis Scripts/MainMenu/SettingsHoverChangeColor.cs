using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsHoverChangeColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI currentKeyText;
    public Color defaultColor;
    public Color hoverColor;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (buttonText == null)
        {
            buttonText = GetComponentInParent<TextMeshProUGUI>();
        }
    
        if (buttonText != null)
        {
            buttonText.color = defaultColor;
            if (currentKeyText != null)
            {
               currentKeyText.color = defaultColor; 
            }
            
        }
           
        else
        {
            Debug.LogError("TextMeshProUGUI component is missing on the button.");
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
        if (currentKeyText != null)
        {   
            currentKeyText.color = hoverColor; 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = defaultColor;
        if (currentKeyText != null)
        {
            if (currentKeyText.text.Equals("None"))
            {
                currentKeyText.color = new Color(1f, 0.16f, 0.16f);
            }
            else
            {
                currentKeyText.color = defaultColor; 
            }
            
        }
    }

    private void OnEnable()
    {
        if (buttonText != null)
        {
            buttonText.color = defaultColor;
        }
        
    }
}
