using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseNodeBehavior : MonoBehaviour
{
    public abstract StatePrediction PredictState();
    public abstract StatePrediction NowState();
    public abstract void SetState(Properties.StateEnum stateEnum);
    
    public struct StatePrediction
    {
        public Properties.StateEnum state;
        public int influence;
        public int additionalInfluence;
        public StatePrediction(Properties.StateEnum s, int i, int j)
        {
            state = s;
            influence = i;
            additionalInfluence = j;
        }
    }
}
