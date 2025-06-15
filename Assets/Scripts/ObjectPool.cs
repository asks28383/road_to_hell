using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private static ObjectPool instance;
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    private GameObject pool;

    // ��������̬���캯��ȷ��������ʼ��
    static ObjectPool()
    {
        instance = new ObjectPool();
        // ���������־û���pool����
        instance.pool = new GameObject("ObjectPool");
        GameObject.DontDestroyOnLoad(instance.pool);
    }

    public static ObjectPool Instance
    {
        get
        {
            // ȷ��pool�������
            if (instance.pool == null)
            {
                instance.pool = new GameObject("ObjectPool");
                GameObject.DontDestroyOnLoad(instance.pool);
            }
            return instance;
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        GameObject _object;
        string cleanName = prefab.name.Replace("(Clone)", string.Empty);

        if (!objectPool.ContainsKey(cleanName) || objectPool[cleanName].Count == 0)
        {
            _object = GameObject.Instantiate(prefab);
            PushObject(_object);

            // ȷ���ӳش����ҳ־û�
            Transform childPool = pool.transform.Find(cleanName + "Pool");
            if (childPool == null)
            {
                childPool = new GameObject(cleanName + "Pool").transform;
                childPool.SetParent(pool.transform);
            }
        }

        _object = objectPool[cleanName].Dequeue();
        _object.SetActive(true);
        _object.transform.SetParent(null); // ȡ��ʱ������ӹ�ϵ
        return _object;
    }

    public void PushObject(GameObject prefab)
    {
        string _name = prefab.name.Replace("(Clone)", string.Empty);

        if (!objectPool.ContainsKey(_name))
            objectPool.Add(_name, new Queue<GameObject>());

        // ȷ������ص���ȷ���ӳ�
        Transform childPool = pool.transform.Find(_name + "Pool");
        if (childPool == null)
        {
            childPool = new GameObject(_name + "Pool").transform;
            childPool.SetParent(pool.transform);
        }
        prefab.transform.SetParent(childPool);

        prefab.SetActive(false);
        objectPool[_name].Enqueue(prefab);
    }

    public bool HasPool(string prefabName)
    {
        return objectPool.ContainsKey(prefabName) && objectPool[prefabName].Count > 0;
    }

    public void PrewarmPool(GameObject prefab, int count)
    {
        string cleanName = prefab.name.Replace("(Clone)", string.Empty);

        // ȷ���ӳش���
        if (!pool.transform.Find(cleanName + "Pool"))
        {
            GameObject childPool = new GameObject(cleanName + "Pool");
            childPool.transform.SetParent(pool.transform);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            PushObject(obj);
        }
    }

    // ����˽�з�����������Ч���ã����ı�ԭ�нӿڣ�
    private void CleanInvalidReferences()
    {
        List<string> keysToRemove = new List<string>();

        foreach (var pair in objectPool)
        {
            // �Ƴ�null����
            while (pair.Value.Count > 0 && pair.Value.Peek() == null)
            {
                pair.Value.Dequeue();
            }

            // ��ǿն���
            if (pair.Value.Count == 0)
            {
                keysToRemove.Add(pair.Key);
            }
        }

        // �Ƴ��ն���
        foreach (var key in keysToRemove)
        {
            objectPool.Remove(key);
        }
    }
}