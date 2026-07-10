using UnityEngine;

public class Shipment : CardType
{
    public Shipment(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        if (MyExtensions.SumOfArray(player.GetTokenDict()[TokenType.BookIcon]) == 0)
            player.CreateLoseToken(4, (1, TokenType.BookIcon), logged);
    }
}
