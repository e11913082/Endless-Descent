using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System.Linq;

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
        public GameObject coinPrefab;
        public List<AudioClip> damageGrunts;
        public List<AudioClip> meleeDamageSounds;
        public AudioClip stepSound;
        public AudioClip dashSound;
        private Rigidbody2D rigid;
        private Animator animator;
        private AutoOrderLayer auto_order;
        private ContactFilter2D contact_filter;
            
        //Weapons
        private bool weaponSwitch;
        private bool action_down;
        private bool weaponDrop;
        
        private bool is_dead = false;
        private Vector2 move;
        private Vector2 move_input;
        private bool attackDown;
        private bool dashDown;
        
        private Vector2 lookat = Vector2.zero;
        private float side = 1f;
        private int sideAnim = 0;

        private bool disable_controls = false;
        private float hit_timer = 0f;

        private static Dictionary<int, PlayerCharacter> character_list = new Dictionary<int, PlayerCharacter>();

        // Mouse Aim
        private Vector2 mouse_pos = Vector2.zero;
        // Stats 
        private PlayerStats stats;
        private PlayerBuildupManager buildup_manager;
        private SpriteRenderer spriteRenderer;
        private bool movementEnabled = true;
        private AudioSource audioSource;
        private float lastStepTime;
        private float lastDashTime;
        private const float stepTime = 0.5f;
        private LayerMask tmpForceReceiveLayers;
        private LayerMask tmpForceSendLayers;

        // Other
        private HaloLogic halo;

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            auto_order = GetComponent<AutoOrderLayer>();
            buildup_manager = GetComponentInChildren<PlayerBuildupManager>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            audioSource = GetComponent<AudioSource>();
            halo = gameObject.GetComponent<HaloLogic>();
        }

        void OnDestroy()
        {
            character_list.Remove(player_id);
        }

        void Start()
        {
            if (PlayerPrefs.GetFloat("EffectVolume") == null || PlayerPrefs.GetFloat("EffectVolume")==0f)
            {
                PlayerPrefs.SetFloat("EffectVolume", 0.3f);
            }
            audioSource.volume = PlayerPrefs.GetFloat("EffectVolume");
            player_id = CharacterIdGenerator.GetCharacterId(gameObject, 0);
            character_list[player_id] = this;
            stats = PlayerStats.GetPlayerStats(player_id);
        }

        private void Update()
        {
            Debug.DrawLine(transform.position, GetMousePos(), Color.red);
            if (stats.currentFear >= stats.maxFear)
            {
                Debug.Log("Died from to much fear");
                Kill();
            }
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
                //float desiredSpeedX = Mathf.Abs(move_input.x) > 0.1f ? move_input.x * move_max : 0f;
                //float accelerationX = Mathf.Abs(move_input.x) > 0.1f ? move_accel : move_deccel;
                //move.x = Mathf.MoveTowards(move.x, desiredSpeedX, accelerationX * Time.fixedDeltaTime);
                //float desiredSpeedY = Mathf.Abs(move_input.y) > 0.1f ? move_input.y * move_max : 0f;
                //float accelerationY = Mathf.Abs(move_input.y) > 0.1f ? move_accel : move_deccel;
                //move.y = Mathf.MoveTowards(move.y, desiredSpeedY, accelerationY * Time.fixedDeltaTime);
                move = move_input.normalized * stats.moveSpeed;
                //move_input = Vector2.zero;

                //Move
                if (movementEnabled is true)
                {
                    rigid.velocity = move;
                }

                if (move.magnitude > 0.001 && Time.time - lastStepTime > stepTime / stats.moveSpeed && stepSound!=null)
                {
                    lastStepTime = Time.time;
                    audioSource.PlayOneShot(stepSound, 0.3f);
                }         
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
                dashDown = controls.GetDashDown();
                if (dashDown)
                {
                    Debug.Log("Dash");
                }
                mouse_pos = controls.GetMousePos();
                action_down = controls.GetActionDown();
                weaponSwitch = controls.GetWeaponSwitch();
                weaponDrop = controls.GetWeaponDrop();
            }

            if (dashDown)
            {
                Dash();
            }

            //Update lookat side
            if (move.magnitude > 0.001f)
                lookat = move.normalized;
                if (Mathf.Abs(lookat.x) > 0.02)
                    side = Mathf.Sign(lookat.x);
                sideAnim = CalculateSide(lookat);
        }

    public int CalculateSide(Vector2 direction)
    {
        float angle = Vector2.Angle(Vector2.up, direction);
        int lookSide = 1;

        if (angle < 45)
            {lookSide = 4;}
        else if (45 <= angle && angle <= 135 && direction.x > 0)
            {lookSide = 1;}
        else if (45 <= angle && angle <= 135 && direction.x < 0)
            {lookSide = 3;}
        else if (angle > 135)
            {lookSide = 2;}

        return lookSide;
    }

        public void HealDamage(float heal)
        {
            if (!is_dead)
            {
                stats.CurrentHealth += heal;
                stats.CurrentHealth = Mathf.Min(stats.CurrentHealth, max_hp);
            }
        }
        
        public void TakeDamage(float damage, Vector2 hitDirection, int weaponType)
        {
            if (!is_dead && !invulnerable) // && hit_timer > 0f)
            {
                spriteRenderer.color = Color.red;
                DamageSetBack(hitDirection);
                Invoke("ResetRenderColor", 0.1f);
                if (weaponType == 0)
                {
                }
                else if (weaponType == 1)
                {
                    audioSource.PlayOneShot(meleeDamageSounds[UnityEngine.Random.Range(0, meleeDamageSounds.Count)]);
                }

                hit_timer = -1f;
                if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    stats.CurrentHealth -= damage;

                    if (stats.CurrentHealth <= 0f)
                    {
                        Instantiate(coinPrefab, transform.position, Quaternion.identity);
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
                    audioSource.PlayOneShot(damageGrunts[UnityEngine.Random.Range(0, damageGrunts.Count)]);
                    LightTouch light_touch = GetComponent<LightTouch>();
                    
                    float damageToPlayer = damage * ((100f - stats.defense) / 100f);
                    
                    
                    stats.CurrentFear += damageToPlayer;
                    if (stats.currentFear >= stats.MaxFear)
                    {
                        Kill();
                    }
                    if (light_touch.inLight > 0)
                    {
                        buildup_manager.PauseBuildup();
                    }
                    
                    else
                    {
                        if (onHit != null)
                            onHit.Invoke();
                    }
                    halo.OnPlayerDamage();
                }
            }
        }

        private void DamageSetBack(Vector2 hitDirection)
        {
            movementEnabled = false;
            rigid.AddForce(hitDirection * 100f, ForceMode2D.Impulse);
            Invoke("EnableMovement", 0.05f);
        }

        private void EnableMovement()
        {
            movementEnabled = true;
        }
        private void ResetRenderColor()
        {
            spriteRenderer.color = Color.white;
        }
        private void SetVulnerable()
        {
            invulnerable = false;
        }
        
        private void ResetForceLayers()
        {
            List<CapsuleCollider2D> capsules = GetComponentsInChildren<CapsuleCollider2D>().Where(go => go.gameObject != this.gameObject).ToList();
            CapsuleCollider2D capsule = capsules[0];
            capsule.forceSendLayers = tmpForceSendLayers;
            capsule.forceReceiveLayers = tmpForceReceiveLayers;
        }
        private void Dash()
        {
            if (Time.time - lastDashTime > stats.dashCoolDown)
            {
                lastDashTime = Time.time;
                audioSource.PlayOneShot(dashSound);
                Color newColor = Color.white;
                newColor.a = 0.5f;
                spriteRenderer.color = newColor;
                movementEnabled = false;
                invulnerable = true;
                List<CapsuleCollider2D> capsules = GetComponentsInChildren<CapsuleCollider2D>().Where(go => go.gameObject != this.gameObject).ToList();
                CapsuleCollider2D capsule = capsules[0];
                tmpForceReceiveLayers = capsule.forceReceiveLayers;
                tmpForceSendLayers = capsule.forceSendLayers;
                capsule.forceReceiveLayers = ~LayerMask.GetMask("Enemy");
                capsule.forceSendLayers = ~LayerMask.GetMask("Enemy");
                Vector2 direction = move.magnitude > 0 ? move.normalized: Vector2.up; 
                rigid.AddForce(direction * 90f, ForceMode2D.Impulse);
                Invoke("EnableMovement", 0.1f);
                Invoke("ResetRenderColor", 0.1f);
                Invoke("SetVulnerable", 0.3f);
                Invoke("ResetForceLayers", 0.1f);
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
            return sideAnim;
            //return (side >= 0) ? 1 : 3;
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
