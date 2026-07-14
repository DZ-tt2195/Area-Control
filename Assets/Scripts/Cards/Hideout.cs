using UnityEngine;

public class Hideout : CardType
{
    public Hideout(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (!CreateGame.inst.IsControlling(player, thisArea))
            ForceAddScout(player, logged, 1);
    }
}
