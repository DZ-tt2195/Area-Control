using UnityEngine;

public class Battlefield : CardType
{
    public Battlefield(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (!CreateGame.inst.IsControlling(player, thisArea))
        {
            player.DrawCardRPC(1, logged);
            ForceRetreat(player, logged, 1);
        }
    }
}
