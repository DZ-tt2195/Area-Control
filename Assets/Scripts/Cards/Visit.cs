using UnityEngine;

public class Visit : CardType
{
    public Visit(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        GetTravelBonus(player, thisArea, logged);
    }
}
