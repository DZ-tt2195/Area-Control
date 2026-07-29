using System.Collections.Generic;
using System.Linq;
public class VisitArea : Turn
{
    public override void MasterStart()
    {
        int currentTurn = TurnManager.inst.GetInt(ConstantStrings.TurnNumber);
        List<Player> whoControls = CreateGame.inst.CalculateControllers();

        Log.inst.MasterText(true, AutoTranslate.Blank());
        for (int i = 1; i<whoControls.Count; i++)
        {
            if (whoControls[i] != null)
                Log.inst.MasterText(true, OnlineTranslate.Online_Who_Controls(whoControls[i].name, i.ToString()));
            else
                Log.inst.MasterText(true, OnlineTranslate.Online_Who_Controls(nameof(AutoTranslate.No_One), i.ToString()));
        }

        Log.inst.MasterText(true, AutoTranslate.Blank());
        Log.inst.MasterText(true, OnlineTranslate.Online_Next_Turn(CreateGame.inst.GetArea(currentTurn).name));
    }
    public override void ForPlayer(Player player)
    {
        int currentTurn = TurnManager.inst.GetInt(ConstantStrings.TurnNumber);
        GetTravelBonus(player, currentTurn, 0);

        Log.inst.NewDecisionContainer(() => PlayCards(player, currentTurn));
        Log.inst.NewDecisionContainer(() => DoArea(player, CreateGame.inst.GetArea(currentTurn), currentTurn));
    }
    void PlayCards(Player player, int area)
    {
        if (player.GetActions() == 0) return;
        List<Card> canPlay = CanAfford(player);
        if (canPlay.Count == 0) return;

        MakeDecision.inst.ChooseCardOnScreen(canPlay, AutoTranslate.Ask_Play(), PlayThis, false);
        MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), NoPlay)}, AutoTranslate.Ask_Play(), false);

        void PlayThis(Card card)
        {
            PlayCard(player, card, area, 0);
            Log.inst.NewDecisionContainer(() => PlayCards(player, area));
        }
        void NoPlay()
        {
            Log.inst.AddMyText(true, OnlineTranslate.Online_No_Play(player.name));            
        }
    }
    void DoArea(Player player, Card card, int area)
    {
        Log.inst.AddMyText(false, OnlineTranslate.Online_Resolve_Card(player.name, card.name), 0);
        Log.inst.NewDecisionContainer(() => card.thisCard.DoInstructions(player, area, 1));
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
