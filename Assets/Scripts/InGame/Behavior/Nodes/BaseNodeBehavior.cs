using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNodeBehavior : MonoBehaviour
{
    public abstract StatePrediction PredictState();
    public abstract void SetState(Properties.StateEnum stateEnum);
    public struct StatePrediction
    {
        public Properties.StateEnum state;
        public int influence;
        public StatePrediction(Properties.StateEnum s, int i)
        {
            state = s;
            influence = i;
        }
    }
}
