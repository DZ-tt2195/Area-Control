using UnityEngine;

public class Plan : CardType
{
    public Plan(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.DrawCardRPC(CreateGame.inst.AreasControlled(player, true).Count, logged);
    }
}
