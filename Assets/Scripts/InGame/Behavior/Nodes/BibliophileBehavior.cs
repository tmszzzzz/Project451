using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BibliophileBehavior : NodeBehavior
{
    public override StatePrediction RefreshState()
    {
        StatePrediction statePrediction = base.RefreshState();
        if (properties.state == Properties.StateEnum.NORMAL && statePrediction.state >= Properties.StateEnum.AWAKENED)
        {
            CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
            if (cb == null)
            {
                return new StatePrediction(Properties.StateEnum.DEAD,0);
            }
            cb.AddNodeNumOfBooks(cb.Me, GlobalVar.Instance.NumOfBibliophileGiveBooks);
            mb.AddMessage($"Bibliophile {gameObject.name} gives you {GlobalVar.Instance.NumOfBibliophileGiveBooks} books.");
        }
        return statePrediction;
    }
}
