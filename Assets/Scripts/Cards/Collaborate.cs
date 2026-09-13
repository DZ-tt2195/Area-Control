using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Collaborate : CardType
{
    public Collaborate(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        int allScouts = MyExtensions.SumOfArray(player.GetScouts());
        ChooseAddScout(player, this.dataFile.cardName, logged, Mathf.FloorToInt(allScouts/2f));
    }
}
