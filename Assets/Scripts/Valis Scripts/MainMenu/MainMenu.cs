using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{   
    private Canvas settingsCanvas;
    private Canvas introCanvas;

    void Start()
    {
        settingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();
        settingsCanvas.enabled = false;
        introCanvas = GameObject.Find("IntroCanvas").GetComponent<Canvas>();
    }
    
    public void PlayGame()
    {   
        SceneManager.LoadScene("stage1.2");
    }
    
    public void OpenSettings()
    {
        if (settingsCanvas.enabled == false)
        {   
            introCanvas.enabled = false;
            settingsCanvas.enabled = true;
        }
        else
        {   
            introCanvas.enabled = true;
            settingsCanvas.enabled = false;
        }
        
    }
    
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
