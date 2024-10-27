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

        void Update()
        {
            //Anims
            animator.SetFloat("Speed", character.GetMove().magnitude);
            animator.SetBool("Attack", character.GetAttackDown());
            side = character.GetSideAnim();
            animator.SetInteger("Side", side);
            if (flipSpriteOnTurn == true)
                spriteRenderer.flipX = (side == 3)? true : false;
            //if(character_item != null)
            //    animator.SetBool("Hold", character_item.GetHeldItem() != null);
        }

        //public void AnimateAttack()
        //{
        //    animator.SetBool("Attack", true);
        //}
        
    }

}