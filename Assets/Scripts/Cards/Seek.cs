using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Seek : CardType
{
    public Seek(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        int total = 0;
        foreach (int num in player.GetDoneThisTurn(NumThisTurn.CoinsGained))
            total+=num;
        if (total >= 5)
            player.ScoutRPC(2, thisArea, logged);
        else
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
    }
}
