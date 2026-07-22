using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Trail : CardType
{
    public Trail(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        AskRetreat(player, this.dataFile.cardName, logged, TrailBonus);
        void TrailBonus(int area)
        {
            for (int i = 0; i<2; i++)
                GetTravelBonus(player, thisArea, logged);
        }
    }
}
