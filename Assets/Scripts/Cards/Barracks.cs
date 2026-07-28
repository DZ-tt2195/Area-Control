using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Barracks : CardType
{
    public Barracks(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetTroops()[4] >= 1)
        {
            player.TroopRPC(1, 4, 3, logged);
            player.ScoutRPC(1, thisArea, logged);
            player.ScoutRPC(1, 4, logged);
        }
    }
}
