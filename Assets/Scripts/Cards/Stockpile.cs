using UnityEngine;

public class Stockpile : CardType
{
    public Stockpile(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.CoinRPC(player.GetHand().Count, logged);
    }
}
