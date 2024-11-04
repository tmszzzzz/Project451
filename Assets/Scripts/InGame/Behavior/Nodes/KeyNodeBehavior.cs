using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyNodeBehavior : NodeBehavior
{
    protected int unlockTag;
    public override void SetState(Properties.StateEnum stateEnum)
    {
        if (properties.state == Properties.StateEnum.NORMAL && stateEnum >= Properties.StateEnum.AWAKENED)
        {
            CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
            cb.UnlockTriggerConnectionsByTag(unlockTag);
        }
        if (stateEnum == Properties.StateEnum.NORMAL && properties.state >= Properties.StateEnum.AWAKENED)
        {
            CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
            cb.UnlockCancelConnectionsByTag(unlockTag);
        }
        base.SetState(stateEnum);
    }
}
