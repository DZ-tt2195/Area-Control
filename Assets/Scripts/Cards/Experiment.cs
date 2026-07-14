using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Experiment : CardType
{
    public Experiment(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        List<Card> hand = player.GetHand();
        for (int i = hand.Count-1; i>=0; i--)
            player.DiscardCardRPC(hand[i], logged);
        player.DrawCardRPC(3, logged);
    }
}
