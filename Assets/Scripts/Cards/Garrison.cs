using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Garrison : CardType
{
    public Garrison(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        AskSpendCoin(player, this.dataFile.cardName, 4, logged, Reward);

        void Reward(bool didIt)
        {
            if (didIt)
            {
                foreach (TroopScoutDisplay display in CreateGame.inst.GetAllDisplays(player))
                {
                    if (display.info.area != thisArea)
                        player.ScoutRPC(1, display.info.area);
                }
            }
        }
    }
}
