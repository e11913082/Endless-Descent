using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DynamicBackground : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public RectTransform backgroundRectTransform;

    void Update()
    {
        if (textComponent != null && backgroundRectTransform != null)
        {
            // Adjust the background size to match the text size with padding
            Vector2 textSize = textComponent.GetPreferredValues();
            textSize *= 0.25f;
            backgroundRectTransform.sizeDelta = new Vector2(textSize.x + 2, textSize.y + 4); // Add padding
        }
    }
}
