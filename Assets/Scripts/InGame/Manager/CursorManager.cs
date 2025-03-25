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

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip tap;

    private bool ifTapSound = true;
    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && !RoundManager.instance.operationForbidden)
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
                }
                else
                {
                    ResetCursorSelected();
                    panelController.DisableNodeInfoPanel();
                }
            }
        }
        else
        {
            ResetCursorSelected();
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
}
