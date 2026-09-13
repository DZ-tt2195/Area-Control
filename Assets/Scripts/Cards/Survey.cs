using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Survey : CardType
{
    public Survey(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        AskSpendAction(player, this.dataFile.cardName, 1, logged, Reward);
        
        void Reward(bool didIt) 
        {
            if (didIt)
                player.DrawCardRPC(1, logged);
        }
    }
}
