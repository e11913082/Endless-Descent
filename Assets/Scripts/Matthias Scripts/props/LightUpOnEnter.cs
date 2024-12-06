using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightUpnEnter : MonoBehaviour
{
    private Light2D[] childLights;
    private List<float> intensitySteps = new List<float>();
    private int numSteps = 10;
    void Start()
    {
        childLights = GetComponentsInChildren<Light2D>();
        foreach (Light2D light in childLights)
        {
            intensitySteps.Add(light.intensity/numSteps);
            light.intensity = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //TODO play secret sound
        if(other.CompareTag("Player"))
        {
            for(int i = 0; i < childLights.Count(); i++)
            {
                StartCoroutine(TurnUpLight(childLights[i], intensitySteps[i]));
            }
        }
    }

    IEnumerator TurnUpLight(Light2D light, float intesityStep)
    {
        for (int i = 0; i < numSteps; i++)
        {
            light.intensity += intesityStep;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
