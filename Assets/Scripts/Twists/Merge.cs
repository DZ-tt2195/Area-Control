using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Merge : CardType
{
    public Merge(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        player.CreateLoseToken(1, (6, TokenType.HouseIcon), logged);
        int max = 2;
        for (int i = 1; i<=max; i++)
        {
            int number = i;
            Log.inst.NewDecisionContainer(() => RemoveHouse(player, number, max, logged));
        }
    }
    void RemoveHouse(Player player, int num, int max, int logged)
    {
        List<TokenDisplay> canLose = player.OfNumber(FindNumber.Minimum, new() {TokenType.HouseIcon}, Player.AllLevels(), 1);
        MakeDecision.inst.ChooseDisplayOnScreen(canLose, AutoTranslate.Ask_Lose(AutoTranslate.HouseIcon(), num.ToString(), max.ToString()), LoseToken);

        void LoseToken((int level, TokenType type) info)
        {
            player.CreateLoseToken(-1, info, logged);
        }
    }
}
