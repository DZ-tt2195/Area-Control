using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Trail : CardType
{
    public Trail(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseRetreat(player, this.dataFile.cardName, false, logged, 1, Reward);
        void Reward(List<int> retreated)
        {
            if (retreated.Count == 1)
                GetTravelBonus(player, thisArea, logged, 2);
        }
    }
}
