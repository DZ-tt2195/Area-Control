using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class GeneralEffects
{

#region New Decision
    public void ChooseDiscard(Player player, string cardName, bool firstMandatory, int logged, int maxNum, Action<List<Card>> whenDone = null)
    {
        if (maxNum >= 1)
            Log.inst.NewDecisionContainer(() => DoDiscard(firstMandatory, new()));
        
        void DoDiscard(bool mandatory, List<Card> currentDiscards)
        {
            List<Card> canDiscard = player.GetHand();
            if (canDiscard.Count < maxNum && !mandatory)
            {
                DidNot();
                return;
            }

            string instructionText = mandatory ? AutoTranslate.Force_Discard(Translator.inst.Translate(cardName), (currentDiscards.Count+1).ToString(), maxNum.ToString()) : AutoTranslate.Ask_Discard(Translator.inst.Translate(cardName));
            MakeDecision.inst.ChooseCardOnScreen(canDiscard, instructionText, DiscardMe, mandatory);
            if (!mandatory)
                MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), DidNot)}, instructionText, false);

            void DiscardMe(Card card)
            {
                player.DiscardCardRPC(card, logged);
                List<Card> newList = new();
                newList.AddRange(currentDiscards); newList.Add(card);

                if (newList.Count < maxNum)
                    Log.inst.NewDecisionContainer(() => DoDiscard(true, newList));
                else
                    whenDone?.Invoke(newList);
            }        
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Discard(player.name), logged);
                whenDone?.Invoke(currentDiscards);
            }
        }
    }
    public void ChooseAdvance(Player player, string cardName, int logged, int maxNum, Action<List<int>> whenDone = null) 
    {
        if (maxNum >= 1)
            Log.inst.NewDecisionContainer(() => DoAdvance(new()));

        void DoAdvance(List<int> currentAdvances)
        {
            List<TroopScoutDisplay> canAdvance = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.area != 4 && display.info.troops >= 1).ToList();
            if (canAdvance.Count == 0)         
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Advance(player.name));
                whenDone?.Invoke(currentAdvances);
                return;
            }

            MakeDecision.inst.ChooseDisplayOnScreen(canAdvance, AutoTranslate.Force_Advance(Translator.inst.Translate(cardName), (currentAdvances.Count+1).ToString(), maxNum.ToString()), AdvanceMe);
            void AdvanceMe((int area, int troops, int scouts) display)
            {
                player.TroopRPC(1, display.area, display.area+1, logged);
                List<int> newList = new();
                newList.AddRange(currentAdvances); newList.Add(display.area);

                if (newList.Count < maxNum)
                    Log.inst.NewDecisionContainer(() => DoAdvance(newList));
                else
                    whenDone?.Invoke(newList);
            }
        }
    }
    public void ChooseRetreat(Player player, string cardName, bool firstMandatory, int logged, int maxNum, Action<List<int>> whenDone = null) 
    {
        if (maxNum >= 1)
            Log.inst.NewDecisionContainer(() => DoRetreat(firstMandatory, new()));

        void DoRetreat(bool mandatory, List<int> currentRetreats)
        {
            List<TroopScoutDisplay> canRetreat = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.area != 1 && display.info.troops >= 1).ToList();
            int numTroops = 0;
            foreach (TroopScoutDisplay display in canRetreat)
                numTroops+=display.info.troops;

            if (numTroops < maxNum && !mandatory)
            {
                DidNot();
                return;
            }

            string instructionText = mandatory ? AutoTranslate.Force_Retreat(Translator.inst.Translate(cardName), currentRetreats+1.ToString(), maxNum.ToString()) : AutoTranslate.Ask_Retreat(Translator.inst.Translate(cardName));
            MakeDecision.inst.ChooseDisplayOnScreen(canRetreat, instructionText, RetreatMe, mandatory);
            if (!mandatory)
                MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), DidNot)}, instructionText, false);

            void RetreatMe((int area, int troops, int scouts) display)
            {
                player.TroopRPC(1, display.area, display.area-1, logged);
                List<int> newList = new();
                newList.AddRange(currentRetreats); newList.Add(display.area);

                if (newList.Count < maxNum)
                    Log.inst.NewDecisionContainer(() => DoRetreat(true, newList));
                else
                    whenDone?.Invoke(newList);
            }        
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Retreat(player.name), logged);
                whenDone?.Invoke(currentRetreats);
            }
        }
    }
    public void ChooseAddScout(Player player, string cardName, int logged, int maxNum, Action<List<int>> whenDone = null) 
    {
        if (maxNum >= 1)
            Log.inst.NewDecisionContainer(() => DoAddScout(new()));
    
        void DoAddScout(List<int> currentAdds)
        {
            List<TroopScoutDisplay> canAdd = CreateGame.inst.GetAllDisplays(player);
            MakeDecision.inst.ChooseDisplayOnScreen(canAdd, AutoTranslate.Force_Add(Translator.inst.Translate(cardName), (currentAdds.Count+1).ToString(), maxNum.ToString()), AddMe);
            void AddMe((int area, int troops, int scouts) display)
            {
                player.ScoutRPC(1, display.area, logged);
                List<int> newList = new();
                newList.AddRange(currentAdds); newList.Add(display.area);

                if (newList.Count < maxNum)
                    Log.inst.NewDecisionContainer(() => DoAddScout(newList));
                else
                    whenDone?.Invoke(newList);
            }
        }
    }
    public void ChooseRemoveScout(Player player, string cardName, bool firstMandatory, int logged, int maxNum, Action<List<int>> whenDone = null) 
    {
        if (maxNum >= 1)
            Log.inst.NewDecisionContainer(() => DoRemove(firstMandatory, new()));

        void DoRemove(bool mandatory, List<int> currentRemoves)
        {
            List<TroopScoutDisplay> canRemove = CreateGame.inst.GetAllDisplays(player).Where(display => display.info.scouts >= 1).ToList();
            int numScouts = 0;
            foreach (TroopScoutDisplay display in canRemove)
                numScouts+=display.info.scouts;

            if (numScouts < maxNum && !mandatory)
            {
                DidNot();
                return;
            }

            string instructionText = mandatory ? AutoTranslate.Force_Remove(Translator.inst.Translate(cardName), (currentRemoves.Count+1).ToString(), maxNum.ToString()) : AutoTranslate.Ask_Remove(Translator.inst.Translate(cardName));
            MakeDecision.inst.ChooseDisplayOnScreen(canRemove, instructionText, RemoveMe, mandatory);
            if (!mandatory)
                MakeDecision.inst.ChooseTextButton(new() {new TextButtonInfo(AutoTranslate.Decline(), DidNot)}, instructionText, false);

            void RemoveMe((int area, int troops, int scouts) display)
            {
                player.ScoutRPC(-1, display.area, logged);
                List<int> newList = new();
                newList.AddRange(currentRemoves); newList.Add(display.area);

                if (newList.Count < maxNum)
                    Log.inst.NewDecisionContainer(() => DoRemove(true, newList));
                else
                    whenDone?.Invoke(newList);
            }        
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Remove(player.name), logged);
                whenDone?.Invoke(currentRemoves);
            }
        }
    }
    public void AskSpendAction(Player player, string cardName, int amount, int logged, Action<bool> whenDone = null)
    {
        if (amount >= 1)
            Log.inst.NewDecisionContainer(() => MaySpendAction());
        
        void MaySpendAction()
        {
            if (player.GetActions() < amount)
            {
                DidNot();
                return;
            }
            List<TextButtonInfo> textButtonInfos = new() {new(AutoTranslate.Confirm(), DidIt), new(AutoTranslate.Decline(), DidNot)};
            MakeDecision.inst.ChooseTextButton(textButtonInfos, AutoTranslate.Ask_Pay(Translator.inst.Translate(cardName), amount.ToString(), AutoTranslate.ActionIcon()));

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
        if (amount >= 1)
            Log.inst.NewDecisionContainer(() => MaySpendCoin());
        
        void MaySpendCoin()
        {
            if (player.GetCoins() < amount)
            {
                DidNot();
                return;
            }
            List<TextButtonInfo> textButtonInfos = new() {new(AutoTranslate.Confirm(), DidIt), new(AutoTranslate.Decline(), DidNot)};
            MakeDecision.inst.ChooseTextButton(textButtonInfos, AutoTranslate.Ask_Pay(Translator.inst.Translate(cardName), amount.ToString(), AutoTranslate.CoinIcon()));

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
    public void GetTravelBonus(Player player, int area, int logged, int multiplier = 1)
    {
        switch (area)
        {
            case 1:
                player.DrawCardRPC(1*multiplier, logged);
                break;
            case 2:
                player.CoinRPC(3*multiplier, logged);
                break;
            case 3:
                player.ActionRPC(1*multiplier, logged);
                break;
            case 4:
                player.CoinRPC(3*multiplier, logged);
                break;
        }        
    }
    public void PlayCard(Player player, Card card, int area, int logged, int iterations = 1, bool payAction = true)
    {
        Log.inst.AddMyText(true, OnlineTranslate.Online_Play_Card(player.name, card.name), logged);
        if (payAction) player.ActionRPC(-1, logged+1);
        player.CoinRPC(-card.dataFile.coinCost, logged+1);
        player.DiscardCardRPC(card, -1);

        ChooseAdvance(player, card.name, logged+1, card.dataFile.troopAdvance);
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