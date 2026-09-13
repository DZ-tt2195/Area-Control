using System.Collections.Generic;
using UnityEngine;

public class Visit : CardType
{
    public Visit(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseRemoveScout(player, this.dataFile.cardName, false, logged, 1, Reward);
        void Reward(List<int> removed)
        {
            if (removed.Count == 1)
                GetTravelBonus(player, thisArea, logged);
        }
    }
}
