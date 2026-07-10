using UnityEngine;
using System.Collections.Generic;

public class Carpenter : CardType
{    
    public Carpenter(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return TypesOrNot(soldTokens, 2, 
            new() {TokenType.HouseIcon, TokenType.ToolIcon}, 
            new() {TokenType.ArtIcon, TokenType.BookIcon});
    }
}
