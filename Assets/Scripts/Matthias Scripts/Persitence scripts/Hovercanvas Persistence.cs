using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovercanvasPersistence : MonoBehaviour
{
    private static HovercanvasPersistence instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
