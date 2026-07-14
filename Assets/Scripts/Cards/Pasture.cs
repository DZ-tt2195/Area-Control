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
        MakeDecision.inst.ChooseFromSlider(numbers, AutoTranslate.Ask_Remove(), Removed);

        void Removed(int num)
        {
            player.ScoutRPC(-num, thisArea, logged);
            player.CoinRPC(2*num, logged);
        }
    }
}
