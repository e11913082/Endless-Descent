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

        void Awake()
        {
            character = GetComponent<PlayerCharacter>();
            character_item = GetComponent<CharacterHoldItem>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
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
                animator.SetFloat("Speed", character.GetMove().magnitude);
                side = character.GetSideAnim();
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