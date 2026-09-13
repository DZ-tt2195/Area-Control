using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Experiment : CardType
{
    public Experiment(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.DrawCardRPC(3-player.GetHand().Count, logged);
    }
}
