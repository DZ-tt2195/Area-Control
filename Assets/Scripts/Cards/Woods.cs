using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Woods : CardType
{
    public Woods(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetScouts()[thisArea] >= 1)
        {
            List<TextButtonInfo> textButtonInfos = new() {new(AutoTranslate.Confirm(), DidIt), new(AutoTranslate.Decline(), DidNot)};
            MakeDecision.inst.ChooseTextButton(textButtonInfos, AutoTranslate.Ask_Remove(Translator.inst.Translate(this.dataFile.cardName)));
            
            void DidIt()
            {
                player.ScoutRPC(-1, thisArea, logged);
                ChooseAdvance(player, this.dataFile.cardName, logged, 1);
            }
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
            }
        }
        else
        {
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
        }
    }
}