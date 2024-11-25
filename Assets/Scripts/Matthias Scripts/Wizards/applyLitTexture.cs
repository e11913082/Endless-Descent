#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class ChangeChildTexturesWizard : ScriptableWizard
{
    private GameObject searchFolder;
    public Material litMaterial;
    private string materialPath = "Assets/Sprites/Sprite-Lit-Default.mat";

    public List<string> excludedMaterial = new List<string> {"MT FX Rune Glow"};      //*add excluded material here*
    public List<string> excludedGameObjects = new List<string>();   //*add excluded props here*

    [MenuItem("Tools/Change Child Textures to Lit")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ChangeChildTexturesWizard>(
            "Change Childmaterial to Lit", "Apply Changes");
    }
    private void OnEnable()
    {
        litMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
    }
    private void OnWizardCreate()
    {
        if(searchFolder == null)
        {
            GameObject mapObject = GameObject.Find("Map");
            Transform propsTransform = mapObject.transform.Find("Props");
            searchFolder = propsTransform.gameObject;
        }
        Renderer[] childRenderers = searchFolder.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in childRenderers)
        {
            if (renderer.sharedMaterial != null && 
                renderer.sharedMaterial != litMaterial &&
                !excludedMaterial.Contains(renderer.sharedMaterial.name) && 
                !isOrHasParent(renderer))
            {
                Debug.Log("Prop: " + renderer.name  +  "Old mat:" +  renderer.sharedMaterial.name);
                renderer.material = litMaterial;
            }
        }
        Debug.Log("Lit material assigned to all props");
    }

    private void OnWizardUpdate()
    {
        saveSettings();
    }

    private Boolean isOrHasParent(Renderer renderer)
    {
        Transform curObj = renderer.transform.parent;
        while (curObj != null)
        {
            if(excludedGameObjects.Contains(renderer.name))
            {
                return true;
            }
            curObj = curObj.parent;
        }
        return false;
    }

    private void saveSettings()
    {
        string excludedGameObjectsStr = string.Join(",", excludedGameObjects);
        EditorPrefs.SetString("ExcludedGameObjects", excludedGameObjectsStr);

        string excludedMaterialsStr = string.Join(",", excludedMaterial);
        EditorPrefs.SetString("ExcludedGameMaterial", excludedMaterialsStr);
    }

    private void loadSettings()
    {
        string excludedGameObjectsStr = EditorPrefs.GetString("ExcludedGameObjects", "");
        foreach (var ObjName in excludedGameObjectsStr.Split(','))
        {
            excludedGameObjects.Add(ObjName);
        }
        

        string excludedMaterialStr = EditorPrefs.GetString("ExcludedGameMaterial", "");
        foreach (var materialName in excludedMaterialStr.Split(','))
        {
            excludedGameObjects.Add(materialName);
        }
    }
}
#endif
