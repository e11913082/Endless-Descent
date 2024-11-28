using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBackgroundHandler : MonoBehaviour
{
    public GameObject darkBackground;
    public GameObject canvas;


    public void EnableDarkBackground()
    {
        darkBackground.SetActive(true);
        canvas.SetActive(true);
    }

    public void DisableDarkBackground()
    {
        darkBackground.SetActive(false);
        canvas.SetActive(false);
    }
}
