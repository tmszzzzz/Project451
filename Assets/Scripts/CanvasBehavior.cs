using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBehavior : MonoBehaviour
{
    // A 2D array to store references to all Cube objects
    public GameObject[,] cubes;
    public GameObject cubePrefab;

    public int gridSizeX = 20;
    public int gridSizeY = 20;
    public float gap = 1.1f;

    // Initialize the cubes array in Start()
    void Start()
    {
        // Assuming all the cubes are direct children of this canvas object
        cubes = new GameObject[gridSizeX, gridSizeY];

        // Populate the cubes array
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                GameObject cube = Instantiate(cubePrefab, new Vector3(transform.position.x + x * gap, transform.position.y, transform.position.z + y * gap), Quaternion.Euler(Vector3.zero), transform);
                CubeBehavior cb = cube.GetComponent<CubeBehavior>();
                if (cb == null)
                {
                    Debug.LogWarning("Script \"CubeBehavior\" not found in cube prefabs.");
                    return;
                }
                cb.posX = x;
                cb.posY = y;
                cubes[x, y] = cube;
            }
        }
    }

    void Update()
    {
        
        // 检测鼠标点击事件
        if (Input.GetMouseButtonDown(0)) // 0 表示左键
        {
            //Debug.Log("3");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 发射射线，检测是否点击到物体
            if (Physics.Raycast(ray, out hit))
            {
                // 检查点击到的物体是否有这个脚本
                CubeBehavior clickedObject = hit.collider.GetComponent<CubeBehavior>();
                //Debug.Log("3");
                if (clickedObject != null)
                {
                    // 触发物体上的事件
                    clickedObject.OnObjectClicked();
                }
            }
        }
    }

    // Method to get the neighbors of a cube at position (x, y)
    public List<GameObject> GetNeighbors(int x, int y)
    {
        List<GameObject> neighbors = new List<GameObject>();

        // Loop through the neighboring positions
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                // Skip the center cube itself
                if (dx == 0 && dy == 0) continue;

                // Calculate neighbor coordinates
                int neighborX = x + dx;
                int neighborY = y + dy;

                // Check if the neighbor is within bounds
                if (neighborX >= 0 && neighborX < gridSizeX && neighborY >= 0 && neighborY < gridSizeY)
                {
                    neighbors.Add(cubes[neighborX, neighborY]);
                }
            }
        }

        return neighbors;
    }

    // Method to refresh the state of all cubes
    public void RefreshAllCubes()
    {
        Properties.StateEnum[,] stateArr = new Properties.StateEnum[gridSizeX,gridSizeY];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                stateArr[x,y] = cubes[x, y].GetComponent<CubeBehavior>().RefreshState();
            }
        }
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                cubes[x, y].GetComponent<CubeBehavior>().SetState(stateArr[x,y]);
            }
        }
    }
    public void ResetAllCubes()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                cubes[x, y].GetComponent<CubeBehavior>().properties.state = Properties.StateEnum.NORMAL;
            }
        }
    }
}
