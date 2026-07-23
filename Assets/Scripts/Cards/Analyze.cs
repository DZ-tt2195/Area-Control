using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Analyze : CardType
{
    public Analyze(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetDoneThisTurn(NumThisTurn.ActionsLost).Count >= 3)
            player.DrawCardRPC(2, logged);
        else
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
    }
}
