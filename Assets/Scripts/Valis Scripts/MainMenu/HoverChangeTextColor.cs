using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverChangeTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{   
    public TextMeshProUGUI text;
    public Color defaultColor;
    public Color hoverColor;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (text == null)
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }
    
        if (text != null)
        {
            text.color = defaultColor;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component is missing on the button.");
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = defaultColor;
    }
}
