using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderLayer : MonoBehaviour
{
    private SpriteRenderer spriteRendererUpper;
    private SpriteRenderer spriteRendererLower;
    

    private void Start()
    {
        Transform upper = transform.Find("Upper");
        Transform lower = transform.Find("Lower");
        

        if (upper != null)
        {
            spriteRendererUpper = upper.GetComponent<SpriteRenderer>();
        }

        if (lower != null)
        {
            spriteRendererLower = lower.GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteRendererUpper != null)
        {
            spriteRendererUpper.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
        if (spriteRendererLower != null)
        {
            spriteRendererLower.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
    }
}
