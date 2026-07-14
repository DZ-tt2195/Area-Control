using UnityEngine;

public class Travel : CardType
{
    public Travel(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ForceAddScout(player, logged, 1);
        ForceRemoveScout(player, logged, 1);
    }
}
