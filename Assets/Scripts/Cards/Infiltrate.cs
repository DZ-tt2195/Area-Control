using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Infiltrate : CardType
{
    public Infiltrate(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        Log.inst.NewDecisionContainer(() => AddToArea());
    
        void AddToArea()
        {
            List<TroopScoutDisplay> canAdd = CreateGame.inst.GetAllDisplays(player);
            MakeDecision.inst.ChooseDisplayOnScreen(canAdd, AutoTranslate.Force_Add(Translator.inst.Translate(this.dataFile.cardName), 1.ToString(), 1.ToString()), AddMe);
            void AddMe((int area, int troops, int scouts) display)
            {
                player.ScoutRPC(3, display.area, logged);
            }
        }    
    }
}
