using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolInfo : MonoBehaviour
{
    /// <summary>
    /// ������ʾ�ĳ���ʱ�䣬��=0��������
    /// </summary>
    [HideInInspector] public float lifetime = 0;
    /// <summary>
    /// ��������ص�Ψһid
    /// </summary>
    [HideInInspector] public string poolName;

    WaitForSeconds m_waitTime;

    void Awake()
    {
        if (lifetime > 0)
        {
            m_waitTime = new WaitForSeconds(lifetime);
        }
    }

    void OnEnable()
    {
        if (lifetime > 0)
        {
            StartCoroutine(CountDown(lifetime));
        }
    }

    IEnumerator CountDown(float lifetime)
    {
        yield return m_waitTime;
        //�������������
        GameObjectPoolManager.Instance.RemoveGameObject(poolName, gameObject);
    }
}
