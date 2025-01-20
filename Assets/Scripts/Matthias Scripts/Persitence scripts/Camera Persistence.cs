using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPersistence : MonoBehaviour
{
   private static CameraPersistence instance;
   public void Awake()
    {
        if(instance == null)
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
