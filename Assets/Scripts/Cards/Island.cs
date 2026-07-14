using UnityEngine;

public class Island : CardType
{
    public Island(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetTroops()[thisArea] >= 4)
            player.TroopRPC(1, 1, 2, logged);
    }
}