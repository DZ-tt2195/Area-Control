using UnityEngine;

public class Highway : CardType
{
    public Highway(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (CreateGame.inst.IsControlling(player, thisArea))
        {
            ForceAdvance(player, logged, 2);
            ForceRetreat(player, logged, 1);
        }
    }
}
