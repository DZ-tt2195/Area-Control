using UnityEngine;
public class Ending : Turn
{
    public override void ForPlayer(Player player)
    {
        player.endPause = false;
    }
}
