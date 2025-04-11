using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NewBullet")]
public class BulletData : ScriptableObject
{
    public Vector3 P_Offset = Vector3.zero;     //λ�õ�ƫ����
    public Vector3 R_Offset = Vector3.zero;     //��ʼ��ת��ƫ����
    public int Count = 1;                  //һ�����ɵ��ӵ�������
    public float LifeTime = 4f;         //�ӵ�����������
    public float CdTime = 0.1f;         //�ӵ����ɼ��ʱ��
    public float Speed = 10f;           //�ӵ��ƶ��ٶ�
    public float Angle = 0f;           //�����ӵ������ת�Ƕ�
    public float Distance = 0f;      //�����ӵ���ľ���
    public float CenterDis = 0f;     //�뷢���ľ���
    public float SelfRotation = 0f;     //ÿ֡��ת�Ƕ�
    public float AddRotation = 0f;     //ÿ֡�Դ��Ƕ�����

    public GameObject prefab;           //�ӵ�Ԥ����

    public float TempShootTime;

    public void ResetTempData()
    {
        TempShootTime = 0;
    }
}
