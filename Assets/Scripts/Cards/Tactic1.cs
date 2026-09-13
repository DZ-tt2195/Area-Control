using UnityEngine;

public class Tactic1 : CardType
{
    public Tactic1(Card card, CardData dataFile) : base(card, dataFile)
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
        Log.inst.NewDecisionContainer(() => player.DrawCardRPC(2, logged));
        ChooseDiscard(player, nameof(Tactic1), true, logged, 2);        
    }
}
