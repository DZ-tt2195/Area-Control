using UnityEngine;
using System;
using System.Collections.Generic;

public class Weaver : CardType
{
    public Weaver(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return TypesInOrder(soldTokens, TokenType.HouseIcon, TokenType.ToolIcon, TokenType.ArtIcon);
    }
}
