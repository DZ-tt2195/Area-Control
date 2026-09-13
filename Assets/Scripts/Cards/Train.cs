using UnityEngine;

public class Train : CardType
{
    public Train(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseAddScout(player, this.dataFile.cardName, logged, CreateGame.inst.AllControl(player, true).Count);
    }
}