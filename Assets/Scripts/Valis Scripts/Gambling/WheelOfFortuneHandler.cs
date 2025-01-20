using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using EndlessDescent;
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
    public TextMeshProUGUI buttonText;
    
    private float currentSpeed;
    private bool isSpinning = false;
    public int spinCount = 0;
    public int maxSpinCount = 3;
    
    public List<ItemData> commonItems; // List of common items
    public List<ItemData> rareItems;

    private PlayerStats stats;
    public Coroutine fadeCoroutine;
    
    private PlayerCharacter playerCharacter;

    private void Awake()
    {   
        buttonText.text = "Spin = 10 Oil ("+(maxSpinCount-spinCount)+" spins left)";
        outText.text = "";
        stats = oilText.GetComponent<CurrentCoinsGambling>().playerStats;
        playerCharacter = GameObject.Find("/Main Character").GetComponent<PlayerCharacter>();
        CharacterGamblingTrader player = GameObject.Find("/Main Character").GetComponent<CharacterGamblingTrader>();
    }

    public void Update()
    {
        if (playerCharacter.GetActionDown())
        {
            if(fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            Time.timeScale = 1f;
            GameObject.Find("/GamblingCanvas").gameObject.SetActive(false);
            
        }
    }

    private IEnumerator FadeOut(int wait)
    {
        yield return new WaitForSecondsRealtime(wait);
        for(float alpha = outText.alpha; alpha >= -1f; alpha -= 0.2f)
        {
            outText.alpha = alpha;
            yield return new WaitForSecondsRealtime(0.1667f);
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

    public void ResetSpin()
    {
        Debug.Log("Delete me");
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
            if (spinCount >= maxSpinCount)
            {
                outText.text = "No more spins left";
                outText.color = Color.red;
                return;
            }
            spinCount++;
            buttonText.text = "Spin = 10 Oil ("+(maxSpinCount-spinCount)+" left)";
            
            stats.coins -= 10;
            spinDuration = Random.Range(2f, 5f); 
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
        float offset = 0f;
        float angle = (transform.eulerAngles.z) % 360;

        int totalSegments = 8;
        float segmentAngleSize = 360f / totalSegments;
        int segmentIndex = Mathf.FloorToInt(angle / segmentAngleSize);


        float targetAngle = segmentIndex * segmentAngleSize + (segmentAngleSize / 2);

        StartCoroutine(SnapToSegment(targetAngle));
        
        ItemData item = null;
        
        switch (segmentIndex)
        {
            case 0:
            case 2:
            case 4:
            case 6:
                item = GetRandomItem(0);
                outText.color = Color.green;
                break;
            case 5:
                item = GetRandomItem((1));
                outText.color = Color.yellow;
                break;
            case 1:    
                outText.color = Color.gray;
                break;
            case 3:
            case 7:
                outText.color = Color.red;
                break;
            
        }
        string result = stats.WheelOfFortune(segmentIndex, item);
        
        Debug.Log("Rewarded with: " + segmentIndex+ " item: " + item);
        
        outText.text = result;
        FadeOut();
    }

    private IEnumerator SnapToSegment(float targetAngle)
    {
        float currentAngle = transform.eulerAngles.z % 360; 
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
        Debug.Log($"Current: {currentAngle}, Target: {targetAngle}, Difference: {angleDifference}");

        yield return new WaitForSecondsRealtime(0.3f);
        
        float snapSpeed = 200f; 
        while (Mathf.Abs(angleDifference) > 0.1f) 
        {
            float rotationStep = Mathf.Sign(angleDifference) * snapSpeed * Time.unscaledDeltaTime;
            if (Mathf.Abs(rotationStep) > Mathf.Abs(angleDifference))
                rotationStep = angleDifference; 

            transform.Rotate(0, 0, rotationStep);
            currentAngle = transform.eulerAngles.z % 360;
            angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
          
            
            yield return null;
        }

        
        transform.eulerAngles = new Vector3(0, 0, targetAngle);
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
