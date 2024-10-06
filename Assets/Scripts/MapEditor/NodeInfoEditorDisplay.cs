using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeInfoEditorDisplay : MonoBehaviour
{
    public GameObject infoTextGo; // �� Inspector ��ָ���� UI Text ������ʾ��Ϣ
    private TextMeshProUGUI infoText;
    private Camera mainCamera;
    private CubeEditorBehavior selectedCB;

    void Start()
    {
        mainCamera = Camera.main;
        infoText = infoTextGo.GetComponent<TextMeshProUGUI>();
        // ��ʼ���ı�Ϊ��
        infoText.text = "";
    }

    void Update()
    {
        // ���߼�����ָ�������
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hoveredObject = hit.collider.gameObject;

            // ��������Ƿ��� CubeBehavior �ű�
            CubeEditorBehavior cubeBehavior = hoveredObject.GetComponent<CubeEditorBehavior>();
            if (cubeBehavior != null)
            {
                // ��ȡ Properties ���ݲ�չʾ
                PropertiesEditor properties = cubeBehavior.properties;
                if (properties != null)
                {
                    // ��ʾ����ֵ
                    infoText.text = $"Name: {hoveredObject.name}\n" +
                                    $"Awake Threshold: {properties.awakeThreshold}\n" +
                                    $"Expose Threshold: {properties.exposeThreshold}\n" +
                                    $"Maximum Books: {properties.maximumNumOfBooks}";
                    cubeBehavior.selected = true;
                    if (selectedCB != null && selectedCB != cubeBehavior) selectedCB.selected = false;
                    selectedCB = cubeBehavior;
                }
            }
        }
    }
}