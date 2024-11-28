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
            currentKeyText.color = defaultColor;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component is missing on the button.");
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
        currentKeyText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = defaultColor;
        currentKeyText.color = defaultColor;
    }
}