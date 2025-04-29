using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    // Update is called once per frame
    // 所有检测鼠标点击、raycast行为
    
    private Camera _mainCamera;
    Plane plane = new Plane(Vector3.up, Vector3.zero);
    [SerializeField] private PanelController panelController;
    [SerializeField] private CanvasBehavior canvas;
    [SerializeField] private GameObject cursorSelected;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip tap;

    private Vector3 cameraStartPosition;                // 记录镜头初始位置
    private Vector3 dragStartPosition;                         // 拖拽起点
    private bool isDragging = false;                    // 拖拽状态
    private Vector3 totalDragOffset;
    
    private GameObject bookMark;
    private bool ifTapSound = true;
    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    private void Update()
    {
        HandleCameraDrag();
        if (!EventSystem.current.IsPointerOverGameObject() && !RoundManager.instance.operationForbidden)
            // if (!RoundManager.instance.operationForbidden)
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
                    //panelController.DisableNodeInfoPanel();
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //右键点击判定箱
                    // RoundManager.instance.BookAllocation(1, hit);
                    if (Input.GetKey(KeyCode.F))
                    {
                        var v = RoundManager.instance.getCancelItemInfo(1, hit);
                        if (v != null)
                        {
                            RoundManager.instance.CancelBookAllocation(v);
                        }
                    }
                    else
                    {
                        RoundManager.instance.GetObjectInfo(1, hit);
                    }
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
                    panelController.NodePanelControl(hit);
                    canvas.SetRegion(hit);
                    canvas.AddConnection(hit);
                    CursorSelected(hit);
                    ShowBookMarkName(hit);
                    HideBookMarkName(hit);
                }
                else
                {
                    ResetCursorSelected();
                    panelController.DisableNodeInfoPanel();
                }
            }
        }
    }

    private void ShowBookMarkName(RaycastHit hit)
    {
        var v = hit.collider.gameObject;
        if (v.GetComponent<BookMark>() != null)
        {
            this.bookMark = v;
            v.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    
    private void HideBookMarkName(RaycastHit hit)
    {
        var v = hit.collider.gameObject;
        if (v.GetComponent<BookMark>() == null || v == null)
        {
            if (this.bookMark != null)
            {
                this.bookMark.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        
        
    }

    private void CursorSelected(RaycastHit hit)
    {
        if (hit.collider.gameObject.GetComponent<NodeBehavior>() != null)
        {
            if (ifTapSound)
            {
                //audioSource.PlayOneShot(tap);
                ifTapSound = false;
            }
            cursorSelected.transform.position = hit.transform.position;
            cursorSelected.transform.rotation = _mainCamera.transform.rotation;
        }
        else
        {
            ifTapSound = true;
            ResetCursorSelected();
        }
    }
    private void ResetCursorSelected()
    {
        cursorSelected.transform.position = new(0,1000,0);
    }
    
    private void HandleCameraDrag()
    {
        // 右键按下开始拖拽
        if (Input.GetMouseButtonDown(1))
        {
            dragStartPosition = Input.mousePosition;
            cameraStartPosition = _mainCamera.transform.position;
            isDragging = true;
            totalDragOffset = Vector3.zero; // 重置累计偏移
        }

        // 右键释放结束拖拽
        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        // 拖拽中处理镜头移动
        if (isDragging)
        {
            // 计算鼠标在屏幕空间的位移差值
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 screenDelta = (currentMousePos - dragStartPosition) * 0.1f; // 灵敏度系数

            // 转换为世界空间移动量（考虑相机朝向）
            Vector3 move = _mainCamera.transform.right * -screenDelta.x 
                           + _mainCamera.transform.up * -screenDelta.y;
        
            // 保持高度不变
            move.y = 0;
            totalDragOffset += move;
            // 应用位移到相机
            _mainCamera.GetComponent<CameraBehavior>().realPosition = cameraStartPosition + move;
        }
    }
}
