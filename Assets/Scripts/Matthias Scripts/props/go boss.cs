using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class goboss : MonoBehaviour
{
    private LayerMask playerLayer;

    private void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
    }

    void OnTriggerEnter2D(Collider2D other) //reload scene when player enters
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(GameObject.Find("Loop Entrance")); //destroy looping entrance to avoid spawning enemies etc
            SceneManager.LoadScene("boss stage");
        }
    }
}
