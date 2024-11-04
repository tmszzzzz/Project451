using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionBehavior : MonoBehaviour
{
    public GameObject startNode;
    public GameObject endNode;
    [SerializeField] protected LineRenderer lineRenderer;
    [SerializeField] protected DetectiveBehavior detective;


    protected virtual void Update()
    {
        if (startNode != null && endNode != null)
        {
            UpdateLine();
        }
    }

    protected virtual void UpdateLine()
    {
        lineRenderer.positionCount = 2; // 连接线有两个点
        lineRenderer.SetPosition(0, startNode.transform.position); // 设置起始点
        lineRenderer.SetPosition(1, endNode.transform.position);   // 设置结束点
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
        if (d > GlobalVar.Instance.ProbabilityOfNodesInspectingDetective)
        {
            GetComponent<LineRenderer>().startColor = Color.white;
            GetComponent<LineRenderer>().endColor = Color.white;
            return;
        }
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
