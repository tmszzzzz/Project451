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
    public GameObject nodePrefab;
    public GameObject connectionPrefab;
    private Camera cam;
    private Plane plane; // ���ڶ���ˮƽ��
    [SerializeField]
    private List<GameObject> nodeList;
    [SerializeField]
    private List<GameObject> connectionList;


    
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
                maximumBooks = 0 // default
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
                GameObject node = Instantiate(nodePrefab, position.GetVector3Position(), Quaternion.identity, transform);
                node.name = $"Node_{position.id}"; // Ϊ�ڵ�����
                NodeBehavior nodeBehavior = node.GetComponent<NodeBehavior>();
                if (nodeBehavior != null)
                {
                    Properties loadedProperties = position.properties ?? new Properties();

                    // ����ֶ�ΪĬ��ֵ�������Ĭ��ֵ
                    nodeBehavior.properties = new Properties
                    {
                        awakeThreshold = loadedProperties.awakeThreshold != 0 ? loadedProperties.awakeThreshold : 0,
                        exposeThreshold = loadedProperties.exposeThreshold != 0 ? loadedProperties.exposeThreshold : 0,
                        maximumBooks = loadedProperties.maximumBooks != 0 ? loadedProperties.maximumBooks : 0
                    };
                }
                nodeList.Add(node);
            }

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

    void Start()
    {
        cam = Camera.main;
        plane = new Plane(Vector3.up, Vector3.zero);
        nodeList = new List<GameObject>();
        connectionList = new List<GameObject>();
    }

    void Update()
    {
        
    }


    public List<GameObject> GetNeighbors(GameObject node)
    {
        List<GameObject> neighbors = new List<GameObject>();

        //TODO

        return neighbors;
    }

    public void RefreshAllNodes()
    {
        //TODO
    }
    public void ResetAllNodes()
    {
        //TODO
    }
    public void CreateNode()
    {
        //create a node in the center of the screen
        //��ɾ��û׼�Ժ�ʲô��չ��������
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        float enter;
        Vector3 point = Vector3.zero;
        if (plane.Raycast(ray, out enter))
        {
            point = ray.GetPoint(enter);
        }
        GameObject newNode = Instantiate(nodePrefab, point, Quaternion.Euler(Vector3.zero), gameObject.transform);
        newNode.name = $"Node";
        nodeList.Add(newNode);

    }
}