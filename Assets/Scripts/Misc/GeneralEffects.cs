using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class GeneralEffects
{

#region Mandatory
    public void ForceDiscard(Player player, int logged, int maxNum, Action whenDone = null)
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoDiscard(1));
        
        void DoDiscard(int currentNum)
        {
            List<Card> canDiscard = player.GetHand();
            if (canDiscard.Count == 0) 
            {
                whenDone?.Invoke();
                return;
            }

            MakeDecision.inst.ChooseCardOnScreen(canDiscard, AutoTranslate.Force_Discard(currentNum.ToString(), maxNum.ToString()), DiscardMe);
            void DiscardMe(Card card)
            {
                player.DiscardCardRPC(card, logged);
                if (currentNum < maxNum)
                {
                    int newNum = currentNum+1;
                    Log.inst.NewDecisionContainer(() => DoDiscard(newNum));
                }
                else
                {
                    whenDone?.Invoke();
                }
            }        
        }
    }
    public void ForceAdvance(Player player, int logged, int maxNum, Action whenDone = null) 
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoAdvance( 1));

        void DoAdvance(int currentNum)
        {
            List<TroopScoutDisplay> canAdvance = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.area != 4 && display.info.troops >= 1).ToList();
            if (canAdvance.Count == 0)         
            {
                whenDone?.Invoke();
                return;
            }

            MakeDecision.inst.ChooseDisplayOnScreen(canAdvance, AutoTranslate.Force_Advance(currentNum.ToString(), maxNum.ToString()), AdvanceMe);
            void AdvanceMe((int area, int troops, int scouts) display)
            {
                player.TroopRPC(1, display.area, display.area+1, logged);
                if (currentNum < maxNum)
                {
                    int newNum = currentNum+1;
                    Log.inst.NewDecisionContainer(() => DoAdvance(newNum));
                }
                else
                {
                    whenDone?.Invoke();
                }
            }
        }
    }
    public void ForceRetreat(Player player, int logged, int maxNum, Action whenDone = null) 
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoRetreat(1));

        void DoRetreat(int currentNum)
        {
            List<TroopScoutDisplay> canRetreat = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.area != 1 && display.info.troops >= 1).ToList();
            if (canRetreat.Count == 0)         
            {
                whenDone?.Invoke();
                return;
            }

            MakeDecision.inst.ChooseDisplayOnScreen(canRetreat, AutoTranslate.Force_Retreat(currentNum.ToString(), maxNum.ToString()), RetreatMe);
            void RetreatMe((int area, int troops, int scouts) display)
            {
                player.TroopRPC(1, display.area, display.area-1, logged);
                if (currentNum < maxNum)
                {
                    int newNum = currentNum+1;
                    Log.inst.NewDecisionContainer(() => DoRetreat(newNum));
                }
                else
                {
                    whenDone?.Invoke();
                }
            }
        }
    }
    public void ForceAddScout(Player player, int logged, int maxNum, Action whenDone = null) 
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
    public void ForceRemoveScout(Player player, int logged, int maxNum, Action whenDone = null) 
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoRemoveScout(1));
    
        void DoRemoveScout(int currentNum)
        {
            List<TroopScoutDisplay> canRemove = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.scouts >= 1).ToList();
            if (canRemove.Count == 0)         
            {
                whenDone?.Invoke();
                return;
            }

            MakeDecision.inst.ChooseDisplayOnScreen(canRemove, AutoTranslate.Force_Retreat(currentNum.ToString(), maxNum.ToString()), RemoveMe);
            void RemoveMe((int area, int troops, int scouts) display)
            {
                player.ScoutRPC(-1, display.area, logged);
                if (currentNum < maxNum)
                {
                    int newNum = currentNum+1;
                    Log.inst.NewDecisionContainer(() => DoRemoveScout(newNum));
                }
                else
                {
                    whenDone?.Invoke();
                }
            }
        }
    }

#endregion

#region Optional
    public void AskDiscard(Player player, string cardName, int logged, Action whenDone = null, Action whenFailed = null)
    {
        Log.inst.NewDecisionContainer(() => MayDiscard());
        void MayDiscard()
        {
            List<Card> canDiscard = player.GetHand();
            if (canDiscard.Count == 0)
            {
                DidNot();
                return;
            }

            MakeDecision.inst.ChooseCardOnScreen(canDiscard, AutoTranslate.Ask_Discard(), DiscardMe, false);
            MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), DidNot)}, AutoTranslate.Ask_Discard(), false);

            void DiscardMe(Card card)
            {
                player.DiscardCardRPC(card, logged);
                whenDone?.Invoke();
            }        
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Decline_Ability(player.name, cardName), logged);
                whenFailed?.Invoke();
            }
        }
    }
    public void AskRetreat(Player player, string cardName, int logged, Action whenDone = null, Action whenFailed = null)
    {
        Log.inst.NewDecisionContainer(() => MayRetreat());
        void MayRetreat()
        {
            List<TroopScoutDisplay> canRetreat = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.area != 1 && display.info.troops >= 1).ToList();
            if (canRetreat.Count == 0)
            {
                DidNot();
                return;
            }

            MakeDecision.inst.ChooseDisplayOnScreen(canRetreat, AutoTranslate.Ask_Retreat(), RetreatMe, false);
            MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), DidNot)}, AutoTranslate.Ask_Retreat(), false);

            void RetreatMe((int area, int troops, int scouts) display)
            {
                player.TroopRPC(1, display.area, display.area-1, logged);
                whenDone?.Invoke();
            }        
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Decline_Ability(player.name, cardName), logged);
                whenFailed?.Invoke();
            }
        }
    }
    public void AskRemoveScout(Player player, string cardName, int logged, Action whenDone = null, Action whenFailed = null)
    {
        Log.inst.NewDecisionContainer(() => MayRemoveScout());
        void MayRemoveScout()
        {
            List<TroopScoutDisplay> canRemove = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.scouts >= 1).ToList();
            if (canRemove.Count == 0) 
            {
                DidNot();
                return;
            }

            MakeDecision.inst.ChooseDisplayOnScreen(canRemove, AutoTranslate.Ask_Remove(), RemoveMe, false);
            MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), DidNot)}, AutoTranslate.Ask_Remove(), false);

            void RemoveMe((int area, int troops, int scouts) display)
            {
                player.ScoutRPC(-1, display.area, logged);
                whenDone?.Invoke();
            }        
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Decline_Ability(player.name, cardName), logged);
                whenFailed?.Invoke();
            }
        }
    }
    public void AskSpendAction(Player player, string cardName, int amount, int logged, Action whenDone = null, Action whenFailed = null)
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
                whenDone?.Invoke();
            }
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Decline_Ability(player.name, cardName), logged);
                whenFailed?.Invoke();
            }
        }
    }
    public void AskSpendCoin(Player player, string cardName, int amount, int logged, Action whenDone = null, Action whenFailed = null)
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
                whenDone?.Invoke();
            }
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Decline_Ability(player.name, cardName), logged);
                whenFailed?.Invoke();
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
        ForceAdvance(player, logged+1, card.dataFile.troopAdvance);
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