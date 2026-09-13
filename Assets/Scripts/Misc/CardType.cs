using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;
using System;

public class CardType : GeneralEffects
{
    public Card cardObject {get; private set;}
    public CardData dataFile { get; private set; }

    public CardType(Card card, CardData dataFile)
    {
        this.cardObject = card;
        this.dataFile = dataFile;
    }
    public virtual void DoInstructions(Player player, int thisArea, int logged)
    {
    }
    public virtual void BetweenTurnInstructions(Player player, int logged)
    {
    }
}