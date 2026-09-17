using System.Collections.Generic;
using UnityEngine;

public class Tax : CardType
{
    public Tax(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        foreach (Player nextPlayer in CreateGame.inst.GetPlayers())
            if (nextPlayer != player) TurnManager.inst.WillAddBetweenTurn(nextPlayer, cardObject);
    }
    public override void BetweenTurnInstructions(Player player, int logged)
    {
        player.CoinRPC(-1*(player.GetHand().Count + player.GetActions()), logged);
    }
}