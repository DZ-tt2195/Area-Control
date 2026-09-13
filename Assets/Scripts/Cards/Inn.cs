using UnityEngine;

public class Inn : CardType
{
    public Inn(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetTroops()[thisArea] == player.GetScouts()[thisArea])
            player.ActionRPC(1, logged);
        else
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
    }
}
