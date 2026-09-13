using UnityEngine;

public class March : CardType
{
    public March(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseAdvance(player, this.dataFile.cardName, logged, CreateGame.inst.AllControl(player, true).Count);
    }
}
