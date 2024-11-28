using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePrefab : MonoBehaviour
{   
    public List<AudioClip> swingSounds;
    public List<AudioClip> hitSounds;
    private Animator animator;
    private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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

    public void PlaySwingSound()
    {
        audioSource.PlayOneShot(swingSounds[UnityEngine.Random.Range(0, swingSounds.Count)]);
    }

    public void PlayHitSound()
    {
        audioSource.PlayOneShot(hitSounds[UnityEngine.Random.Range(0, hitSounds.Count)]);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    
}
