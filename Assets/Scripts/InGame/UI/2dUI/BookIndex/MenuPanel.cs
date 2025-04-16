using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using TMPro;

public class MenuPanel : MonoBehaviour
{
    public Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();
    public GameObject firstMenuPrefab;
    public GameObject secondElementPrefab;
    public GameObject contentPanel;

    private void Start()
    {
        GetExcel();
        InstantiateUI();
    }

    public void GetExcel()
    {
        string csvPath = Application.dataPath + "/Resources/Books/书表.csv";
        string[] lines = File.ReadAllLines(csvPath);
        Dictionary<string, List<string>> keyValuePairs = new();
        for (int i = 1; i < 142; i++)      // 第2行开始
        {
            var row = lines[i].Split(',');
            if (!keyValuePairs.ContainsKey(row[5]))     // 第5列
            {
                List<string> values = new();
                keyValuePairs.Add(row[5], values);
            }
            keyValuePairs[row[5]].Add(row[1]) ;
        }
        this.keyValuePairs = keyValuePairs;
    }

    public void InstantiateUI()
    {
        foreach (var item in this.keyValuePairs.Keys)
        {
            int i = 0;
            var firstMenu = Instantiate(firstMenuPrefab, contentPanel.transform);
            firstMenu.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = item;
            var secondMenu = firstMenu.transform.GetChild(1);
            secondMenu.gameObject.SetActive(false);

            foreach (var sonItem in keyValuePairs[item])
            {
                var secondElement = Instantiate(secondElementPrefab, secondMenu.transform);
                secondElement.GetComponent<SecondElement>().bookName = sonItem;
                secondElement.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = keyValuePairs[item][i++];
            }
        }
    }
}
