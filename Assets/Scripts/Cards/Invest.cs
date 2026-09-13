using UnityEngine;

public class Invest : CardType
{
    public Invest(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.CoinRPC(player.GetCoins(), logged);
    }
}
