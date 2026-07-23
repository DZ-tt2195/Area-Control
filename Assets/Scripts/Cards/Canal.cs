using System.Collections.Generic;
using UnityEngine;

public class Canal : CardType
{
    public Canal(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        List<int> allAdvances = player.GetDoneThisTurn(NumThisTurn.TroopsAdvanced);
        if (allAdvances.Contains(thisArea) || allAdvances.Contains(thisArea-1))
            player.DrawCardRPC(1, logged);
        else
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
    }
}
