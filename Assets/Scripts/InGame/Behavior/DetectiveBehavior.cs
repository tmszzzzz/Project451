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
                    stayRounds[i] = 0;
                    focusOnNodes[i] = list[Random.Range(0, list.Count)];
                    focusPointers[i].transform.position = focusOnNodes[i].transform.position + new Vector3(0, 10, 0);
                }
                else //neighbor 1
                {
                    stayRounds[i] = 0;
                    focusOnNodes[i] = exposedNeighbors[Random.Range(0, exposedNeighbors.Count)];
                    focusPointers[i].transform.position = focusOnNodes[i].transform.position + new Vector3(0, 10, 0);
                }
            }else //self 1
            {
                if (exposedNeighbors.Count == 0) //neighbor 0
                {
                    stayRounds[i]++;
                    return;
                }
                else //neighbor 1
                {
                    stayRounds[i] = 0;
                    focusOnNodes[i] = exposedNeighbors[Random.Range(0, exposedNeighbors.Count)];
                    focusPointers[i].transform.position = focusOnNodes[i].transform.position + new Vector3(0, 3, 0);
                }
            }
        } 
    }

    public void AddGlobalExposureValue()
    {
        foreach(var i in focusOnNodes)
        {
            if(i.GetComponent<NodeBehavior>().properties.state == Properties.StateEnum.EXPOSED)
            {
                GlobalVar.Instance.AddGlobalExposureValue(GlobalVar.Instance.exposureValueAdditionOfDetective + GlobalVar.Instance.exposureValueAccelerationOfDetective * stayRounds[focusOnNodes.IndexOf(i)]);
            }
        }
    }
}