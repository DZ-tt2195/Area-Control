using UnityEngine;

public class Visit : CardType
{
    public Visit(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseRemoveScout(player, this.dataFile.cardName, false, logged, 1, Reward);
        void Reward(int num)
        {
            if (num == 1)
                GetTravelBonus(player, thisArea, logged);
        }
    }
}
