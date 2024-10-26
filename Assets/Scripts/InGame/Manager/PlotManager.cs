using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotManager : MonoBehaviour
{

    public static PlotManager Instance;

    public delegate void UserActionEventHandler(bool forSelect,string selectedFlag);
    public event UserActionEventHandler UserAction;
    public event Action PlotStart;
    int next;
    Dictionary<int, string> nameMap;
    Dictionary<string, int> flagMap;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        UserAction += NextStep;
        PlotStart += OnStart;
        flagMap = new Dictionary<string, int>();
        nameMap = new Dictionary<int, string>();
    }

    public void StartPlot(string filePath)
    {
        next = 0;
        flagMap.Clear();
        nameMap.Clear();
        string[] lines = System.IO.File.ReadAllLines(filePath);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("define "))
            {
                string[] parts = line.Split(new[] { ' ' }, 3);
                if (parts.Length == 3 && int.TryParse(parts[1], out int id))
                {
                    string name = parts[2].Trim('\'');
                    nameMap[id] = name;
                }
            }
            //构建nameMap用于名称代号
            if (line.StartsWith("flag "))
            {
                string flagName = line.Substring(5).Trim('\'');
                flagMap[flagName] = i;
            }
            //构建flagMap用于跳转行号
        }
        PushStart();
    }

    private void OnStart()
    {
        NextStep(false," ");
    }

    private void NextStep(bool forSelect, string selectedFlag)
    {
        if(forSelect)
        {
            next = GetFlagLineNumber(selectedFlag) + 1;
        }

        //TODO
        //这里解析next对应行
        //接下来的行动可能是继续读取行，或者调用下面的三个Push组件方法

        next++;
    }

    private void PushStart()
    {
        //TODO
        //这里向ui发去初始化剧情的信息，ui初始化完成后应当触发此类内事件PlotStart
    }


    private void PushDialog(string name,string content)
    {
        //TODO
        //这里向ui推送一个对侧对话的信息，ui检测到用户进行“继续”动作后应当触发此类内事件UserAction
        //置其首参数为false表示当前不需要响应select
    }

    private void PushSelfDialog(string name, string content)
    {
        //TODO
        //这里向ui推送一个self侧对话的信息，ui检测到用户进行“继续”动作后应当触发此类内事件UserAction
        //置其首参数为false表示当前不需要响应select
    }

    private void PushSelect(int num, List<string> selectItems)
    {
        //TODO
        //这里向ui推送一个选择，ui检测到用户进行“选择”动作后应当触发此类内事件UserAction
        //置其首参数为true表示当前需要响应select，第二个参数填入响应时跳转到的flag
    }

    private int GetFlagLineNumber(string flagName)
    {
        if (flagMap.TryGetValue(flagName, out int lineNumber))
        {
            return lineNumber;
        }
        return -1;
    }

}
