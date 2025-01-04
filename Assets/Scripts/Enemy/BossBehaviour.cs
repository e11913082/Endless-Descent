using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.TextCore.Text;
using EndlessDescent;


public class BossBehaviour : MonoBehaviour
{
    public int playerId;
    public List<GameObject> InitialStageEnemies = new List<GameObject> ();
    public float vulnerableTimeSpan = 5f;
    private float vulnerableStartTime;

    private enum Stage{
        IdleStage,
        InitialStage,
        FinalStage
    }
    private Stage currentStage = Stage.IdleStage;
    private PlayerStats stats;
    private GameObject targetCharacter;
    private List<GameObject> instantiatedEnemies = new List<GameObject> ();
    private List<GameObject> enemiesToSpawn = new List<GameObject> ();
    private PlayerCharacter character;
    private bool firstLoop = true;
    private HaloLogic halo;
    private CharacterAnim characterAnim;
    private bool invokingSpawnEnemies = false;
    private WeaponAttack weaponAttack;

    
    void Awake()
    {
        character = GetComponent<PlayerCharacter> ();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerId = CharacterIdGenerator.GetCharacterId(gameObject, 0);
        stats = PlayerStats.GetPlayerStats(playerId);
        character.invulnerable = true;
        halo = GetComponent<HaloLogic>();
        characterAnim = GetComponent<CharacterAnim>();
        weaponAttack = GetComponent<WeaponAttack>();
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
        bool spawnedEnemiesDead = instantiatedEnemies.All(x => x.IsDestroyed() == true);
        if (spawnedEnemiesDead && invokingSpawnEnemies is false)
        {
            if (character.invulnerable is true)
            {
                character.invulnerable = false;
                vulnerableStartTime = Time.time;
            }    
            else if (Time.time - vulnerableStartTime > vulnerableTimeSpan || firstLoop)
            {
                enemiesToSpawn = InitialStageEnemies;
                halo.OnSummonEnemies();
                Vector2 direction = (targetCharacter.transform.position - transform.position).normalized;
                characterAnim.AnimateAttack(1, weaponAttack.GetAttackSide(direction));
                Invoke("SpawnEnemies", 0.155f);
                character.invulnerable = true;
                firstLoop = false;
                invokingSpawnEnemies = true;
            }        
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

    private void SpawnEnemies()
    {
        instantiatedEnemies.Clear();
        float spawnRadius = 1.5f;
        float spawnAngle;
        foreach (GameObject enemy in enemiesToSpawn)
        {
            spawnAngle = Random.Range(0, 360);
            Vector2 spawnPos = transform.position + Quaternion.Euler(0, 0, spawnAngle) * Vector2.up * spawnRadius;
            instantiatedEnemies.Add(Instantiate(enemy, spawnPos, transform.rotation));
        }
        invokingSpawnEnemies = false;
    }
}
