using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mine : CardType
{
    public Mine(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetTroops()[thisArea] >= 1)
            AskSpendAction(player, this.dataFile.cardName, 1, logged, () => player.CoinRPC(player.GetTroops()[thisArea]));
    }
}
