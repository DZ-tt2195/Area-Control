using UnityEngine;

public class Quantity : CardType
{
    public Quantity(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        player.CreateLoseToken(1, (1, TokenType.ArtIcon), logged);
        player.CreateLoseToken(1, (1, TokenType.HouseIcon), logged);
        player.CreateLoseToken(1, (1, TokenType.ToolIcon), logged);
        player.CreateLoseToken(1, (1, TokenType.BookIcon), logged);
    }
}
