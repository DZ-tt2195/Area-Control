using UnityEngine;

public class Barracks : CardType
{
    public Barracks(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetTroops()[thisArea] == player.GetScouts()[thisArea])
            player.DrawCardRPC(1, logged);
    }
}
