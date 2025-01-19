using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{   
    private Canvas settingsCanvas;
    private Canvas introCanvas;

    public Texture2D cursor;
    private CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        if (cursor != null)
        {
            Cursor.SetCursor(cursor, Vector2.zero, cursorMode);
        }
        
        settingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();
        settingsCanvas.enabled = false;
        introCanvas = GameObject.Find("IntroCanvas").GetComponent<Canvas>();
    }
    
    public void PlayGame()
    {   
        Time.timeScale = 1f;
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
