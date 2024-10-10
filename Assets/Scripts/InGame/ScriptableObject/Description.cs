using UnityEngine;

[CreateAssetMenu(fileName = "NodeDescriptions", menuName = "Custom/NodeDescriptions")]
public class Description : ScriptableObject
{
    [System.Serializable]
    public class NodeDescription
    {
        public int nodeID;       // 节点的唯一标识符
        [TextArea]
        public string description;  // 节点的描述性文字
    }

    public NodeDescription[] descriptions; // 存储所有节点描述的数组

    // 根据节点ID获取描述
    public string GetDescriptionByID(int id)
    {
        foreach (NodeDescription desc in descriptions)
        {
            if (desc.nodeID == id)
            {
                return desc.description;
            }
        }
        return "Description not found";
    }
}