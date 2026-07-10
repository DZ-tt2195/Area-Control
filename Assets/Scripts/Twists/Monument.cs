using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Monument : CardType
{
    public Monument(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        player.CreateLoseToken(1, (1, TokenType.ArtIcon), logged);
        int[] toolArray = player.GetTokenDict()[TokenType.ToolIcon];
        int upgrade = toolArray[3] + toolArray[4];
        player.UpDowngradeToken(1, (1, TokenType.ArtIcon), upgrade, logged);
    }
}
