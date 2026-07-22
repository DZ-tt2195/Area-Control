using UnityEngine;

public class Trade : CardType
{
    public Trade(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        AskDiscard(player, this.dataFile.cardName, logged, card => player.CoinRPC(5, logged));
    }
}
