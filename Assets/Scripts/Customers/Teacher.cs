using UnityEngine;
using System;
using System.Collections.Generic;

public class Teacher : CardType
{
    public Teacher(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return HigherTypeVs(soldTokens, 4, TokenType.BookIcon);
    }
}
