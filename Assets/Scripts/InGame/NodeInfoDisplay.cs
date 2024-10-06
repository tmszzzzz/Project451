using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeInfoDisplay : MonoBehaviour
{
    public GameObject infoTextGo; // 在 Inspector 中指定的 UI Text 用于显示信息
    private TextMeshProUGUI infoText;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        infoText = infoTextGo.GetComponent<TextMeshProUGUI>();
        // 初始化文本为空
        infoText.text = "";
    }

    void Update()
    {
        // 射线检测鼠标指向的物体
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hoveredObject = hit.collider.gameObject;

            // 检查物体是否有 CubeBehavior 脚本
            NodeBehavior node = hoveredObject.GetComponent<NodeBehavior>();
            if (node != null)
            {
                // 获取 Properties 数据并展示
                Properties properties = node.properties;
                if (properties != null)
                {
                    // 显示属性值
                    infoText.text = $"Name: {hoveredObject.name}\n" +
                                    $"Identity: {properties.type}\n" +
                                    $"Awake Threshold: {properties.awakeThreshold}\n" +
                                    $"Expose Threshold: {properties.exposeThreshold}\n" +
                                    $"Books: {properties.numOfBooks}/{properties.maximumNumOfBooks}";
                }
            }
        }
    }
}
