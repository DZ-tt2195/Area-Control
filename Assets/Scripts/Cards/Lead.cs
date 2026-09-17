using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lead : CardType
{
    public Lead(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        int lowestTroops = 100;
        int[] playerTroops = player.GetTroops();

        for (int i = 0; i<playerTroops.Length; i++)
        {
            if (playerTroops[i] < lowestTroops)
                lowestTroops = playerTroops[i];
        }
        
    }
}