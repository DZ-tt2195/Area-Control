using UnityEngine;

public class City : CardType
{
    public City(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (CreateGame.inst.IsControlling(player, thisArea))
            player.ActionRPC(Mathf.FloorToInt(player.GetHand().Count/2f), logged);
        else
            Log.inst.AddMyText(false, OnlineTranslate.Online_Miss_Ability(player.name, this.dataFile.cardName), logged);                

    }
}
