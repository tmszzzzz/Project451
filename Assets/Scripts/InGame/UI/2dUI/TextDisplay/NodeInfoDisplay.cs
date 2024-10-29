using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeInfoDisplay : MonoBehaviour
{
    public bool updateInfoWithoutClick = true; // 是否在点击时更新信息
    public GameObject infoTextGo; // 在 Inspector 中指定的 UI Text 用于显示信息
    public GameObject currentNode; // 当前选中的物体
    private TextMeshProUGUI infoText;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        infoText = infoTextGo.GetComponent<TextMeshProUGUI>();
        // 初始化文本为空
        infoText.text = "";
        currentNode = null;
    }

    void Update()
    {
        // 射线检测鼠标指向的物体
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //return before updating the info text if the updateInfoWithoutClick is false and the left mouse button is not clicked
            //either wise the info text will be updated every frame
            // Debug.Log(updateInfoWithoutClick);
            if (updateInfoWithoutClick) 
            goto Update;

            if (Input.GetMouseButton(0) != true)
            {
                return;
            }

Update:
            GameObject hoveredObject = hit.collider.gameObject;

            // 检查物体是否有 CubeBehavior 脚本
            NodeBehavior node = hoveredObject.GetComponent<NodeBehavior>();
            if (node != null)
            {
                currentNode = hoveredObject;
                // 获取 Properties 数据并展示
                Properties properties = node.properties;
                if (properties != null)
                {
                    // 显示属性值
                    infoText.text = $"姓名: {hoveredObject.name}\n" +
                                    $"状态: {properties.type}\n" +
                                    $"觉醒阈值: {properties.awakeThreshold}\n" +
                                    $"暴露阈值: {properties.exposeThreshold}\n" +
                                    $"持有书籍: {properties.numOfBooks}/{properties.maximumNumOfBooks}";
                }
            }
        }
    }
}
