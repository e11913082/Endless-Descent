using System;
using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestInteraction : MonoBehaviour
{
    private bool inChestCollider = false;
    private PlayerCharacter playerCharacter;
    private GameObject chest;
    
    public int chestHintCounter = 0;
    public int chestHintMaxCounter = 3;
    private TextMeshProUGUI hintText;
    
    private Coroutine hintCoroutine = null;
    private bool hintActive = false;

    private Image panelImage;
    
    private void Start()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
        hintText = GameObject.Find("/HintCanvas").GetComponentInChildren<TextMeshProUGUI>(true);
        panelImage = hintText.gameObject.transform.parent.gameObject.GetComponent<Image>();
        hintText.gameObject.transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        if (inChestCollider)
        {
            if (playerCharacter.GetActionDown())
            {
              ChestController chestController = chest.GetComponent<ChestController>();

              if (chestController.isOpen)
              {
                  chestController.ReceiveItem();
                  chest.GetComponent<CircleCollider2D>().enabled = false;  
              }
              else
              {
                  chestController.OpenChest();
              }
            }
            if (chestHintCounter < chestHintMaxCounter && !hintActive)
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
        if (inChestCollider)
        {   
            Debug.Log("Hint triggered");
            hintText.gameObject.transform.parent.gameObject.SetActive(true);
            // readjust alpha
            hintText.alpha = 1f;
            Color color = panelImage.color;
            color.a = 1f;
            panelImage.color = color;
            
            string inputKey = PlayerPrefs.GetString("interact", "E");
            hintText.text = "Press " + inputKey + " to open the chest!";
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
        chestHintCounter++;
        hintText.gameObject.transform.parent.gameObject.SetActive(false);
        
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Chest"))
        {
            inChestCollider = true;
            chest = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Chest"))
        {
            inChestCollider = false;
            chest = null;
        }
    }
}
