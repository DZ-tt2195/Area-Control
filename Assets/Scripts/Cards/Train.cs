using UnityEngine;

public class Train : CardType
{
    public Train(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ForceAddScout(player, logged, CreateGame.inst.AreasControlled(player, true).Count);
    }
}
