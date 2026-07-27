using UnityEngine;

public class March : CardType
{
    public March(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseAdvance(player, this.dataFile.cardName, logged, CreateGame.inst.AllControl(player, true).Count);
    }
}
