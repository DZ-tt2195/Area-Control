using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pasture : CardType
{
    public Pasture(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.ScoutRPC(1, thisArea, logged);
        Log.inst.NewDecisionContainer(() => ScoutSlider(player, thisArea, logged));
    }
    void ScoutSlider(Player player, int thisArea, int logged)
    {
        List<int> numbers = MakeDecision.NumbersInOrder(0, player.GetScouts()[thisArea]);
        MakeDecision.inst.ChooseFromSlider(numbers, AutoTranslate.Ask_Remove(Translator.inst.Translate(this.dataFile.cardName)), Removed);

        void Removed(int num)
        {
            if (num == 0)
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Choose_Zero(player.name), logged);                
            }
            else
            {
                player.ScoutRPC(-num, thisArea, logged);
                player.CoinRPC(2*num, logged);
            }
        }
    }
}
