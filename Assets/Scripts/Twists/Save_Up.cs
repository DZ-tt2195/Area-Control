using UnityEngine;

public class Save_Up : CardType
{
    public Save_Up(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        if (player.GetCoins() >= 5)
            player.CreateLoseToken(4, (1, TokenType.ArtIcon), logged);
    }
}
