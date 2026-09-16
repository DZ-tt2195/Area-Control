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
        ChooseAddScout(player, nameof(Tactic3), logged, 1, AddedScout);
        void AddedScout(List<int> addedScouts)
        {
            GetTravelBonus(player, addedScouts[0], logged);
        }
    }
}