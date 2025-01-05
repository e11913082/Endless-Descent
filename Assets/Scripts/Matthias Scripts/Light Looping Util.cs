using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightLoopingUtil : MonoBehaviour
{
    private Light2D outerLight;
    private List<GameObject> lanterns;

    private CircleCollider2D lightCollider;

    //light multipliers for transitioning to lv1
    const float LANTERN_INNER_MULTIPLIER = 0.85f;
    const float LANTERN_OUTER_MULTIPLIER = 0.7f;
    const float OUTER_MULTIPLIER = 0.6f;
    const float LIGHT_INTENSITY = 0.45f;

    void Awake()
    {
        Transform outerLightTrans = transform.Find("outer light");

        if (outerLightTrans != null)
        {
            outerLight = outerLightTrans.GetComponent<Light2D>();
            lightCollider = outerLightTrans.GetComponent<CircleCollider2D>();
        }
        else
        {
            lightCollider = gameObject.GetComponent<CircleCollider2D>();
        }
    }

    public List<GameObject> GetLanterns() //searches for all small lamps in the gameobject its direct childs
    {
        List<GameObject> outputLanterns = new List<GameObject>();

        if (gameObject.name.Contains("small"))
        {
            outputLanterns.Add(gameObject);
        }
        foreach (Transform child in transform)
        {
            if (child.name.Contains("small"))
            {
                outputLanterns.Add(child.gameObject);
            }
        }
        return outputLanterns;
    }

    public void SetLanternLv(int lv)
    {
        lanterns = GetLanterns();

        if (lv == 1)
        {
            foreach (GameObject lantern in lanterns)
            {
                lantern.GetComponent<Animator>().SetInteger("lv", 1);
                Light2D lanternOuterLight = lantern.GetComponent<Light2D>();
                Light2D lanternInnerLight = lantern.transform.GetChild(0).GetComponent<Light2D>();

                lanternOuterLight.pointLightOuterRadius *= LANTERN_OUTER_MULTIPLIER;
                lanternOuterLight.pointLightInnerRadius *= LANTERN_OUTER_MULTIPLIER;
                lanternOuterLight.intensity *= LIGHT_INTENSITY;

                lanternInnerLight.pointLightOuterRadius *= LANTERN_INNER_MULTIPLIER;
                lanternInnerLight.pointLightInnerRadius *= LANTERN_INNER_MULTIPLIER;
                lanternInnerLight.intensity *= LIGHT_INTENSITY;
            }

            if (outerLight != null)
            {
                lightCollider.radius *= OUTER_MULTIPLIER;
                outerLight.pointLightOuterRadius *= OUTER_MULTIPLIER;
                outerLight.pointLightInnerRadius *= OUTER_MULTIPLIER;
                outerLight.intensity *= LIGHT_INTENSITY;
            }
            else
            {
                lightCollider.radius *= LANTERN_OUTER_MULTIPLIER;
            }
        }
        else if (lv == 2)
        {
            foreach(GameObject lantern in lanterns)
            {
                lantern.GetComponent<Animator>().SetInteger("lv", 2);
                lantern.GetComponent<Light2D>().intensity = 0;
                lantern.transform.GetChild(0).GetComponent<Light2D>().intensity = 0;
            }
            
            Destroy(lightCollider);
            if (outerLight != null)
            {
                outerLight.intensity = 0;
            }
        }
    }
}
