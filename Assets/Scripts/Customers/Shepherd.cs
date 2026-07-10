using UnityEngine;
using System;
using System.Collections.Generic;

public class Shepherd : CardType
{
    public Shepherd(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return HigherTypeVs(soldTokens, 4, TokenType.HouseIcon);
    }
}
