using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

namespace EndlessDescent
{
    public class Projectile : MonoBehaviour
    {
        public float Speed;
        public float Range = 4f;
        private Rigidbody2D Rigid;
        private Animator Animator;
        public float DestructionDelay;
        private GameObject Shooter;
        private AudioSource audioSource;
        public AudioClip CreationSound;
        public AudioClip DestructionSound;
        public LayerMask targetLayer;
        private LayerMask destructionLayer; 
        private Vector2 shootDirection;
        private Vector2 origin;
        private float effectVolume;
        // Start is called before the first frame update
        void Start()
        {
            effectVolume = PlayerPrefs.GetFloat("EffectVolume", 0.5f);
            audioSource.volume = effectVolume;
            audioSource.PlayOneShot(CreationSound);
        }

        void Awake()
        {
            Rigid = GetComponent<Rigidbody2D> ();
            Animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            origin = transform.position;
            destructionLayer = LayerMask.GetMask("Props");
        }

        // Update is called once per frame
        void Update()
        {
            if (((Vector2) transform.position - origin).magnitude > Range)
            {
                Destroy(gameObject);
            }
        }

        public void Shoot(Vector2 direction, LayerMask target)
        {
            Rigid.velocity = direction * Speed;
            transform.rotation = Quaternion.Euler(0, 0, (float) Math.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            targetLayer = target;
            shootDirection = direction;
            Destroy(gameObject, 10f);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            //if (collision.collider.gameObject != Shooter) 
            if (collision.collider.gameObject.layer == targetLayer) 
            {   
                // enemy hit logic
                PlayerCharacter player = Shooter.GetComponent<PlayerCharacter>();
                float damage = PlayerStats.GetPlayerStats(player.player_id).damage;

                if (collision.gameObject.layer == targetLayer)
                {
                    collision.gameObject.GetComponent<PlayerCharacter>().TakeDamage(damage, shootDirection, 0);
                }
                Rigid.velocity = Vector2.zero;
                Animator.SetBool("Hit", true);
                audioSource.Stop();
                AudioSource.PlayClipAtPoint(DestructionSound, transform.position, effectVolume);
                Invoke("DestroyProjectile", DestructionDelay); 
            }
            //destroys projectile on collision "Props" layer
            else if((destructionLayer.value & (1 << collision.collider.gameObject.layer)) != 0) 
            {
                Rigid.velocity = Vector2.zero;
                Animator.SetBool("Hit", true);
                audioSource.Stop();
                AudioSource.PlayClipAtPoint(DestructionSound, transform.position, effectVolume);
                Invoke("DestroyProjectile", DestructionDelay);
            }
           
        }

        private void DestroyProjectile()
        {
            Destroy(gameObject);
        }

        public void SetShooter(GameObject shooter)
        {
            Shooter = shooter;
        }

    }
}
