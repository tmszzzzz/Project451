using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNodeBehavior : MonoBehaviour
{
    public abstract Properties.StateEnum RefreshState();
    public abstract void SetState(Properties.StateEnum stateEnum);
}
