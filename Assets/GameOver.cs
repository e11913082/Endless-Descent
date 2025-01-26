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
    public TextMeshProUGUI endText;
    public GameObject gameOverScreen;
    private PlayerCharacter player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("/Main Character").GetComponent<PlayerCharacter>();
        player.SetGameOverScreen(gameObject.transform.parent.gameObject.gameObject, this);
        gameOverScreen.SetActive(false);
    }

    public void OpenGameOverScreen()
    {
        StartCoroutine(GameOverScreen());
    }
    
    private IEnumerator GameOverScreen()
    {
        yield return new WaitForSeconds(1.5f);
        gameOverScreen.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(TypeSentence(endSentence));
    }
    
    IEnumerator TypeSentence(string sentence)
    {
        string[] array = sentence.Split(' ');
        endText.text = array[0];
        for( int i = 1 ; i < array.Length ; ++ i)
        {
            yield return new WaitForSeconds(0.1f);
            endText.text += " " + array[i];
        }
    }
    
    public void ReturnToMainMenu()
    {
        StopAllCoroutines();
        Destroy(GameObject.Find("/Hud V2"));
        Destroy(GameObject.Find("/BackgroundMusic"));
        Destroy(GameObject.Find("/Loop Entrance"));
        SceneManager.LoadScene("MainMenu");
    }
}
