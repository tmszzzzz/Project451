using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.UI;

public class QuestUnit : MonoBehaviour
{
    public string questDescription = "测试描述";
    public string questTitle = "测试标题";
    public bool isQuestCompleted = false;
    
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;

    public virtual bool CheckIfQuestIsFinished()
    {
        isQuestCompleted = false;
        return false;
    }

    public virtual string UpdateDescription()
    {
        return questDescription;
    }
    
    // Update is called once per frame
    public void Update()
    {
        questDescriptionText.text = UpdateDescription();
        questTitleText.text = questTitle;
        
        if (CheckIfQuestIsFinished())
        {
            isQuestCompleted = true;
        }
    }
}
