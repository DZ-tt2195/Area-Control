using UnityEngine;

public class Gate : CardType
{
    public Gate(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.DrawCardRPC(Mathf.FloorToInt(player.GetScouts()[thisArea]/2f), logged);
    }
}
