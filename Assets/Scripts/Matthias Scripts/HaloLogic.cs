using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HaloLogic : MonoBehaviour
{
    private readonly Color meleeAttackCol = new Color(1f, 1f, 1f);
    private readonly Color magicAttackCol = new Color(0.125f, 0f, 1f);
    private readonly Color hitCol = new Color(0.527f, 0f, 0.585f);
    private readonly Color enemyBeforeMeleeCol = new Color(255f, 95f, 31f);
    private readonly Color summonEnemiesCol = new Color(102f, 0f, 102f);
    private Light2D halo;
    private float origIntensity;
    private float origInnerRadius;
    private float origOuterRadius;
    private Color originalCol;
    private float lastIntensity;
    private float lastInnerRadius;
    private float lastOuterRadius;
    private Color lastCol;
    public Coroutine currentFlickerCoroutine;

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
        SaveLast();
        StopCurCoroutine();
        currentFlickerCoroutine = StartCoroutine(Flicker(0.5f, 0.8f, 4.55f, 1.35f, magicAttackCol));
    }

    public void OnMeleeAttack()
    {
        SaveLast();
        StopCurCoroutine();
        currentFlickerCoroutine = StartCoroutine(Flicker(0.5f, 0.2f, 1.5f, 1f, meleeAttackCol));
    }

    public void BeforeEnemyMeleeAttack()
    {
        SaveLast();
        StopCurCoroutine();
        currentFlickerCoroutine = StartCoroutine(Flicker(0.5f, 0.1f, 0.3f, 0.5f, enemyBeforeMeleeCol));
    }

    public void OnSummonEnemies()
    {
        SaveLast();
        StopCurCoroutine();
        currentFlickerCoroutine = StartCoroutine(Flicker(0.5f, 0.16f, 0.5f, 0.5f, summonEnemiesCol));
    }

    public void OnPlayerDamage()
    {
        SaveLast();
        StopCurCoroutine();
        currentFlickerCoroutine = StartCoroutine(Flicker(0.02f,0.17f, 4.5f, 0.75f, hitCol));
    }

    private const float DELTA = 0.01666f;
    public IEnumerator Flicker(float waitBetween, float time, float maxIntensity, float sizeFactor, Color newCol) //waitBetween: relative fraction of total waiting time before halo build down
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
        
        ResetToLast();
    }

    private void Reset()
    {

        halo.intensity = origIntensity;
        halo.pointLightInnerRadius = origInnerRadius;
        halo.pointLightOuterRadius = origOuterRadius;
        halo.color = originalCol;
    }

    private void SaveLast()
    {
        lastIntensity = halo.intensity;
        lastInnerRadius = halo.pointLightInnerRadius;
        lastOuterRadius = halo.pointLightOuterRadius;
        lastCol = halo.color;        
    }
    private void ResetToLast()
    {

        halo.intensity = lastIntensity;
        halo.pointLightInnerRadius = lastInnerRadius;
        halo.pointLightOuterRadius = lastOuterRadius;
        halo.color = lastCol;
    }

    public void StopCurCoroutine()
    {
        if (currentFlickerCoroutine != null)
        {
            StopCoroutine(currentFlickerCoroutine);
            currentFlickerCoroutine = null;
            Reset();
        }
    }
}
