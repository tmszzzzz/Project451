using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CanvasEditorBehavior : MonoBehaviour
{
    // A 2D array to store references to all Cube objects
    public GameObject cubePrefab;
    //public List<GameObject> cubePrefabMap;
    public GameObject connectionPrefab;
    private Camera cam;
    private Plane plane; // 用于定义水平面
    [SerializeField]
    private List<GameObject> cubeList;
    [SerializeField]
    private List<GameObject> connectionList;
    private List<GameObject> connectionCreating;
    public bool cubeDelete = false;
    public bool connectionDelete = false;
    public int uniqueId = 0;


    public void SavePositions()
    {
        List<NodeData> positions = new List<NodeData>();
        List<ConnectionData> connections = new List<ConnectionData>();

        // 保存位置数据
        for (int i = 0; i < cubeList.Count; i++)
        {
            GameObject obj = cubeList[i];
            float[] f = { obj.transform.position.x, obj.transform.position.y, obj.transform.position.z };
            CubeEditorBehavior cubeBehavior = obj.GetComponent<CubeEditorBehavior>();
            PropertiesEditor properties = cubeBehavior != null ? cubeBehavior.properties : null; 

            NodeData data = new NodeData(i, f, properties);

            positions.Add(data);
        }

        // 保存连接数据
        for (int i = 0; i < connectionList.Count; i++)
        {
            GameObject connectionObj = connectionList[i];
            ConnectionEditorBehavior connectionScript = connectionObj.GetComponent<ConnectionEditorBehavior>();

            if (connectionScript != null)
            {
                // 查找 startNode 和 endNode 的 ID
                int startNodeId = cubeList.IndexOf(connectionScript.startNode);
                int endNodeId = cubeList.IndexOf(connectionScript.endNode);

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
        //存储cube时使用
        public int id;
        public float[] position;
        public PropertiesEditor properties;
        public NodeData(int id, float[] position, PropertiesEditor properties = null)
        {
            this.id = id;
            this.position = new float[] { position[0], position[1], position[2] };
            this.properties = properties ?? new PropertiesEditor
            {
                type = PropertiesEditor.typeEnum.NORMAL,
                awakeThreshold = 0, // default
                exposeThreshold = 0, // default
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
        //存储connection时使用
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
                GameObject node = Instantiate(cubePrefab, position.GetVector3Position(), Quaternion.identity, transform);
                node.name = $"Node_{position.id}"; // 为节点命名
                uniqueId++;
                CubeEditorBehavior cubeBehavior = node.GetComponent<CubeEditorBehavior>();
                if (cubeBehavior != null)
                {
                    PropertiesEditor loadedProperties = position.properties ?? new PropertiesEditor();

                    // 如果字段为默认值，则填充默认值
                    cubeBehavior.properties = new PropertiesEditor
                    {
                        type = (PropertiesEditor.typeEnum)(loadedProperties.type != 0 ? loadedProperties.type : 0),
                        awakeThreshold = loadedProperties.awakeThreshold != 0 ? loadedProperties.awakeThreshold : 0,
                        exposeThreshold = loadedProperties.exposeThreshold != 0 ? loadedProperties.exposeThreshold : 0,
                        maximumNumOfBooks = loadedProperties.maximumNumOfBooks != 0 ? loadedProperties.maximumNumOfBooks : 0
                    };
                }
                cubeList.Add(node);
            }

            // 实例化连接
            foreach (ConnectionData connection in configData.connections)
            {
                if (connection.startNodeId < cubeList.Count && connection.endNodeId < cubeList.Count)
                {
                    GameObject connectionObj = Instantiate(connectionPrefab, transform);
                    ConnectionEditorBehavior connectionScript = connectionObj.GetComponent<ConnectionEditorBehavior>();

                    if (connectionScript != null)
                    {
                        connectionScript.startNode = cubeList[connection.startNodeId];
                        connectionScript.endNode = cubeList[connection.endNodeId];
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

    // Initialize the cubes array in Start()
    void Start()
    {
        cam = Camera.main;
        plane = new Plane(Vector3.up, Vector3.zero);
        cubeList = new List<GameObject>();
        connectionList = new List<GameObject>();
        connectionCreating = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            cubeDelete = true;
        }
        else cubeDelete = false;
        if (Input.GetKey(KeyCode.Q))
        {
            connectionDelete = true;
        }
        else connectionDelete = false;

        if (Input.GetMouseButtonDown(0) && cubeDelete) // 0 表示左键
        {
            //Debug.Log("3");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 发射射线，检测是否点击到物体
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<CubeEditorBehavior>() != null)
                {
                    cubeList.Remove(hit.collider.gameObject);
                    Destroy(hit.collider.gameObject);
                    List<GameObject> toDelete = new List<GameObject>();
                    foreach(GameObject go in connectionList)
                    {
                        if (go.GetComponent<ConnectionEditorBehavior>().startNode == hit.collider.gameObject || go.GetComponent<ConnectionEditorBehavior>().endNode == hit.collider.gameObject)
                        {
                            toDelete.Add(go);
                        }
                    }
                    foreach (GameObject go in toDelete) {
                        connectionList.Remove(go);
                        Destroy(go);
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (connectionDelete)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.GetComponent<CubeEditorBehavior>() != null)
                    {
                        connectionCreating.Add(hit.collider.gameObject);
                        if (connectionCreating.Count > 1)
                        {
                            GameObject con = ConnectionExist(connectionCreating[0], connectionCreating[1]);
                            if (connectionCreating[0] != connectionCreating[1] && con != null)
                            {
                                connectionList.Remove(con);
                                Destroy(con);
                                Debug.Log("Removed connection.");
                            }
                            connectionCreating.Clear();
                        }
                    }
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.GetComponent<CubeEditorBehavior>() != null)
                    {
                        connectionCreating.Add(hit.collider.gameObject);
                        if (connectionCreating.Count > 1)
                        {
                            if (connectionCreating[0] != connectionCreating[1] && ConnectionExist(connectionCreating[0], connectionCreating[1]) == null)
                            {
                                GameObject connection = Instantiate(connectionPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), gameObject.transform);
                                connection.GetComponent<ConnectionEditorBehavior>().startNode = connectionCreating[0];
                                connection.GetComponent<ConnectionEditorBehavior>().endNode = connectionCreating[1];
                                connectionList.Add(connection);
                                Debug.Log("Created connection.");
                            }
                            connectionCreating.Clear();
                        }
                    }
                }
            }
        }
    }

    public GameObject ConnectionExist(GameObject g1,GameObject g2)
    {
        foreach(GameObject go in connectionList)
        {
            ConnectionEditorBehavior script = go.GetComponent<ConnectionEditorBehavior>();
            if ((script.startNode == g1 && script.endNode == g2) || (script.startNode == g2 && script.endNode == g1))
            {
                return go;
            }
        }
        return null;
    }
}
