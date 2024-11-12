using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePrefab : MonoBehaviour
{   
    
    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack(Vector2 direction)
    {
        //animator.SetTrigger("Attack");
        transform.rotation = Quaternion.Euler(0, 0, (float) Math.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    
}
