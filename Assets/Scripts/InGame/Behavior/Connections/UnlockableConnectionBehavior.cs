using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableConnectionBehavior : ConnectionBehavior
{
    public bool available = false;
    public int unlockState = 0;

    void Awake()
    {
        type = 1;
    }

    public override GameObject GetTheOtherNodeIfExist(GameObject node)
    {
        if (!available) return null;
        return base.GetTheOtherNodeIfExist(node);
    }
    public override void UnlockTrigger(int inputUnlockTag)
    {
        if (inputUnlockTag == unlockTag)
        {
            unlockState++;
            if (unlockState >= unlockDemand) available = true;
        }
    }
    public override void UnlockCancel(int inputUnlockTag)
    {
        if (inputUnlockTag == unlockTag)
        {
            unlockState--;
            if (unlockState < unlockDemand) available = false;
        }
    }
    public override void RefreshColor(DetectiveBehavior db)
    {
        if (available) base.RefreshColor(db);
        else
        {
            GetComponent<LineRenderer>().startColor = Color.black;
            GetComponent<LineRenderer>().endColor = Color.black;
        }
    }
}
