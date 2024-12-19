using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

namespace EndlessDescent
{
    [RequireComponent(typeof(PlayerCharacter))]
    [RequireComponent(typeof(Animator))]
    public class CharacterAnim : MonoBehaviour
    {
        private PlayerCharacter character;
        private CharacterHoldItem character_item;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        public bool flipSpriteOnTurn = false;
        private int side;
        private PlayerStats stats;

        void Awake()
        {
            character = GetComponent<PlayerCharacter>();
            character_item = GetComponent<CharacterHoldItem>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            stats = PlayerStats.GetPlayerStats(character.player_id);
        }

        void Start()
        {
            character.onDeath += AnimateDeath;
        }

        void LateUpdate()
        {
            //Anims
            if (!character.IsDead())
            {
                animator.SetFloat("AttackSpeed", stats.attackAnimationSpeed);
                float speed = character.GetMove().magnitude;
                animator.SetFloat("Speed", speed);
                if (speed > 0.001)
                {
                    side = character.GetSideAnim();
                }
                animator.SetInteger("Side", side);
            }

            if (flipSpriteOnTurn == true)
                spriteRenderer.flipX = (side == 3)? true : false;
            //if(character_item != null)
            //    animator.SetBool("Hold", character_item.GetHeldItem() != null);
        }

        public void AnimateAttack(int type, int attackSide)
        {
            if (!character.IsDead())
            {
                switch (type) {
                    case 0:
                        animator.SetInteger("AttackSide", attackSide);
                        animator.SetTrigger("AttackDistance");
                        break;

                    case 1:
                        animator.SetInteger("AttackSide", attackSide);
                        animator.SetTrigger("AttackMelee");
                        break;
                }

            }
        }

        public void AnimateDeath()
        {
            animator.SetTrigger("Die");
        }
        
    }

}