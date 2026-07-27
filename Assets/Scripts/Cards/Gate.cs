using System.Collections.Generic;
using UnityEngine;

public class Gate : CardType
{
    public Gate(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (CreateGame.inst.IsControlling(player, thisArea))
            AskSpendCoin(player, this.dataFile.cardName, 4, logged, Reward);
        else
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);

        void Reward(bool didIt)
        {
            if (didIt)
                ChooseAdvance(player, this.dataFile.cardName, logged, 2);
        }
    }
}
