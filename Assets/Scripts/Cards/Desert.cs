using System.Collections.Generic;
using UnityEngine;

public class Desert : CardType
{
    public Desert(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseRemoveScout(player, this.dataFile.cardName, true, logged, 2, Check);

        void Check(List<int> removed)
        {
            int numFromThisArea = 0;
            foreach (int next in removed)
            {
                if (next == thisArea)
                    numFromThisArea++;
            }
            if (numFromThisArea == 1)
                player.DrawCardRPC(2, logged);
        }
    }
}
