using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamblingcanvasPersistence : MonoBehaviour
{
    private static GamblingcanvasPersistence instance;
    public GameObject fortuneWheel;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            WheelOfFortuneHandler wheelHandler = instance.fortuneWheel.GetComponent<WheelOfFortuneHandler>();
            wheelHandler.ResetSpin();
            Destroy(gameObject);
        }
        instance.gameObject.SetActive(false);
    }
}
