using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotDisplay : MonoBehaviour
{
    public void NextStep()
    {
        PlotManager.Instance.TriggerUserAction(false, 0);
    }

    public void Select()
    {
        PlotManager.Instance.TriggerUserAction(true, 1);
    }
}
