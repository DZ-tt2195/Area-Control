using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mine : CardType
{
    public Mine(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetTroops()[thisArea] >= 1)
            AskSpendAction(player, this.dataFile.cardName, 1, logged, Reward);
        else
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                

        void Reward(bool didIt)
        {
            if (didIt)
                player.CoinRPC(player.GetTroops()[thisArea]);
        }
    }
}
