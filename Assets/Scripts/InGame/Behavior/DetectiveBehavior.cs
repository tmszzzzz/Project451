using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DetectiveBehavior : MonoBehaviour
{
    [SerializeField] List<GameObject> focusOnNodes;
    [SerializeField] List<GameObject> focusPointers;
    public GameObject pointerPrefab;
    public CanvasBehavior canvas;
    [SerializeField] private List<int> stayRounds;
    public TaskCompletionSource<bool> Tcs1;
    public TaskCompletionSource<bool> Tcs2;
    private void Start()
    {
        focusOnNodes = new List<GameObject>();
        focusPointers = new List<GameObject>();
        stayRounds = new List<int>();

        List<GameObject> nodeList = canvas.GetNodeList();
        for (int i = 0; i < Mathf.Min(GlobalVar.instance.numOfDetectiveOnStart,nodeList.Count);i++) { 
            
            int j = Random.Range(0, nodeList.Count);
            focusOnNodes.Add(nodeList[j]);
            focusPointers.Add(Instantiate(pointerPrefab, focusOnNodes[focusOnNodes.Count - 1].transform.position, Quaternion.Euler(0, 0, 0)));
            stayRounds.Add(0);
        }
        canvas.RefreshAllConnections();
    }

    public void AddADetective()
    {
        List<GameObject> exposedList = canvas.ExposedList();
        if (exposedList.Count > 0)
        {
            focusOnNodes.Add(exposedList[Random.Range(0, exposedList.Count)]);
            focusPointers.Add(Instantiate(pointerPrefab, focusOnNodes[focusOnNodes.Count - 1].transform.position,Quaternion.Euler(0,0,0)));
            stayRounds.Add(0);
        }
        else
        {
            Debug.Log("There is no EXPOSED node in canvas. A detective will firstly appear in an EXPOSED node only.");
        }
    }
    public void AddDetectivesInRegion(int num, int region) 
    {
        List<GameObject> regionList = canvas.GetRegionNodes(region);
        var toRemove = new List<GameObject>();
        foreach (GameObject regionNode in regionList)
        {
            if (focusOnNodes.Contains(regionNode))
            {
                toRemove.Add(regionNode);
            }
        }
        foreach (GameObject regionNode in toRemove)
        {
            regionList.Remove(regionNode);
        }
        var targets = new List<GameObject>();
        for (int i = 0; i < Mathf.Min(num,regionList.Count); i++)
        {
            var tar = regionList[Random.Range(0, regionList.Count)];
            targets.Add(tar);
            regionList.Remove(tar);
        }
        foreach (var target in targets)
        {
            focusOnNodes.Add(target);
            focusPointers.Add(Instantiate(pointerPrefab, target.transform.position,Quaternion.Euler(0,0,0)));
            stayRounds.Add(0);
        }
    }

    public void a()
    {
        AddDetectivesInRegion(4,2);
    }
    
    public void CheckForDuplicates()
    {
        Dictionary<GameObject, int> nodeIndexMap = new Dictionary<GameObject, int>();
        List<int> duplicateIndices = new List<int>();

        for (int i = 0; i < focusOnNodes.Count; i++)
        {
            GameObject node = focusOnNodes[i];
            
            if (nodeIndexMap.ContainsKey(node))
            {
                // 记录第一个重复元素的下标
                if (!duplicateIndices.Contains(nodeIndexMap[node]))
                {
                    duplicateIndices.Add(nodeIndexMap[node]);
                }
                // 记录当前重复元素的下标
                duplicateIndices.Add(i);
            }
            else
            {
                nodeIndexMap[node] = i;
            }
        }

        if (duplicateIndices.Count > 0)
        {
            Debug.Log("Duplicate indices found:");
            foreach (int index in duplicateIndices)
            {
                Debug.Log($"Duplicate at index: {index}");
            }
        }
        else
        {
            Debug.Log("No duplicates found.");
        }
    }

    public void DetectiveMove()
    {
        for(int i=0;i<focusOnNodes.Count;i++)
        {
            var list = canvas.GetNeighbors(focusOnNodes[i]);
            var toRemove = new List<GameObject>();
            foreach (GameObject neighbor in list)
            {
                if (focusOnNodes.Contains(neighbor))
                {
                    toRemove.Add(neighbor);
                }
            }

            foreach (GameObject neighbor in toRemove)
            {
                list.Remove(neighbor);
            }
            var exposedNeighbors = new List<GameObject>();
            foreach(var j in list)
            {
                if (j.GetComponent<NodeBehavior>().properties.state == Properties.StateEnum.EXPOSED) exposedNeighbors.Add(j);
            }

            if (list.Count <= 0)
            {
                stayRounds[i]++;
            }else if(focusOnNodes[i].GetComponent<NodeBehavior>().properties.state != Properties.StateEnum.EXPOSED) //self 0
            {
                if (exposedNeighbors.Count == 0) //neighbor 0
                {
                    //Debug.Log(i + " 00");
                    stayRounds[i] = 0;
                    focusOnNodes[i] = list[Random.Range(0, list.Count)];
                    
                }
                else //neighbor 1
                {
                    //Debug.Log(i + " 01");
                    stayRounds[i] = 0;
                    focusOnNodes[i] = exposedNeighbors[Random.Range(0, exposedNeighbors.Count)];
                }
            }else //self 1
            {
                if (exposedNeighbors.Count == 0) //neighbor 0
                {
                    //Debug.Log(i + " 10");
                    stayRounds[i]++;
                }
                else //neighbor 1
                {
                    //Debug.Log(i + " 11");
                    stayRounds[i] = 0;
                    focusOnNodes[i] = exposedNeighbors[Random.Range(0, exposedNeighbors.Count)];
                }
            }
            focusPointers[i].transform.position = focusOnNodes[i].transform.position;
            //focusPointers[i].transform.DOMove(new(0, 0, 0), 1, false);
        } 
    }

    public async Task AddGlobalExposureValue()
    {
        await DetectedVis();
        await AllVis();
        await AllInvis();
        foreach(var i in focusOnNodes)
        {
            if (RoundManager.instance.BookAllocationMap[i] != 0)
            {
                GlobalVar.instance.AddGlobalExposureValue(GlobalVar.instance.exposureValueAdditionOfDetective);
            }
        }
    }

    public bool IsDetected(GameObject go)
    {
        return focusOnNodes.Contains(go);
    }

    public async Task DetectedVis()
    {
        Tcs1 = new TaskCompletionSource<bool>();
        bool skip = true;
        int l = focusPointers.Count;
        for(int i =0;i<l;i++)
        {
            if (RoundManager.instance.BookAllocationMap[focusOnNodes[i]] != 0)
            {
                skip = false;
                focusPointers[i].GetComponent<Animator>().SetTrigger("Detected");
            }
        }

        if(!skip) await Tcs1.Task;
    }

    public async Task AllVis()
    {
        int l = focusPointers.Count;
        for(int i =0;i<l;i++)
        {
            if (RoundManager.instance.BookAllocationMap[focusOnNodes[i]] == 0)
            {
                focusPointers[i].GetComponent<Animator>().SetTrigger("Vis");
            }
        }

        await Task.Delay(4000);
    }

    public async Task AllInvis()
    {
        Tcs2 = new TaskCompletionSource<bool>();
        int l = focusPointers.Count;
        bool skip = true;
        for(int i =0;i<l;i++)
        {
            skip = false;
            focusPointers[i].GetComponent<Animator>().SetTrigger("Invis");
        }
        
        if(!skip) await Tcs2.Task;
    }
}
