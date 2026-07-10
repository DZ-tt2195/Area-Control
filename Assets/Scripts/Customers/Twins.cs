using UnityEngine;
using System;
using System.Collections.Generic;

public class Twins : CardType
{
    public Twins(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        foreach (TokenType token in Enum.GetValues(typeof(TokenType)))
        {
            for (int i = 0; i<soldTokens[token].Length; i++)
            {
                if (soldTokens[token][i] >= 2)
                    return true;
            }
        }
        return false;
    }
}
