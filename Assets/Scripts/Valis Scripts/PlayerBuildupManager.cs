using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBuildupManager : MonoBehaviour
{
    public int player_id;
    
    private PlayerStats stats;
    
    private Coroutine buildupCoroutine;

    private static Dictionary<int, PlayerBuildupManager> buildupManagers = new Dictionary<int, PlayerBuildupManager>();

    
    
    void Awake()
    {
        player_id = CharacterIdGenerator.GetCharacterId(gameObject, 1);
        buildupManagers[player_id] = this;
        if (gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            this.enabled = false;
        }
        stats = PlayerStats.GetPlayerStats(player_id);
    }

    void OnDestroy()
    {
        buildupManagers.Remove(player_id);
    }

    // Start is called before the first frame update
    void Start()
    {   
        StartBuildup();
    }

    // Update is called once per frame
    void Update()
    {
        if (stats.CurrentFear >= stats.MaxFear)
        {
            // ? handling somewhere else ?
        }
    }

    public void StartBuildup()
    {
        if (buildupCoroutine != null)
        {
            StopCoroutine(buildupCoroutine);
        }
        buildupCoroutine = StartCoroutine(BuildupCoroutine());
    }

    public void PauseBuildup()
    {
        if (buildupCoroutine != null)
        {
            StopCoroutine(buildupCoroutine);
        }
        buildupCoroutine = StartCoroutine(DecreaseCoroutine());
    }

    private IEnumerator BuildupCoroutine()
    {
        while (stats.CurrentFear < stats.MaxFear)
        {
            stats.CurrentFear = Mathf.Min(stats.CurrentFear + stats.fearIncrease, stats.MaxFear);
            yield return new WaitForSeconds(1);
        }    
    }

    private IEnumerator DecreaseCoroutine()
    {
        while (stats.CurrentFear > 0)
        {
            stats.CurrentFear = Mathf.Max(stats.CurrentFear - (stats.fearDecrease), 0);
            yield return new WaitForSeconds(1);
        }
    }


    public static PlayerBuildupManager GetPlayerBuildupManager(int player_id)
    {
        foreach (PlayerBuildupManager manager in GetAll())
        {
            if (manager.player_id == player_id)
            {
                return manager;
            }
        }
        return null;
    }

    public static PlayerBuildupManager[] GetAll()
    {
        PlayerBuildupManager[] list = new PlayerBuildupManager[buildupManagers.Count];
        buildupManagers.Values.CopyTo(list, 0);
        return list;
    }
    
    
    
}
