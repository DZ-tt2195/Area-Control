using UnityEngine;

public class Farmland : CardType
{
    public Farmland(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (CreateGame.inst.IsControlling(player, thisArea))
            player.CoinRPC(2*player.GetActions(), logged);
    }
}
