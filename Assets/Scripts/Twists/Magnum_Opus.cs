using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Magnum_Opus : CardType
{
    public Magnum_Opus(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        player.CreateLoseToken(1, (6, TokenType.BookIcon), logged);
        int downgrade = Mathf.FloorToInt(player.AllTotalTokens() / 3f);
        player.UpDowngradeToken(1, (6, TokenType.BookIcon), -1*downgrade, logged);
    }
}
