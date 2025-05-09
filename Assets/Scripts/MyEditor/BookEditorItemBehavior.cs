using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BookEditorItemBehavior : MonoBehaviour
{
    private bool _tag;
    public bool Tag
    {
        set
        {
            gameObject.GetComponent<UnityEngine.UI.Image>().color = !value ? Color.white : Color.gray;
            _tag = value;
        }
        get
        {
            return _tag;
        }
    }
    

    private void Start()
    {
        Tag = false;
    }

    public void Select()
    {
        NodeInfoEditorDisplay.instance.Select(gameObject);
    }
}
