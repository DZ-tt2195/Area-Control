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
        AskDiscard(player, this.dataFile.cardName, logged, card => ForceAddScout(player, logged, 2));
    }
}
