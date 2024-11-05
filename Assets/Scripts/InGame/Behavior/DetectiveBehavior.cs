using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectiveBehavior : MonoBehaviour
{
    [SerializeField] List<GameObject> focusOnNodes;
    [SerializeField] List<GameObject> focusPointers;
    public GameObject pointerPrefab;
    public CanvasBehavior canvas;
    [SerializeField] private List<int> stayRounds;
    private void Start()
    {
        focusOnNodes = new List<GameObject>();
        focusPointers = new List<GameObject>();
        stayRounds = new List<int>();

        List<GameObject> nodeList = canvas.GetNodeList();
        for (int i = 0; i < Mathf.Min(GlobalVar.instance.numOfDetectiveOnStart,nodeList.Count);i++) { 
            
            int j = Random.Range(0, nodeList.Count);
            focusOnNodes.Add(nodeList[j]);
            focusPointers.Add(Instantiate(pointerPrefab, focusOnNodes[focusOnNodes.Count - 1].transform.position + new Vector3(0, 10, 0), Quaternion.Euler(0, 0, 0)));
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
            focusPointers.Add(Instantiate(pointerPrefab, focusOnNodes[focusOnNodes.Count - 1].transform.position + new Vector3(0,10,0),Quaternion.Euler(0,0,0)));
            stayRounds.Add(0);
        }
        else
        {
            Debug.Log("There is no EXPOSED node in canvas. A detective will firstly appear in an EXPOSED node only.");
        }
    }

    public void DetectiveMove()
    {
        for(int i=0;i<focusOnNodes.Count;i++)
        {
            var list = canvas.GetNeighbors(focusOnNodes[i]);
            var exposedNeighbors = new List<GameObject>();
            foreach(var j in list)
            {
                if (j.GetComponent<NodeBehavior>().properties.state == Properties.StateEnum.EXPOSED) exposedNeighbors.Add(j);
            }
            if(focusOnNodes[i].GetComponent<NodeBehavior>().properties.state != Properties.StateEnum.EXPOSED) //self 0
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
            focusPointers[i].transform.position = focusOnNodes[i].transform.position + new Vector3(0, 10, 0);
            //focusPointers[i].transform.DOMove(new(0, 0, 0), 1, false);
        } 
    }

    public void AddGlobalExposureValue()
    {
        foreach(var i in focusOnNodes)
        {
            if (RoundManager.Instance.BookAllocationMap[i] != 0)
            {
                GlobalVar.instance.AddGlobalExposureValue(GlobalVar.instance.exposureValueAdditionOfDetective);
            }
        }
    }

    public bool IsDetected(GameObject go)
    {
        return focusOnNodes.Contains(go);
    }
}
