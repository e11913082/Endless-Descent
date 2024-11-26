using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SidebarRefresher : MonoBehaviour
{
    private const string LF = "\n";
    private const float delta = 0.1667f; //frame delta

    public int maximumDecimalPlaces = 1;
    private string stringMask; //used for limiting the decimal places without rounding

    private TextMeshProUGUI statsTmp;
    private TextMeshProUGUI statChangeTmp;

    private StringBuilder statStrB;
    private StringBuilder statChangeStrB; //displays stat adjustment on itempickup

    private PlayerStats stats;
    private UnityAction onItemPickup;
    
    private Coroutine fadeCoroutine;
    

    private void Awake()
    {
        onItemPickup = new UnityAction(ShowChangedStats);
    }

    void Start()
    {
        statsTmp = GameObject.Find("Hud/SideBar/Stats").GetComponent<TextMeshProUGUI>();
        statChangeTmp = GameObject.Find("Hud/SideBar/ChangeStats").GetComponent<TextMeshProUGUI>();

        statStrB = new StringBuilder();
        statChangeStrB = new StringBuilder();

        stats = PlayerStats.GetPlayerStats(Hud.GetPlayerId());
        stringMask = Hud.CreateStringMask(maximumDecimalPlaces);

        //set the appropriate size for stats-textmesh and backround
        //Vector2 preferredValues = statsTmp.GetPreferredValues();
        //GameObject.Find("Hud/SideBar/Stats/Background").GetComponent<RectTransform>().sizeDelta = preferredValues;
    }

    // Update is called once per frame
    void Update()
    {
        refreshStats();
    }

    private void OnEnable()
    {
        EventManager.StartListening("ItemPickup", onItemPickup);
    }

    private void OnDisable()
    {
        EventManager.StopListening("ItemPickup", onItemPickup);
    }


    //Dirty approach  TODO find a more modular but still performant solution for creating stat strings
    void refreshStats() 
    {
        statStrB.Clear();
        statStrB.Append("Movespeed: ");
        statStrB.Append(stats.moveSpeed.ToString(stringMask));
        statStrB.Append(LF);

        statStrB.Append("Atk: ");
        statStrB.Append(stats.damage.ToString(stringMask));
        statStrB.Append(LF);

        statStrB.Append("Atkrange: ");
        statStrB.Append(stats.attackRange.ToString(stringMask));
        statStrB.Append(LF);

        statStrB.Append("Atkspeed: ");
        statStrB.Append(stats.attackSpeed.ToString(stringMask));
        statStrB.Append(LF);

        statStrB.Append("Fearinc: ");
        statStrB.Append(stats.fearIncrease.ToString(stringMask));
        statStrB.Append(LF);

        statStrB.Append("Feardec: ");
        statStrB.Append(stats.fearDecrease.ToString(stringMask));
        statsTmp.text = statStrB.ToString();
    }

    void ShowChangedStats()
    {
        Dictionary<string, float> statChangesDict = stats.lastItem.GetAttributesDictionary();
        statChangeStrB.Clear();
        AppendStatChange("moveSpeed");
        AppendStatChange("damage");
        AppendStatChange("attackRange");
        AppendStatChange("attackSpeed");
        AppendStatChange("fearIncrease");
        AppendStatChange("fearDecrease");

        statChangeTmp.text = statChangeStrB.ToString();
        statChangeTmp.alpha = 1f; //make statChanges visible

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        fadeCoroutine = StartCoroutine(FadeOut(wait: 2));



        void AppendStatChange(string statName)
        {
            if (statChangesDict.TryGetValue(statName, out float statModifier))
            {
                if (statModifier > 0)
                {
                    statChangeStrB.Append("<color=green>+");
                    statChangeStrB.Append(statModifier.ToString(stringMask));
                    
                }
                else
                {
                    statChangeStrB.Append("<color=red>");
                    statChangeStrB.Append(statModifier.ToString(stringMask));
                }

                statChangeStrB.Append("%");
                statChangeStrB.Append("</color>");
            }
            
            statChangeStrB.Append(LF);
        }

        IEnumerator FadeOut(int wait) //fades out statChanged string after "wait" seconds
        {
            yield return new WaitForSeconds(wait);
            for(float alpha = statChangeTmp.alpha; alpha >= -1f; alpha -= 0.2f)
            {
                statChangeTmp.alpha = alpha;
                yield return new WaitForSeconds(delta);
            }
        }
    }
}
