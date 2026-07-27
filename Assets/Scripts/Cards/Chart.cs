using System.Collections.Generic;
using UnityEngine;

public class Chart : CardType
{
    public Chart(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseDiscard(player, this.dataFile.cardName, false, logged, 1, Reward);
        void Reward(List<Card> discarded)
        {
            if (discarded.Count == 1)
                ChooseAdvance(player, this.dataFile.cardName, logged, 1);
        }
    }
}
