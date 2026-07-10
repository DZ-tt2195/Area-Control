using UnityEngine;

public class Mass_Market : CardType
{
    public Mass_Market(CardData dataFile) : base(dataFile)
    {
    }

    public override void TwistEffect(Player player, int logged)
    {
        int getCustomersSold = TurnManager.inst.GetInt(ConstantStrings.CustomersSold, player);
        player.CoinRPC(Mathf.FloorToInt(getCustomersSold/2f), logged);
    }
}
