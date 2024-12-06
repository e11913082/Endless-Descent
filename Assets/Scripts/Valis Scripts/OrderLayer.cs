using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderLayer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Transform characterTransform;

    public float layerOffset;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterTransform = GameObject.Find("/Main Character").transform;
    }


    private void Update()
    {
        if (characterTransform.position.y > transform.position.y + layerOffset)
        {
            spriteRenderer.sortingLayerName = "Props";
            spriteRenderer.sortingOrder = 1;
        }
        else
        {
            spriteRenderer.sortingLayerName = "Ground";
            spriteRenderer.sortingOrder = 2;
        }
    }
}
