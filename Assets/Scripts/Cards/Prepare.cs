using UnityEngine;

public class Prepare : CardType
{
    public Prepare(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetActions() >= 2)
            player.DrawCardRPC(1, logged);
        else
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
    }
}
