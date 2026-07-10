using System.Collections.Generic;
using UnityEngine;
using System;

public class Tourists : CardType
{
    public Tourists(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        if (WithLevel(player.GetTokenDict(), FindNumber.Minimum, 2, 3))
            player.CreateLoseToken(4, (1, TokenType.HouseIcon), logged);
    }
}
