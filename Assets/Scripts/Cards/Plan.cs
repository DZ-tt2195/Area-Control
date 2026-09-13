using UnityEngine;

public class Plan : CardType
{
    public Plan(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.DrawCardRPC(CreateGame.inst.AllControl(player, true).Count, logged);
    }
}
