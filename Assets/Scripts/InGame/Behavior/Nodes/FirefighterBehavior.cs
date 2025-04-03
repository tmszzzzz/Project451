using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FirefighterBehavior : NodeBehavior
{
    protected override void Start()
    {
        RoundManager.instance.RoundChange += OnRoundChange;
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

            cb.AddNodeNumOfBooks(this.gameObject, GlobalVar.instance.numOfFirefighterGiveBooks);
            mb.AddMessage($"消防员{NameManager.instance.ConvertNodeNameToName(gameObject.name)}今日获得了{GlobalVar.instance.numOfFirefighterGiveBooks}本书.");
        }
    }

    
}
