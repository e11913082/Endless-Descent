using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    private CharacterWeaponInventory inventory;
    private PlayerCharacter character;
    private PlayerStats stats;
    private CharacterAnim characterAnim;

    private float lastUse;


    public LayerMask enemies;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<CharacterWeaponInventory>();
        character = GetComponent<PlayerCharacter>();
        stats = PlayerStats.GetPlayerStats(character.player_id);
        characterAnim = GetComponent<CharacterAnim>();
    }

    // Update is called once per frame
    void Update()
    {
        if (character.GetAttackDown())
        {
            if (inventory.equippedWeapon != null)
            {
                //distance weapon
                if (inventory.equippedWeapon.type == 0)
                {
                    if (Time.time - lastUse > stats.attackSpeed)
                    {
                        characterAnim.AnimateAttack();
                        lastUse = Time.time;
                        Invoke("UseDistance", inventory.equippedWeapon.delay);
                    }
                } // melee weapon
                else
                {
                    if (Time.time - lastUse > stats.attackSpeed)
                    {
                        characterAnim.AnimateAttack();
                        lastUse = Time.time;
                        Invoke("UseMelee", inventory.equippedWeapon.delay);
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
        Projectile.Shoot(direction);

        Debug.Log("Attacked with " + inventory.equippedWeapon.weaponName + " in direction: " + direction + " with damage: " +
                  PlayerStats.GetPlayerStats(character.player_id).damage);
    }

    private void UseMelee()
    {
        Vector2 direction = (Vector2)(character.GetMousePos() - transform.position).normalized;
        Vector3 attackPos = transform.position + new Vector3(direction.x, direction.y, 0) * stats.attackRange;

       // LayerMask enemies = LayerMask.NameToLayer("Enemy");
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos, 1f, enemies);

        MeleePrefab melee = Instantiate(inventory.equippedWeapon.projectilePrefab, attackPos, transform.rotation)
            .GetComponent<MeleePrefab>();
        melee.Attack(direction);

        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            float damage = PlayerStats.GetPlayerStats(character.player_id).damage;
            Enemy e = enemiesToDamage[i].GetComponent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(damage);
            }
            
        }

        Debug.Log("Attacked with "+ inventory.equippedWeapon.weaponName +" on position: " + attackPos + " with damage: " + stats.damage);
    }
}