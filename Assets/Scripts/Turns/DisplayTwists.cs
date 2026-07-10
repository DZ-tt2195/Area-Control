using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class DisplayTwists : Turn
{
    public override void MasterStart()
    {
        CreateGame.inst.CreateAreas();
    }

    public override void ForPlayer(Player player)
    {
        CreateGame.inst.AddPlayerRPC(player);
        player.DrawCustomerRPC(4);
    }
}
