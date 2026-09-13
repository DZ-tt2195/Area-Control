using UnityEngine;

public class Tactic2 : CardType
{
    public Tactic2(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        foreach (Player nextPlayer in CreateGame.inst.GetPlayers())
            if (nextPlayer != player) TurnManager.inst.WillAddBetweenTurn(nextPlayer, cardObject);
    }
    public override void BetweenTurnInstructions(Player player, int logged)
    {
        ChooseRemoveScout(player, nameof(Tactic2), true, logged, 2);
    }
}