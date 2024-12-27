using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoopInteraction : MonoBehaviour
{
    private LayerMask playerLayer;
    private static GameObject curPlayer;
    void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
        if(curPlayer != null) //replace new player if a player already exists from the last iteration
        {
            GameObject newPlayer = GameObject.Find("Main Character");
            curPlayer.transform.position = newPlayer.transform.position;
            Destroy(newPlayer);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //TODO play secret sound
        if ((playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            if(curPlayer == null) { //store player for the next iteration
                curPlayer = other.gameObject;
                DontDestroyOnLoad(curPlayer);
            }
            ReloadScene();
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
