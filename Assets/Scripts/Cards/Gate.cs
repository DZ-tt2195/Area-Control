using UnityEngine;

public class Gate : CardType
{
    public Gate(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseAdvance(player, logged, Mathf.FloorToInt(player.GetScouts()[thisArea]/3f));
    }
}
