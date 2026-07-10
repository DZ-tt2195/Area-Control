using UnityEngine;
using System.Collections.Generic;

public class Scribe : CardType
{    
    public Scribe(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return TypesOrNot(soldTokens, 2, 
            new() {TokenType.ToolIcon, TokenType.BookIcon}, 
            new() {TokenType.ArtIcon, TokenType.HouseIcon});
    }
}
