using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BibliophileBehavior : NodeBehavior
{
    public override StatePrediction PredictState()
    {
        return base.PredictState();
    }
    public override void SetState(Properties.StateEnum stateEnum)
    {
        if (properties.state == Properties.StateEnum.NORMAL && stateEnum >= Properties.StateEnum.AWAKENED && !hadAwakenedBefore)
        {
            CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
            cb.AddNodeNumOfBooks(cb.Me, GlobalVar.Instance.NumOfBibliophileGiveBooks);
            mb.AddMessage($"Bibliophile {gameObject.name} gives you {GlobalVar.Instance.NumOfBibliophileGiveBooks} books.");
        }
        base.SetState(stateEnum);
    }
}
