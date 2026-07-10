using UnityEngine;
using System.Collections.Generic;

public class Sculptor : CardType
{    
    public Sculptor(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return TypesOrNot(soldTokens, 2, 
            new() {TokenType.ArtIcon, TokenType.ToolIcon}, 
            new() {TokenType.HouseIcon, TokenType.BookIcon});
    }
}
