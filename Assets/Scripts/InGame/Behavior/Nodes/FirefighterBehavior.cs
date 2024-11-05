using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FirefighterBehavior : NodeBehavior
{
    protected override void Start()
    {
        RoundManager.Instance.RoundChange += OnRoundChange;
        base.Start();
    }

    protected void OnRoundChange()
    {
        if (properties.state >= Properties.StateEnum.AWAKENED)
        {
            CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
            if (cb == null)
            {
                Debug.LogWarning("Script \"CanvasBehavior\" not found in canvas.");
                return;
            }
            cb.AddNodeNumOfBooks(cb.Me, GlobalVar.instance.numOfFirefighterGiveBooks);
            mb.AddMessage($"Firefighter {gameObject.name} gives you {GlobalVar.instance.numOfFirefighterGiveBooks} books in this round.");
        }
    }

    
}
