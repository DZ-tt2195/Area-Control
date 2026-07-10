using UnityEngine;
using System.Collections.Generic;

public class Family : CardType
{
    public Family(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return WithLevel(soldTokens, FindNumber.Minimum, 3, 4);
    }
}
