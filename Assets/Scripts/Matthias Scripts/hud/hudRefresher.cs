using UnityEngine;
using EndlessDescent;
using System.Text;

public class hudRefresher : MonoBehaviour
{
    public int player_id = 0;
    private PlayerStats stats;
    private PlayerCharacter player;

    void Awake()
    {
        
    }

    private void Start()
    {
        player = PlayerCharacter.Get(player_id);
        stats = player.GetStats();
    }

    public PlayerStats getStats()
    {
        return stats;
    }

    public PlayerCharacter getCharacter()
    {
        return player;
    }

    public static string createStringMask(int decimalNum) //used by subcomponents for setting the number of decimal places
    {
        StringBuilder outputMask = new StringBuilder("0.");
        for(int i = 0; i < decimalNum; i++)
        {
            outputMask.Append("#");
        }
        return outputMask.ToString();
    }
}
