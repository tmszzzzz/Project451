using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
    public List<GameObject> questsList = new List<GameObject>();
    
    [SerializeField] private GameObject quest0Prefab;
    [SerializeField] private GameObject questOfficePrefab;
    [SerializeField] private GameObject questPoliceStationPrefab;
    [SerializeField] private GameObject questFireHousePrefab;
    [SerializeField] private GameObject questDealPrefab;

    [SerializeField] private float heightBetweenQuests = 20;
    [SerializeField] private float startHeight = 0f;
    [SerializeField] private float fixPositionX = 0f;

    [SerializeField] private RectTransform initTransform;
    
    public void AddQuest(string questType)
    {
        GameObject newQuest = null;
        switch (questType)
        {
            case "Zero":
                newQuest = Instantiate(quest0Prefab, initTransform);
                break;
            case "Office":
                newQuest = Instantiate(questOfficePrefab, initTransform);
                break;
            case "PoliceStation":
                newQuest = Instantiate(questPoliceStationPrefab, initTransform);
                break;
            case "FireHouse":
                newQuest = Instantiate(questFireHousePrefab, initTransform);
                break;
            case "Deal":
                newQuest = Instantiate(questDealPrefab, initTransform);
                break;
            default:
                newQuest = Instantiate(quest0Prefab, initTransform);
                break;
        }
        questsList.Add(newQuest);
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
