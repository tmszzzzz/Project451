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

            int org = properties.numOfBooks;
            cb.AddNodeNumOfBooks(this.gameObject, GlobalVar.instance.numOfFirefighterGiveBooks);
            int gained = properties.numOfBooks - org;
            if(gained != 0)mb.AddMessage($"消防员{gameObject.name}今日获得了{GlobalVar.instance.numOfFirefighterGiveBooks}本书.");
            else
            {
                mb.AddMessage($"消防员{gameObject.name}今日持有书达上限，未获取任何书.");
            }
        }
    }

    
}
