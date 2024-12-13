using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class SidebarRefresher : MonoBehaviour
{
    public int maximumDecimalPlaces = 1;
    public Sprite upArrowGreen;
    public Sprite downArrowGreen;
    public Sprite downArrowRed;

    private const string LF = "\n";
    private const float DELTA = 0.01667f; //frame delta
    private readonly float VERTSTATPADDING = 0.08f * Screen.height; //vetical distance between statChanges | measured in proportion to the screenheigth
    
    private string stringMask; //used for limiting the decimal places without rounding

    private StringBuilder statChangeStrB; //displays stat adjustment on itempickup

    private PlayerStats stats;
    private UnityAction onItemPickup;
    
    private GameObject sampleStatChange;

    


    private static Vector2 statPosition;

    private void Awake()
    {
        onItemPickup = new UnityAction(ShowChangedStats);
    }

    void Start()
    {
        statChangeStrB = new StringBuilder();

        stats = PlayerStats.GetPlayerStats(Hud.GetPlayerId());
        stringMask = Hud.CreateStringMask(maximumDecimalPlaces);

        sampleStatChange = GameObject.Find("Hud V2/SideBar/SampleStatChange");
        sampleStatChange.SetActive(false);
        statPosition = sampleStatChange.transform.position;
    }
   

    private void OnEnable()
    {
        EventManager.StartListening("ItemPickup", onItemPickup);
    }

    private void OnDisable()
    {
        EventManager.StopListening("ItemPickup", onItemPickup);
    }

    void ChangeAlphaOfStatChange(GameObject parentObj, float alpha)
    {
        Image image = parentObj.GetComponentInChildren<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

        TextMeshProUGUI text = parentObj.GetComponentInChildren<TextMeshProUGUI>();
        text.alpha = alpha;
    }

    void ShowChangedStats()
    {
        foreach (KeyValuePair<string, float> statChange in stats.lastItem.GetAttributesDictionary())
        {
            GameObject newStatChange = CreateStatChange(statChange.Key, statChange.Value);
            StartCoroutine(DisplayStatChange(newStatChange));
        }
    }

    GameObject CreateStatChange(string name, float value)
    {
        GameObject outputObj = Instantiate(sampleStatChange);
        TextMeshProUGUI statsText = outputObj.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        StringBuilder textSb = new StringBuilder();

        if (value > 0)
        {
            textSb.Append("<color=green>+");
            outputObj.transform.Find("Image").GetComponent<Image>().sprite = upArrowGreen;
        }
        else if (name == "fearIncrease") // value <= 0
        {
            textSb.Append("<color=green>");
            outputObj.transform.Find("Image").GetComponent<Image>().sprite = downArrowGreen;
        }
        else
        {
            textSb.Append("<color=red>");
            outputObj.transform.Find("Image").GetComponent<Image>().sprite = downArrowRed;
        }

        textSb.Append(value.ToString(stringMask));
        textSb.Append("% ");
        textSb.Append(LF);
        textSb.Append(name);
        statChangeStrB.Append("</color>");

        statsText.text = textSb.ToString();
        return outputObj;
    }

    
    IEnumerator DisplayStatChange(GameObject statChange)
    {
        statChange.transform.SetParent(gameObject.transform, false);
        statChange.transform.position = statPosition;
        statPosition.y -= VERTSTATPADDING;
        ChangeAlphaOfStatChange(statChange, 0);
        statChange.SetActive(true);
        for (float alpha = 0; alpha <= 1; alpha += 0.15f)
        {
            ChangeAlphaOfStatChange(statChange, alpha);
            yield return new WaitForSeconds(DELTA);
        }
        yield return new WaitForSeconds(3f);
        for(float alpha = 1; alpha >= 0; alpha -= 0.05f)
        {
            ChangeAlphaOfStatChange(statChange, alpha);
            yield return new WaitForSeconds(DELTA);
        }
        Destroy(statChange);
        statPosition.y += VERTSTATPADDING;
    }
}
