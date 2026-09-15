using System.Collections.Generic;
using UnityEngine;

public class City : CardType
{
    public City(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (CreateGame.inst.IsControlling(player, thisArea))
            ChooseDiscard(player, this.dataFile.cardName, false, logged, 1, Reward);
        else
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);

        void Reward(List<Card> discarded)
        {
            if (discarded.Count == 1)
                player.ActionRPC(2, logged);
        }
    }
}