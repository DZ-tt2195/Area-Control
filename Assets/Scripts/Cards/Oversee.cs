using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Oversee : CardType
{
    public Oversee(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        player.ActionRPC(2-player.GetActions(), logged);
    }
}
