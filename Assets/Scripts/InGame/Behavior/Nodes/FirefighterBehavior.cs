using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FirefighterBehavior : NodeBehavior
{
    protected override void Start()
    {
        base.Start();
        RoundManager.Instance.OnRoundChange += OnRoundChange;
    }
    public override Properties.StateEnum RefreshState()
    {
        return base.RefreshState();
    }

    private void OnRoundChange()
    {
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

    private void OnDestroy()
    {
        RoundManager.Instance.OnRoundChange -= OnRoundChange;
    }
}