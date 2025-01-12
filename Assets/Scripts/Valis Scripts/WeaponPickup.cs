using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    
    public Weapon weapon;
    private TextMeshProUGUI textGUI;
    public Texture2D defaultCursor;
    public Texture2D cursor;
    private CursorMode cursorMode = CursorMode.Auto;
    private GameObject canvas;
    private SpriteRenderer spriteRenderer;
    
    // pickup "animation"
    public Transform hudTarget;
    private bool isBeingCollected = false;
    private Camera mainCamera;
    private Vector3 originalScale;
    
    public ParticleSystem particleSystem;
    private ParticleSystem particlesSystemInstance;

    [SerializeField] private float hopHeight = 1.0f;
    [SerializeField] private float animationDuration = 0.5f;
    
    
    private void Start()
    {   
        mainCamera = Camera.main;
        originalScale = transform.localScale;
        //particleSystem = GetComponentInChildren<ParticleSystem>();

        hudTarget = GameObject.FindGameObjectWithTag("Hud").transform.Find("Inventory").transform.Find("SampleInvSlot");
        spriteRenderer = GetComponent<SpriteRenderer>();
        canvas = GameObject.Find("/HoverCanvas");
        textGUI = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        if (textGUI == null)
        {
            Debug.LogWarning("No text renderer");
        }
        
        if (weapon == null)
        {
            Debug.LogWarning("no weapon assigned");
        }
        else
        {   
            spriteRenderer.sprite = weapon.sprite;
            textGUI.gameObject.transform.parent.gameObject.SetActive(false);
            textGUI.text = weapon.description;
        }
    }
    
    

    
    // called when the weapon is dropped for initialization of text and sprite
    public void InitializeDropped(Weapon weapon)
    {   
        mainCamera = Camera.main;
        originalScale = transform.localScale;
        
        this.weapon = weapon;
        canvas = GameObject.Find("/HoverCanvas");
        textGUI = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        gameObject.layer = LayerMask.NameToLayer("Weapon");
        hudTarget = GameObject.FindGameObjectWithTag("Hud").transform.Find("Inventory").transform.Find("SampleInvSlot");

        // initialize sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Props";
        // Set up visuals based on the weapon details
        spriteRenderer.sprite = weapon.sprite;
        textGUI.text = weapon.description;
        textGUI.gameObject.transform.parent.gameObject.SetActive(false);
        
        transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
    }
    
    
    public Weapon GetWeapon()
    {
        return weapon;
    }
    
    private void OnMouseEnter()
    {
        if (textGUI != null)
        {   
            Cursor.SetCursor(cursor, Vector2.zero, cursorMode);
            
            textGUI.gameObject.transform.parent.parent.position = new Vector3(transform.position.x, transform.position.y+1, 0);
            textGUI.gameObject.transform.parent.gameObject.SetActive(true);
            textGUI.text = "Weapon:\n" + weapon.description;
        }
        else
        {
            Debug.LogWarning("No text renderer OnMouseEnter");
        }
        
    }

    private void OnMouseExit()
    {
        if (textGUI != null)
        {   
            Cursor.SetCursor(defaultCursor, Vector2.zero, cursorMode);
            
            textGUI.gameObject.transform.parent.gameObject.SetActive(false);
        }
        
    }

    private void OnDestroy()
    {
        if (textGUI != null)
        {
            textGUI.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
    
    public void StartPickupAnimation()
    {
        if (!isBeingCollected)
        {
            StartCoroutine(PickupAnimation());
        }
    }

    private IEnumerator PickupAnimation()
    {
        isBeingCollected = true;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 peakPosition = startPosition + Vector3.up * hopHeight;
        
        Vector3 hudWorldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(hudTarget.position.x, hudTarget.position.y, Mathf.Abs(mainCamera.transform.position.z))
        );
        
        hudWorldPosition.z = 0;
        
        Debug.Log($"Start Position: {startPosition}");
        Debug.Log($"HUD World Position: {hudWorldPosition}");
        
        Vector3 movementDirection = (hudWorldPosition - (transform.position + Vector3.up * hopHeight)).normalized;
        SpawnParticles(Quaternion.LookRotation(-movementDirection));
        
        spriteRenderer.sortingLayerName = "Top";
        
        float lastTriggeredStep = -1;
        while (elapsedTime < animationDuration)
        {   
            hudWorldPosition = mainCamera.ScreenToWorldPoint(
                new Vector3(hudTarget.position.x, hudTarget.position.y, Mathf.Abs(mainCamera.transform.position.z))
            );
        
            hudWorldPosition.z = 0;
            movementDirection = (hudWorldPosition - (transform.position)).normalized;
            particlesSystemInstance.transform.rotation = Quaternion.LookRotation(-movementDirection);
            
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / animationDuration;
            
            int currentStep = Mathf.FloorToInt(normalizedTime * 10);
            
            if (normalizedTime <= 0.5f)
            {
                transform.position = Vector3.Lerp(startPosition, peakPosition, normalizedTime * 2f);
            }
            else
            {
                transform.position = Vector3.Lerp(peakPosition, hudWorldPosition, (normalizedTime - 0.5f) * 2f);
                if (currentStep % 5 == 0 && currentStep != lastTriggeredStep)
                { 
                   lastTriggeredStep = currentStep;
                   Debug.Log("Start Particles");
                   
                }
                //SpawnParticles(Quaternion.LookRotation(-movementDirection));
            }
            
            //transform.localScale = Vector3.Lerp(originalScale, originalScale / 2, normalizedTime);
            
            yield return null;
        }
        
        transform.position = hudWorldPosition;
        EventManager.TriggerEvent("InventoryChange");
        Destroy(gameObject);
    }

    private void SpawnParticles(Quaternion rotation)
    {
        particlesSystemInstance = Instantiate(particleSystem, transform.position, rotation);
        particlesSystemInstance.Play();
        particlesSystemInstance.transform.parent = gameObject.transform;
    }
    
}
