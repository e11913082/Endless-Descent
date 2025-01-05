using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using EndlessDescent;
//using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyCharacter : MonoBehaviour
{
    public int playerId;
    private Vector3 idlePosition;
    public float idleRadius;
    public float meleeDistance = 1.3f;
    public float shootDistance = 3f;
    public float returnIdleTime = 5f;
    public Weapon weapon;
    public bool disableEnemyBehaviour = false;
    private enum State{
        Idle,
        Attack,
        AttackToIdle
    }
    private State state;
    private PlayerStats stats;
    private Rigidbody2D rigid;
    private Vector2 movementDirection;
    private PlayerCharacter playerCharacter;
    private bool outsidePreviously;
    private bool enemyEnabled;
    public GameObject targetCharacter {get; set;}
    private ContactFilter2D contactFilter;
    private float originalMoveSpeed;
    private float idleMoveSpeed;
    private PathFinder pathFinder;
    private float timeDisappeared = 0.0f;
    private bool targetContactBefore = true;
    private bool withinDetectionTrigger;
    private PlayerControls controls;
    private CharacterWeaponInventory weaponInventory;
    private HaloLogic halo;
    private float lastUse;
    private float nextAttackTime;

    void Awake()
    {
        enemyEnabled = true;
        if (gameObject.layer != LayerMask.NameToLayer("Enemy") || disableEnemyBehaviour)
        {
            enemyEnabled = false;
        }
        rigid = GetComponent<Rigidbody2D> ();
        state = State.Idle;
        playerCharacter = GetComponent<PlayerCharacter> ();
        gameObject.AddComponent<PathFinder> ();
        pathFinder = gameObject.GetComponent<PathFinder> ();
        weaponInventory = GetComponent<CharacterWeaponInventory> ();
        halo = GetComponent<HaloLogic>();

    }
    // Start is called before the first frame update
    void Start()
    {
        //if (!enemyEnabled)
        //    return;

        playerId = CharacterIdGenerator.GetCharacterId(gameObject, 0);
        controls = PlayerControls.Get(playerId);
        //playerCharacter.DisableControls(); 
        ResetMovementDirection();
        stats = PlayerStats.GetPlayerStats(playerId);
        idlePosition = transform.position;
        contactFilter = new ContactFilter2D() {
            useTriggers = false,
            layerMask = ~ LayerMask.GetMask("Player", "Enemy"),
            useLayerMask = true
            };
        originalMoveSpeed = stats.MoveSpeed;
        idleMoveSpeed = 0.3f * originalMoveSpeed;
        pathFinder.SetReturnPoint(transform.position);
        weaponInventory.pickupWeapon(weapon);
        weaponInventory.EquipWeapon(0);
        lastUse = 0f;
        nextAttackTime = stats.attackSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyEnabled)
            return;

        switch (state) {

            default:	
            case State.Idle:
                IdleBehaviour();
                break;

            case State.Attack:
                AttackBehaviour();
                break;

            case State.AttackToIdle:
                AttackToIdleBehaviour();
                break;
        }
    }

    private void IdleBehaviour()
    {
        if (withinDetectionTrigger is true && 
            pathFinder.TargetVisible(transform.position, targetCharacter.transform.position - transform.position) is true)
        {
            state = State.Attack;
        }
        stats.MoveSpeed = idleMoveSpeed;
        CalculateMovementDirection();
        //rigid.velocity = movementDirection * stats.MoveSpeed;
        controls.SetMove(movementDirection);
    }

    private void AttackBehaviour()
    {
        if (targetCharacter.IsDestroyed())
        {
            controls.SetAttack(false);
            state = State.AttackToIdle;
            return;
        }

        bool targetVisible = pathFinder.TargetVisible(transform.position, targetCharacter.transform.position - transform.position);

        float combatDistance = meleeDistance;
        bool distanceWeapon = weapon.type == 0;
        if (distanceWeapon) // if weapon is distance weapon
        {
            combatDistance = shootDistance;
        }

        stats.MoveSpeed = originalMoveSpeed;
        Vector3 playerDirection = targetCharacter.transform.position - gameObject.transform.position;

        if (playerDirection.magnitude < combatDistance && targetVisible)
        {
            controls.SetMove(Vector2.zero);
            Attack();
            controls.SetMousePos(targetCharacter.transform.position);
            return;
        }
        else
        {
            controls.SetAttack(false);
        }

        (bool targetContact, Vector2 newMoveDirection) = pathFinder.GetNextMoveDirection(transform.position, targetCharacter.transform.position);

        if (targetContact is true)
        {
            controls.SetMove(newMoveDirection);
            targetContactBefore = true;
        }
        else if (targetContactBefore is true)
        {
            targetContactBefore = false;
            timeDisappeared = Time.time;
            controls.SetMove(newMoveDirection);
            return;
        }
        else if (Time.time - timeDisappeared > returnIdleTime)
        {
            state = State.AttackToIdle;
            return;
        }
    }

    private void AttackToIdleBehaviour()
    {
        if (withinDetectionTrigger is true &&
            pathFinder.TargetVisible(transform.position, targetCharacter.transform.position - transform.position) is true)
        {
            state = State.Attack;
            return;
        }

        (bool backAtSpawn, Vector2 returnMoveDirection) = pathFinder.GetReturnMoveDirection(transform.position);

        if (backAtSpawn != true)
        {
            controls.SetMove(returnMoveDirection);
        }
        else
        {
            state = State.Idle;
            return;
        }
    }

    private void CalculateMovementDirection()
    {
        if ((transform.position - idlePosition).magnitude > idleRadius)
        {
            Vector2 newDirection = idlePosition - transform.position;
            movementDirection = newDirection.normalized;
            outsidePreviously = true;
        }
        else
        {
            if (outsidePreviously is true)
            {
                ResetMovementDirection();
            }
            outsidePreviously = false;
        }

    }

    private void ResetMovementDirection()
    {
        Vector2 newDirection = idlePosition - transform.position;
        newDirection = Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.Range(-40.0f, 40.0f)) * newDirection;
        
        movementDirection = newDirection.normalized;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enemyEnabled)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (pathFinder.TargetVisible(transform.position, other.gameObject.transform.position - transform.position) is true)
            {
                state = State.Attack;
                targetCharacter = other.gameObject;
                withinDetectionTrigger = true;
            }
        }  
    }

    private void Attack()
    {
        if (Time.time - lastUse > nextAttackTime)
        {
            halo.BeforeEnemyMeleeAttack();
            lastUse = Time.time;
            Invoke("AttackInvoke", 0.4f);
            nextAttackTime = UnityEngine.Random.Range(stats.attackSpeed, 2f * stats.attackSpeed);
        }
    }

    private void AttackInvoke()
    {
        controls.SetAttack(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!enemyEnabled)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            withinDetectionTrigger = false;
        }  
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!enemyEnabled)
        {
            return;
        }
        if (collision.collider.isTrigger is false)
        {
            ResetMovementDirection();
        } 
           
    }

    public void activateEnemyBehaviour()
    {
        disableEnemyBehaviour = false;
        enemyEnabled = true;
    }

    public void deactivateEnemyBehaviour()
    {
        disableEnemyBehaviour = true;
        enemyEnabled = false;
    }

    public void SetTarget(GameObject target)
    {
        targetCharacter = target;
        state = State.Attack;
    }
}
