using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.TextCore.Text;
using EndlessDescent;
using UnityEngine.Rendering.Universal;
using System;


public class BossBehaviour : MonoBehaviour
{
    public int playerId;
    public List<GameObject> InitialStageEnemies = new List<GameObject> ();
    public RuntimeAnimatorController finalStageAnimatorController;
    public List<AudioClip> spawnEnemiesSounds = new List<AudioClip> ();
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
    private EnemyCharacter enemyCharacter;
    private Animator animator;
    private AudioSource audioSource;
    private Rigidbody2D rigid;
    private Light2D light2D;
    private float defaultHaloIntensity;
    private float defaultHaloInnerRadius;
    private float defaultHaloOuterRadius;
    private Color defaultHaloColor;
    private bool levitate;

    
    void Awake()
    {
        character = GetComponent<PlayerCharacter> ();
        halo = GetComponent<HaloLogic>();
        characterAnim = GetComponent<CharacterAnim>();
        weaponAttack = GetComponent<WeaponAttack>();
        enemyCharacter = GetComponent<EnemyCharacter>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody2D>();
        light2D = GetComponent<Light2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerId = CharacterIdGenerator.GetCharacterId(gameObject, 0);
        stats = PlayerStats.GetPlayerStats(playerId);
        character.invulnerable = true;
        audioSource.volume = PlayerPrefs.GetFloat("EffectVolume");
        rigid.isKinematic = true;
        defaultHaloIntensity = light2D.intensity;
        defaultHaloInnerRadius = light2D.pointLightInnerRadius;
        defaultHaloOuterRadius = light2D.pointLightOuterRadius;
        defaultHaloColor = light2D.color;
        levitate = true;
        StartCoroutine(Levitate());
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
        Vector2 targetDirection = (targetCharacter.transform.position - transform.position).normalized;
        characterAnim.SetSide(character.CalculateSide(targetDirection));

        if (stats.currentHealth < 0.5 * stats.maxHealth)
        {
            ResetDefaultHalo();
            FinalStageHalo();
            enemyCharacter.activateEnemyBehaviour();
            currentStage = Stage.FinalStage;
            animator.runtimeAnimatorController = finalStageAnimatorController;
            enemyCharacter.SetTarget(targetCharacter);
            levitate = false;
            StopCoroutine(Levitate());
            return;
        }

        bool spawnedEnemiesDead = instantiatedEnemies.All(x => x.IsDestroyed() == true);
        if (spawnedEnemiesDead && invokingSpawnEnemies is false)
        {
            if (character.invulnerable is true)
            {
                character.invulnerable = false;
                vulnerableStartTime = Time.time;
                rigid.isKinematic = false;
                ResetDefaultHalo();
                
            }    
            else if (Time.time - vulnerableStartTime > vulnerableTimeSpan || firstLoop)
            {
                InvulnerableHalo();
                enemiesToSpawn = InitialStageEnemies;
                halo.OnSummonEnemies();
                characterAnim.AnimateAttack(1, character.CalculateSide(targetDirection));
                Invoke("SpawnEnemies", 0.155f);
                character.invulnerable = true;
                firstLoop = false;
                invokingSpawnEnemies = true;
                rigid.isKinematic = true;
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
        audioSource.PlayOneShot(spawnEnemiesSounds[UnityEngine.Random.Range(0, spawnEnemiesSounds.Count)]);
        instantiatedEnemies.Clear();
        float spawnRadius = 1.5f;
        float spawnAngle;
        foreach (GameObject enemy in enemiesToSpawn)
        {
            spawnAngle = UnityEngine.Random.Range(0, 360);
            Vector2 spawnPos = transform.position + Quaternion.Euler(0, 0, spawnAngle) * Vector2.up * spawnRadius;
            instantiatedEnemies.Add(Instantiate(enemy, spawnPos, transform.rotation));
        }
        invokingSpawnEnemies = false;
    }

    private void FinalStageHalo()
    {
        halo.StopCurCoroutine();
        halo.currentFlickerCoroutine = StartCoroutine(halo.Flicker(0.5f, 0.16f, 0.5f, 0.5f, Color.red));
    }

    private void InvulnerableHalo()
    {
        light2D.intensity = 10f;
        light2D.pointLightInnerRadius = 0.5f;
        light2D.pointLightOuterRadius = 1f;
        light2D.color = Color.blue;
    }

    private void ResetDefaultHalo()
    {
        light2D.intensity = defaultHaloIntensity;
        light2D.pointLightInnerRadius = defaultHaloInnerRadius;
        light2D.pointLightOuterRadius = defaultHaloOuterRadius;
        light2D.color = defaultHaloColor;
    }

    private IEnumerator Levitate()
    {
        float delta = 0.01666f;
        float period = 2f;
        float curPhase = 0f;
        float amplitude = 0.2f;
        Vector3 originalPosition = transform.position;
        Transform shadowTransform = transform.Find("Shadow");
        Vector3 originalShadowPosition = shadowTransform.position;
        Vector3 positionDelta = originalPosition - originalShadowPosition;
        while (levitate is true)
        {
            transform.position = originalPosition + amplitude * new Vector3 (0f, Mathf.Sin(curPhase * (float) Math.PI / period), 0f);
            shadowTransform.position = originalShadowPosition;
            curPhase += delta;
            yield return new WaitForSeconds(delta);
        }
        shadowTransform.position = originalPosition - positionDelta; 
    }
}
