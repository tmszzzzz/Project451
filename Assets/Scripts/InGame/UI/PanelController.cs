using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PanelController : MonoBehaviour
{
    public GameObject NodeInfoPanel;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        NodeInfoPanel.SetActive(false);
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0) != true)
            {
                return;
            }

            GameObject hoveredObject = hit.collider.gameObject;
            NodeBehavior node = hoveredObject.GetComponent<NodeBehavior>();
            if (node != null)
            {
                EnableNodeInfoPanel();
                // Additional logic to update the NodeInfoPanel with node information
            }
            else
            {
                DisableNodeInfoPanel();
            }
        }
    }

    public void EnableNodeInfoPanel()
    {   
        if (NodeInfoPanel == null)
        {
            Debug.LogWarning("NodeInfoPanel is not assigned in the inspector.");
            return;
        }

        NodeInfoPanel.SetActive(true);
    }

    public void DisableNodeInfoPanel()
    {
        if (NodeInfoPanel == null)
        {
            Debug.LogWarning("NodeInfoPanel is not assigned in the inspector.");
            return;
        }
        NodeInfoPanel.SetActive(false);
    }
}
