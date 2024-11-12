using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

/// <summary>
/// Player controls for platformer demo
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace EndlessDescent
{

    public class PlayerControls : MonoBehaviour
    {
        public int player_id;
        public KeyCode left_key;
        public KeyCode right_key;
        public KeyCode up_key;
        public KeyCode down_key;
        public KeyCode action_key;
        public KeyCode attackKey;
        public KeyCode switchKey;

        private Vector2 move = Vector2.zero;
        private bool action_press = false;
        private bool action_hold = false;
        private bool attackPress = false;
        private bool weaponSwitch = false;
        
        // mouse logic
        private Vector3 mouse_pos = Vector3.zero;
        private Camera mainCamera;

        private static Dictionary<int, PlayerControls> controls = new Dictionary<int, PlayerControls>();

        void Awake()
        {
            controls[player_id] = this;
            mainCamera = Camera.main;
        }

        void OnDestroy()
        {
            controls.Remove(player_id);
        }

        void Update()
        {
            if (gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                return;
            }

            mouse_pos = Input.mousePosition;
            mouse_pos.z = Mathf.Abs(mainCamera.transform.position.z);
            mouse_pos = mainCamera.ScreenToWorldPoint(mouse_pos);
            
            move = Vector2.zero;
            action_hold = false;
            action_press = false;
            attackPress = false;
            weaponSwitch = false;

            if (Input.GetKey(left_key))
                move += -Vector2.right;
            if (Input.GetKey(right_key))
                move += Vector2.right;
            if (Input.GetKey(up_key))
                move += Vector2.up;
            if (Input.GetKey(down_key))
                move += -Vector2.up;
            if (Input.GetKey(action_key))
                action_hold = true;
            if (Input.GetKeyDown(action_key))
                action_press = true;
            if (Input.GetKeyDown(attackKey))
                attackPress = true;
            if (Input.GetKeyDown(switchKey))
                weaponSwitch = true;
            float move_length = Mathf.Min(move.magnitude, 1f);
            move = move.normalized * move_length;
        }


        //------ These functions should be called from the Update function, not FixedUpdate
        public Vector2 GetMove()
        {
            return move;
        }

        public void SetMove(Vector2 newMove)
        {
            move = newMove;
        }

        public bool GetActionDown()
        {
            return action_press;
        }

        public bool GetAttackDown()
        {
            return attackPress;
        }

        public void SetAttack(bool attack)
        {
            attackPress = attack;
        }

        public bool GetActionHold()
        {
            return action_hold;
        }

        public bool GetWeaponSwitch()
        {
            return weaponSwitch;
        }

        public Vector2 GetMousePos()
        {
            return mouse_pos;
        }

        public void SetMousePos(Vector2 newMousePos)
        {
            mouse_pos = newMousePos;
        }

        //-----------

        public static PlayerControls Get(int player_id)
        {
            foreach (PlayerControls control in GetAll())
            {
                if (control.player_id == player_id)
                {
                    return control;
                }
            }
            return null;
        }

        public static PlayerControls[] GetAll()
        {
            PlayerControls[] list = new PlayerControls[controls.Count];
            controls.Values.CopyTo(list, 0);
            return list;
        }

    }

}