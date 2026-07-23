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
        AskSpendAction(player, this.dataFile.cardName, 1, logged, Reward);
        
        void Reward(bool didIt)
        {
            if (didIt)
                ChooseAdvance(player, logged, 1);
        }
    }
}
