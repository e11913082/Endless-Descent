using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightGlow : MonoBehaviour
{
    private Light2D light2D;
    private float startRadius;
    private float startIntensity;
    public float maxDifference = 0.5f;
    
    public float radiusAmplitude = 0.5f; 
    public float radiusFrequency = 1f; 
    
    public float intensityAmplitude = 0.3f;
    public float intensityFrequency = 1f;
    void Start()
    {
        light2D = GetComponent<Light2D>();
        //StartCoroutine(LightGlowCoroutine());
        
        startRadius = light2D.pointLightOuterRadius;
        startIntensity = light2D.intensity;
    }
    
    void Update()
    {
        if(startIntensity != 0)
        {
            float time = Time.time;

            // Calculate new radius
            float radiusOffset = Mathf.Sin(time * radiusFrequency * 2 * Mathf.PI) * radiusAmplitude;
            light2D.pointLightOuterRadius = startRadius + radiusOffset;

            // Calculate new intensity
            float intensityOffset = Mathf.Sin(time * intensityFrequency * 2 * Mathf.PI) * intensityAmplitude;
            light2D.intensity = startIntensity + intensityOffset;
        }
    }

    private IEnumerator LightGlowCoroutine()
    {
        
        while (true)
        {
            light2D.pointLightOuterRadius = startRadius + Random.Range(0, maxDifference);
            yield return new WaitForSeconds(Random.Range(0.3f,0.7f));
        } 
    }
}
