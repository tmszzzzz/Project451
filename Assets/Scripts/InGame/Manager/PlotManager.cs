using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class PlotManager : MonoBehaviour
{

    public static PlotManager instance;

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
    public event Action PlotEnd;
    public void TriggerPlotEnd()
    {
        PlotEnd?.Invoke();
    }
    int next;
    [SerializeField] private PlotDisplay plotDisplay;
    Dictionary<int, string> nameMap;
    Dictionary<string, int> flagMap;
    List<string> choices;
    List<string> selectionFlags;
    string[] lines;
    bool duringPlot;
    public bool GetDuringPlot() { return duringPlot; }
    public class PlotQueueItem
    {
        public string PlotFile;
        public GameObject FocusNode;

        public PlotQueueItem(string plotFile, GameObject focusNode)
        {
            PlotFile = plotFile;
            FocusNode = focusNode;
        }
    }

    public Queue<PlotQueueItem> PlotQueue;
    [SerializeField] private CameraBehavior camB;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        UserAction += OnNextStep;
        PlotStart += OnStart;
        PlotEnd += OnEnd;
        flagMap = new Dictionary<string, int>();
        nameMap = new Dictionary<int, string>();
        choices = new List<string>();
        selectionFlags = new List<string>();
        duringPlot = false;
        PlotQueue = new Queue<PlotQueueItem>();
    }

    public bool isNextLineEnd()
    {
        return next >= lines.Length;
    }

    public bool isNextLineSelfDialog()
    {
        return lines[next].Trim().StartsWith("Isay");
    }

    public string getNextLineText()
    {
        return lines[next].Trim();
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
        RoundManager.instance.OperationForbidden();
        OnNextStep(false,0);
    }
    private void OnEnd()
    {
        RoundManager.instance.OperationRelease();
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
            else if (line.StartsWith("func "))
            {
                string func = line.Substring(5).Trim();
                var method = PlotFuncManager.instance.GetType().GetMethod(func);
                method?.Invoke(PlotFuncManager.instance,null);
            }
            next++;
        }

        next++;
    }

    private void PushStart()
    {
        //TODO
        //这里向ui发去初始化剧情的信息，ui初始化完成后应当触发此类内事件PlotStart
        plotDisplay.PushedStart();
        Debug.Log("开始一段新剧情");
    }

    private void PushEnd()
    {
        //TODO
        //这里向ui发去结束剧情的信息，ui初始化完成后应当触发此类内事件PlotEnd
        plotDisplay.PushedEnd();
        Debug.Log("END.");
    }


    private void PushDialog(string name,string content)
    {
        //TODO
        //这里向ui推送一个对侧对话的信息，ui检测到用户进行“继续”动作后应当触发此类内事件UserAction
        //置其首参数为false表示当前不需要响应select
        plotDisplay.ShowDialog(name, content);
        Debug.Log($"{name} : \n    {content}");
    }

    private void PushSelfDialog(string name, string content)
    {
        //TODO
        //这里向ui推送一个self侧对话的信息，ui检测到用户进行“继续”动作后应当触发此类内事件UserAction
        //置其首参数为false表示当前不需要响应select
        plotDisplay.ShowSelfDialog(name, content);
        Debug.Log($"{name} (Me) : \n    {content}");
    }

    private void PushNarration(string content)
    {
        //TODO
        //这里向ui推送一个self侧对话的信息，ui检测到用户进行“继续”动作后应当触发此类内事件UserAction
        //置其首参数为false表示当前不需要响应select
        plotDisplay.ShowNarration(content);
        Debug.Log($"旁白: \n    {content}");
    }

    private void PushSelection(List<string> selectItems)
    {
        //TODO
        //这里向ui推送一个选择，ui检测到用户进行“选择”动作后应当触发此类内事件UserAction
        //置其首参数为true表示当前需要响应select，第二个参数填入响应时跳转到的flag
        plotDisplay.ShowSelection(selectItems);
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

    public async Task ReadPlotQueue()
    {
        while (PlotQueue.Count > 0)
        {
            var item = PlotQueue.Dequeue();
            if (item.FocusNode != null)
            {
                await camB.PlotFocusEnter(item.FocusNode);
            }
            StartPlot(item.PlotFile);
            var tcs = new TaskCompletionSource<bool>();

            // 订阅事件，并在事件触发时设置 TaskCompletionSource 完成
            Action handler = null;
            handler = () =>
            {
                // 设置完成，并取消订阅事件
                tcs.SetResult(true);
                PlotEnd -= handler;
            };

            PlotEnd += handler;
            await tcs.Task;
            if (item.FocusNode != null)
            {
                await camB.FocusExit();
            }

            await Task.Delay(1000);
            if (RoundManager.instance.switching1)
            {
                RoundManager.instance.mainCamera.GetComponent<AudioSource>().PlayOneShot(RoundManager.instance.s1,0.7f);
                RoundManager.instance.switchingPanel1.SetActive(true);
                await RoundManager.instance.mainCamera.SwitchTo1Enter();
                
                await Task.Delay(5000);
                await RoundManager.instance.mainCamera.FocusExit();
                BackgroundMusicManager.instance.switchTo(0);
            }
            if (RoundManager.instance.switching2)
            {
                RoundManager.instance.mainCamera.GetComponent<AudioSource>().PlayOneShot(RoundManager.instance.s2,0.7f);
                RoundManager.instance.switchingPanel2.SetActive(true);
                await RoundManager.instance.mainCamera.SwitchTo2Enter();
                
                await Task.Delay(5000);
                await RoundManager.instance.mainCamera.FocusExit();
                BackgroundMusicManager.instance.switchTo(1);
            }
            if (RoundManager.instance.switching3)
            {
                RoundManager.instance.mainCamera.GetComponent<AudioSource>().PlayOneShot(RoundManager.instance.s3,0.7f);
                RoundManager.instance.switchingPanel3.SetActive(true);
                await RoundManager.instance.mainCamera.SwitchTo3Enter();
                
                await Task.Delay(5000);
                await RoundManager.instance.mainCamera.FocusExit();
                BackgroundMusicManager.instance.switchTo(2);
            }
        }
    }

    public void AddPlotQueue(string filename, GameObject target)
    {
        var item = new PlotQueueItem(filename, target);
        PlotQueue.Enqueue(item);
    }

    public void Apq(string f){
        AddPlotQueue(f,RoundManager.instance.canvas.GetNodeList()[31]);
    }
}
