using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Castle : CardType
{
    public Castle(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (CreateGame.inst.IsControlling(player, thisArea) && 
        (player.GetScouts()[thisArea] == 0 || player.GetTroops()[thisArea] == 0))
            player.CoinRPC(4, logged);
    }
}