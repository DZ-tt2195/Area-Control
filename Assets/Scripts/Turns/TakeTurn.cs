using System.Collections.Generic;
using System.Linq;
public class TakeTurn : Turn
{
    public override void MasterStart()
    {
        int currentTurn = TurnManager.inst.GetInt(ConstantStrings.TurnNumber);
        Card card = CreateGame.inst.GetArea(currentTurn);
        Log.inst.MasterText(true, AutoTranslate.Blank());
        Log.inst.MasterText(true, OnlineTranslate.Online_Next_Turn(card.name));
    }
    public override void ForPlayer(Player player)
    {
        CreateGame.inst.CalculateControllers();
        int currentTurn = TurnManager.inst.GetInt(ConstantStrings.TurnNumber);

        switch (currentTurn)
        {
            case 1:
                player.ActionRPC(1);
                break;
            case 2:
                player.CoinRPC(3);
                break;
            case 3:
                player.DrawCardRPC(1);
                break;
            case 4:
                player.CoinRPC(3);
                break;
        }

        Card card = CreateGame.inst.GetArea(currentTurn);
        Log.inst.NewDecisionContainer(() => card.thisCard.DoInstructions(player, currentTurn, 0));
        Log.inst.NewDecisionContainer(() => PlayCards(player, currentTurn));
    }
    void PlayCards(Player player, int area)
    {
        if (player.GetActions() == 0) return;
        List<Card> canPlay = new();
        foreach (Card card in player.GetHand())
        {
            if (card.dataFile.coinCost <= player.GetCoins())
                canPlay.Add(card);
        }
        if (canPlay.Count == 0) return;

        MakeDecision.inst.ChooseCardOnScreen(canPlay, AutoTranslate.Ask_Play(), PlayThis, false);
        MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), EndTurn)}, AutoTranslate.Ask_Play(), false);

        void EndTurn()
        {
            Log.inst.AddMyText(true, OnlineTranslate.Online_End_Turn(player.name));            
        }

        void PlayThis(Card card)
        {
            Log.inst.AddMyText(true, OnlineTranslate.Online_Play_Card(player.name, card.name));
            player.ActionRPC(-1, 1);
            player.CoinRPC(-card.dataFile.coinCost, 1);
            player.DiscardCardRPC(card, -1);
            ForceAdvance(player, 1, card.dataFile.troopAdvance);

            Log.inst.NewDecisionContainer(() => card.thisCard.DoInstructions(player, area, 1));
            Log.inst.NewDecisionContainer(() => PlayCards(player, area));
        }
    }
    public override void MasterEnd()
    {
        int newNum = (TurnManager.inst.GetInt(ConstantStrings.TurnNumber)%4) + 1;
        PhotonCompatible.InstantChangeRoomProp(ConstantStrings.TurnNumber, newNum);

        List<Player> playersWon = new();
        foreach (Player player in CreateGame.inst.GetPlayers())
        {
            if (player.GetScore() == 0)
                playersWon.Add(player);
        }
        if (playersWon.Count == 1)
        {
            PhotonCompatible.InstantChangeRoomProp(ConstantStrings.NextPhase, "");
            TurnManager.inst.TextForEnding(OnlineTranslate.Online_Player_Won(playersWon[0].name), -1);
        }
        else if (playersWon.Count >= 2)
        {
            PhotonCompatible.InstantChangeRoomProp(ConstantStrings.NextPhase, "");
            TurnManager.inst.TextForEnding(OnlineTranslate.Online_Tie_Game(), -1);            
        }
    }
}
