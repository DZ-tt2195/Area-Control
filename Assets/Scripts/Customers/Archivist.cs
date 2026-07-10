using UnityEngine;
using System.Collections.Generic;

public class Archivist : CardType
{    
    public Archivist(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return TypesOrNot(soldTokens, 2, 
            new() {TokenType.HouseIcon, TokenType.BookIcon}, 
            new() {TokenType.ArtIcon, TokenType.ToolIcon});
    }
}
