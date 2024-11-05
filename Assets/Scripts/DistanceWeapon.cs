using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace EndlessDescent
{
        public class DistanceWeapon : MonoBehaviour
    {   
        public GameObject ProjectilePrefab;

        private Weapon equippedWeapon;
        public bool isAvailable;
        public bool Selected = false;
        private PlayerCharacter Character;
        private Projectile Projectile;
        public float ProjectileOffsetX;
        public float ProjectileOffsetY;
        public float Delay;
        public float ReUseTime;
        private float LastUse;
        private CharacterAnim PlayerAnim;

        private PlayerStats stats;
        // Start is called before the first frame update
        void Start()
        {
            Character = GetComponent<PlayerCharacter>();
            PlayerAnim = GetComponent<CharacterAnim>();
            stats = PlayerStats.GetPlayerStats(Character.player_id);
        }

        // Update is called once per frame
        void Update()
        {
            if (Selected == true && Character.GetAttackDown() == true)
            {
                if (Time.time - LastUse > stats.attackSpeed)
                {
                    PlayerAnim.AnimateAttack();
                    LastUse = Time.time;
                    Invoke("Use", Delay);
                }
            }
        }
        
        private void Use()
        {   
            Vector2 spawnPosition = transform.position + new Vector3(ProjectileOffsetX, ProjectileOffsetY, 0);
            Projectile = Instantiate(ProjectilePrefab, spawnPosition, transform.rotation).GetComponent<Projectile> ();
            Projectile.SetShooter(gameObject);
            
            Vector2 direction = (Vector2) (Character.GetMousePos() - transform.position).normalized;
            Projectile.Shoot(direction);
            
            Debug.Log("Attacked with "+equippedWeapon.name +" in direction: " + direction + " with damage: " + PlayerStats.GetPlayerStats(Character.player_id).damage);
        
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
}
