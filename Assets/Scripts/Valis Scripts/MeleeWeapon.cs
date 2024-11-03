using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{   
    
    public GameObject meleePrefab;
    
    public bool isAvailable;
    public bool Selected = false;
    private PlayerCharacter character;
    
    public Weapon equippedWeapon;
    public float delay;
    private float lastUse;
    private PlayerStats stats;
    
    public LayerMask enemies;
    
    private CharacterAnim playerAnim;
    
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<PlayerCharacter>();
        playerAnim = GetComponent<CharacterAnim>();
        stats = PlayerStats.GetPlayerStats(character.player_id);
    }

    // Update is called once per frame
    void Update()
    {
        if (Selected && character.GetAttackDown())
        {
            if (Time.time - lastUse > stats.attackSpeed)
            {   
                playerAnim.AnimateAttack();
                lastUse = Time.time;
                Invoke("Use", delay);
            }
            
            // Debug purpose
            Vector2 direction = (Vector2) (character.GetMousePos() - transform.position).normalized;
            Vector3 attackPos = new Vector3(direction.x,direction.y, 0) * stats.attackRange;
            Debug.DrawLine(transform.position, attackPos);
        }
    }

    private void Use()
    {   
        
        Vector2 direction = (Vector2) (character.GetMousePos() - transform.position).normalized;
        
        Vector3 attackPos = transform.position + new Vector3(direction.x,direction.y, 0) * stats.attackRange;
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos, 1f, enemies);
        
        MeleePrefab melee = Instantiate(meleePrefab, attackPos, transform.rotation).GetComponent<MeleePrefab>();
        melee.Attack(direction);
        
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {   
            enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(stats.damage);
        }
        Debug.Log("Attacked with melee on position: " + attackPos + " with damage: " + stats.damage);
    }
    
    public Weapon GetEquippedWeapon()
    {
        return equippedWeapon;
    }
    
    public void equipWeapon(Weapon weapon)
    {
        isAvailable = true;
        equippedWeapon = weapon;
    }

    public void unequipWeapon()
    {
        isAvailable = false;
        equippedWeapon = null;
    }
    
    public void Select()
    {
        Selected = true;
    }

    public void Deselect()
    {
        Selected = false;
    }
    
    public bool IsSelected()
    {
        return Selected;
    }

    public void SetAvailable()
    {
        isAvailable = true;
    }

    public void SetUnavailable()
    {
        isAvailable = false;
    }

    public bool IsAvailable()
    {
        return isAvailable;
    }
}
