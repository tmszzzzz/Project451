using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoundManager : MonoBehaviour
{
    // ����ʵ��
    public static RoundManager Instance { get; private set; }
    private Camera mainCamera;
    public GameObject textPrefab; // ָ��TextMeshProUIԤ����
    public MessageBar messageBar;
    public int roundNum = 1;
    public int allocated = 0;
    public int held = 0;
    public CanvasBehavior canvas;
    public Canvas uiCanvas; // �������UI Canvas
    public Dictionary<GameObject, int> bookAllocationMap; //<node,value>
    private Dictionary<GameObject, GameObject> activeTextMap = new Dictionary<GameObject, GameObject>();

    //�������¼�
    public event Action OnRoundChange;

    // ������ʼ��ʱ����
    private void Awake()
    {
        // �������ʵ���Ҳ��ǵ�ǰʵ�������ٵ�ǰʵ����ȷ������Ψһ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // ����ǰʵ����Ϊ����ʵ��
        Instance = this;

        // ѡ�����������ʹ���ڳ����л�ʱ���ᱻ����
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        mainCamera = Camera.main;
        bookAllocationMap = new Dictionary<GameObject, int>();
        foreach(var i in canvas.GetNodeList())
        {
            bookAllocationMap.Add(i, 0);
        }
    }
    private void Update()
    {
        // ���������
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Left Click");
            BookAllocation(0);
        }
        // ����Ҽ����
        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Right Click");
            BookAllocation(1);
        }
        BookTexts();
    }
    void BookAllocation(int mouseButton)
    {
        // �����λ�ô�������
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // ������߻���������
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                NodeBehavior nb = hit.collider.GetComponent<NodeBehavior>();
                if (nb != null && mouseButton == 0)
                {
                    if ((int)nb.properties.state >= 1 && nb.properties.numOfBooks + bookAllocationMap[hit.collider.gameObject] < nb.properties.maximumNumOfBooks && held > 0)
                    {
                        bookAllocationMap[hit.collider.gameObject]++;
                        held--;
                    }
                }
                else if (nb != null && mouseButton == 1)
                {
                    if ((int)nb.properties.state >= 1 && nb.properties.numOfBooks + bookAllocationMap[hit.collider.gameObject] > 0 && !(GetNeedToAllocate() >= GlobalVar.Instance.allocationLimit && bookAllocationMap[hit.collider.gameObject] <= 0))
                    {
                        bookAllocationMap[hit.collider.gameObject]--;
                        held++;
                    }
                }
            }
        }
    }
    public int GetNeedToAllocate()
    {
        int v = 0;
        var keys = new List<GameObject>(bookAllocationMap.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            if (bookAllocationMap[keys[i]] < 0) v -= bookAllocationMap[keys[i]];

        }
        return v;
    }
    
    public void NextRound()
    {
        if (held == 0)
        {
            OnRoundChange?.Invoke();
            var keys = new List<GameObject>(bookAllocationMap.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                canvas.AddNodeNumOfBooks(keys[i], bookAllocationMap[keys[i]]);
            }
            roundNum++;
            canvas.RefreshAllNodes();
            for (int i = 0; i < keys.Count; i++)
            {
                bookAllocationMap[keys[i]] = 0;
            }
            messageBar.AddMessage("NextRound");
        }else
        {
            messageBar.AddMessage("There are still books that have not been assigned.");
        }
    }
    public void LimitIncreaseBy(int i)
    {
        GlobalVar.Instance.allocationLimit += i;
    }
    public void BookNumOfMeIncreaseBy(int i)
    {
        canvas.Me.GetComponent<NodeBehavior>().properties.numOfBooks++;
    }

    private void BookTexts()
    {
        // ����bookAllocationMap��������
        foreach (var entry in bookAllocationMap)
        {
            GameObject node = entry.Key;
            int bookCount = entry.Value;

            if (bookCount != 0)
            {
                // �����Node���ı���δ���ɣ�������֮
                if (!activeTextMap.ContainsKey(node))
                {
                    // ��������ʾ�ı�
                    GameObject textObj = Instantiate(textPrefab,node.transform.position,Quaternion.LookRotation(node.transform.position - Camera.main.transform.position), node.transform);
                    TextMeshPro textComponent = textObj.GetComponent<TextMeshPro>();
                    if (textComponent != null)
                    {
                        textComponent.text = bookCount.ToString();
                    }

                    // �洢��ǰ�ı�����
                    activeTextMap[node] = textObj;
                }
                else
                {
                    // ����Ѿ����ı��������ı�����
                    TextMeshPro textComponent = activeTextMap[node].GetComponent<TextMeshPro>();
                    if (textComponent != null)
                    {
                        textComponent.text = bookCount.ToString();
                    }
                }
            }
            else
            {
                // �����ǰ��bookCountΪ0���ı�������ʾ���������ı�
                if (activeTextMap.ContainsKey(node))
                {
                    Destroy(activeTextMap[node]);
                    activeTextMap.Remove(node);
                }
            }
        }
    }
}
