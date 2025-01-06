using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIdGenerator : MonoBehaviour
{
    private static int characterCount = 0;
    private static Dictionary<int, int> existingCharacterIds = new Dictionary<int, int>();

    
    void Awake()
    {
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static int GetCharacterId(GameObject callingObject, int hierarchyLayerDepth)
    {   
        GameObject currentObject = callingObject;

        for (int i = 0; i < hierarchyLayerDepth; i++)
        {
            currentObject = currentObject.transform.parent.gameObject;
        }

        int parentInstanceId = currentObject.GetInstanceID();

        if (existingCharacterIds.ContainsKey(parentInstanceId))
        {
            //print(existingCharacterIds[parentInstanceId]);
            return existingCharacterIds[parentInstanceId];
        }
        else
        {
            existingCharacterIds.Add(parentInstanceId, characterCount);
            characterCount++;
            return characterCount - 1;
        }
    }
}
