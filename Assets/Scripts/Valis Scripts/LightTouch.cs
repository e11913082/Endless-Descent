using System;
using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using UnityEngine;

public class LightTouch : MonoBehaviour
{
    public int inLight = 0;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Light"))
        {   
            inLight++;
            
            if (inLight == 1)
            {
                PlayerCharacter player = GetComponent<PlayerCharacter>();
                PlayerBuildupManager manager = PlayerBuildupManager.GetPlayerBuildupManager(player.player_id);
                manager.PauseBuildup();
            }
        }
    }
    
    
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Light"))
        {
            inLight = Mathf.Max(0, inLight - 1);
            
            if (inLight == 0)
            {
                PlayerCharacter player = GetComponent<PlayerCharacter>();
                PlayerBuildupManager manager = PlayerBuildupManager.GetPlayerBuildupManager(player.player_id);
                manager.StartBuildup();
            }
        }
    }
}
