using UnityEngine;

public class Safeguard : CardType
{
    public Safeguard(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        foreach (TroopScoutDisplay display in CreateGame.inst.GetAllDisplays(player))
        {
            if (display.info.scouts == 0)
                return;
        }
        ForceAdvance(player, logged, 1);
    }
}
