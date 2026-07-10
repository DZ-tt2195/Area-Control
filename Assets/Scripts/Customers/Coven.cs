using UnityEngine;
using System.Collections.Generic;

public class Coven : CardType
{
    public Coven(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return WithLevel(soldTokens, FindNumber.Minimum, 2, 4);
    }
}
