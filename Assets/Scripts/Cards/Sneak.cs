using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sneak : CardType
{
    public Sneak(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        List<TroopScoutDisplay> allDisplays = CreateGame.inst.AreasControlled(player, false);
        MakeDecision.inst.ChooseDisplayOnScreen(allDisplays, AutoTranslate.Force_Add(1.ToString(), 1.ToString()), AddScout);
        void AddScout((int area, int troops, int scouts) info)
        {
            player.ScoutRPC(2, info.area, logged);
        }
    }
}
