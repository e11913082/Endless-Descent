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
        // Start is called before the first frame update
        void Start()
        {

        }

        void Awake()
        {
            Rigid = GetComponent<Rigidbody2D> ();
            Animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(CreationSound);

            destructionLayer = (1 << LayerMask.NameToLayer("Props"));
        }

        // Update is called once per frame
        void Update()
        {

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
                    collision.gameObject.GetComponent<PlayerCharacter>().TakeDamage(damage, shootDirection);
                }
                
                Rigid.velocity = Vector2.zero;
                Animator.SetBool("Hit", true);
                audioSource.Stop();
                AudioSource.PlayClipAtPoint(DestructionSound, transform.position, 1000);
                Invoke("DestroyProjectile", DestructionDelay); 
            }
            else if((destructionLayer.value & (1 << collision.collider.gameObject.layer)) != 0)
            {
                Debug.Log("hit");
                Rigid.velocity = Vector2.zero;
                Animator.SetBool("Hit", true);
                audioSource.Stop();
                AudioSource.PlayClipAtPoint(DestructionSound, transform.position, 1000);
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
