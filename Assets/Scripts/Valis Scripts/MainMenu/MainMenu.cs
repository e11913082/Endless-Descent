using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{   
    private Canvas settingsCanvas;
    private AudioSource audioSource;
    
    void Start()
    {
        settingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();
        settingsCanvas.enabled = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("EffectVolume", 1f);
    }

    void Update()
    {
        audioSource.volume = PlayerPrefs.GetFloat("EffectVolume", 1f);
    }
    
    public void PlayGame()
    {   
        SceneManager.LoadScene("Scenes/Matthias Playground/stage1.1");
        audioSource.Play();
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
        audioSource.Play();
    }
    
    public void QuitGame()
    {
        audioSource.Play();
        Debug.Log("Quit");
        Application.Quit();
    }
}
