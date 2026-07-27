using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Parry : CardType
{
    public Parry(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        Log.inst.NewDecisionContainer(() => AddToControl(true));
        Log.inst.NewDecisionContainer(() => AddToControl(false));
    
        void AddToControl(bool controlIt)
        {
            List<TroopScoutDisplay> canAdd = CreateGame.inst.AllControl(player, controlIt);
            if (canAdd.Count == 0)
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
                return;
            } 

            MakeDecision.inst.ChooseDisplayOnScreen(canAdd, AutoTranslate.Force_Add(Translator.inst.Translate(this.dataFile.cardName), 1.ToString(), 1.ToString()), AddMe);
            void AddMe((int area, int troops, int scouts) display)
            {
                player.ScoutRPC(1, display.area, logged);
            }
        }
    }
}
