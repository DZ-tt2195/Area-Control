using UnityEngine;
using System.Collections.Generic;

public class Lender : CardType
{
    public Lender(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return TurnManager.inst.GetString(ConstantStrings.ChosenToken, player).Equals(TokenType.HouseIcon.ToString());
    }
}
