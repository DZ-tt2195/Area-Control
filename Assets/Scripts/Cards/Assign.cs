using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Assign : CardType
{
    public Assign(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseAddScout(player, nameof(Assign), logged, 1, AddedScout);
        void AddedScout(List<int> addedScouts)
        {
            GetTravelBonus(player, addedScouts[0], logged);
        }
    }
}