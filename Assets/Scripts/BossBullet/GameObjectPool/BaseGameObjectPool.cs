using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseGameObjectPool
{

    /// <summary>
    /// ���У���Ŷ������û���õ��Ķ��󣬼��ɷ������
    /// </summary>
    protected Queue m_queue;
    /// <summary>
    /// ������д���������
    /// </summary>
    protected int m_maxCount;
    /// <summary>
    /// ����Ԥ��
    /// </summary>
    protected GameObject m_prefab;
    /// <summary>
    /// �ö���ص�transform
    /// </summary>
    protected Transform m_trans;
    /// <summary>
    /// ÿ������ص����ƣ���Ψһid
    /// </summary>
    protected string m_poolName;
    /// <summary>
    /// Ĭ���������
    /// </summary>
    protected const int m_defaultMaxCount = 10;

    public BaseGameObjectPool()
    {
        m_maxCount = m_defaultMaxCount;
        m_queue = new Queue();
    }

    public virtual void Init(string poolName, Transform trans)
    {
        m_poolName = poolName;
        m_trans = trans;
    }

    public GameObject prefab
    {
        set
        {
            m_prefab = value;
        }
    }

    public int maxCount
    {
        set
        {
            m_maxCount = value;
        }
    }

    /// <summary>
    /// ����һ������
    /// </summary>
    /// <param name="position">��ʼ����</param>
    /// <param name="lifetime">������ڵ�ʱ��</param>
    /// <returns>���ɵĶ���</returns>
    public virtual GameObject Get(Vector3 position, float lifetime)
    {
        if (lifetime < 0)
        {
            //lifetime<0ʱ������null  
            return null;
        }
        GameObject returnObj;
        if (m_queue.Count > 0)
        {
            //�����д��������
            returnObj = (GameObject)m_queue.Dequeue();
        }
        else
        {
            //����û�пɷ�������ˣ�������һ��
            returnObj = GameObject.Instantiate(m_prefab) as GameObject;
            returnObj.transform.SetParent(m_trans);
            returnObj.SetActive(false);
        }
        //ʹ��PrefabInfo�ű�����returnObj��һЩ��Ϣ
        GameObjectPoolInfo info = returnObj.GetComponent<GameObjectPoolInfo>();
        if (info == null)
        {
            info = returnObj.AddComponent<GameObjectPoolInfo>();
        }
        info.poolName = m_poolName;
        if (lifetime > 0)
        {
            info.lifetime = lifetime;
        }
        returnObj.transform.position = position;
        returnObj.SetActive(true);
        return returnObj;
    }

    /// <summary>
    /// ��ɾ�����󡱷�������
    /// </summary>
    /// <param name="obj">����</param>
    public virtual void Remove(GameObject obj)
    {
        //����������Ѿ��ڶ������  
        if (m_queue.Contains(obj))
        {
            return;
        }
        if (m_queue.Count > m_maxCount)
        {
            //��ǰ����object����������ֱ������
            GameObject.Destroy(obj);
        }
        else
        {
            //�������أ����
            m_queue.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    /// <summary>
    /// ���ٸö����
    /// </summary>
    public virtual void Destroy()
    {
        m_queue.Clear();
    }
}

