using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private static ObjectPool instance;
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    private GameObject pool;

    // 新增：静态构造函数确保单例初始化
    static ObjectPool()
    {
        instance = new ObjectPool();
        // 立即创建持久化的pool对象
        instance.pool = new GameObject("ObjectPool");
        GameObject.DontDestroyOnLoad(instance.pool);
    }

    public static ObjectPool Instance
    {
        get
        {
            // 确保pool对象存在
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

            // 确保子池存在且持久化
            Transform childPool = pool.transform.Find(cleanName + "Pool");
            if (childPool == null)
            {
                childPool = new GameObject(cleanName + "Pool").transform;
                childPool.SetParent(pool.transform);
            }
        }

        _object = objectPool[cleanName].Dequeue();
        _object.SetActive(true);
        _object.transform.SetParent(null); // 取出时解除父子关系
        return _object;
    }

    public void PushObject(GameObject prefab)
    {
        string _name = prefab.name.Replace("(Clone)", string.Empty);

        if (!objectPool.ContainsKey(_name))
            objectPool.Add(_name, new Queue<GameObject>());

        // 确保对象回到正确的子池
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

        // 确保子池存在
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

    // 新增私有方法：清理无效引用（不改变原有接口）
    private void CleanInvalidReferences()
    {
        List<string> keysToRemove = new List<string>();

        foreach (var pair in objectPool)
        {
            // 移除null引用
            while (pair.Value.Count > 0 && pair.Value.Peek() == null)
            {
                pair.Value.Dequeue();
            }

            // 标记空队列
            if (pair.Value.Count == 0)
            {
                keysToRemove.Add(pair.Key);
            }
        }

        // 移除空队列
        foreach (var key in keysToRemove)
        {
            objectPool.Remove(key);
        }
    }
}