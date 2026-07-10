using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using System;
public class TakeTurn : Turn
{
    public override void MasterStart()
    {
        int currentTurn = TurnManager.inst.GetInt(ConstantStrings.TurnNumber);
        Log.inst.MasterText(true, AutoTranslate.Blank());
        Log.inst.MasterText(true, OnlineTranslate.Online_Next_Turn(currentTurn.ToString()));
    }
    public override void ForPlayer(Player player)
    {
        Log.inst.NewDecisionContainer(() => ChooseToken(player, 0));
        Dictionary<TokenType, int[]> newDictionary = new();
        foreach (TokenType token in Enum.GetValues(typeof(TokenType)))
        {
            int arrayLength = player.GetTokenDict()[token].Length;
            newDictionary.Add(token, new int[arrayLength]);
        }
        Log.inst.NewDecisionContainer(() => DoSelling(player, newDictionary, null, 0));
    }
    void ChooseToken(Player player, int logged)
    {
        List<TextButtonInfo> addTokens = new()
        {
            new(AutoTranslate.ArtIcon(), () => AddThis(TokenType.ArtIcon)),
            new(AutoTranslate.HouseIcon(), () => AddThis(TokenType.HouseIcon)),
            new(AutoTranslate.ToolIcon(), () => AddThis(TokenType.ToolIcon)),
            new(AutoTranslate.BookIcon(), () => AddThis(TokenType.BookIcon))
        };
        MakeDecision.inst.ChooseTextButton(addTokens, AutoTranslate.Ask_Create(AutoTranslate.TokenIcon(), "1", "1"));

        void AddThis(TokenType type)
        {
            TurnManager.inst.WillChangePlayerProperty(player, ConstantStrings.ChosenToken, type.ToString());
            Log.inst.AddMyText(true, OnlineTranslate.Online_Chose_Token(player.name, type.ToString()), logged);
            player.CreateLoseToken(1, (1, type), logged+1);
            Log.inst.NewDecisionContainer(() => AdvanceToken(player, type, logged+1));
        }
    }
    void AdvanceToken(Player player, TokenType token, int logged)
    {
        List<int> levelsToAdvance = Player.AllLevelsBut(TurnManager.inst.GetInt(ConstantStrings.MaxLevel));
        List<TokenDisplay> canAdvance = player.OfNumber(FindNumber.Minimum, new() {token}, levelsToAdvance, 1);
        MakeDecision.inst.ChooseDisplayOnScreen(canAdvance, AutoTranslate.Ask_Upgrade(Translator.inst.Translate(token.ToString()), "1", "1"), AdvanceThis);

        void AdvanceThis((int level, TokenType type) info)
        {
            player.UpDowngradeToken(1, info, 1, logged);
        }
    }
    void DoSelling(Player player, Dictionary<TokenType, int[]> soldTokens, DecisionContainer rewind, int logged)
    {
        int minimum = 2;
        List<Card> customersInHand = player.GetHand();
        List<TokenDisplay> tokensToSubmit = player.OfNumber(FindNumber.Minimum, Player.AllTokens(), Player.AllLevels(), 1);
        DecisionContainer restartContainer = rewind;

        if (rewind == null)
        {
            restartContainer = Log.inst.currentContainer;
            if (player.AllTotalTokens() < minimum || customersInHand.Count < minimum)
            {
                NoSelling();
                return;
            }
        }

        int CountTotal()
        {
            int answer = 0;
            foreach (TokenType token in Enum.GetValues(typeof(TokenType)))
                answer += MyExtensions.SumOfArray(soldTokens[token]);
            return answer;        
        }
        int totalTokens = CountTotal();
        List<Card> customersHappy = new();
        foreach (Card card in customersInHand)
        {
            if (totalTokens >= 2 && card.thisCard.CanSell(player, soldTokens))
            {
                customersHappy.Add(card);
                card.selectMe.SetBorder(true, Color.yellow);
            }
            else
            {
                card.selectMe.SetBorder(false);
            }
        }

        List<TextButtonInfo> textOptions = new();
        if (totalTokens >= minimum && customersHappy.Count >= minimum)
            textOptions.Add(new(AutoTranslate.Confirm(), CompleteSell));
        if (totalTokens == 0)
            textOptions.Add(new(AutoTranslate.Decline(), NoSelling));
        else
            textOptions.Add(new(AutoTranslate.Undo_All(), UndoAll));

        MakeDecision.inst.ChooseTextButton(textOptions, AutoTranslate.Ask_Sell(), false);
        MakeDecision.inst.ChooseDisplayOnScreen(tokensToSubmit, AutoTranslate.Ask_Sell(), SellToken, false);

        void NoSelling()
        {
            TurnManager.inst.WillChangePlayerProperty(player, ConstantStrings.CustomersSold, 0);
            Log.inst.AddMyText(false, OnlineTranslate.Online_No_Sell(player.name), logged);            
        }

        void UndoAll()
        {
            Log.inst.InvokeUndo(rewind, false);
        }

        void CompleteSell()
        {
            Log.inst.AddMyText(true, OnlineTranslate.Online_Make_Sell(player.name, totalTokens.ToString(), customersHappy.Count.ToString()), logged);
            TurnManager.inst.WillChangePlayerProperty(player, ConstantStrings.CustomersSold, customersHappy.Count);
            int totalScore = 0;
            foreach (Card card in customersHappy)
            {
                totalScore += card.dataFile.coinAmount;
                player.DiscardCustomerRPC(card, logged+1);
                card.selectMe.SetBorder(false);
            }
            player.CoinRPC(totalScore, logged, true);
        }

        void SellToken((int value, TokenType token) info)
        {
            player.CreateLoseToken(-1, info, logged);
            Dictionary<TokenType, int[]> newDictionary = soldTokens;
            newDictionary[info.token][info.value]++;
            Log.inst.NewDecisionContainer(() => DoSelling(player, newDictionary, restartContainer, logged));
        }
    }
    public override void MasterEnd()
    {
        ExitGames.Client.Photon.Hashtable toChange = new();
        bool triggeredTwist = false;

        foreach (Player player in CreateGame.inst.GetPlayers())
        {
            string selectedToken = TurnManager.inst.GetString(ConstantStrings.ChosenToken, player);
            //Debug.Log($"{player.name}, {selectedToken}");

            string targetString = ConstantStrings.TokenCounter(selectedToken);
            if (!toChange.ContainsKey(targetString))
            {
                int currentCounter = TurnManager.inst.GetInt(targetString);
                toChange[targetString] = currentCounter - 1;
                if (currentCounter - 1 <= 0)
                    triggeredTwist = true;
            }
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(toChange);
        PhotonCompatible.InstantChangeRoomProp(ConstantStrings.TurnNumber, TurnManager.inst.GetInt(ConstantStrings.TurnNumber)+1);
        if (triggeredTwist)
            PhotonCompatible.InstantChangeRoomProp(ConstantStrings.NextPhase, nameof(ResolveTwists));
    }
}
