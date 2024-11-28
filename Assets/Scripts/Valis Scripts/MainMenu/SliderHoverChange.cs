using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderHoverChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{   
    public TextMeshProUGUI settingText;
    public Image glow;
    public Color defaultColor;
    public Color hoverColor;
    
    
    public Material glowMaterial;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float speed = 2f;

    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        glow.enabled = true;
        settingText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        glow.enabled = false;
        settingText.color = defaultColor;
    }
    
    
    
    void Start()
    {
        glow.enabled = false;
    }
    
    private float t;
    
    void Update()
    {
        t += Time.deltaTime * speed;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(t) + 1f) / 2f);
        glowMaterial.SetFloat("_Intensity", intensity);
    }


    
}
