using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class Tactic4 : CardType
{
    public Tactic4(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        foreach (Player nextPlayer in CreateGame.inst.GetPlayers())
            if (nextPlayer != player) TurnManager.inst.WillAddBetweenTurn(nextPlayer, cardObject);
    }
    public override void BetweenTurnInstructions(Player player, int logged)
    {
        if (player.GetHand().Count == 0)
            Log.inst.AddMyText(false, OnlineTranslate.Online_Avoid_Ability(player.name, nameof(Tactic4)), logged);

        ChooseDiscard(player, nameof(Tactic4), player.GetActions() == 0, logged, 1, NoDiscard);
        void NoDiscard(List<Card> discarded)
        {
            if (discarded.Count == 0)
                player.ActionRPC(-1, logged);
        }
    }
}