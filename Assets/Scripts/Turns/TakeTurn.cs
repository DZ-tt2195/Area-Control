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

            for (int i = 0; i<card.dataFile.troopAdvance; i++)
            {
                int currentNum = i+1;
                Log.inst.NewDecisionContainer(() => AdvanceTroop(player, 1, currentNum, card.dataFile.troopAdvance));
            }

            Log.inst.NewDecisionContainer(() => card.thisCard.DoInstructions(player, area, 1));
            Log.inst.NewDecisionContainer(() => PlayCards(player, area));
        }
    }
    void AdvanceTroop(Player player, int logged, int currentNum, int maxNum)
    {
        List<TroopScoutDisplay> canAdvance = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.area != 4 && display.info.troops >= 1).ToList();
        if (canAdvance.Count == 0) return;

        MakeDecision.inst.ChooseDisplayOnScreen(canAdvance, AutoTranslate.Force_Advance(currentNum.ToString(), maxNum.ToString()), AdvanceMe);
        void AdvanceMe((int area, int troops, int scouts) display)
        {
            player.TroopRPC(1, display.area, display.area+1, logged);
        }
    }
    public override void MasterEnd()
    {
        int newNum = (TurnManager.inst.GetInt(ConstantStrings.TurnNumber)%4) + 1;
        PhotonCompatible.InstantChangeRoomProp(ConstantStrings.TurnNumber, newNum);
    }
}
