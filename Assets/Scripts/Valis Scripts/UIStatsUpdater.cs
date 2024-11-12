using System.Collections;
using System.Collections.Generic;
using EndlessDescent;
using TMPro;
using UnityEngine;

public class UIStatsUpdater : MonoBehaviour
{   
    private const string LF = "\n";
    
    public int player_id;
    private PlayerCharacter character;
    private PlayerStats stats;

    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        character = PlayerCharacter.Get(player_id);
        stats = PlayerStats.GetPlayerStats(player_id);
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Player Stats" + LF +
                    "maxHealth: " + stats.MaxHealth + LF +
                    "currentHealth: " + stats.currentHealth + LF +
                    "moveSpeed: " + stats.moveSpeed + LF +
                    "damage: " + stats.damage + LF +
                    "attackRange: " + stats.attackRange + LF +
                    "attackSpeed: " + stats.attackSpeed + LF +
                    "maxFear: " + stats.maxFear + LF +
                    "currentFear: " + stats.currentFear + LF +
                    "fearIncrease: " + stats.fearIncrease + LF +
                    "fearDecrease: " + stats.fearDecrease;

    }
}
