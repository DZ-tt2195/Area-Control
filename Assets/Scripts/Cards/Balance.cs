using UnityEngine;

public class Balance : CardType
{
    public Balance(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.CoinRPC(Mathf.Min(player.GetTroops()[2], player.GetTroops()[3]), logged);
    }
}
