using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class BetweenEffects : Turn
{
    public override void MasterStart()
    {
        Log.inst.MasterText(true, AutoTranslate.Blank());
        Log.inst.MasterText(true, OnlineTranslate.Online_Between_Turns());
    }
    public override void ForPlayer(Player player)
    {
        List<Card> listOfCards = new();
        string[] effects = (string[])PhotonCompatible.GetRoomProperty(ConstantStrings.BetweenEffects);
        foreach (string next in effects)
        {
            string[] splitUp = next.Split('\t');
            if (int.Parse(splitUp[0]) == player.photonView.ViewID)
                listOfCards.Add(PhotonView.Find(int.Parse(splitUp[1])).GetComponent<Card>());
        }
        if (listOfCards.Count >= 1)
            Log.inst.NewDecisionContainer(() => ResolveEffects(player, listOfCards, 0));
        else
            player.endPause = false;
    }
    void ResolveEffects(Player player, List<Card> listOfCards, int logged)
    {
        List<CardButtonInfo> cardInfo = new();
        foreach (Card card in listOfCards)
            cardInfo.Add(new CardButtonInfo(card, PickedCard));
        MakeDecision.inst.ChooseCardInPopup(cardInfo, AutoTranslate.Choose_Resolve_Effect());

        void PickedCard(Card card)
        {
            List<Card> newList = new();
            newList.AddRange(listOfCards);
            newList.Remove(card);

            Log.inst.AddMyText(false, OnlineTranslate.Online_Resolve_Card(player.name, card.name), logged);
            Log.inst.NewDecisionContainer(() => card.thisCard.BetweenTurnInstructions(player, logged+1));
            Log.inst.NewDecisionContainer(() => ResolveEffects(player, newList, logged));
        }
    }
    public override void MasterEnd()
    {
        PhotonCompatible.InstantChangeRoomProp(ConstantStrings.NextPhase, nameof(VisitArea));
    }
}