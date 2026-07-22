using UnityEngine;

public class Visit : CardType
{
    public Visit(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        AskRemoveScout(player, this.dataFile.cardName, logged, area => GetTravelBonus(player, thisArea, logged));
    }
}
