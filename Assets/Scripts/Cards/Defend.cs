using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Defend : CardType
{
    public Defend(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        int highestTroops = 0;
        List<int> highestAreas = new();
        int[] playerTroops = player.GetTroops();

        for (int i = 0; i<playerTroops.Length; i++)
        {
            if (playerTroops[i] > highestTroops)
            {
                highestTroops = playerTroops[i];
                highestAreas.Clear(); highestAreas.Add(i);
            }
            else if (playerTroops[i] == highestTroops)
            {
                highestAreas.Add(i);
            }
        }

        List<TextButtonInfo> textButtons = new();
        foreach (int next in highestAreas)
        {
            int number = next;
            textButtons.Add(new TextButtonInfo(Translator.inst.Translate($"Area_{next}"), () => GetTravel(number)));
        }
        MakeDecision.inst.ChooseTextButton(textButtons, AutoTranslate.Choose_One(nameof(Defend)));

        void GetTravel(int area)
        {
            GetTravelBonus(player, area, logged, 2);
        }
    }
}