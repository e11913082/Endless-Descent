using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;


public class BossBehaviour : MonoBehaviour
{
    public int playerId;
    public List<GameObject> InitialStageEnemies = new List<GameObject> ();

    private enum Stage{
        IdleStage,
        InitialStage,
        FinalStage
    }
    private Stage currentStage = Stage.IdleStage;
    private PlayerStats stats;
    private GameObject targetCharacter;
    private List<GameObject> instantiatedEnemies = new List<GameObject> ();
    
    void Awake()
    {
        playerId = CharacterIdGenerator.GetCharacterId(gameObject, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        stats = PlayerStats.GetPlayerStats(playerId);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentStage) {

            default:	
            case Stage.IdleStage:
                IdleStageBehaviour();
                break;
                
            case Stage.InitialStage:
                InitialStageBehaviour();
                break;

            case Stage.FinalStage:
                FinalStageBehaviour();
                break;
        }
    }

    private void IdleStageBehaviour()
    {

    }
    private void InitialStageBehaviour()
    {
        //bool spawnedEnemiesDead = true;
        //foreach (GameObject enemy in instantiatedEnemies)
        //{
        //    if (!enemy.IsDestroyed())
        //    {
        //        spawnedEnemiesDead = false;
        //    }
        //}
        
        bool spawnedEnemieDead = instantiatedEnemies.All(x => x.IsDestroyed() == true);
        if (spawnedEnemieDead)
        {
            SpawnEnemies(InitialStageEnemies);
        }
    }

    private void FinalStageBehaviour()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentStage == Stage.IdleStage && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            currentStage = Stage.InitialStage;
            targetCharacter = other.gameObject;
        }  
    }

    private void SpawnEnemies(List<GameObject> enemies)
    {
        instantiatedEnemies.Clear();
        float spawnRadius = 1.5f;
        float spawnAngle;
        foreach (GameObject enemy in enemies)
        {
            spawnAngle = Random.Range(0, 360);
            Vector2 spawnPos = transform.position + Quaternion.Euler(0, 0, spawnAngle) * Vector2.up * spawnRadius;
            instantiatedEnemies.Add(Instantiate(enemy, spawnPos, transform.rotation));
        }
    }
}
