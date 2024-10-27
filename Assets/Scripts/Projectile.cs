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
        // Start is called before the first frame update
        void Start()
        {

        }

        void Awake()
        {
            Rigid = GetComponent<Rigidbody2D> ();
            Animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Shoot(Vector2 direction)
        {
            Rigid.velocity = direction * Speed;
            transform.rotation = Quaternion.Euler(0, 0, (float) Math.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.gameObject != Shooter) 
            {
                Rigid.velocity = Vector2.zero;
                Animator.SetBool("Hit", true);
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
