using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Collaborate : CardType
{
    public Collaborate(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        int allScouts = MyExtensions.SumOfArray(player.GetScouts());
        ChooseAddScout(player, logged, Mathf.FloorToInt(allScouts/2f));
    }
}
