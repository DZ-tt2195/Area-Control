using System;
using System.Collections.Generic;
using UnityEngine;

public class Collector : CardType
{
    public Collector(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        HashSet<TokenType> required = new() {TokenType.ArtIcon, TokenType.HouseIcon, TokenType.ToolIcon, TokenType.BookIcon};
        return TypesOrNot(soldTokens, 1, required, new());
    }
}