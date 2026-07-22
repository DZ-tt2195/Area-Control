using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class DisplayStart : Turn
{
    public override void MasterStart()
    {
        CreateGame.inst.CreateAreas();
        PhotonCompatible.InstantChangeRoomProp(ConstantStrings.NextPhase, nameof(VisitArea));
    }
    public override void ForPlayer(Player player)
    {
        player.DrawCardRPC(3);
        player.CoinRPC(3);
        CreateGame.inst.AddPlayerRPC(player);
    }
}
