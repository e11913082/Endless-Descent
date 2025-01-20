using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hintcanvaspersistence : MonoBehaviour
{
    private static Hintcanvaspersistence instance;
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
