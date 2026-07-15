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
            Log.inst.NewDecisionContainer(() => DoDiscard(player, logged, 1, maxNum, whenDone));
    }
    void DoDiscard(Player player, int logged, int currentNum, int maxNum, Action whenDone)
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
                Log.inst.NewDecisionContainer(() => DoDiscard(player, logged, newNum, maxNum, whenDone));
            }
            else
            {
                whenDone?.Invoke();
            }
        }        
    }
    public void ForceAdvance(Player player, int logged, int maxNum, Action whenDone = null) 
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoAdvance(player, logged, 1, maxNum, whenDone));
    }
    void DoAdvance(Player player, int logged, int currentNum, int maxNum, Action whenDone)
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
                Log.inst.NewDecisionContainer(() => DoAdvance(player, logged, newNum, maxNum, whenDone));
            }
            else
            {
                whenDone?.Invoke();
            }
        }
    }
    public void ForceRetreat(Player player, int logged, int maxNum, Action whenDone = null) 
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoRetreat(player, logged, 1, maxNum, whenDone));
    }
    void DoRetreat(Player player, int logged, int currentNum, int maxNum, Action whenDone)
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
                Log.inst.NewDecisionContainer(() => DoRetreat(player, logged, newNum, maxNum, whenDone));
            }
            else
            {
                whenDone?.Invoke();
            }
        }
    }
    public void ForceAddScout(Player player, int logged, int maxNum, Action whenDone = null) 
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoAddScout(player, logged, 1, maxNum, whenDone));
    }
    void DoAddScout(Player player, int logged, int currentNum, int maxNum, Action whenDone)
    {
        List<TroopScoutDisplay> canAdd = CreateGame.inst.GetAllDisplays(player);
        MakeDecision.inst.ChooseDisplayOnScreen(canAdd, AutoTranslate.Force_Add(currentNum.ToString(), maxNum.ToString()), AddMe);
        void AddMe((int area, int troops, int scouts) display)
        {
            player.ScoutRPC(1, display.area, logged);
            if (currentNum < maxNum)
            {
                int newNum = currentNum+1;
                Log.inst.NewDecisionContainer(() => DoAddScout(player, logged, newNum, maxNum, whenDone));
            }
            else
            {
                whenDone?.Invoke();
            }
        }
    }
    public void ForceRemoveScout(Player player, int logged, int maxNum, Action whenDone = null) 
    {
        if (maxNum > 1)
            Log.inst.NewDecisionContainer(() => DoRemoveScout(player, logged, 1, maxNum, whenDone));
    }
    void DoRemoveScout(Player player, int logged, int currentNum, int maxNum, Action whenDone)
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
                Log.inst.NewDecisionContainer(() => DoRemoveScout(player, logged, newNum, maxNum, whenDone));
            }
            else
            {
                whenDone?.Invoke();
            }
        }
    }

#endregion

#region Optional
    public void AskDiscard(Player player, string cardName, int logged, Action whenDone = null, Action whenFailed = null)
    {
        Log.inst.NewDecisionContainer(() => MayDiscard(player, cardName, logged, whenDone, whenFailed));
    }
    void MayDiscard(Player player, string cardName, int logged, Action whenDone, Action whenFailed)
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
    public void AskRetreat(Player player, string cardName, int logged, Action whenDone = null, Action whenFailed = null)
    {
        Log.inst.NewDecisionContainer(() => MayRetreat(player, cardName, logged, whenDone, whenFailed));
    }
    void MayRetreat(Player player, string cardName, int logged, Action whenDone, Action whenFailed)
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
    public void AskRemoveScout(Player player, string cardName, int logged, Action whenDone = null, Action whenFailed = null)
    {
        Log.inst.NewDecisionContainer(() => MayRemoveScout(player, cardName, logged, whenDone, whenFailed));
    }
    void MayRemoveScout(Player player, string cardName, int logged, Action whenDone, Action whenFailed)
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
    public void AskSpendAction(Player player, string cardName, int amount, int logged, Action whenDone = null, Action whenFailed = null)
    {
        Log.inst.NewDecisionContainer(() => MaySpendAction(player, cardName, amount, logged, whenDone, whenFailed));
    }
    void MaySpendAction(Player player, string cardName, int amount, int logged, Action whenDone, Action whenFailed)
    {
        if (player.GetActions() < amount)
        {
            DidNot();
            return;
        }
        List<TextButtonInfo> textButtonInfos = new() {new(AutoTranslate.Confirm(), DidIt), new(AutoTranslate.Decline(), DidNot)};
        MakeDecision.inst.ChooseTextButton(textButtonInfos, AutoTranslate.Card_Use_Ability(cardName));

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
    public void AskSpendCoin(Player player, string cardName, int amount, int logged, Action whenDone = null, Action whenFailed = null)
    {
        Log.inst.NewDecisionContainer(() => MaySpendCoin(player, cardName, amount, logged, whenDone, whenFailed));
    }
    void MaySpendCoin(Player player, string cardName, int amount, int logged, Action whenDone, Action whenFailed)
    {
        if (player.GetCoins() < amount)
        {
            DidNot();
            return;
        }
        List<TextButtonInfo> textButtonInfos = new() {new(AutoTranslate.Confirm(), DidIt), new(AutoTranslate.Decline(), DidNot)};
        MakeDecision.inst.ChooseTextButton(textButtonInfos, AutoTranslate.Card_Use_Ability(cardName));

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
#endregion

}
