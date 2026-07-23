using UnityEngine;

public class Continue : CardType
{
    public Continue(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetDoneThisTurn(CardThisTurn.CardsDrew).Count >= 2)
            player.ActionRPC(2, logged);
        else
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
    }
}
