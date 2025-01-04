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
    public static LoopInteraction instance;

    private static GameObject curPlayer;
    private static Vector3 spawnPos;
    private static int loopCount = 0;

    public GameObject loopEnemies;
    private LayerMask playerLayer;
 
    private void Awake()
    {
        if (instance == null) //case: game start first iteration
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            spawnPos = GameObject.Find("Main Character/Managers").transform.position;

        }
        else //case: player is looping
        {
            loopCount++;
            curPlayer.transform.position = spawnPos;
            Destroy(gameObject);
        }
        HandleEnemies();
    }
    void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            curPlayer = GameObject.Find("Main Character/Managers");
            ReloadScene();
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void HandleEnemies() //fills the map with an appropriate amount of enemies based on the number if the loop
    {
        //DeactivateAllLoopEnemies();
        float spawnThreshold = (10f - 2*loopCount) / 10f;
        int enemyCount = loopEnemies.transform.childCount;
        Debug.Log("New spawnThreshold: " + spawnThreshold);

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
    void DeactivateAllLoopEnemies()
    {
        int childCount = loopEnemies.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            GameObject enemy = loopEnemies.transform.GetChild(i).gameObject;
            enemy.SetActive(false);
        }
    }
}
