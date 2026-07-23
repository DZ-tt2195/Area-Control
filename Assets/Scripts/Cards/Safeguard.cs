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
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
                return;
            }
        }
        ChooseAdvance(player, logged, 1);
    }
}
