using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class SideBarRefresher : MonoBehaviour
{
    private const string LF = "\n";
    private TextMeshProUGUI statsTmp;
    private PlayerStats stats;
    private StringBuilder textStrB;
    // Start is called before the first frame update
    void Start()
    {
        statsTmp = GameObject.Find("Hud/SideBar/Stats").GetComponent<TextMeshProUGUI>();
        textStrB = new StringBuilder();
        stats = PlayerStats.GetPlayerStats(Hud.GetPlayerId());
    }

    // Update is called once per frame
    void Update()
    {
        refreshStats();
    }

    void refreshStats()
    {
        textStrB.Clear();
        textStrB.Append("Movespeed: ");
        textStrB.Append(stats.moveSpeed);
        textStrB.Append(LF);

        textStrB.Append("Atk: ");
        textStrB.Append(stats.damage);
        textStrB.Append(LF);

        textStrB.Append("Atkrange: ");
        textStrB.Append(stats.attackRange);
        textStrB.Append(LF);

        textStrB.Append("Atkspeed: ");
        textStrB.Append(stats.attackSpeed);
        textStrB.Append(LF);

        textStrB.Append("Fearinc: ");
        textStrB.Append(stats.fearIncrease);
        textStrB.Append(LF);

        textStrB.Append("Feardec: ");
        textStrB.Append(stats.fearDecrease);
        statsTmp.text = textStrB.ToString();
    }
}
