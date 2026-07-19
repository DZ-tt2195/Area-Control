using UnityEngine;

public class Secure : CardType
{
    public Secure(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetTroops()[thisArea] == 0 || player.GetScouts()[thisArea] == 0)
            player.CoinRPC(4, logged);
    }
}
