using UnityEngine;
using EndlessDescent;
using System.Text;
using Unity.VisualScripting;
using System;
using System.Collections.Generic;
using System.Linq;

public class Hud : MonoBehaviour
{
    private int player_id = 0;

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
            Debug.Log("Player found with id: " + playerStats[0]);
            player_id = playerStats[0].player_id;
            EventManager.TriggerEvent("PlayerIdFetched");
        }
    }

    public static string CreateStringMask(int decimalNum) //used by subcomponents for setting the number of decimal places
    {
        StringBuilder outputMask = new StringBuilder("0.");
        for(int i = 0; i < decimalNum; i++)
        {
            outputMask.Append("#");
        }
        return outputMask.ToString();
    }

    public static int GetPlayerId()
    {
        Hud hudRefresher = GameObject.FindObjectOfType<Hud>();
        return hudRefresher.player_id;
    }
}
