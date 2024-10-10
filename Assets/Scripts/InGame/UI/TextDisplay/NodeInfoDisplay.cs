using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeInfoDisplay : MonoBehaviour
{
    private bool updateInfoWithoutClick = false; // 是否在点击时更新信息
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
            //return before updating the info text if the updateInfoWithoutClick is false and the left mouse button is not clicked
            //either wise the info text will be updated every frame
            if ((updateInfoWithoutClick != true) && (Input.GetMouseButton(0) != true))
            {
                return;
            }

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
                                    $"Books: {properties.numOfBooks}/{properties.maximumNumOfBooks}"+
                                    $"\n\n    {properties.description}";
                }
            }
        }
    }
}
