using UnityEngine;

public class Invest : CardType
{
    public Invest(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.CoinRPC(player.GetCoins(), logged);
    }
}
