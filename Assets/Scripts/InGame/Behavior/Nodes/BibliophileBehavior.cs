using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BibliophileBehavior : NodeBehavior
{
    public override Properties.StateEnum RefreshState()
    {
        Properties.StateEnum state = base.RefreshState();
        if (properties.state == Properties.StateEnum.NORMAL && state >= Properties.StateEnum.AWAKENED)
        {
            CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
            if (cb == null)
            {
                return Properties.StateEnum.DEAD;
            }
            cb.AddNodeNumOfBooks(cb.Me, GlobalVar.Instance.NumOfBibliophileGiveBooks);
            mb.AddMessage($"Bibliophile {gameObject.name} gives you {GlobalVar.Instance.NumOfBibliophileGiveBooks} books.");
        }
        return state;
    }
}
