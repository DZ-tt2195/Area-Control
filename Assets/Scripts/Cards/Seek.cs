using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Seek : CardType
{
    public Seek(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetDoneThisTurn(ThisTurn.CoinsGained) >= 5)
            ForceAdvance(player, logged, 1);
    }
}
