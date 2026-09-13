using UnityEngine;

public class Direct : CardType
{
    public Direct(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.ActionRPC(CreateGame.inst.AllControl(player, true).Count, logged);
    }
}
