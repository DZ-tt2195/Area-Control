using UnityEngine;
using System.Collections.Generic;

public class Quality : CardType
{
    public Quality(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        List<TextButtonInfo> addTokens = new()
        {
            new(AutoTranslate.ArtIcon(), () => AddThis(TokenType.ArtIcon)),
            new(AutoTranslate.HouseIcon(), () => AddThis(TokenType.HouseIcon)),
            new(AutoTranslate.ToolIcon(), () => AddThis(TokenType.ToolIcon)),
            new(AutoTranslate.BookIcon(), () => AddThis(TokenType.BookIcon))
        };
        MakeDecision.inst.ChooseTextButton(addTokens, AutoTranslate.Ask_Create(AutoTranslate.TokenIcon(), "1", "1"));

        void AddThis(TokenType type)
        {
            player.CreateLoseToken(1, (4, type), logged);
        }
    }
}
