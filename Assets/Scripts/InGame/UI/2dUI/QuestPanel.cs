using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
    public List<GameObject> questsList = new List<GameObject>();
    public List<int> questIds = new List<int>();
    
    [SerializeField] private GameObject quest0Prefab;
    [SerializeField] private GameObject questOfficePrefab;
    [SerializeField] private GameObject questPoliceStationPrefab;
    [SerializeField] private GameObject questFireHousePrefab;
    [SerializeField] private GameObject questDealPrefab;

    [SerializeField] private float heightBetweenQuests = 20;
    [SerializeField] private float startHeight = 0f;
    [SerializeField] private float fixPositionX = 0f;

    [SerializeField] private RectTransform initTransform;

    public static QuestPanel instance;

    
    private void Awake()
    {
        // 如果已有实例且不是当前实例，销毁当前实例，确保单例唯一性
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 将当前实例设为单例实例
        instance = this;
    }

    public void AddQuest(string questType)
    {
        GameObject newQuest = null;
        int i = 0;
        switch (questType)
        {
            case "Zero":
                newQuest = Instantiate(quest0Prefab, initTransform);
                i = 0;
                break;
            case "Office":
                newQuest = Instantiate(questOfficePrefab, initTransform);
                i = 1;
                break;
            case "Police":
                newQuest = Instantiate(questPoliceStationPrefab, initTransform);
                i = 2;
                break;
            case "FireHouse":
                newQuest = Instantiate(questFireHousePrefab, initTransform);
                i = 3;
                break;
            case "Deal":
                newQuest = Instantiate(questDealPrefab, initTransform);
                i = 4;
                break;
            default:
                newQuest = Instantiate(quest0Prefab, initTransform);
                i = 0;
                break;
        }
        questsList.Add(newQuest);
        questIds.Add(i);
    }

    void RemoveAQuest(GameObject quest)
    {
        questsList.Remove(quest);
        Destroy(quest.gameObject);
    }
    
    private IEnumerator DelayedAction(GameObject quest, float delay)
    {
        questsList.Remove(quest);
        Destroy(quest);
        yield return new WaitForSeconds(delay);
    }
    
    private void Update()
    {
        for (int i = 0; i < questsList.Count; i++)
        {
            GameObject quest = questsList[i];
            float targetY = startHeight + i * heightBetweenQuests;
            Vector2 targetPosition = new Vector2(fixPositionX, -targetY);
            quest.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(quest.GetComponent<RectTransform>().anchoredPosition, targetPosition, Time.deltaTime * 5);
            
            if (quest.GetComponent<QuestUnit>().isQuestCompleted)
            {
                StartCoroutine(DelayedAction(quest, 5f));
            }
        }
    }
}
