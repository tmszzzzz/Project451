using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodesPlotAndPageData", menuName = "Custom/NodesPlotAndPageData")]
public class Node2PlotAndPageData : ScriptableObject
{
    [System.Serializable]
    public class NodePlotAndPage
    {
        public int nodeID;
        public string plotFileNames;
        public Sprite pageSprites;
    }

    public NodePlotAndPage[] nodePlotAndPages;

    public string GetPlotFileNameByID(int id)
    {
        foreach (NodePlotAndPage nodePlotAndPage in nodePlotAndPages)
        {
            if (nodePlotAndPage.nodeID == id)
            {
                return nodePlotAndPage.plotFileNames;
            }
        }
        return null;
    }

    public Sprite GetPageSpriteByID(int id)
    {
        foreach (NodePlotAndPage nodePlotAndPage in nodePlotAndPages)
        {
            if (nodePlotAndPage.nodeID == id)
            {
                return nodePlotAndPage.pageSprites;
            }
        }
        return null;
    }
}
