using UnityEngine;

public class Scavenge : CardType
{
    public Scavenge(Card card, CardData dataFile) : base(card, dataFile)
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
        ChooseDiscard(player, nameof(Scavenge), true, logged, 2);        
    }
}
