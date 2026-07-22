using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Analyze : CardType
{
    public Analyze(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetDoneThisTurn(ThisTurn.CardsDiscarded) >= 3)
            player.DrawCardRPC(2, logged);
    }
}
