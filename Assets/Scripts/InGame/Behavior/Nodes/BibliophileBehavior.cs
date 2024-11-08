using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BibliophileBehavior : NodeBehavior
{
    public override void SetState(Properties.StateEnum stateEnum)
    {
        if (properties.state == Properties.StateEnum.NORMAL && stateEnum >= Properties.StateEnum.AWAKENED && !hadAwakenedBefore)
        {
            CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();

            //由于藏书家给书的书目会增多，这里保证增加的书不超过本点的上限
            cb.AddNodeNumOfBooks(this.gameObject, math.min(GlobalVar.instance.numOfBibliophileGiveBooks, properties.maximumNumOfBooks));
            mb.AddMessage($"Bibliophile {gameObject.name} gives you {GlobalVar.instance.numOfBibliophileGiveBooks} books.");
        }
        base.SetState(stateEnum);
    }
}
