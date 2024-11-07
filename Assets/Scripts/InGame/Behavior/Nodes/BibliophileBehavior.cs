using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BibliophileBehavior : NodeBehavior
{
    public override void SetState(Properties.StateEnum stateEnum)
    {
        if (properties.state == Properties.StateEnum.NORMAL && stateEnum >= Properties.StateEnum.AWAKENED && !hadAwakenedBefore)
        {
            CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
            cb.AddNodeNumOfBooks(this.gameObject, GlobalVar.instance.numOfBibliophileGiveBooks);
            mb.AddMessage($"Bibliophile {gameObject.name} gives you {GlobalVar.instance.numOfBibliophileGiveBooks} books.");
        }
        base.SetState(stateEnum);
    }
}
