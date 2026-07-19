using UnityEngine;
using System.Collections.Generic;

public class Specialize : CardType
{
    public Specialize(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        List<Card> canPlay = CanAfford(player);
        if (canPlay.Count == 0) return;

        MakeDecision.inst.ChooseCardOnScreen(canPlay, AutoTranslate.Ask_Play(), PlayThis, false);
        MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), EndTurn)}, AutoTranslate.Ask_Play(), false);

        void PlayThis(Card card)
        {
            PlayCard(player, card, thisArea, logged, 2, false);
        }
        void EndTurn()
        {
            Log.inst.AddMyText(true, OnlineTranslate.Online_Decline_Ability(player.name, this.dataFile.cardName));            
        }
    }
}
