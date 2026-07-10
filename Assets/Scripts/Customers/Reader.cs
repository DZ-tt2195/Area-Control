using UnityEngine;
using System.Collections.Generic;

public class Reader : CardType
{
    public Reader(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return TurnManager.inst.GetString(ConstantStrings.ChosenToken, player).Equals(TokenType.BookIcon.ToString());
    }
}
