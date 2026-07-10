using UnityEngine;
using System.Collections.Generic;

public class Curator : CardType
{    
    public Curator(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return TypesOrNot(soldTokens, 2, 
            new() {TokenType.ArtIcon, TokenType.HouseIcon}, 
            new() {TokenType.ToolIcon, TokenType.BookIcon});
    }
}
