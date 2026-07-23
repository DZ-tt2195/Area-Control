using UnityEngine;

public class Battlefield : CardType
{
    public Battlefield(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (!CreateGame.inst.IsControlling(player, thisArea))
        {
            player.DrawCardRPC(1, logged);
            ChooseRetreat(player, this.dataFile.cardName, true, logged, 1);
        }
        else
        {
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
        }
    }
}
