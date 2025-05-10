using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DetectiveBehavior : MonoBehaviour
{
    [SerializeField]public List<GameObject> focusOnNodes = new List<GameObject>();
    [SerializeField]public List<GameObject> focusPointers = new List<GameObject>();
    public GameObject pointerPrefab;
    public CanvasBehavior canvas;
    public TaskCompletionSource<bool> Tcs1;
    public TaskCompletionSource<bool> Tcs2;
    [SerializeField] private GameObject DetectedFx;
    [SerializeField] private GameObject PointFx;
    [SerializeField] private Transform LightconeCenter;
    [SerializeField] private GameObject LightconePrefab;
    [SerializeField] private List<GameObject> Lightcones;
    public AudioClip detedted;
    public AudioClip spotlight;
    public AudioClip notice;
    private void Start()
    {
        if (GameLoader.instance != null && !GameLoader.instance.loadingAnExistingGame)
        {
            canvas.RefreshAllConnections();
        }
    }

    public void AddADetective()
    {
        List<GameObject> exposedList = canvas.ExposedList();
        if (exposedList.Count > 0)
        {
            focusOnNodes.Add(exposedList[Random.Range(0, exposedList.Count)]);
            focusPointers.Add(Instantiate(pointerPrefab, focusOnNodes[focusOnNodes.Count - 1].transform.position,Quaternion.Euler(0,0,0)));
        }
        else
        {
            Debug.Log("There is no EXPOSED node in canvas. A detective will firstly appear in an EXPOSED node only.");
        }
    }
    public void AddDetectivesInRegion(int region, int num) 
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
        int l = regionList.Count;
        for (int i = 0; i < Mathf.Min(num,l); i++)
        {
            var tar = regionList[Random.Range(0, regionList.Count)];
            targets.Add(tar);
            regionList.Remove(tar);
        }
        foreach (var target in targets)
        {
            focusOnNodes.Add(target);
            focusPointers.Add(Instantiate(pointerPrefab, target.transform.position,Quaternion.Euler(0,0,0)));
        }
    }

    public void a()
    {
        AddDetectivesInRegion(4,2);
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
            }else if(focusOnNodes[i].GetComponent<NodeBehavior>().properties.state != Properties.StateEnum.EXPOSED) //self 0
            {
                if (exposedNeighbors.Count == 0) //neighbor 0
                {
                    //Debug.Log(i + " 00");
                    focusOnNodes[i] = list[Random.Range(0, list.Count)];
                    
                }
                else //neighbor 1
                {
                    //Debug.Log(i + " 01");
                    focusOnNodes[i] = exposedNeighbors[Random.Range(0, exposedNeighbors.Count)];
                }
            }else //self 1
            {
                if (exposedNeighbors.Count == 0) //neighbor 0
                {
                    //Debug.Log(i + " 10");
                }
                else //neighbor 1
                {
                    //Debug.Log(i + " 11");
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
        LightconeInvis();
        await AllVis();
        //foreach(var i in focusOnNodes)
        //{
        //    if (RoundManager.instance.BookAllocationMap[i] != 0)
        //    {
        //        GlobalVar.instance.AddGlobalExposureValue(GlobalVar.instance.exposureValueAdditionOfDetective);
        //    }
        //}
    }

    public bool IsDetected(GameObject go)
    {
        return focusOnNodes.Contains(go);
    }

    public async Task DetectedVis()
    {
        bool skip = true;
        int l = focusPointers.Count;
        List<GameObject> founds = new List<GameObject>();
        for(int i =0;i<l;i++)
        {
            var list = RoundManager.instance.BookAllocationItems;
            bool found = false;
            foreach (var bookAllocationItem in list)
            {
                if (focusOnNodes[i] == bookAllocationItem.Begin || focusOnNodes[i] == bookAllocationItem.End)
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                skip = false;
                founds.Add(focusOnNodes[i]);
                Instantiate(DetectedFx,focusOnNodes[i].transform.position,Quaternion.Euler(0,0,0));
                
            }
        }

        if (!skip)
        {
            CameraBehavior.instance.GetComponent<AudioSource>().PlayOneShot(detedted);
            await Task.Delay(1000);
        }
        foreach (var i in founds)
        {
            GameObject target = i;
            Vector3 horizontalDirection = target.transform.position - LightconeCenter.transform.position;
            horizontalDirection.y = 0;
            horizontalDirection = horizontalDirection.normalized;
            Vector3 pos = LightconeCenter.position + 3f * horizontalDirection; 
            Vector3 facing = target.transform.position - pos;
            Lightcones.Add(Instantiate(LightconePrefab, pos, Quaternion.LookRotation(facing)));
            int v = GlobalVar.instance.exposureValueAdditionOfDetective;
            if (GlobalVar.instance.exposureCoefficient == 0.8f)
            {
                v = v * 4 / 5;
            }
            if (GlobalVar.instance.exposureCoefficient == 1.2f)
            {
                v = v * 6 / 5;
            }
            GlobalVar.instance.AddGlobalExposureValue(v);
            CameraBehavior.instance.GetComponent<AudioSource>().PlayOneShot(spotlight);
            await Task.Delay(250);

        }

        if (!skip) await Task.Delay(1000);
    }

    public async Task AllVis()
    {
        bool skip = true;
        int l = focusPointers.Count;
        
        for(int i =0;i<l;i++)
        {
            var list = RoundManager.instance.BookAllocationItems;
            bool found = false;
            foreach (var bookAllocationItem in list)
            {
                if (focusOnNodes[i] == bookAllocationItem.Begin || focusOnNodes[i] == bookAllocationItem.End)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                skip = false;
                Instantiate(PointFx,focusOnNodes[i].transform.position,Quaternion.Euler(0,0,0));
                
            }
        }

        if (!skip)
        {
            CameraBehavior.instance.GetComponent<AudioSource>().PlayOneShot(notice);
            await Task.Delay(4000);
        }
    }

    public async Task LightconeInvis()
    {
        await Task.Delay(2000);
        foreach (var lightcone in Lightcones)
        {
            Destroy(lightcone);
            await Task.Delay(250);
        }
        Lightcones.Clear();
    }
}
