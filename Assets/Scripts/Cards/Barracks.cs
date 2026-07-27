using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Barracks : CardType
{
    public Barracks(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseRetreat(player, this.dataFile.cardName, true, logged, 1, Reward);

        void Reward(List<int> retreated)
        {
            if (retreated.Count == 1)
            {
                player.ScoutRPC(1, thisArea, logged);
                player.ScoutRPC(1, retreated[0]-1, logged);
            }
        }
    }
}
