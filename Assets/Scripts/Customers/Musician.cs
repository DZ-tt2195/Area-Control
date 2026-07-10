using UnityEngine;
using System;
using System.Collections.Generic;

public class Musician : CardType
{
    public Musician(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return soldTokens[TokenType.ArtIcon][6] >= 1;
    }
}
