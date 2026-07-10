using UnityEngine;

public class WaitForJoiners : Turn
{
    public override void ForPlayer(Player player)
    {
        player.endPause = false;
    }
}
