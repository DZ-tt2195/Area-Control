using System.Collections.Generic;
using UnityEngine;

public class Trade : CardType
{
    public Trade(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseDiscard(player, this.dataFile.cardName, false, logged, 1, Reward);
        void Reward(List<Card> discarded)
        {
            if (discarded.Count == 1)
                player.CoinRPC(5, logged);
        }
    }
}
