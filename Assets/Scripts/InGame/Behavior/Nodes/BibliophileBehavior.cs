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
            //^ 2025.4.3 已弃用的特性
            cb.AddNodeNumOfBooks(this.gameObject, GlobalVar.instance.numOfBibliophileGiveBooks);
            mb.AddMessage($"藏书家{NameManager.instance.ConvertNodeNameToName(gameObject.name)}携带{GlobalVar.instance.numOfBibliophileGiveBooks}本书加入.");
        }
        base.SetState(stateEnum);
    }
}
