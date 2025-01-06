using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System;
using System.Runtime.InteropServices;

public class LoopInteraction : MonoBehaviour
{
    private static LoopInteraction instance;

    private static GameObject curPlayer;
    private static Vector3 spawnPos;
    private static int loopCount = 0;

    public GameObject loopEnemies; //parentobj of all additional loop enemies
    public GameObject lanterns; //parentobj of all lantern groups

    private LayerMask playerLayer;
 
    private void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
        if (instance == null) //case: game start (first iteration)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            curPlayer = GameObject.Find("Main Character");
            spawnPos = GameObject.Find("Main Character").transform.position;
        }
        else //case: player is looping
        {
            loopCount++;
            curPlayer.transform.position = spawnPos;
            HandleEnemies();
            HandleLanterns();

            Destroy(gameObject);
        }
        
    }

    void OnTriggerEnter2D(Collider2D other) //reload scene when player enters
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void HandleEnemies() //fills the map with an appropriate amount of enemies based on the number if the loop
    {
        float spawnThreshold = (10f - 2*loopCount) / 10f;
        int enemyCount = loopEnemies.transform.childCount;

        for (int i = 0; i < enemyCount; i++)
        {
            float randomeFloat = UnityEngine.Random.Range(0.0f, 1.0f);
            if(randomeFloat > spawnThreshold)
            {
                GameObject enemy = loopEnemies.transform.GetChild(i).gameObject;
                enemy.SetActive(true);
            }
        }
    }

    void HandleLanterns()
    {
        int lanternGroupCount = lanterns.transform.childCount;

        for (int i = 0; i < lanternGroupCount; i++)
        {
            HandleLanternGroup(lanterns.transform.GetChild(i).gameObject);
        }
    }

    void HandleLanternGroup(GameObject lanternGroup)
    {
        int lanternCount = lanternGroup.transform.childCount;
        int activeLanterns = lanternCount; //to ensure one lantern is at least active in every group

        float lv1Threshold = (10f - 2.2f*loopCount) / 10f;
        float lv2Threshold = (10f - 1.1f*loopCount) / 10f;

        for (int i = 0; i < lanternCount; i++)
        {
            float randomeFloat = UnityEngine.Random.Range(0.0f, 1.0f);
            LightLoopingUtil lanternUtil;

            if(randomeFloat > lv2Threshold && activeLanterns > 1)
            {
                lanternUtil = lanternGroup.transform.GetChild(i).gameObject.GetComponent<LightLoopingUtil>();
                lanternUtil.SetLanternLv(2);
                activeLanterns--;
            }
            else if(randomeFloat > lv1Threshold)
            {
                lanternUtil = lanternGroup.transform.GetChild(i).gameObject.GetComponent<LightLoopingUtil>();
                lanternUtil.SetLanternLv(1);
            }
        }
    }
}
