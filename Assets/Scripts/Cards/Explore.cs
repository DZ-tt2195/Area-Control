using UnityEngine;

public class Explore : CardType
{
    public Explore(Card card, CardData dataFile) : base(card, dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        int[] troops = player.GetTroops();
        for (int i = 1; i<troops.Length; i++)
        {
            if (troops[i] == 0)
                player.ScoutRPC(2, i, logged);
        }
    }
}
