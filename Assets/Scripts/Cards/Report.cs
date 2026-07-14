using UnityEngine;

public class Report : CardType
{
    public Report(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        AskRemoveScout(player, this.dataFile.cardName, logged, () => player.DrawCardRPC(1, logged));
    }
}
