using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasBehavior : MonoBehaviour
{
    // A 2D array to store references to all Cube objects
    public GameObject cubePrefab;
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


    public void SavePositions()
    {/*
        List<PositionData> positions = new List<PositionData>();

        for (int i = 0; i < cubeList.Count; i++)
        {
            GameObject obj = cubeList[i];
            PositionData data = new PositionData(i, obj.transform.position);
            positions.Add(data);
        }

        string json = JsonConvert.SerializeObject(positions, Formatting.Indented);
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string saveFilePath = $"Assets/Resources/positions_{timestamp}.json"; // 添加时间戳到文件名
        System.IO.File.WriteAllText(saveFilePath, json);*/
        List<PositionData> positions = new List<PositionData>();
        List<ConnectionData> connections = new List<ConnectionData>();

        // 保存位置数据
        for (int i = 0; i < cubeList.Count; i++)
        {
            GameObject obj = cubeList[i];
            float[] f = { obj.transform.position.x, obj.transform.position.y, obj.transform.position.z };
            PositionData data = new PositionData(i, f);
            positions.Add(data);
        }

        // 保存连接数据
        for (int i = 0; i < connectionList.Count; i++)
        {
            GameObject connectionObj = connectionList[i];
            Connection connectionScript = connectionObj.GetComponent<Connection>();

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
    public class PositionData
    {
        public int id;
        public float[] position;
        public PositionData(int id, float[] position)
        {
            this.id = id;
            this.position = new float[] { position[0], position[1], position[2] };
        }

        public Vector3 GetVector3Position()
        {
            return new Vector3(position[0], position[1], position[2]);
        }
    }

    [System.Serializable]
    public class ConnectionData
    {
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
        public List<PositionData> positions;
        public List<ConnectionData> connections;
    }

    public void LoadConfiguration(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            ConfigurationData configData = JsonConvert.DeserializeObject<ConfigurationData>(json);

            // 实例化节点
            foreach (PositionData position in configData.positions)
            {
                GameObject node = Instantiate(cubePrefab, position.GetVector3Position(), Quaternion.identity, transform);
                node.name = $"Node_{position.id}"; // 为节点命名
                cubeList.Add(node);
            }

            // 实例化连接
            foreach (ConnectionData connection in configData.connections)
            {
                if (connection.startNodeId < cubeList.Count && connection.endNodeId < cubeList.Count)
                {
                    GameObject connectionObj = Instantiate(connectionPrefab, transform);
                    Connection connectionScript = connectionObj.GetComponent<Connection>();

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
                if (hit.collider.GetComponent<CubeBehavior>() != null)
                {
                    cubeList.Remove(hit.collider.gameObject);
                    Destroy(hit.collider.gameObject);
                    List<GameObject> toDelete = new List<GameObject>();
                    foreach(GameObject go in connectionList)
                    {
                        if (go.GetComponent<Connection>().startNode == hit.collider.gameObject || go.GetComponent<Connection>().endNode == hit.collider.gameObject)
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
                    if (hit.collider.GetComponent<CubeBehavior>() != null)
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
                    if (hit.collider.GetComponent<CubeBehavior>() != null)
                    {
                        connectionCreating.Add(hit.collider.gameObject);
                        if (connectionCreating.Count > 1)
                        {
                            if (connectionCreating[0] != connectionCreating[1] && ConnectionExist(connectionCreating[0], connectionCreating[1]) == null)
                            {
                                GameObject connection = Instantiate(connectionPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), gameObject.transform);
                                connection.GetComponent<Connection>().startNode = connectionCreating[0];
                                connection.GetComponent<Connection>().endNode = connectionCreating[1];
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

    
    public List<GameObject> GetNeighbors(GameObject cube)
    {
        List<GameObject> neighbors = new List<GameObject>();

        //TODO

        return neighbors;
    }

    // Method to refresh the state of all cubes
    public void RefreshAllCubes()
    {
        //TODO
    }
    public void ResetAllCubes()
    {
        //TODO
    }
    public void CreateCubes()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        float enter;
        Vector3 point = Vector3.zero;
        if (plane.Raycast(ray, out enter))
        {
            point = ray.GetPoint(enter);
        }
        cubeList.Add(Instantiate(cubePrefab, point, Quaternion.Euler(Vector3.zero), gameObject.transform));
    }
    public GameObject ConnectionExist(GameObject g1,GameObject g2)
    {
        foreach(GameObject go in connectionList)
        {
            Connection script = go.GetComponent<Connection>();
            if ((script.startNode == g1 && script.endNode == g2) || (script.startNode == g2 && script.endNode == g1))
            {
                return go;
            }
        }
        return null;
    }
}
