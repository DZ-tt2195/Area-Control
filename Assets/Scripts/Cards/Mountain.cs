using System.Collections.Generic;
using UnityEngine;

public class Mountain : CardType
{
    public Mountain(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (CreateGame.inst.IsControlling(player, thisArea))
            player.DrawCardRPC(Mathf.FloorToInt(player.GetHand().Count/2f), logged);
        else
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);
    }
}
