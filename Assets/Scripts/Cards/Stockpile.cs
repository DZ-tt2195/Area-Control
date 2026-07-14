using UnityEngine;

public class Stockpile : CardType
{
    public Stockpile(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.CoinRPC(player.GetHand().Count, logged);
    }
}
