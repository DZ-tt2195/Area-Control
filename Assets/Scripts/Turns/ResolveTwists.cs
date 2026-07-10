using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ResolveTwists : Turn
{
    (List<TokenType>, List<Card>) TwistsNoCounter()
    {
        List<TokenType> typesToResolve = new();
        List<Card> twistsToResolve = new();
        foreach (TokenType type in Enum.GetValues(typeof(TokenType)))
        {
            string tokencounter = ConstantStrings.TokenCounter(type);
            if (TurnManager.inst.GetInt(tokencounter) <= 0)
            {
                typesToResolve.Add(type);
                twistsToResolve.Add(CreateGame.inst.GetTwist(type).card);
            }
        }
        return (typesToResolve, twistsToResolve);        
    }

    public override void MasterStart()
    {
        List<Card> twistsToResolve = TwistsNoCounter().Item2;
        Log.inst.MasterText(true, AutoTranslate.Blank());
        Log.inst.MasterText(true, OnlineTranslate.Online_Twists_To_Resolve(twistsToResolve.Count.ToString()));
    }

    public override void ForPlayer(Player player)
    {
        List<Card> twistsToResolve = TwistsNoCounter().Item2;
        player.DrawCustomerRPC(2*twistsToResolve.Count);
        Log.inst.NewDecisionContainer(() => ChooseTwist(player, twistsToResolve));
    }

    void ChooseTwist(Player player, List<Card> twistsToResolve)
    {
        MakeDecision.inst.ChooseCardOnScreen(twistsToResolve, AutoTranslate.Ask_Resolve(), DoTwist);

        void DoTwist(Card card)
        {
            Log.inst.AddMyText(false, OnlineTranslate.Online_Resolve_Card(player.name, card.name));
            Log.inst.NewDecisionContainer(() => card.thisCard.TwistEffect(player, 1));

            List<Card> newList = twistsToResolve;
            newList.Remove(card);
            if (newList.Count > 0)
                Log.inst.NewDecisionContainer(() => ChooseTwist(player, newList));
        }
    }

    public override void MasterEnd()
    {
        List<TokenType> twistsToAdd = TwistsNoCounter().Item1;
        foreach (TokenType type in twistsToAdd)
        {
            string tokencounter = ConstantStrings.TokenCounter(type);
            PhotonCompatible.InstantChangeRoomProp(tokencounter, 2);
        }
    }
}
