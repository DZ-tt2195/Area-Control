using UnityEngine;

public class Thoroughfare : CardType
{
    public Thoroughfare(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseAdvance(player, this.dataFile.cardName, logged, Mathf.FloorToInt(player.GetScouts()[thisArea]/3f));
    }
}
