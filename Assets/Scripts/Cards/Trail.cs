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
        ChooseRetreat(player, this.dataFile.cardName, false, logged, 1, Reward);
        void Reward(int num)
        {
            if (num == 1)
            {
                for (int i = 0; i<2; i++)
                    GetTravelBonus(player, thisArea, logged);
            }
        }
    }
}
