using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Recruit : CardType
{
    public Recruit(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        AskSpendAction(player, this.dataFile.cardName, 1, logged, () => ForceAddScout(player, logged, 2));
    }
}
