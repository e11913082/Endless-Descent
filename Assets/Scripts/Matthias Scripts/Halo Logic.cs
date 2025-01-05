using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HaloLogic : MonoBehaviour
{
    private readonly Color meleeAttackCol = new Color(1f, 1f, 1f);
    private readonly Color magicAttackCol = new Color(0.125f, 0f, 1f);
    private readonly Color hitCol = new Color(0.527f, 0f, 0.585f);

    private Light2D halo;
    private float origIntensity;
    private float origInnerRadius;
    private float origOuterRadius;
    private Color originalCol;

    private Coroutine currentFlickerCoroutine;

    void Start()
    {
        halo = transform.GetComponent<Light2D>();

        origIntensity = halo.intensity;
        origInnerRadius = halo.pointLightInnerRadius;
        origOuterRadius = halo.pointLightOuterRadius;
        originalCol = halo.color;
    }

    private void OnEnable()
    {
        EventManager.StartListening("PlayerAttack", OnMagicAttack);
        EventManager.StartListening("OnPLayerDamage", OnPlayerDamage);
    }

    private void OnDisable()
    {
        EventManager.StopListening("PlayerAttack", OnMagicAttack);
        EventManager.StopListening("OnPLayerDamage", OnPlayerDamage);
    }

    public void OnMagicAttack()
    {
        StopCurCoroutine();
        currentFlickerCoroutine = StartCoroutine(Flicker(0.5f, 0.8f, 4.55f, 1.35f, magicAttackCol));
    }

    public void OnMeleeAttack()
    {
        StopCurCoroutine();
        currentFlickerCoroutine = StartCoroutine(Flicker(0.5f, 0.2f, 1.5f, 1f, meleeAttackCol));
    }

    public void OnPlayerDamage()
    {
        StopCurCoroutine();
        currentFlickerCoroutine = StartCoroutine(Flicker(0.02f,0.17f, 4.5f, 0.75f, hitCol));
    }

    private const float DELTA = 0.01666f;
    IEnumerator Flicker(float waitBetween, float time, float maxIntensity, float sizeFactor, Color newCol) //waitBetween: relative fraction of total waiting time before halo build down
    {
        int timeSteps = (int)System.Math.Round((time * (1 - waitBetween) / 2) / DELTA);

        float intensityStep = (maxIntensity - halo.intensity) / timeSteps;
        float sizeStep = (halo.pointLightOuterRadius * sizeFactor - halo.pointLightOuterRadius) / timeSteps;
        for (int i = 1; i <= timeSteps; i++)
        {
            halo.intensity += intensityStep;

            halo.pointLightInnerRadius += sizeStep;
            halo.pointLightOuterRadius += sizeStep;
            halo.color = Color.Lerp(originalCol, newCol, timeSteps / i);

            yield return new WaitForSeconds(DELTA);
        }
        yield return new WaitForSeconds(time * waitBetween);
        for (int i = 1; i <= timeSteps; i++)
        {
            halo.intensity -= intensityStep;

            halo.pointLightInnerRadius -= sizeStep;
            halo.pointLightOuterRadius -= sizeStep;


            yield return new WaitForSeconds(DELTA);
        }
        halo.color = originalCol;
    }

    private void Reset()
    {

        halo.intensity = origIntensity;
        halo.pointLightInnerRadius = origInnerRadius;
        halo.pointLightOuterRadius = origOuterRadius;
        halo.color = originalCol;
    }

    private void StopCurCoroutine()
    {
        if (currentFlickerCoroutine != null)
        {
            StopCoroutine(currentFlickerCoroutine);
            currentFlickerCoroutine = null;
            Reset();
        }
    }
}