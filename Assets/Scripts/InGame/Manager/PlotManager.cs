using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlotManager : MonoBehaviour
{

    public static PlotManager Instance;

    public delegate void UserActionEventHandler(bool forSelect,int selectIndex);
    public event UserActionEventHandler UserAction;
    public void TriggerUserAction(bool forSelect,int selectIndex)
    {
        UserAction?.Invoke(forSelect,selectIndex);
    }

    public event Action PlotStart;
    public void TriggerPlotStart()
    {
        PlotStart?.Invoke();
    }
    int next;
    Dictionary<int, string> nameMap;
    Dictionary<string, int> flagMap;
    List<string> choices;
    List<string> selectionFlags;
    string[] lines;
    bool duringPlot;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        UserAction += OnNextStep;
        PlotStart += OnStart;
        flagMap = new Dictionary<string, int>();
        nameMap = new Dictionary<int, string>();
        choices = new List<string>();
        selectionFlags = new List<string>();
        duringPlot = false;
    }

    public void StartPlot(string filePath)
    {
        if (duringPlot)
        {
            Debug.LogWarning("不可重复触发剧情。");
            return;
        }
        duringPlot = true;
        next = 0;
        flagMap.Clear();
        nameMap.Clear();
        choices.Clear();
        selectionFlags.Clear();
        lines = System.IO.File.ReadAllLines(filePath);

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
                string flagName = line.Substring(5).Trim();
                flagMap[flagName] = i;
            }
            //构建flagMap用于跳转行号
        }
        PushStart();
    }

    private void OnStart()
    {
        OnNextStep(false,0);
    }
    private void OnEnd()
    {
        duringPlot = false;
    }

    private void OnNextStep(bool forSelect, int selectIndex)
    {
        if(forSelect)
        {
            next = GetFlagLineNumber(selectionFlags[selectIndex]) + 1;
            choices.Clear();
            selectionFlags.Clear();
        }

        //这里解析next对应行
        while(true)
        {
            if(next >= lines.Length)
            {
                PushEnd();
                break;
            }
            // 获取当前行并去除空格
            string line = lines[next].Trim();

            if (line.StartsWith("say "))
            {

                StringBuilder commandBuilder = new StringBuilder(line);

                while (commandBuilder.ToString().EndsWith("\\"))
                {
                    commandBuilder.Length--;
                    next++;
                    commandBuilder.Append(lines[next].Trim());
                }

                string fullCommand = commandBuilder.ToString();

                int firstSpaceIndex = fullCommand.IndexOf(' ', 4);
                if (firstSpaceIndex != -1 && int.TryParse(fullCommand.Substring(4, firstSpaceIndex - 4), out int characterId))
                {
                    string dialogContent = fullCommand.Substring(firstSpaceIndex + 1).Trim('\'');
                    dialogContent = dialogContent.Replace("\\n", "\n");

                    // 推送对话
                    PushDialog(nameMap[characterId], dialogContent);
                    break; // 中断循环，等待用户操作后再调用 NextStep
                }
            }
            else if (line.StartsWith("Isay "))
            {

                StringBuilder commandBuilder = new StringBuilder(line);

                while (commandBuilder.ToString().EndsWith("\\"))
                {
                    commandBuilder.Length--;
                    next++;
                    commandBuilder.Append(lines[next].Trim());
                }

                string fullCommand = commandBuilder.ToString();

                int firstSpaceIndex = fullCommand.IndexOf(' ', 5);
                if (firstSpaceIndex != -1 && int.TryParse(fullCommand.Substring(5, firstSpaceIndex - 5), out int characterId))
                {
                    string dialogContent = fullCommand.Substring(firstSpaceIndex + 1).Trim('\'');
                    dialogContent = dialogContent.Replace("\\n", "\n");

                    // 推送对话
                    PushSelfDialog(nameMap[characterId], dialogContent);
                    break; // 中断循环，等待用户操作后再调用 NextStep
                }
            }
            else if (line.StartsWith("narration "))
            {

                StringBuilder commandBuilder = new StringBuilder(line);

                while (commandBuilder.ToString().EndsWith("\\"))
                {
                    commandBuilder.Length--;
                    next++;
                    commandBuilder.Append(lines[next].Trim());
                }

                string fullCommand = commandBuilder.ToString();

                string dialogContent = fullCommand.Substring(10).Trim().Trim('\'');
                dialogContent = dialogContent.Replace("\\n", "\n");

                PushNarration(dialogContent);
                break;

            }
            else if (line.StartsWith("selectItem "))
            {
                StringBuilder choiceStr = new StringBuilder();

                string s = line.Substring(11).Trim().Substring(1);
                int p = 0;
                while (s[p] != '\'')
                {
                    choiceStr.Append(s[p++]);
                }
                string choiceFlag = s.Substring(p + 1).Trim();
                choices.Add(choiceStr.ToString());
                selectionFlags.Add(choiceFlag);
            }
            else if (line.StartsWith("select"))
            {
                PushSelection(choices);
                break;
            }
            else if (line.StartsWith("goto "))
            {
                string flag = line.Substring(5).Trim();
                int i = flagMap[flag];
                next = i;
            }
            else if (line.StartsWith("exit"))
            {
                PushEnd();
                break;
            }
            next++;
        }

        next++;
    }

    private void PushStart()
    {
        //TODO
        //这里向ui发去初始化剧情的信息，ui初始化完成后应当触发此类内事件PlotStart
        Debug.Log("开始一段新剧情");
    }

    private void PushEnd()
    {
        //TODO
        //这里向ui发去结束剧情的信息，ui初始化完成后应当触发此类内事件PlotEnd
        Debug.Log("END.");
    }


    private void PushDialog(string name,string content)
    {
        //TODO
        //这里向ui推送一个对侧对话的信息，ui检测到用户进行“继续”动作后应当触发此类内事件UserAction
        //置其首参数为false表示当前不需要响应select
        Debug.Log($"{name} : \n    {content}");
    }

    private void PushSelfDialog(string name, string content)
    {
        //TODO
        //这里向ui推送一个self侧对话的信息，ui检测到用户进行“继续”动作后应当触发此类内事件UserAction
        //置其首参数为false表示当前不需要响应select
        Debug.Log($"{name} (Me) : \n    {content}");
    }

    private void PushNarration(string content)
    {
        //TODO
        //这里向ui推送一个self侧对话的信息，ui检测到用户进行“继续”动作后应当触发此类内事件UserAction
        //置其首参数为false表示当前不需要响应select
        Debug.Log($"旁白: \n    {content}");
    }

    private void PushSelection(List<string> selectItems)
    {
        //TODO
        //这里向ui推送一个选择，ui检测到用户进行“选择”动作后应当触发此类内事件UserAction
        //置其首参数为true表示当前需要响应select，第二个参数填入响应时跳转到的flag
        StringBuilder s = new StringBuilder("请做出选择：\n");
        for(int i=0;i<selectItems.Count;i++)
        {
            s.Append($"{i + 1}. {selectItems[i]}\n");
        }
        Debug.Log(s.ToString());
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
