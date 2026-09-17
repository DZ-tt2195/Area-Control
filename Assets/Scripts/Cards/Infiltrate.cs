using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Infiltrate : CardType
{
    public Infiltrate(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        Log.inst.NewDecisionContainer(() => AddToArea());
    
        void AddToArea()
        {
            List<TroopScoutDisplay> canAdd = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.scouts == 0).ToList();;
            if (canAdd.Count == 0)
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, nameof(Infiltrate)));
                return;
            }

            MakeDecision.inst.ChooseDisplayOnScreen(canAdd, AutoTranslate.Force_Add(Translator.inst.Translate(this.dataFile.cardName), 1.ToString(), 1.ToString()), AddMe);
            void AddMe((int area, int troops, int scouts) display)
            {
                player.ScoutRPC(3, display.area, logged);
            }
        }    
    }
}
