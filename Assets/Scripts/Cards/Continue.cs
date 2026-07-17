using UnityEngine;

public class Continue : CardType
{
    public Continue(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetDoneThisTurn(ThisTurn.CardsDrew) >= 3)
        {
            player.DrawCardRPC(1, logged);
            player.ActionRPC(1, logged);
        }
    }
}
