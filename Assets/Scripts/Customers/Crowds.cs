using System.Collections.Generic;
using UnityEngine;

public class Crowds : CardType
{
    public Crowds(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return player.AllTotalTokens() == 0;
    }
}
