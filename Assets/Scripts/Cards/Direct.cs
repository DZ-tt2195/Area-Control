using UnityEngine;

public class Direct : CardType
{
    public Direct(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.ActionRPC(CreateGame.inst.AllControl(player, true).Count, logged);
    }
}
