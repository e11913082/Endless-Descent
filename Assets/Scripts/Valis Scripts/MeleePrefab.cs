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
       // StartCoroutine(DestroyAfterAnimation());
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private IEnumerator DestroyAfterAnimation()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        while (!stateInfo.IsName("swordslash"))
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
        
        yield return new WaitForSeconds(stateInfo.length);
        
        Destroy(gameObject);
    }
}
