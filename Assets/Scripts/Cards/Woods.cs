using UnityEngine;

public class Woods : CardType
{
    public Woods(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetTroops()[thisArea] <= 3)
            player.ActionRPC(1, logged);
    }
}
