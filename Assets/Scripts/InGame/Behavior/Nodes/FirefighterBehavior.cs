using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FirefighterBehavior : NodeBehavior
{
    protected override void Start()
    {
        base.Start();
    }
    public override StatePrediction PredictState()
    {
        return base.PredictState();
    }

    protected override void OnRoundChange()
    {
        base.OnRoundChange();
        if (properties.state >= Properties.StateEnum.AWAKENED)
        {
            CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
            if (cb == null)
            {
                Debug.LogWarning("Script \"CanvasBehavior\" not found in canvas.");
                return;
            }
            cb.AddNodeNumOfBooks(cb.Me, GlobalVar.Instance.NumOfFirefighterGiveBooks);
            mb.AddMessage($"Firefighter {gameObject.name} gives you {GlobalVar.Instance.NumOfFirefighterGiveBooks} books in this round.");
        }
    }

    
}
