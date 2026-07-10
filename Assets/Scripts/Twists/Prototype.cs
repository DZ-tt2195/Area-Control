using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Prototype : CardType
{
    public Prototype(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        int numArt = player.GetTokenDict()[TokenType.ArtIcon][1];
        player.CreateLoseToken(1, (1, TokenType.ToolIcon), logged);
        player.UpDowngradeToken(1, (1, TokenType.ToolIcon), numArt, logged);
    }
}
