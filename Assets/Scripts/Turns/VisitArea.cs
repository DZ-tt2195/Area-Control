using System.Collections.Generic;
using System.Linq;
public class VisitArea : Turn
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
        GetTravelBonus(player, currentTurn, 0);

        Card card = CreateGame.inst.GetArea(currentTurn);
        Log.inst.NewDecisionContainer(() => card.thisCard.DoInstructions(player, currentTurn, 0));
        Log.inst.NewDecisionContainer(() => PlayCards(player, currentTurn));
    }
    void PlayCards(Player player, int area)
    {
        if (player.GetActions() == 0) return;
        List<Card> canPlay = CanAfford(player);
        if (canPlay.Count == 0) return;

        MakeDecision.inst.ChooseCardOnScreen(canPlay, AutoTranslate.Ask_Play(), PlayThis, false);
        MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), EndTurn)}, AutoTranslate.Ask_Play(), false);

        void PlayThis(Card card)
        {
            PlayCard(player, card, area, 1);
            Log.inst.NewDecisionContainer(() => PlayCards(player, area));
        }
        void EndTurn()
        {
            Log.inst.AddMyText(true, OnlineTranslate.Online_End_Turn(player.name));            
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
