using EndlessDescent;
using UnityEngine;

public class CharacterWeaponPickup : MonoBehaviour
{
    private PlayerCharacter character;
    private CharacterWeaponInventory inventory;
    
    
    public bool inPickupRange = false;
    private GameObject weaponObject;
    
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<PlayerCharacter>();
        inventory = GetComponent<CharacterWeaponInventory>();
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
                Weapon weapon = weaponObject.GetComponent<WeaponPickup>().GetWeapon();
                if (inventory.pickupWeapon(weapon))
                {
                    Debug.Log("Picked up weapon: " + weapon.name);
                    Destroy(weaponObject);
                }
                else
                {
                    Debug.Log("Cannot pick up weapon, inventory full");
                }
            }
        }
    }

   /* private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {   
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null && rb.IsSleeping())
            {
                rb.WakeUp();
            }
            if (character.GetActionDown())
            {   
                Weapon weapon = other.GetComponent<WeaponPickup>().GetWeapon();
                if (inventory.pickupWeapon(weapon))
                {
                    Debug.Log("Picked up weapon: " + weapon.name);
                    Destroy(other.gameObject);
                }
                else
                {
                    Debug.Log("Cannot pick up weapon, inventory full");
                }
            }
            Debug.Log("In trigger ");
        }
        
    }*/
}
