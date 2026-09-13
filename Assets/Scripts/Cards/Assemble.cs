using UnityEngine;

public class Assemble : CardType
{
    public Assemble(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.CoinRPC(player.GetDoneThisTurn(NumThisTurn.TroopsAdvanced).Count, logged);
    }
}
