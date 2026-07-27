using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Camp : CardType
{
    public Camp(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        (int area, int highestScouts) best = (0, 0);
        int[] playerScouts = player.GetScouts();
        for (int i = 0; i<playerScouts.Length; i++)
        {
            if (playerScouts[i] >= best.highestScouts)
                best = (i, playerScouts[i]);
        }
        if (best.area == thisArea)
        {
            Log.inst.NewDecisionContainer(() => AddToOther(player, thisArea, logged));
        }
        else
        {
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);  
        }
    }
    void AddToOther(Player player, int thisArea, int logged)
    {
        List<TroopScoutDisplay> canAdd = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.area != thisArea).ToList();
        MakeDecision.inst.ChooseDisplayOnScreen(canAdd, AutoTranslate.Force_Add(Translator.inst.Translate(this.dataFile.cardName), 1.ToString(), 1.ToString()), AddMe);
        
        void AddMe((int area, int troops, int scouts) display)
        {
            player.ScoutRPC(2, display.area, logged);
        }
    }
}
