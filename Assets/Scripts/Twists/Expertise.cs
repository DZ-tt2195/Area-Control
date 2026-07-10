using System.Collections.Generic;
using UnityEngine;
using System;

public class Expertise : CardType
{
    public Expertise(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        List<TokenDisplay> canUpgrade = player.OfNumber(FindNumber.Minimum, Player.AllTokens(), Player.AllLevelsBut(6), 1);
        MakeDecision.inst.ChooseDisplayOnScreen(canUpgrade, AutoTranslate.Ask_Create(AutoTranslate.TokenIcon(), "1", "1"), DuplicateThis);

        void DuplicateThis((int level, TokenType type) info)
        {
            player.CreateLoseToken(4, info, logged);
        }
    }
}
