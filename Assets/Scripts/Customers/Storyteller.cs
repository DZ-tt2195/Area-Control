using UnityEngine;
using System;
using System.Collections.Generic;

public class Storyteller : CardType
{
    public Storyteller(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return soldTokens[TokenType.BookIcon][6] >= 1;
    }
}
