using UnityEngine;

public class Chart : CardType
{
    public Chart(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        ChooseDiscard(player, this.dataFile.cardName, false, logged, 1, Reward);
        void Reward(int num)
        {
            if (num == 1)
                ChooseAdvance(player, logged, 1);
        }
    }
}
