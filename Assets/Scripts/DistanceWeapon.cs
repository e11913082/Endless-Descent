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
        public float ProjectileOffsetX;
        public float ProjectileOffsetY;
        public float Delay;
        public float ReUseTime;
        private float LastUse;
        private CharacterAnim PlayerAnim;
        // Start is called before the first frame update
        void Start()
        {
            Character = GetComponent<PlayerCharacter>();
            PlayerAnim = GetComponent<CharacterAnim>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Selected == true && Character.GetAttackDown() == true)
            {
                if (Time.time - LastUse > ReUseTime)
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
            Projectile.Shoot(Character.GetFacing());
        }
    }
}
