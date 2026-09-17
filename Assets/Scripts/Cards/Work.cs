using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Work : CardType
{
    public Work(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        int allScouts = MyExtensions.SumOfArray(player.GetScouts());
        player.CoinRPC(allScouts, logged);
    }
}
