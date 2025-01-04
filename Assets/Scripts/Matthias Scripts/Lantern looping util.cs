using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class lightsourcelooping : MonoBehaviour
{
    private int lv = 0;

    private Light2D outherLight;

    private List<GameObject> lanterns;
    private List<Light2D> lantern_outherLights;
    private List<Light2D> lantern_innerLights;

    //multiplier for transitioning to lv1
    const float LANTERN_INNER_MULTIPLIER = 0.9f;
    const float LANTERN_OUTER_MULTIPLIER = 0.7f;
    const float OUTER_MULTIPLIER = 0.6f;

    //void Awake()
    //{
    //    lanterns = GetLanterns();
    //    lantern_outherLights = new List<Light2D>();
    //    lantern_innerLights = new List<Light2D>();

    //    ExtractLights(lanterns);
    //}


    public List<GameObject> GetLanterns() //searches for all small lamps in the gameobject its direct childs
    {
        List<GameObject> outputLanterns = new List<GameObject>();
        if (gameObject.name.Contains("small"))
        {
            outputLanterns.Append(gameObject);
        }
        foreach (Transform child in transform)
        {
            if (gameObject.name.Contains("small"))
            {
                outputLanterns.Append(gameObject);
            }
        }
        return outputLanterns;
    }

    public void ExtractLights(List<GameObject> inputLanterns)
    {
        foreach(GameObject lantern in inputLanterns) {
            lantern_outherLights.Append(lantern.GetComponent<Light2D>());
            lantern_innerLights.Append(lantern.transform.GetChild(0).GetComponent<Light2D>());
        }
        Transform outherLightTrans = transform.Find("outher light");
        if(outherLightTrans != null)
        {
            outherLight = outherLightTrans.GetComponent<Light2D>();
        }
    }

    public void SetLanternLv(int lv)
    {
        CircleCollider2D lightCollider = gameObject.GetComponent<CircleCollider2D>();
        if (lv == 1)
        {
            foreach (GameObject lantern in lanterns)
            {
                lantern.GetComponent<Animator>().SetFloat("lv", 1);
            }

            if (outherLight != null)
            {
                lightCollider.radius *= OUTER_MULTIPLIER;
                outherLight.pointLightOuterRadius *= OUTER_MULTIPLIER;
                outherLight.pointLightInnerRadius *= OUTER_MULTIPLIER;
                outherLight.intensity *= OUTER_MULTIPLIER;
            }
            else
            {
                lightCollider.radius *= LANTERN_OUTER_MULTIPLIER;
            }

            foreach (Light2D lanternLight in lantern_outherLights)
            {
                lanternLight.pointLightOuterRadius *= LANTERN_OUTER_MULTIPLIER;
                lanternLight.pointLightInnerRadius *= LANTERN_OUTER_MULTIPLIER;
                lanternLight.intensity *= LANTERN_OUTER_MULTIPLIER;
            }
            foreach (Light2D lanternLight in lantern_innerLights)
            {
                lanternLight.pointLightOuterRadius *= LANTERN_INNER_MULTIPLIER;
                lanternLight.pointLightInnerRadius *= LANTERN_INNER_MULTIPLIER;
                lanternLight.intensity *= LANTERN_INNER_MULTIPLIER;
            }
        }
        else if (lv == 2)
        {
            foreach(GameObject lantern in lanterns)
            {
                lantern.GetComponent<Animator>().SetFloat("lv", 2);
            }
            
            Destroy(lightCollider);
            if (outherLight != null)
            {
                outherLight.intensity = 0;
            }
            foreach (Light2D lanternLight in lantern_outherLights.Concat(lantern_innerLights))
            {
                lanternLight.intensity = 0;
            }
        }
    }
}
