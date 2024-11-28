using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{   
    private Canvas settingsCanvas;

    void Start()
    {
        settingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();
        settingsCanvas.enabled = false;
    }
    
    public void PlayGame()
    {   
        SceneManager.LoadScene("Scenes/Matthias Playground/stage1.1");
    }
    
    public void OpenSettings()
    {
        if (settingsCanvas.enabled == false)
        {
            settingsCanvas.enabled = true;
        }
        else
        {
            settingsCanvas.enabled = false;
        }
        
    }
    
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
