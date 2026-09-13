using System.Collections.Generic;
using UnityEngine;

public class Trade : CardType
{
    public Trade(Card card, CardData dataFile) : base(card, dataFile)
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
