using UnityEngine;

public class Camp : CardType
{
    public Camp(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        (int area, int highestScouts) best = (0, 0);
        int[] playerScouts = player.GetScouts();
        for (int i = 0; i<playerScouts.Length; i++)
        {
            if (playerScouts[i] >= best.highestScouts)
                best = (i, playerScouts[i]);
        }
        if (best.area == thisArea)
            player.ActionRPC(1, logged);
        else
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                
    }
}
