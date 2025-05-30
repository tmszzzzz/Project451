using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ConnectionBehavior : MonoBehaviour
{
    [SerializeField]public int type = 0;
    public GameObject startNode;
    public GameObject endNode;
    public int unlockTag; //only used by Unlockable
    public int unlockDemand; //only used by Unlockable
    [SerializeField] protected LineRenderer lineRenderer;
    [SerializeField] protected DetectiveBehavior detective;
    public bool isDisplayingInfo = false;


    protected virtual void Update()
    {
        if (startNode != null && endNode != null)
        {
            UpdateLine();
        }
    }

    public virtual void UpdateLine()
    {
        lineRenderer.positionCount = 2; // 连接线有两个点
        lineRenderer.SetPosition(0, startNode.transform.position + new Vector3(0,0.5f,0)); // 设置起始点
        lineRenderer.SetPosition(1, endNode.transform.position+new Vector3(0,0.5f,0));   // 设置结束点
    }

    public virtual GameObject GetTheOtherNodeIfExist(GameObject node)
    {
        if(node == startNode) return endNode;
        if (node == endNode) return startNode;
        return null;
    }

    public virtual void UnlockTrigger(int unlockTag)
    {
        //这个方法提供统一的unlock接口，但是它仅在子类Unlockable有非空实现。
        return;
    }
    public virtual void UnlockCancel(int unlockTag)
    {
        //这个方法提供统一的unlock接口，但是它仅在子类Unlockable有非空实现。
        return;
    }
    public virtual void RefreshColor(DetectiveBehavior db)
    {
        System.Random r = new System.Random();
        double d = r.NextDouble();
        if (d > GlobalVar.instance.probabilityOfNodesInspectingDetective / 100f)
        {
            NonInfoColor();
            return;
        }
        if (this.GetComponent<ConnectionBehavior>().startNode.GetComponent<NodeBehavior>().properties.state !=
            Properties.StateEnum.NORMAL
            || this.GetComponent<ConnectionBehavior>().endNode.GetComponent<NodeBehavior>().properties.state !=
            Properties.StateEnum.NORMAL)
        {
            InfoColor(db);
        }
        else
        {
            NonInfoColor();
        }
    }

    public virtual void NonInfoColor()
    {
        GetComponent<LineRenderer>().startColor = Color.white;
        GetComponent<LineRenderer>().endColor = Color.white;
        isDisplayingInfo = false;
    }

    public virtual void InfoColor(DetectiveBehavior db)
    {
        isDisplayingInfo = true;
        int n = 0;
        if (db.IsDetected(GetComponent<ConnectionBehavior>().startNode)) n++;
        if (db.IsDetected(GetComponent<ConnectionBehavior>().endNode)) n++;
        switch (n)
        {
            case 0:
                GetComponent<LineRenderer>().startColor = Color.green;
                GetComponent<LineRenderer>().endColor = Color.green;
                break;
            case 1:
                GetComponent<LineRenderer>().startColor = Color.yellow;
                GetComponent<LineRenderer>().endColor = Color.yellow;
                break;
            case 2:
                GetComponent<LineRenderer>().startColor = Color.red;
                GetComponent<LineRenderer>().endColor = Color.red;
                break;
            default:
                break;
        }
    }
}
