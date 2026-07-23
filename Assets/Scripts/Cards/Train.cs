using UnityEngine;

public class Train : CardType
{
    public Train(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseAddScout(player, logged, CreateGame.inst.AllControl(player, true).Count);
    }
}
