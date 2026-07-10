using UnityEngine;
using System.Collections.Generic;

public class Poet : CardType
{    
    public Poet(CardData dataFile) : base(dataFile)
    {
    }

    public override bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return TypesOrNot(soldTokens, 2, 
            new() {TokenType.ArtIcon, TokenType.BookIcon}, 
            new() {TokenType.HouseIcon, TokenType.ToolIcon});
    }
}
