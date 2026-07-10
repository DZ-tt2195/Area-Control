using UnityEngine;

public class Turn
{
    public Turn()
    {
        
    }
    public virtual void MasterStart()
    {

    }

    public virtual void ForPlayer(Player player)
    {
        //Log.inst.NewDecisionContainer(this, () => InstantDraw(player), 0);
    }

    public virtual void MasterEnd()
    {

    }
}
