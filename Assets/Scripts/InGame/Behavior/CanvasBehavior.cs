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
    private Plane plane; // ���ڶ���ˮƽ��
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
    public GameObject Me;//����ָʾ��ҽڵ�Ķ������ã�������Ե��趨Ϊid=0�Ľڵ㣬�����ٸ�


    
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
        //��ȡʱʹ��
        public List<NodeData> positions;
        public List<ConnectionData> connections;
    }

    public void LoadConfiguration(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            ConfigurationData configData = JsonConvert.DeserializeObject<ConfigurationData>(json);

            // ʵ�����ڵ�
            foreach (NodeData position in configData.positions)
            {
                GameObject node = Instantiate(nodePrefabMap[(int)position.properties.type], position.GetVector3Position(), Quaternion.identity, transform);
                node.name = $"Node_{position.id}"; // Ϊ�ڵ�����
                NodeBehavior nodeBehavior = node.GetComponent<NodeBehavior>();
                if (nodeBehavior != null)
                {
                    Properties loadedProperties = position.properties ?? new Properties();

                    // ����ֶ�ΪĬ��ֵ�������Ĭ��ֵ
                    nodeBehavior.properties = new Properties
                    {
                        type = loadedProperties.type != 0 ? loadedProperties.type : 0,
                        awakeThreshold = loadedProperties.awakeThreshold != 0 ? loadedProperties.awakeThreshold : 0,
                        exposeThreshold = loadedProperties.exposeThreshold != 0 ? loadedProperties.exposeThreshold : 0,
                        //NumOfBooks = loadedProperties.NumOfBooks != 0 ? loadedProperties.NumOfBooks : 0,
                        maximumNumOfBooks = loadedProperties.maximumNumOfBooks != 0 ? loadedProperties.maximumNumOfBooks : 0
                    };
                }
                nodeList.Add(node);
            }
            Me = nodeList[0];
            // ʵ��������
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
        //������һЩ��Ҫ��ʼ������Ϣ
        Me.GetComponent<NodeBehavior>().properties.numOfBooks = 1;
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
        // ��һ�����ռ����нڵ�ļ����ı�Ϊ��״̬
        Dictionary<GameObject, Properties.StateEnum> newStateMap = new Dictionary<GameObject, Properties.StateEnum>();

        foreach (GameObject node in nodeList)
        {
            //Debug.Log(2);
            NodeBehavior nodeBehavior = node.GetComponent<NodeBehavior>();

            if (nodeBehavior != null)
            {
                // �ռ�ÿ���ڵ����״̬
                Properties.StateEnum newState = nodeBehavior.RefreshState();
                newStateMap.Add(node, newState);
            }
            else
            {
                Debug.LogWarning("Missing script \"NodeBehavior\" in " + node.name);
            }
        }

        // �ڶ�����ͳһӦ�����нڵ����״̬
        foreach (var entry in newStateMap)
        {
            GameObject node = entry.Key;
            Properties.StateEnum newState = entry.Value;

            NodeBehavior nodeBehavior = node.GetComponent<NodeBehavior>();

            if (nodeBehavior != null)
            {
                // Ӧ����״̬
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
}
