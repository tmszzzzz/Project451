using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyNodeBehavior : NodeBehavior
{
    public override void SetState(Properties.StateEnum stateEnum)
    {
        if (properties.state == Properties.StateEnum.NORMAL && stateEnum >= Properties.StateEnum.AWAKENED)
        {
            CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
            cb.UnlockTriggerConnectionsByTag(properties.unlockTag);
        }
        if (stateEnum == Properties.StateEnum.NORMAL && properties.state >= Properties.StateEnum.AWAKENED)
        {
            CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
            cb.UnlockCancelConnectionsByTag(properties.unlockTag);
        }
        base.SetState(stateEnum);
    }
}
