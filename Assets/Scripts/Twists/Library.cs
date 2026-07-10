using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Library : CardType
{
    public Library(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        player.CreateLoseToken(1, (1, TokenType.HouseIcon), logged);
        int[] bookArray = player.GetTokenDict()[TokenType.BookIcon];
        int upgrade = 0;
        for (int i = 0; i<bookArray.Length; i++)
        {
            if (bookArray[i] >= 1)
                upgrade++;
        }
        player.UpDowngradeToken(1, (1, TokenType.HouseIcon), upgrade, logged);
    }
}
