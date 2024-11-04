using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    // Update is called once per frame
    // 所有检测鼠标点击、raycast行为
    Camera mainCamera;
    [SerializeField] PanelController panelController;
    [SerializeField] CanvasBehavior canvas;
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //左键点击判定箱
                    panelController.NodePanelControl(hit);
                    panelController.UpdateInfo(hit);
                }
                else
                {
                    //左键空击
                    panelController.DisableNodeInfoPanel();
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //右键点击判定箱
                    RoundManager.Instance.BookAllocation(1, hit);
                }
                else
                {
                    //右键空击
                }
            }
            else
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //鼠标悬停
                    canvas.SetRegion(hit);
                }
            }
        }
    }


}
