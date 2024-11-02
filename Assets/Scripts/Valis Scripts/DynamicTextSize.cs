using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DynamicTextSize : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    private RectTransform rectTransform;

    void Start()
    {   
        textComponent = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        AdjustToTextSize();
    }

    void Update()
    {
        AdjustToTextSize(); // Call this in Update if the text is frequently changing
    }

    private void AdjustToTextSize()
    {
        // Get the preferred width and height of the text
        float preferredWidth = textComponent.preferredWidth;
        float preferredHeight = textComponent.preferredHeight;

        // Set the RectTransform's sizeDelta to match the preferred width and height
        rectTransform.sizeDelta = new Vector2(preferredWidth, preferredHeight);
    }
}
