using System.Collections.Generic;
using UnityEngine;

public class Laboratory : CardType
{
    public Laboratory(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.ActionRPC(player.GetScouts()[thisArea], logged);
        player.CoinRPC(-player.GetScouts()[thisArea], logged);
    }
}
