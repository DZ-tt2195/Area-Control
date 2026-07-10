using UnityEngine;
using System;
using System.Collections.Generic;

public class Bishop : CardType
{
    public Bishop(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return TypesInOrder(soldTokens, TokenType.ArtIcon, TokenType.BookIcon, TokenType.HouseIcon);
    }
}
