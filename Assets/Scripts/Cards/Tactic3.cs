using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Tactic3 : CardType
{
    public Tactic3(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        Log.inst.NewDecisionContainer(() => AddToArea());
    
        void AddToArea()
        {
            List<TroopScoutDisplay> canAdd = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.troops >= 3).ToList();
            if (canAdd.Count == 0)
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
                return;
            } 

            MakeDecision.inst.ChooseDisplayOnScreen(canAdd, AutoTranslate.Force_Add(Translator.inst.Translate(this.dataFile.cardName), 1.ToString(), 1.ToString()), AddMe);
            void AddMe((int area, int troops, int scouts) display)
            {
                player.ScoutRPC(2, display.area, logged);
            }
        }
    }
}