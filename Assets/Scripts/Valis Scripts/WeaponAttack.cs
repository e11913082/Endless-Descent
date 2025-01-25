using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using EndlessDescent;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    private HaloLogic halo;
    private CharacterWeaponInventory inventory;
    private PlayerCharacter character;
    private PlayerStats stats;
    private CharacterAnim characterAnim;

    private float lastUse;
    private LayerMask enemies;
    private float effectVolume;

    // Start is called before the first frame update
    void Start()
    {
        halo = GetComponent<HaloLogic>();
        inventory = GetComponent<CharacterWeaponInventory>();
        character = GetComponent<PlayerCharacter>();
        stats = PlayerStats.GetPlayerStats(CharacterIdGenerator.GetCharacterId(gameObject, 0));
        characterAnim = GetComponent<CharacterAnim>();
        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemies = LayerMask.NameToLayer("Player");
        }
        else if (gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            enemies = LayerMask.NameToLayer("Enemy");
        }
        effectVolume = PlayerPrefs.GetFloat("EffectVolume", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (character.GetAttackDown() && !character.IsDead())
        {
            if (inventory.equippedWeapon != null)
            {
                //distance weapon
                if (inventory.equippedWeapon.type == 0)
                {
                    if (Time.time - lastUse > stats.attackSpeed)
                    {
                        Vector2 attackDirection = (Vector2)(character.GetMousePos() - transform.position).normalized;
                        characterAnim.AnimateAttack(0, GetAttackSide(attackDirection));
                        lastUse = Time.time;
                        Invoke("UseDistance", inventory.equippedWeapon.delay);
                        halo.OnMagicAttack();
                    }
                } // melee weapon
                else
                {
                    if (Time.time - lastUse > stats.attackSpeed)
                    {
                        Vector2 attackDirection = (Vector2)(character.GetMousePos() - transform.position).normalized;
                        characterAnim.AnimateAttack(1, GetAttackSide(attackDirection));
                        lastUse = Time.time;
                        Invoke("UseMelee", inventory.equippedWeapon.delay);
                        print(inventory.equippedWeapon.delay);
                        print(stats.attackSpeed);
                        halo.OnMeleeAttack();
                    }
                }
            }
            else
            {
                Debug.Log("No equipped weapon");
            }
        }
    }

    private void UseDistance()
    {
        Vector2 spawnPosition = transform.position + new Vector3(inventory.equippedWeapon.projectileOffsetX,
            inventory.equippedWeapon.projectileOffsetY, 0);
        Projectile Projectile =
            Instantiate(inventory.equippedWeapon.projectilePrefab, spawnPosition, transform.rotation)
                .GetComponent<Projectile>();
        Projectile.SetShooter(gameObject);

        Vector2 direction = (Vector2)(character.GetMousePos() - transform.position).normalized;
        Projectile.Shoot(direction, enemies);

        Debug.Log("Attacked with " + inventory.equippedWeapon.weaponName + " in direction: " + direction + " with damage: " +
                  PlayerStats.GetPlayerStats(character.player_id).damage);
    }

    private void UseMelee()
    {
        Vector2 direction = (Vector2)(character.GetMousePos() - transform.position).normalized;
        Vector3 attackPos = transform.position + new Vector3(direction.x, direction.y, 0) * stats.attackRange;

       // LayerMask enemies = LayerMask.NameToLayer("Enemy");
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(transform.position, stats.attackRange); // , enemies); // the layer filtering here does not work for some reason

        MeleePrefab melee = Instantiate(inventory.equippedWeapon.projectilePrefab, attackPos, transform.rotation)
            .GetComponent<MeleePrefab>();
        melee.Attack(direction, gameObject, stats.attackRange);
        melee.PlaySwingSound(effectVolume);

        bool hitEnemy = false;
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            float damage = PlayerStats.GetPlayerStats(character.player_id).damage;
            Collider2D eCollider = enemiesToDamage[i];
            PlayerCharacter e = eCollider.GetComponent<PlayerCharacter>();

            if (e == null)
            {
                continue;
            }

            Vector2 enemyDirection = (e.transform.position - transform.position).normalized;
            bool withinAngle = Math.Abs(Vector2.Angle(direction, enemyDirection)) < 20;

            if (eCollider.gameObject.layer == enemies && eCollider.isTrigger is false && withinAngle)
            {
                e.TakeDamage(damage, (e.transform.position - transform.position).normalized, 1);
                hitEnemy = true;
            }
            
        }
        

        Debug.Log("Attacked with "+ inventory.equippedWeapon.weaponName +" on position: " + attackPos + " with damage: " + stats.damage);
    }

    public int GetAttackSide(Vector2 direction)
    {
        float angle = Vector2.Angle(Vector2.up, direction);
        int attackSide = 1;

        if (angle < 45)
            {attackSide = 4;}
        else if (45 <= angle && angle < 135 && direction.x > 0)
            {attackSide = 1;}
        else if (45 <= angle && angle < 135 && direction.x <= 0)
            {attackSide = 3;}
        else if (angle >= 135)
            {attackSide = 2;}

        return attackSide;
    }
}