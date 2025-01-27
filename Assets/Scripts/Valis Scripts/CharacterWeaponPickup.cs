using System.Collections;
using EndlessDescent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterWeaponPickup : MonoBehaviour
{
    private PlayerCharacter character;
    private CharacterWeaponInventory inventory;
    
    
    public bool inPickupRange = false;
    private GameObject weaponObject;

    public int weaponHintCounter = 0;
    public int weaponHintMaxCounter = 3;
    private TextMeshProUGUI hintText;
    
    private Coroutine hintCoroutine = null;
    private bool hintActive = false;

    private Image panelImage;
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<PlayerCharacter>();
        inventory = GetComponent<CharacterWeaponInventory>();
        hintText = GameObject.Find("/HintCanvas").GetComponentInChildren<TextMeshProUGUI>(true);
        panelImage = hintText.gameObject.transform.parent.gameObject.GetComponent<Image>();
        hintText.gameObject.transform.parent.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {   
            weaponObject = other.gameObject;
            inPickupRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {   
            weaponObject = null;
            inPickupRange = false;
        }
    }
    
    private void Update()
    {
        if (inPickupRange)
        {
            if (character.GetActionDown())
            {
                WeaponPickup pickup = weaponObject.GetComponent<WeaponPickup>();
                Weapon weapon = pickup.GetWeapon();
                if (inventory.pickupWeapon(weapon))
                {
                    Debug.Log("Picked up weapon: " + weapon.name);
                    pickup.StartPickupAnimation();
                }
                else
                {
                    Debug.Log("Cannot pick up weapon, inventory full");
                }
            }

            /*if (weaponHintCounter < weaponHintMaxCounter && !hintActive)
            {   
                hintActive = true;
                if (hintCoroutine != null)
                {
                    StopCoroutine(hintCoroutine);
                }
                hintCoroutine = StartCoroutine(Hint());
            }*/
        }
    }

    private IEnumerator Hint()
    {
        yield return new WaitForSeconds(4f);
        if (inPickupRange)
        {   
            Debug.Log("Hint triggered");
            hintText.gameObject.transform.parent.gameObject.SetActive(true);
            // readjust alpha
            hintText.alpha = 1f;
            Color color = panelImage.color;
            color.a = 1f;
            panelImage.color = color;
            
            string inputKey = PlayerPrefs.GetString("interact", "E");
            hintText.text = "Press " + inputKey + " to pickup the weapon!";
            FadeOut();
        }
        
    }
    
    private Coroutine fadeCoroutine = null;
    
    private void FadeOut()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOut(1));
    }
    
    private IEnumerator FadeOut(int wait)
    {   
        
        
        yield return new WaitForSeconds(wait);
        
        Color panelColor = panelImage.color;
        float fadeDuration = 3f;
        float fadeInterval = 0.1667f;
        
        float panelStartAlpha = panelImage.color.a;
        float hintTextStartAlpha = hintText.alpha;
        
        for (float t = 0; t <= fadeDuration; t += fadeInterval)
        {
            float normalizedTime = t / fadeDuration;
            
            float currentPanelAlpha = Mathf.Lerp(panelStartAlpha, 0, normalizedTime);
            float currentHintTextAlpha = Mathf.Lerp(hintTextStartAlpha, 0, normalizedTime);
            
            panelColor.a = currentPanelAlpha;
            panelImage.color = panelColor;
            
            hintText.alpha = currentHintTextAlpha;
            
            yield return new WaitForSeconds(fadeInterval);
        }
        hintActive = false;
        weaponHintCounter++;
        hintText.gameObject.transform.parent.gameObject.SetActive(false);
        
    }
    
}
