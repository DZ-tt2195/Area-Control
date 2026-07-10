using System.Collections.Generic;
using System;
using System.Linq;

public class Prioritize : CardType
{
    public Prioritize(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        player.CreateLoseToken(1, (6, TokenType.ToolIcon), logged);
        int max = 2;
        for (int i = 1; i<= max; i++)
        {
            int number = i;
            Log.inst.NewDecisionContainer(() => DowngradeToken(player, logged, number, max));
        }
    }
    void DowngradeToken(Player player, int logged, int num, int max)
    {
        List<TokenDisplay> canDowngrade = player.OfNumber(FindNumber.Minimum, Player.AllTokens(), Player.AllLevelsBut(1), 1);
        MakeDecision.inst.ChooseDisplayOnScreen(canDowngrade, AutoTranslate.Ask_Downgrade(AutoTranslate.TokenIcon(), num.ToString(), max.ToString()), DowngradeThis);

        void DowngradeThis((int level, TokenType type) info)
        {
            player.UpDowngradeToken(1, info, -1, logged);
        }
    }
}
