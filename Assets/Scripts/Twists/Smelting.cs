using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Smelting : CardType
{
    public Smelting(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        player.CreateLoseToken(1, (6, TokenType.ArtIcon), logged);
        Log.inst.NewDecisionContainer(() => LoseToken(player, logged, TokenType.HouseIcon));
        Log.inst.NewDecisionContainer(() => LoseToken(player, logged, TokenType.ToolIcon));
        Log.inst.NewDecisionContainer(() => LoseToken(player, logged, TokenType.BookIcon));
    }
    void LoseToken(Player player, int logged, TokenType type)
    {
        List<TokenDisplay> canLose = player.OfNumber(FindNumber.Minimum, new() {type}, Player.AllLevels(), 1);
        MakeDecision.inst.ChooseDisplayOnScreen(canLose, AutoTranslate.Ask_Lose(Translator.inst.Translate(type.ToString()), "1", "1"), LoseToken);

        void LoseToken((int level, TokenType type) info)
        {
            player.CreateLoseToken(-1, info, logged);
        }        
    }
}
