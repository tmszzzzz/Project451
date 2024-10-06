using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVar : MonoBehaviour
{
    public static GlobalVar Instance { get; set; }
    //�˴���Ÿ���ȫ����Ҫ�õ��ĳ��������������Ϸ���̿�����һ�����޸ġ�
    //��ע��Ϊ�˼�㣬�˴���ֵ�ɳ�Ա������ʼ���õ����⽫�����ڴ˽ű����޸���Щ������Ч����Ҫ��inspector���޸ġ�

    public int allocationLimit = 1;
    public float exposeToDeathRate = 0.30f;
    public int NumOfBibliophileGiveBooks = 2;
    public int NumOfFirefighterGiveBooks = 1;

    //���õ���ģʽ���������ο�ͨ�������ľ�̬����Instance���ô�Ψһʵ����
    private void Awake()
    {
        // �������ʵ���Ҳ��ǵ�ǰʵ�������ٵ�ǰʵ����ȷ������Ψһ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // ����ǰʵ����Ϊ����ʵ��
        Instance = this;

        // ѡ�����������ʹ���ڳ����л�ʱ���ᱻ����
        DontDestroyOnLoad(gameObject);
    }
}
