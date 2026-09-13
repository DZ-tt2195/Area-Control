using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Transport : CardType
{
    public Transport(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        AskSpendAction(player, this.dataFile.cardName, 1, logged, Reward);
        
        void Reward(bool didIt)
        {
            if (didIt)
                ChooseAdvance(player, this.dataFile.cardName, logged, 1);
        }
    }
}
