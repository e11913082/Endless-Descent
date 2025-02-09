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
    public List<AudioClip> idleSounds = new List<AudioClip> ();
    public List<AudioClip> actionSounds = new List<AudioClip> ();
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
    float effectVolume;
    
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
        effectVolume = PlayerPrefs.GetFloat("EffectVolume", 0.5f);
        audioSource.volume = 0.2f * effectVolume;
        rigid.isKinematic = true;
        defaultHaloIntensity = light2D.intensity;
        defaultHaloInnerRadius = light2D.pointLightInnerRadius;
        defaultHaloOuterRadius = light2D.pointLightOuterRadius;
        defaultHaloColor = light2D.color;
        levitate = true;
        StartCoroutine(Levitate());
        audioSource.PlayOneShot(idleSounds[UnityEngine.Random.Range(0, idleSounds.Count)]);
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

    void FixedUpdate()
    {
        effectVolume = PlayerPrefs.GetFloat("EffectVolume");
        print(effectVolume);
    }

    private void IdleStageBehaviour()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = idleSounds[UnityEngine.Random.Range(0, idleSounds.Count)];
            audioSource.volume = 0.2f * effectVolume;
            audioSource.PlayDelayed(UnityEngine.Random.Range(1f, 5f));
        }
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
                audioSource.volume = effectVolume;
                audioSource.PlayOneShot(actionSounds[UnityEngine.Random.Range(0, actionSounds.Count)]);
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
        if (!audioSource.isPlaying)
        {
            audioSource.clip = actionSounds[UnityEngine.Random.Range(0, actionSounds.Count)];
            audioSource.volume = effectVolume;
            audioSource.PlayDelayed(UnityEngine.Random.Range(1f, 5f));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentStage == Stage.IdleStage && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            currentStage = Stage.InitialStage;
            targetCharacter = other.gameObject;
            audioSource.volume = effectVolume;
        }  
    }

    private void SpawnEnemies()
    {
        audioSource.volume = effectVolume;
        audioSource.PlayOneShot(actionSounds[UnityEngine.Random.Range(0, actionSounds.Count)]);
        audioSource.PlayOneShot(spawnEnemiesSounds[UnityEngine.Random.Range(0, spawnEnemiesSounds.Count)]);
        //PlayActionAudio();
        instantiatedEnemies.Clear();
        float spawnRadius = 1.5f;
        float spawnAngle;
        foreach (GameObject enemy in enemiesToSpawn)
        {
            spawnAngle = UnityEngine.Random.Range(0, 360);
            Vector2 spawnPos = transform.position + Quaternion.Euler(0, 0, spawnAngle) * Vector2.up * spawnRadius;
            GameObject enemyGameObject = Instantiate(enemy, spawnPos, transform.rotation);
            instantiatedEnemies.Add(enemyGameObject);
            enemyGameObject.GetComponent<EnemyCharacter>().SetTarget(targetCharacter);
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
        halo.origIntensity = 10f;
        halo.origInnerRadius = 0.5f;
        halo.origOuterRadius = 1f;
        halo.originalCol = Color.blue;
        halo.Reset();
    }

    private void ResetDefaultHalo()
    {
        halo.origIntensity = defaultHaloIntensity;
        halo.origInnerRadius = defaultHaloInnerRadius;
        halo.origOuterRadius = defaultHaloOuterRadius;
        halo.originalCol = defaultHaloColor;
        halo.Reset();
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
