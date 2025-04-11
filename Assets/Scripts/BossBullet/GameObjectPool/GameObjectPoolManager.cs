using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : SingleClass<GameObjectPoolManager>
{
    /// <summary>
    /// ������еĶ����
    /// </summary>
    Dictionary<string, BaseGameObjectPool> m_poolDic = new Dictionary<string, BaseGameObjectPool>();
    /// <summary>
    /// ������ڳ����еĸ��ؼ�
    /// </summary>
    Transform m_parentTrans;

    /// <summary>
    /// ����һ���µĶ����
    /// </summary>
    /// <typeparam name="T">���������</typeparam>
    /// <param name="poolName">��������ƣ�Ψһid</param>
    /// <returns>����ض���</returns>
    GameObject AllPool;
    public T CreatGameObjectPool<T>(string poolName) where T : BaseGameObjectPool, new()
    {
        if (m_poolDic.ContainsKey(poolName))
        {
            return (T)m_poolDic[poolName];
        }
        //����һ���µ�GameObject������еĶ���ض���
        if (AllPool == null)
            AllPool = new GameObject("AllPool");
        m_parentTrans = AllPool.transform;
        GameObject obj = new GameObject(poolName);
        obj.transform.SetParent(m_parentTrans);
        T pool = new T();
        pool.Init(poolName, obj.transform);
        m_poolDic.Add(poolName, pool);
        return pool;
    }

    /// <summary>
    /// �Ӷ������ȡ���µĶ���
    /// </summary>
    /// <param name="poolName">���������</param>
    /// <param name="position">����������</param>
    /// <param name="lifeTime">������ʾʱ��</param>
    /// <returns>�¶���</returns>
    public GameObject GetGameObject(string poolName, Vector3 position, float lifeTime)
    {
        if (m_poolDic.ContainsKey(poolName))
        {
            return m_poolDic[poolName].Get(position, lifeTime);
        }
        return null;
    }

    /// <summary>
    /// ���������������
    /// </summary>
    /// <param name="poolName">���������</param>
    /// <param name="go">����</param>
    public void RemoveGameObject(string poolName, GameObject go)
    {
        if (m_poolDic.ContainsKey(poolName))
        {
            m_poolDic[poolName].Remove(go);
        }
    }

    public int GetPoolCount()
    {
        return m_poolDic.Count;
    }

    /// <summary>
    /// �������ж���ز���
    /// </summary>
    public void Destroy()
    {
        m_poolDic.Clear();
        GameObject.Destroy(m_parentTrans);
    }
}