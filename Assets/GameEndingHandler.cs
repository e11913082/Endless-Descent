using System;
using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public string endSentence;
    public string eventName;
    public TextMeshProUGUI endText;
    public GameObject activationScreen;
    private PlayerCharacter player;
    
    // Start is called before the first frame update
    void Start()
    {
        activationScreen.SetActive(false);
    }

    public void OpenActivationScreen()
    {
        StartCoroutine(ActivationScreen());
        Time.timeScale = 0f;
    }
    
    private IEnumerator ActivationScreen()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        activationScreen.SetActive(true);
        StartCoroutine(TypeSentence(endSentence));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        EventManager.StartListening(eventName,OpenActivationScreen);
    }

    private void OnDisable()
    {
        EventManager.StopListening(eventName,OpenActivationScreen);
    }
    
    IEnumerator TypeSentence(string sentence)
    {
        string[] array = sentence.Split(' ');
        endText.text = array[0];
        for( int i = 1 ; i < array.Length ; ++ i)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            endText.text += " " + array[i];
        }
    }
    
    public void ReturnToMainMenu()
    {
        StopAllCoroutines();
        Destroy(GameObject.Find("/Hud V2"));
        Destroy(GameObject.Find("/BackgroundMusic"));
        Destroy(GameObject.Find("/Loop Entrance"));
        GameObject playerObj = GameObject.Find("/Main Character");
        if (playerObj != null) //Case: Win
        {
            Destroy(playerObj);
        }
        SceneManager.LoadScene("MainMenu");
    }
}
