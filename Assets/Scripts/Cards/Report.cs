using System.Collections.Generic;
using UnityEngine;

public class Report : CardType
{
    public Report(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseRemoveScout(player, this.dataFile.cardName, false, logged, 2, Reward);
        void Reward(List<int> removed)
        {
            if (removed.Count == 2)
                player.ActionRPC(2, logged);
        }
    }
}
