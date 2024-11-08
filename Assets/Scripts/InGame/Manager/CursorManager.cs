using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    // Update is called once per frame
    // 所有检测鼠标点击、raycast行为
    private Camera _mainCamera;
    [SerializeField] private PanelController panelController;
    [SerializeField] private CanvasBehavior canvas;
    [SerializeField] private GameObject cursorSelected;
    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //左键点击判定箱
                    panelController.NodePanelControl(hit);
                }
                else
                {
                    //左键空击
                    panelController.DisableNodeInfoPanel();
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //右键点击判定箱
                    RoundManager.instance.BookAllocation(1, hit);
                }
                else
                {
                    //右键空击
                }
            }
            else
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //鼠标悬停
                    //canvas.SetRegion(hit);
                    panelController.UpdateInfo(hit);
                    CursorSelected(hit);
                }
                else
                {
                    ResetCursorSelected();
                }
            }
        }
    }

    private void CursorSelected(RaycastHit hit)
    {
        if (hit.collider.gameObject.GetComponent<NodeBehavior>() != null)
        {
            cursorSelected.transform.position = hit.transform.position;
            cursorSelected.transform.rotation = _mainCamera.transform.rotation;
        }
        else
        {
            ResetCursorSelected();
        }
    }
    private void ResetCursorSelected()
    {
        cursorSelected.transform.position = new(0,1000,0);
    }
}
