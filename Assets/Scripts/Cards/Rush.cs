using UnityEngine;

public class Rush : CardType
{
    public Rush(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        AskDiscard(player, this.dataFile.cardName, logged, () => player.ActionRPC(1, logged));
    }
}