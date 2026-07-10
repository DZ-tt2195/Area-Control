using UnityEngine;

public class Produce : CardType
{
    public Produce(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        if (WithLevel(player.GetTokenDict(), FindNumber.Minimum, 6, 1))
            player.CreateLoseToken(4, (1, TokenType.ToolIcon), logged);
    }
}
