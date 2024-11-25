using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
        
        // can be private or removed
        
        private float max_hp;
        
        
        // can be moved to playerstats
        [Header("Status")]
        public bool invulnerable = false;

        public UnityAction onDeath;
        public UnityAction onHit;

        private Rigidbody2D rigid;
        private Animator animator;
        private AutoOrderLayer auto_order;
        private ContactFilter2D contact_filter;
            
        //Weapons
        private bool weaponSwitch;
        private bool action_down;
        private bool weaponDrop;
        private MeleeWeapon meleeWeapon;
        
        private bool is_dead = false;
        private Vector2 move;
        private Vector2 move_input;
        private bool attackDown;
        
        private Vector2 lookat = Vector2.zero;
        private float side = 1f;

        private bool disable_controls = false;
        private float hit_timer = 0f;

        private static Dictionary<int, PlayerCharacter> character_list = new Dictionary<int, PlayerCharacter>();

        // Mouse Aim
        private Vector2 mouse_pos = Vector2.zero;
        // Stats 
        private PlayerStats stats;
        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            auto_order = GetComponent<AutoOrderLayer>();
            meleeWeapon = GetComponent<MeleeWeapon>();
            
            player_id = CharacterIdGenerator.GetCharacterId(gameObject, 0);
            character_list[player_id] = this;
            stats = PlayerStats.GetPlayerStats(player_id);
        }

        void OnDestroy()
        {
            character_list.Remove(player_id);
        }

        void Start()
        {
            max_hp = stats.MaxHealth;
        }

        private void Update()
        {
            Debug.DrawLine(transform.position, GetMousePos(), Color.red);
        }

        //Handle physics
        void FixedUpdate()
        {   
            // Set MoveSpeed and change accel and deccel accordingly
            if (!is_dead)
            {
                float move_max = stats.MoveSpeed;
                float move_accel = stats.MoveSpeed * 2;
                float move_deccel = stats.MoveSpeed * 2;
                //update hp and max_hp
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
                action_down = controls.GetActionDown();
                weaponSwitch = controls.GetWeaponSwitch();
                weaponDrop = controls.GetWeaponDrop();
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
                stats.CurrentHealth += heal;
                stats.CurrentHealth = Mathf.Min(stats.CurrentHealth, max_hp);
            }
        }
        
        
        
        public void TakeDamage(float damage)
        {
            print("taking damage");
            if (!is_dead && !invulnerable && hit_timer > 0f)
            {
                hit_timer = -1f;
                if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    stats.CurrentHealth -= damage;

                    if (stats.CurrentHealth <= 0f)
                    {
                        print("kill");
                        Kill();
                    }
                    else
                    {
                        if (onHit != null)
                            onHit.Invoke();
                    }
                }
                else if (gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    stats.CurrentFear += damage;

                    if (stats.currentFear >= stats.MaxFear)
                    {
                        print("kill");
                        Kill();
                    }
                    else
                    {
                        if (onHit != null)
                            onHit.Invoke();
                    }
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

        public bool GetWeaponDrop()
        {
            return weaponDrop;
        }

        public bool GetActionDown()
        {
            return action_down;
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

        public PlayerStats GetStats()
        {
            return stats;
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
