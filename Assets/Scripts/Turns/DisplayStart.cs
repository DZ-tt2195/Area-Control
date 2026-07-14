using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class DisplayStart : Turn
{
    public override void MasterStart()
    {
        CreateGame.inst.CreateAreas();
        PhotonCompatible.InstantChangeRoomProp(ConstantStrings.NextPhase, nameof(TakeTurn));
    }
    public override void ForPlayer(Player player)
    {
        CreateGame.inst.AddPlayerRPC(player);
        player.DrawCardRPC(2);
        player.CoinRPC(3);
    }
}
