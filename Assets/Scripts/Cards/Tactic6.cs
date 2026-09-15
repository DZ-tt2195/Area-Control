using UnityEngine;

public class Tactic6 : CardType
{
    public Tactic6(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        Effect(player, logged);
        foreach (Player nextPlayer in CreateGame.inst.GetPlayers())
            if (nextPlayer != player) TurnManager.inst.WillAddBetweenTurn(nextPlayer, cardObject);
    }
    public override void BetweenTurnInstructions(Player player, int logged)
    {
        Effect(player, logged);
    }
    void Effect(Player player, int logged)
    {
        player.ActionRPC(1, logged);
    }
}
