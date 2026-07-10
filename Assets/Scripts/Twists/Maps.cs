using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Maps : CardType
{
    public Maps(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        player.CreateLoseToken(1, (1, TokenType.HouseIcon), logged);
        int[] houseArray = player.GetTokenDict()[TokenType.HouseIcon];
        for (int i = houseArray.Length-1; i>=0; i--)
        {
            if (houseArray[i] >= 1)
            {
                player.UpDowngradeToken(1, (1, TokenType.HouseIcon), i-1, logged);
                break;
            }
        }
    }
}
