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
    private Vector2 attackDirection;
    private float attackRange;
    private GameObject weaponBearer;
    
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
        audioSource.volume = PlayerPrefs.GetFloat("EffectVolume");
        if (attackDirection != null)
        {
            transform.position = weaponBearer.transform.position + (Vector3) attackDirection * 0.5f * attackRange;
        }
    }

    public void Attack(Vector2 direction, GameObject bearer, float range)
    {
        //animator.SetTrigger("Attack");
        attackRange = range;
        attackDirection = direction;
        weaponBearer = bearer;
        transform.rotation = Quaternion.Euler(0, 0, (float) Math.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    public void PlaySwingSound()
    {
        //audioSource.PlayOneShot(swingSounds[UnityEngine.Random.Range(0, swingSounds.Count)]);
        AudioSource.PlayClipAtPoint(swingSounds[UnityEngine.Random.Range(0, swingSounds.Count)], transform.position, PlayerPrefs.GetFloat("EffectVolume"));
    }

    public void PlayHitSound()
    {
        //audioSource.PlayOneShot(hitSounds[UnityEngine.Random.Range(0, hitSounds.Count)]);
        AudioSource.PlayClipAtPoint(hitSounds[UnityEngine.Random.Range(0, hitSounds.Count)], transform.position, PlayerPrefs.GetFloat("EffectVolume"));
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    
}