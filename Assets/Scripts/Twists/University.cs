using System.Collections.Generic;
using System;

public class University : CardType
{
    public University(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        Dictionary<TokenType, int[]> playerTokens = player.GetTokenDict();
        foreach (TokenType token in Enum.GetValues(typeof(TokenType)))
        {
            int number = playerTokens[token][3];
            player.UpDowngradeToken(number, (3, token), 1, logged);
        }
    }
}
