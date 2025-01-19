using System;
using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterGamblingTrader : MonoBehaviour
{
    public GameObject canvas;
    private bool inTrigger = false;
    private PlayerCharacter playerCharacter;

    public int traderHintCounter = 0;
    public int traderHintMaxCounter = 3;
    private TextMeshProUGUI hintText;
    
    private Coroutine hintCoroutine = null;
    private bool hintActive = false;

    private Image panelImage;
    void Start()
    {
        canvas = GameObject.Find("/GamblingCanvas");
        canvas.SetActive(false);
        hintText = GameObject.Find("/HintCanvas").GetComponentInChildren<TextMeshProUGUI>(true);
        panelImage = hintText.gameObject.transform.parent.gameObject.GetComponent<Image>();
        hintText.gameObject.transform.parent.gameObject.SetActive(false);
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GamblingTrader"))
        {
            inTrigger = true;
            playerCharacter = GetComponent<PlayerCharacter>();
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("GamblingTrader"))
        {
            inTrigger = false;
            playerCharacter = GetComponent<PlayerCharacter>();
        }
    }
    private void Update()
    {
        if (inTrigger) 
        {
            
           // canvas = GameObject.Find("/GamblingCanvas");
           if (playerCharacter.GetActionDown())
           {
              canvas.gameObject.SetActive(true);
              Time.timeScale = 0f; 
           }
           if (traderHintCounter < traderHintMaxCounter && !hintActive)
           {   
               hintActive = true;
               if (hintCoroutine != null)
               {
                   StopCoroutine(hintCoroutine);
               }
               hintCoroutine = StartCoroutine(Hint());
           }
        }
        
        
    }
    private IEnumerator Hint()
    {
        yield return new WaitForSeconds(4f);
        if (inTrigger)
        {   
            Debug.Log("Hint triggered");
            hintText.gameObject.transform.parent.gameObject.SetActive(true);
            // readjust alpha
            hintText.alpha = 1f;
            Color color = panelImage.color;
            color.a = 1f;
            panelImage.color = color;
            
            string inputKey = PlayerPrefs.GetString("interact", "E");
            hintText.text = "Press " + inputKey + " to talk to the trader!";
            FadeOut();
        }
        
    }
    
    private Coroutine fadeCoroutine = null;
    
    private void FadeOut()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOut(1));
    }
    
    private IEnumerator FadeOut(int wait)
    {   
        
        
        yield return new WaitForSeconds(wait);
        
        Color panelColor = panelImage.color;
        float fadeDuration = 3f;
        float fadeInterval = 0.1667f;
        
        float panelStartAlpha = panelImage.color.a;
        float hintTextStartAlpha = hintText.alpha;
        
        for (float t = 0; t <= fadeDuration; t += fadeInterval)
        {
            float normalizedTime = t / fadeDuration;
            
            float currentPanelAlpha = Mathf.Lerp(panelStartAlpha, 0, normalizedTime);
            float currentHintTextAlpha = Mathf.Lerp(hintTextStartAlpha, 0, normalizedTime);
            
            panelColor.a = currentPanelAlpha;
            panelImage.color = panelColor;
            
            hintText.alpha = currentHintTextAlpha;
            
            yield return new WaitForSeconds(fadeInterval);
        }
        hintActive = false;
        traderHintCounter++;
        hintText.gameObject.transform.parent.gameObject.SetActive(false);
        
    }
}
