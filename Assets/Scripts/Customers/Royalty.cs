using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Royalty : CardType
{
    public Royalty(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return WithLevel(soldTokens, FindNumber.Minimum, 6, 2);
    }
}
