using UnityEngine;
using System.Collections.Generic;

public class Engineer : CardType
{
    public Engineer(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return SequentialLevels(soldTokens, TokenType.HouseIcon, 4);
    }
}
