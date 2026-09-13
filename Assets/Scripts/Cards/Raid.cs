using UnityEngine;

public class Raid : CardType
{
    public Raid(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.CoinRPC(CreateGame.inst.AllControl(player, true).Count, logged);
    }
}
