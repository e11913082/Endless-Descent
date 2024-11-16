using UnityEngine;
using EndlessDescent;
using System.Text;
using Unity.VisualScripting;
using System;
using System.Collections.Generic;

public class Hud : MonoBehaviour
{
    public int player_id = 0;

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
