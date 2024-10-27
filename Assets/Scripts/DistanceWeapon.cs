using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessDescent
{
        public class DistanceWeapon : MonoBehaviour
    {
        public GameObject ProjectilePrefab;
        public bool Selected;
        private PlayerCharacter Character;
        private Projectile Projectile;
        public float Delay;
        public float ReUseTime;
        private float LastUse;
        private Animator PlayerAnimator;
        // Start is called before the first frame update
        void Start()
        {
            Character = GetComponent<PlayerCharacter>();
            PlayerAnimator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Selected == true && Character.GetAttackDown() == true)
            {
                //Instantiate(ProjectilePrefab, transform);
                //if (Time.time - LastUse < ReUseTime)
                //{
                LastUse = Time.time;
                Invoke("Use", Delay);
                //}
            }
        }

        private void Use()
        {
            Projectile = Instantiate(ProjectilePrefab, transform.position, transform.rotation).GetComponent<Projectile> ();
            Projectile.SetShooter(gameObject);
            Projectile.Shoot(Character.GetFacing());
        }
    }
}
