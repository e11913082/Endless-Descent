using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using TMPro;
using UnityEngine;
using System.Linq;
public class CurrentCoinsGambling : MonoBehaviour
{
    private int player_id;
    
    private TextMeshProUGUI textMesh;
    public PlayerStats playerStats;
    
    public void Awake()
    {
        PlayerStats[] playerStats = FindObjectsOfType<PlayerStats>();
        playerStats = System.Array.FindAll(playerStats, ps => ps.gameObject.layer == LayerMask.NameToLayer("Player"));
        if(playerStats.Count() > 1)
        {
            Debug.Log("More than one active player in the scene");
        }
        else if(playerStats.Count() == 0)
        {
            Debug.Log("No active playerobject in the scene");
        }
        else
        {
            player_id = playerStats[0].player_id;
        }
        this.playerStats = playerStats[0];
        textMesh = GetComponent<TextMeshProUGUI>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        textMesh.text = playerStats.coins.ToString("0.0") + "\n Lantern Oil";
        
        if (playerStats.coins > 30f)
        {
            textMesh.color = new Color(0.082f,0.451f,0f,1f);
        }
        else if (playerStats.coins is <= 30f and > 15f)
        {
            textMesh.color = new Color(0.555f, 0.559f, 0f, 1f);
        }
        else if (playerStats.coins is <= 15f and > 5f)
        {
            textMesh.color = new Color(0.82f, 0.38f, 0f, 1f);
        }
        else
        {
            textMesh.color = Color.red;
        }
    }
}
