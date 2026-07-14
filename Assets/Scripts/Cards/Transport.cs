using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Transport : CardType
{
    public Transport(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (thisArea < 4)
            AskSpendAction(player, this.dataFile.cardName, 1, logged, () => player.TroopRPC(1, thisArea, thisArea+1, logged));
    }
}
