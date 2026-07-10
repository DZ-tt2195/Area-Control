using UnityEngine;
using System.Collections.Generic;

public class Author : CardType
{
    public Author(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return TypesInOrder(soldTokens, TokenType.ToolIcon, TokenType.ArtIcon, TokenType.BookIcon);
    }
}
