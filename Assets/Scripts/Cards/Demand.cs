using UnityEngine;

public class Demand : CardType
{
    public Demand(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        foreach (TroopScoutDisplay display in CreateGame.inst.AllControl(player, true))
            GetTravelBonus(player, display.info.area, logged);
    }
}
