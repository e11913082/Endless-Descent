using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WheelOfFortuneHandler : MonoBehaviour
{
    public float spinDuration = 3f;
    public float maxSpeed = 720f;
    public TextMeshProUGUI outText;
    public TextMeshProUGUI oilText;
    
    private float currentSpeed;
    private bool isSpinning = false;

    public List<ItemData> commonItems; // List of common items
    public List<ItemData> rareItems;

    private PlayerStats stats;
    public Coroutine fadeCoroutine;

    private void Awake()
    {
        outText.text = "";
        stats = oilText.GetComponent<CurrentCoinsGambling>().playerStats;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.Find("/GamblingCanvas").gameObject.SetActive(false);
            StopCoroutine(fadeCoroutine);
            Time.timeScale = 1f;
        }
    }

    public IEnumerator FadeOut(int wait) //fades out statChanged string after "wait" seconds
    {
        yield return new WaitForSeconds(wait);
        for(float alpha = outText.alpha; alpha >= -1f; alpha -= 0.2f)
        {
            outText.alpha = alpha;
            yield return new WaitForSeconds(0.1667f);
        }
    }

    private void FadeOut()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOut(2));
    }
    
    public void StartSpin()
    {
        if (!isSpinning)
        {
            stats = oilText.GetComponent<CurrentCoinsGambling>().playerStats;
            if (stats.coins < 10)
            {
                outText.text = "You do not have enough lantern oil";
                return;
            }
            stats.coins -= 10;
            spinDuration = Random.Range(2f, 5f); // Random duration between 2 and 5 seconds
            maxSpeed = Random.Range(500f, 1000f);
            StartCoroutine(Spin());
        }
    }

    private IEnumerator Spin()
    {
        isSpinning = true;
        float time = 0;

        while (time < spinDuration / 2f)
        {
            time += Time.unscaledDeltaTime;
            currentSpeed = Mathf.Lerp(0, maxSpeed, time / (spinDuration / 2f));
            transform.Rotate(0,0,-currentSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        while (time < spinDuration)
        {
            time += Time.unscaledDeltaTime;
            currentSpeed = Mathf.Lerp(maxSpeed, 0, (time - spinDuration / 2f) / (spinDuration / 2f));
        }

        currentSpeed = 0;
        isSpinning = false;

        HandleReward();
    }

    private void HandleReward()
    {
        float offset = 45f;
        float angle = (transform.eulerAngles.z - offset + 360) % 360;

        int totalSegments = 8;
        int segmentIndex = Mathf.FloorToInt(angle / (360f / totalSegments));
        
        
        ItemData item = null;
        
        switch (segmentIndex)
        {
            case 1:
            case 3:
            case 5:
            case 7:
                item = GetRandomItem(0);
                outText.color = Color.green;
                break;
            case 0:
            case 4:
                outText.color = Color.yellow;
                break;
            case 2:
                outText.color = Color.red;
                break;
            case 6:
                item = GetRandomItem(1);
                outText.color = Color.red;
                break;
        }
        string result = stats.WheelOfFortune(segmentIndex, item);
        
        Debug.Log("Rewarded with: " + segmentIndex+ " item: " + item);
        
        outText.text = result;
        FadeOut();
    }
    
    
    /**
     * @param rarity: 0 = common, 1 = rare
     * @returns random itemdata from the given list
     */
    public ItemData GetRandomItem(int rarity)
    {
        // Determine if the drop is rare or common

        // Select an item from the corresponding pool
        if (rarity == 1)
        {
            return rareItems[Random.Range(0, rareItems.Count)];
        }
        else if (rarity == 0)
        {
            return commonItems[Random.Range(0, commonItems.Count)];
        }

        // Fallback if no items are available
        Debug.LogWarning("Unknown Rarity");
        return null;
    }
}
