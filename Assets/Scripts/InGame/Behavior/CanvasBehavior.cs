using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CanvasBehavior : MonoBehaviour
{
    //public GameObject nodePrefab;
    public List<GameObject> nodePrefabMap;
    public GameObject connectionPrefab;
    private Camera cam;
    private Plane plane; // 用于定义水平面
    [SerializeField]
    private List<GameObject> nodeList;
    public List<GameObject> GetNodeList()
    {
        return nodeList;
    }
    [SerializeField]
    private List<GameObject> connectionList;
    public List<GameObject> GetConnectionList()
    {
        return connectionList;
    }
    public GameObject Me;//用于指示玩家节点的对象引用，这里粗略地设定为id=0的节点，后续再改
    public Description description;

    public void SavePositions()
    {
        List<NodeData> positions = new List<NodeData>();
        List<ConnectionData> connections = new List<ConnectionData>();

        // 保存位置数据
        for (int i = 0; i < nodeList.Count; i++)
        {
            GameObject obj = nodeList[i];
            float[] f = { obj.transform.position.x, obj.transform.position.y, obj.transform.position.z };
            NodeBehavior cubeBehavior = obj.GetComponent<NodeBehavior>();
            Properties properties = cubeBehavior != null ? cubeBehavior.properties : null;

            NodeData data = new NodeData(i, f, properties);

            positions.Add(data);
        }

        // 保存连接数据
        for (int i = 0; i < connectionList.Count; i++)
        {
            GameObject connectionObj = connectionList[i];
            ConnectionBehavior connectionScript = connectionObj.GetComponent<ConnectionBehavior>();

            if (connectionScript != null)
            {
                // 查找 startNode 和 endNode 的 ID
                int startNodeId = nodeList.IndexOf(connectionScript.startNode);
                int endNodeId = nodeList.IndexOf(connectionScript.endNode);

                // 确保 ID 有效
                if (startNodeId != -1 && endNodeId != -1)
                {
                    ConnectionData connectionData = new ConnectionData(startNodeId, endNodeId);
                    connections.Add(connectionData);
                }
            }
        }

        // 序列化位置和连接数据
        var combinedData = new
        {
            positions = positions,
            connections = connections
        };

        string json = JsonConvert.SerializeObject(combinedData, Formatting.Indented);
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string saveFilePath = $"Assets/Resources/positions_{timestamp}.json"; // 添加时间戳到文件名
        System.IO.File.WriteAllText(saveFilePath, json);
    }

    [System.Serializable]
    public class NodeData
    {
        //node
        public int id;
        public float[] position;
        public Properties properties;
        public NodeData(int id, float[] position, Properties properties = null)
        {
            this.id = id;
            this.position = new float[] { position[0], position[1], position[2] };
            this.properties = properties ?? new Properties
            {
                awakeThreshold = 0, // default
                exposeThreshold = 0, // default
                //NumOfBooks = 0,
                maximumNumOfBooks = 0 // default
            };
        }

        public Vector3 GetVector3Position()
        {
            return new Vector3(position[0], position[1], position[2]);
        }
    }

    [System.Serializable]
    public class ConnectionData
    {
        //connection
        public int startNodeId;
        public int endNodeId;

        public ConnectionData(int startId, int endId)
        {
            startNodeId = startId;
            endNodeId = endId;
        }
    }

    [System.Serializable]
    public class ConfigurationData
    {
        //读取时使用
        public List<NodeData> positions;
        public List<ConnectionData> connections;
    }

    public void LoadConfiguration(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            ConfigurationData configData = JsonConvert.DeserializeObject<ConfigurationData>(json);

            // 实例化节点
            foreach (NodeData position in configData.positions)
            {
                GameObject node = Instantiate(nodePrefabMap[(int)position.properties.type], position.GetVector3Position(), Quaternion.identity, transform);
                node.name = $"Node_{position.id}"; // 为节点命名
                NodeBehavior nodeBehavior = node.GetComponent<NodeBehavior>();
                if (nodeBehavior != null)
                {
                    Properties loadedProperties = position.properties ?? new Properties();

                    // 如果字段为默认值，则填充默认值
                    nodeBehavior.properties = new Properties
                    {
                        type = loadedProperties.type != 0 ? loadedProperties.type : 0,
                        awakeThreshold = loadedProperties.awakeThreshold != 0 ? loadedProperties.awakeThreshold : 0,
                        exposeThreshold = loadedProperties.exposeThreshold != 0 ? loadedProperties.exposeThreshold : 0,
                        //NumOfBooks = loadedProperties.NumOfBooks != 0 ? loadedProperties.NumOfBooks : 0,
                        maximumNumOfBooks = loadedProperties.maximumNumOfBooks != 0 ? loadedProperties.maximumNumOfBooks : 0,
                        description = description.GetDescriptionByID(position.id)
                    };
                }
                nodeList.Add(node);
            }
            Me = nodeList[0];
            // 实例化连接
            foreach (ConnectionData connection in configData.connections)
            {
                if (connection.startNodeId < nodeList.Count && connection.endNodeId < nodeList.Count)
                {
                    GameObject connectionObj = Instantiate(connectionPrefab, transform);
                    ConnectionBehavior connectionScript = connectionObj.GetComponent<ConnectionBehavior>();

                    if (connectionScript != null)
                    {
                        connectionScript.startNode = nodeList[connection.startNodeId];
                        connectionScript.endNode = nodeList[connection.endNodeId];
                    }
                    connectionList.Add(connectionObj);
                }
            }
        }
        else
        {
            Debug.LogError($"File not found: {filePath}");
        }
    }

    public void Initialization()
    {
        //这里是一些需要初始化的信息
        Me.GetComponent<NodeBehavior>().properties.numOfBooks = 2;
        Me.GetComponent<NodeBehavior>().properties.state = Properties.StateEnum.AWAKENED;
    }

    void Start()
    {
        cam = Camera.main;
        plane = new Plane(Vector3.up, Vector3.zero);
        nodeList = new List<GameObject>();
        connectionList = new List<GameObject>();
        LoadConfiguration("Assets/Maps/map.json");
        Initialization();
    }

    void Update()
    {
        
    }

    public int GetTotalBookNum()
    {
        int total = 0;
        foreach (var i in nodeList)
        {
            NodeBehavior nb = i.GetComponent<NodeBehavior>();
            if (nb != null) total += nb.properties.numOfBooks;
        }

        return total;
    }

    public List<GameObject> GetNeighbors(GameObject node)
    {
        List<GameObject> neighbors = new List<GameObject>();

        foreach(GameObject con in connectionList)
        {
            ConnectionBehavior conB = con.GetComponent<ConnectionBehavior>();
            if (conB != null)
            {
                if (conB.startNode == node) neighbors.Add(conB.endNode);
                if (conB.endNode == node) neighbors.Add(conB.startNode);
            }
        }

        return neighbors;
    }

    public void RefreshAllNodes()
    {
        //Debug.Log(1);
        // 第一步：收集所有节点的即将改变为的状态
        Dictionary<GameObject, Properties.StateEnum> newStateMap = new Dictionary<GameObject, Properties.StateEnum>();

        foreach (GameObject node in nodeList)
        {
            //Debug.Log(2);
            NodeBehavior nodeBehavior = node.GetComponent<NodeBehavior>();

            if (nodeBehavior != null)
            {
                // 收集每个节点的新状态
                Properties.StateEnum newState = nodeBehavior.PredictState().state;
                newStateMap.Add(node, newState);
            }
            else
            {
                Debug.LogWarning("Missing script \"NodeBehavior\" in " + node.name);
            }
        }

        // 第二步：统一应用所有节点的新状态
        foreach (var entry in newStateMap)
        {
            GameObject node = entry.Key;
            Properties.StateEnum newState = entry.Value;

            NodeBehavior nodeBehavior = node.GetComponent<NodeBehavior>();

            if (nodeBehavior != null)
            {
                // 应用新状态
                nodeBehavior.SetState(newState);
            }
        }
    }
    public void SetNodeNumOfBooks(GameObject node,int v)
    {
        NodeBehavior nb = node.GetComponent<NodeBehavior>();
        if (nb != null) nb.properties.numOfBooks = v;
        else Debug.LogWarning("NodeBehavior script is null.");
    }
    public void AddNodeNumOfBooks(GameObject node, int v)
    {
        NodeBehavior nb = node.GetComponent<NodeBehavior>();
        if (nb != null) nb.properties.numOfBooks += v;
        else Debug.LogWarning("NodeBehavior script is null.");
    }
    public void RefreshGlobalExposureValue()
    {
        bool exposed = false;
        foreach(var i in nodeList)
        {
            NodeBehavior nb;
            if((nb = i.GetComponent<NodeBehavior>()) != null && nb.properties.state == Properties.StateEnum.EXPOSED)
            {
                exposed = true;
                GlobalVar.Instance.AddGlobalExposureValue(GlobalVar.Instance.exposureValueAdditionOfExposedNode);
            }
        }
        if(!exposed) GlobalVar.Instance.RuduceGlobalExposureValue(GlobalVar.Instance.exposureValueReductionOfNoExposedNode);
    }
    public List<GameObject> ExposedList()
    {
        List<GameObject> exposed = new List<GameObject>();
        foreach(var i in nodeList)
        {
            if (i.GetComponent<NodeBehavior>().properties.state == Properties.StateEnum.EXPOSED)
            {
                exposed.Add(i);
            }
        }
        return exposed;
    }
}
