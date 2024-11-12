using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Top down character movement
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace EndlessDescent
{
    public class PlayerCharacter : MonoBehaviour
    {
        public int player_id;

        [Header("Stats")]
        public float max_hp = 100f;

        [Header("Status")]
        public bool invulnerable = false;

        [Header("Movement")]
        public float move_accel = 1f;
        public float move_deccel = 1f;
        public float move_max = 1f;
        public bool disable_controls = false;

        public UnityAction onDeath;
        public UnityAction onHit;

        private Rigidbody2D rigid;
        private Animator animator;
        private AutoOrderLayer auto_order;
        private ContactFilter2D contact_filter;
            
        //Weapons
        private bool weaponSwitch;
        private DistanceWeapon distanceWeapon;
        private MeleeWeapon meleeWeapon;
        
        private float hp;
        private bool is_dead = false;
        private Vector2 move;
        private Vector2 move_input;
        private bool attackDown;
        
        private Vector2 lookat = Vector2.zero;
        private float side = 1f;
        private float hit_timer = 0f;

        private static Dictionary<int, PlayerCharacter> character_list = new Dictionary<int, PlayerCharacter>();

        // Mouse Aim
        private Vector2 mouse_pos = Vector2.zero;
        // Stats 
        private PlayerStats stats;
        void Awake()
        {
            character_list[player_id] = this;
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            auto_order = GetComponent<AutoOrderLayer>();
            distanceWeapon = GetComponent<DistanceWeapon>();
            meleeWeapon = GetComponent<MeleeWeapon>();
        }

        void OnDestroy()
        {
            character_list.Remove(player_id);
        }

        void Start()
        {
            stats = PlayerStats.GetPlayerStats(player_id);
            stats.resetStats();
            if (meleeWeapon.IsSelected() && meleeWeapon.IsAvailable())
            {
                stats.damage += meleeWeapon.GetEquippedWeapon().damageBonus;
            } else if (distanceWeapon.IsSelected() && distanceWeapon.IsAvailable())
            {
                stats.damage += distanceWeapon.GetEquippedWeapon().damageBonus;
            }
        }

        private void Update()
        {
            Debug.DrawLine(transform.position, GetMousePos(), Color.red);
        }

        //Handle physics
        void FixedUpdate()
        {   
            // Set MoveSpeed and change accel and deccel accordingly
            move_max = stats.MoveSpeed;
            move_accel = stats.MoveSpeed * 2;
            move_deccel = stats.MoveSpeed * 2;
            hp = stats.CurrentHealth;
            max_hp = stats.MaxHealth;
            //Movement velocity
            float desiredSpeedX = Mathf.Abs(move_input.x) > 0.1f ? move_input.x * move_max : 0f;
            float accelerationX = Mathf.Abs(move_input.x) > 0.1f ? move_accel : move_deccel;
            move.x = Mathf.MoveTowards(move.x, desiredSpeedX, accelerationX * Time.fixedDeltaTime);
            float desiredSpeedY = Mathf.Abs(move_input.y) > 0.1f ? move_input.y * move_max : 0f;
            float accelerationY = Mathf.Abs(move_input.y) > 0.1f ? move_accel : move_deccel;
            move.y = Mathf.MoveTowards(move.y, desiredSpeedY, accelerationY * Time.fixedDeltaTime);
            //move_input = Vector2.zero;

            //Move
            rigid.velocity = move;
        }

        //Handle render and controls
        void LateUpdate()
        {
            hit_timer += Time.deltaTime;
            //move_input = Vector2.zero;
            attackDown = false;

            //Controls
            if (!disable_controls)
            {
                //Controls
                PlayerControls controls = PlayerControls.Get(player_id);
                move_input = controls.GetMove();
                attackDown = controls.GetAttackDown();
                mouse_pos = controls.GetMousePos();

                weaponSwitch = controls.GetWeaponSwitch();
            }

            //Update lookat side
            if (move.magnitude > 0.1f)
                lookat = move.normalized;
            if (Mathf.Abs(lookat.x) > 0.02)
                side = Mathf.Sign(lookat.x);
            
        }

        public void HealDamage(float heal)
        {
            if (!is_dead)
            {
                hp += heal;
                hp = Mathf.Min(hp, max_hp);
            }
        }

        public void TakeDamage(float damage)
        {
            if (!is_dead && !invulnerable && hit_timer > 0f)
            {
                hp -= damage;
                hit_timer = -1f;

                if (hp <= 0f)
                {
                    Kill();
                }
                else
                {
                    if (onHit != null)
                        onHit.Invoke();
                }
            }
        }

        public void Kill()
        {
            if (!is_dead)
            {
                is_dead = true;
                rigid.velocity = Vector2.zero;
                move = Vector2.zero;
                move_input = Vector2.zero;

                if (onDeath != null)
                    onDeath.Invoke();
            }
        }
        
        public void Teleport(Vector3 pos)
        {
            transform.position = pos;
            move = Vector2.zero;
        }

        public Vector2 GetMove()
        {
            return move;
        }

        public Vector3 GetMousePos()
        {
            return mouse_pos;
        }
        public bool GetAttackDown()
        {
            return attackDown;
        }

        public bool GetWeaponSwitch()
        {
            return weaponSwitch;
        }

        public Vector2 GetFacing()
        {
            return lookat;
        }

        public int GetSortOrder()
        {
            return auto_order.GetSortOrder();
        }

        //Get Character side
        public float GetSide()
        {
            return side; //Return 1 frame before to let anim do transitions
        }

        public int GetSideAnim()
        {
            return (side >= 0) ? 1 : 3;
        }

        public bool IsDead()
        {
            return is_dead;
        }
        
        // Item / Weapon pickup logic
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
            {
                stats.PickupItem(other.gameObject.GetComponent<ItemPickup>().getItemData());
                
                Destroy(other.gameObject);
            } else if (other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                Weapon weapon = other.gameObject.GetComponent<WeaponPickup>().GetWeapon();

                if (weapon.type == 0)
                {   
                    distanceWeapon.equipWeapon(weapon);
                    if (!meleeWeapon.IsAvailable())
                    {
                        stats.damage += weapon.damageBonus; 
                    }
                } else if(weapon.type == 1)
                {
                    meleeWeapon.equipWeapon(weapon);
                    if (!distanceWeapon.IsAvailable())
                    {
                        stats.damage += weapon.damageBonus; 
                    }
                }
                
                Destroy(other.gameObject);
            }
        }
        

        public void DisableControls() { disable_controls = true; }
        public void EnableControls() { disable_controls = false; }
        
        public static PlayerCharacter GetNearest(Vector3 pos, float range = 999f, bool alive_only=true)
        {
            PlayerCharacter nearest = null;
            float min_dist = range;
            foreach (PlayerCharacter character in character_list.Values)
            {
                if (!alive_only || !character.IsDead())
                {
                    float dist = (pos - character.transform.position).magnitude;
                    if (dist < min_dist)
                    {
                        min_dist = dist;
                        nearest = character;
                    }
                }
            }
            return nearest;
        }

        public static PlayerCharacter Get(int player_id)
        {
            foreach (PlayerCharacter character in character_list.Values)
            {
                if (character.player_id == player_id)
                {
                    return character;
                }
            }
            return null;
        }

        public static PlayerCharacter[] GetAll()
        {
            PlayerCharacter[] list = new PlayerCharacter[character_list.Count];
            character_list.Values.CopyTo(list, 0);
            return list;
        }
    }
}
