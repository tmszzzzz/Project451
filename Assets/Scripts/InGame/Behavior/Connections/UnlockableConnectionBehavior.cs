using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableConnectionBehavior : ConnectionBehavior
{
    private bool available = false;
    private int unlockState = 0;

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
            GetComponent<LineRenderer>().startColor = Color.gray;
            GetComponent<LineRenderer>().endColor = Color.gray;
        }
    }
}