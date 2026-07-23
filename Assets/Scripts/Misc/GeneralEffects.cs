using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class GeneralEffects
{

#region New Decision
    public void ChooseDiscard(Player player, string cardName, bool firstMandatory, int logged, int maxNum, Action<int> whenDone = null)
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoDiscard(firstMandatory, 0));
        
        void DoDiscard(bool mandatory, int currentNum)
        {
            List<Card> canDiscard = player.GetHand();
            if (canDiscard.Count < maxNum)
            {
                if (mandatory) DidNot();
                else whenDone.Invoke(currentNum);
                return;
            }

            string instructionText = mandatory ? AutoTranslate.Force_Discard(currentNum+1.ToString(), maxNum.ToString()) : AutoTranslate.Ask_Discard();
            MakeDecision.inst.ChooseCardOnScreen(canDiscard, instructionText, DiscardMe, mandatory);
            if (!mandatory)
                MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), DidNot)}, instructionText, false);

            void DiscardMe(Card card)
            {
                player.DiscardCardRPC(card, logged);
                int newNum = currentNum+1;
                if (newNum < maxNum)
                    Log.inst.NewDecisionContainer(() => DoDiscard(true, newNum));
                else
                    whenDone?.Invoke(newNum);
            }        
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, cardName), logged);
                whenDone?.Invoke(currentNum);
            }
        }
    }
    public void ChooseAdvance(Player player, int logged, int maxNum, Action<int> whenDone = null) 
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoAdvance(0));

        void DoAdvance(int currentNum)
        {
            List<TroopScoutDisplay> canAdvance = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.area != 4 && display.info.troops >= 1).ToList();
            if (canAdvance.Count == 0)         
            {
                whenDone?.Invoke(currentNum);
                return;
            }

            MakeDecision.inst.ChooseDisplayOnScreen(canAdvance, AutoTranslate.Force_Advance(currentNum.ToString(), maxNum.ToString()), AdvanceMe);
            void AdvanceMe((int area, int troops, int scouts) display)
            {
                player.TroopRPC(1, display.area, display.area+1, logged);
                int newNum = currentNum+1;
                if (newNum < maxNum)
                    Log.inst.NewDecisionContainer(() => DoAdvance(newNum));
                else
                    whenDone?.Invoke(newNum);
            }
        }
    }
    public void ChooseRetreat(Player player, string cardName, bool firstMandatory, int logged, int maxNum, Action<int> whenDone = null) 
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoRetreat(firstMandatory, 0));

        void DoRetreat(bool mandatory, int currentNum)
        {
            List<TroopScoutDisplay> canRetreat = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.area != 1 && display.info.troops >= 1).ToList();
            int numTroops = 0;
            foreach (TroopScoutDisplay display in canRetreat)
                numTroops+=display.info.scouts;

            if (numTroops < maxNum)
            {
                if (mandatory) DidNot();
                else whenDone.Invoke(currentNum);
                return;
            }

            string instructionText = mandatory ? AutoTranslate.Force_Retreat(currentNum.ToString(), maxNum.ToString()) : AutoTranslate.Ask_Retreat();
            MakeDecision.inst.ChooseDisplayOnScreen(canRetreat, instructionText, RetreatMe, mandatory);
            if (!mandatory)
                MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), DidNot)}, instructionText, false);

            void RetreatMe((int area, int troops, int scouts) display)
            {
                player.TroopRPC(1, display.area, display.area-1, logged);
                int newNum = currentNum+1;
                if (newNum < maxNum)
                    Log.inst.NewDecisionContainer(() => DoRetreat(true, newNum));
                else
                    whenDone?.Invoke(newNum);
            }        
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, cardName), logged);
                whenDone?.Invoke(currentNum);
            }
        }
    }
    public void ChooseAddScout(Player player, int logged, int maxNum, Action whenDone = null) 
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoAddScout(1));
    
        void DoAddScout(int currentNum)
        {
            List<TroopScoutDisplay> canAdd = CreateGame.inst.GetAllDisplays(player);
            MakeDecision.inst.ChooseDisplayOnScreen(canAdd, AutoTranslate.Force_Add(currentNum.ToString(), maxNum.ToString()), AddMe);
            void AddMe((int area, int troops, int scouts) display)
            {
                player.ScoutRPC(1, display.area, logged);
                if (currentNum < maxNum)
                {
                    int newNum = currentNum+1;
                    Log.inst.NewDecisionContainer(() => DoAddScout(newNum));
                }
                else
                {
                    whenDone?.Invoke();
                }
            }
        }
    }
    public void ChooseRemoveScout(Player player, string cardName, bool firstMandatory, int logged, int maxNum, Action<int> whenDone = null) 
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoRemove(firstMandatory, 0));

        void DoRemove(bool mandatory, int currentNum)
        {
            List<TroopScoutDisplay> canRemove = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.scouts >= 1).ToList();
            int numScouts = 0;
            foreach (TroopScoutDisplay display in canRemove)
                numScouts+=display.info.scouts;

            if (numScouts < maxNum)
            {
                if (mandatory) DidNot();
                else whenDone.Invoke(currentNum);
                return;
            }

            string instructionText = mandatory ? AutoTranslate.Force_Remove(currentNum.ToString(), maxNum.ToString()) : AutoTranslate.Ask_Remove();
            MakeDecision.inst.ChooseDisplayOnScreen(canRemove, instructionText, RemoveMe, mandatory);
            if (!mandatory)
                MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), DidNot)}, instructionText, false);

            void RemoveMe((int area, int troops, int scouts) display)
            {
                player.ScoutRPC(-1, display.area, logged);
                int newNum = currentNum+1;
                if (newNum < maxNum)
                    Log.inst.NewDecisionContainer(() => DoRemove(true, newNum));
                else
                    whenDone?.Invoke(newNum);
            }        
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, cardName), logged);
                whenDone?.Invoke(currentNum);
            }
        }
    }
    public void AskSpendAction(Player player, string cardName, int amount, int logged, Action<bool> whenDone = null)
    {
        Log.inst.NewDecisionContainer(() => MaySpendAction());
        void MaySpendAction()
        {
            if (player.GetActions() < amount)
            {
                DidNot();
                return;
            }
            List<TextButtonInfo> textButtonInfos = new() {new(AutoTranslate.Confirm(), DidIt), new(AutoTranslate.Decline(), DidNot)};
            MakeDecision.inst.ChooseTextButton(textButtonInfos, AutoTranslate.Ask_Pay(amount.ToString(), AutoTranslate.ActionIcon()));

            void DidIt()
            {
                player.ActionRPC(-amount, logged);
                whenDone?.Invoke(true);
            }
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, cardName), logged);
                whenDone?.Invoke(false);
            }
        }
    }
    public void AskSpendCoin(Player player, string cardName, int amount, int logged, Action<bool> whenDone = null)
    {
        Log.inst.NewDecisionContainer(() => MaySpendCoin());
        void MaySpendCoin()
        {
            if (player.GetCoins() < amount)
            {
                DidNot();
                return;
            }
            List<TextButtonInfo> textButtonInfos = new() {new(AutoTranslate.Confirm(), DidIt), new(AutoTranslate.Decline(), DidNot)};
            MakeDecision.inst.ChooseTextButton(textButtonInfos, AutoTranslate.Ask_Pay(amount.ToString(), AutoTranslate.CoinIcon()));

            void DidIt()
            {
                player.CoinRPC(-amount, logged);
                whenDone?.Invoke(true);
            }
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, cardName), logged);
                whenDone?.Invoke(false);
            }
        }
    }
#endregion

#region Misc
    public void GetTravelBonus(Player player, int area, int logged)
    {
        switch (area)
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
    }
    public void PlayCard(Player player, Card card, int area, int logged, int iterations = 1, bool payAction = true)
    {
        Log.inst.AddMyText(true, OnlineTranslate.Online_Play_Card(player.name, card.name), logged);
        if (payAction) player.ActionRPC(-1, logged+1);
        player.CoinRPC(-card.dataFile.coinCost, logged+1);
        player.DiscardCardRPC(card, -1);
        ChooseAdvance(player, logged+1, card.dataFile.troopAdvance);
        for (int i = 0; i<iterations; i++)
            Log.inst.NewDecisionContainer(() => card.thisCard.DoInstructions(player, area, logged+1));
    }
    public List<Card> CanAfford(Player player)
    {
        List<Card> toReturn = new();
        foreach (Card card in player.GetHand())
        {
            if (card.dataFile.coinCost <= player.GetCoins())
                toReturn.Add(card);
        }
        return toReturn;
    }

#endregion

}