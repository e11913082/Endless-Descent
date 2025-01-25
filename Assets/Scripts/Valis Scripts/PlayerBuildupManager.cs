using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBuildupManager : MonoBehaviour
{
    public int player_id;

    public float fearChangeSpeed = 1.18f;
    public float fearChangeDelay = 1f;

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
        EventManager.TriggerEvent("FearRefresh");
        StartBuildup();
    }

    public bool IsEmpty()
    {
        return buildupCoroutine == null;
    }

    public void StartBuildup()
    {
        if (buildupCoroutine != null)
        {
            StopCoroutine(buildupCoroutine);
            EventManager.TriggerEvent("FearRefresh");
        }
        buildupCoroutine = StartCoroutine(BuildupCoroutine());
    }

    public void PauseBuildup()
    {
        if (buildupCoroutine != null)
        {
            StopCoroutine(buildupCoroutine);
            EventManager.TriggerEvent("FearRefresh");
        }
        buildupCoroutine = StartCoroutine(DecreaseCoroutine());
    }

    private IEnumerator BuildupCoroutine()
    {
        EventManager.TriggerEvent("FearRefresh");
        yield return new WaitForSeconds(fearChangeDelay);
        float curFearInc = 1;
        while (stats.CurrentFear < stats.MaxFear)
        {
            stats.CurrentFear = Mathf.Min(stats.CurrentFear + (stats.fearIncrease * curFearInc), stats.MaxFear);
            curFearInc = curFearInc * fearChangeSpeed;
            EventManager.TriggerEvent("FearRefresh");
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator DecreaseCoroutine()
    {
        EventManager.TriggerEvent("FearRefresh");
        yield return new WaitForSeconds(fearChangeDelay);
        float curFearDec = 1;
        while (stats.CurrentFear > 0)
        {
            stats.CurrentFear = Mathf.Max(stats.CurrentFear - (stats.fearDecrease * curFearDec), 0);
            curFearDec = curFearDec * fearChangeSpeed;
            EventManager.TriggerEvent("FearRefresh");
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
