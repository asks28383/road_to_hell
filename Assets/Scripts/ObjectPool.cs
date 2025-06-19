using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPool
{
    // 单例实现
    private static ObjectPool instance;
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    private GameObject pool;

    // 场景关联管理
    private class SceneLinkedObject
    {
        public GameObject obj;
        public string sceneName;
    }
    private static List<SceneLinkedObject> sceneLinkedObjects = new List<SceneLinkedObject>();

    // 静态构造函数初始化
    static ObjectPool()
    {
        instance = new ObjectPool();
        instance.pool = new GameObject("ObjectPool");
        Object.DontDestroyOnLoad(instance.pool);

        SceneManager.sceneLoaded += OnSceneChanged;
    }

    // 场景切换回调
    private static void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        CleanupSceneObjects(scene.name);
    }

    // 清理非当前场景的对象
    private static void CleanupSceneObjects(string currentSceneName)
    {
        for (int i = sceneLinkedObjects.Count - 1; i >= 0; i--)
        {
            var item = sceneLinkedObjects[i];

            // 清理无效引用
            if (item.obj == null)
            {
                sceneLinkedObjects.RemoveAt(i);
                continue;
            }

            // 销毁非当前场景的非持久化对象
            if (item.sceneName != currentSceneName && !IsPersistentObject(item.obj.name))
            {
                if (item.obj.activeInHierarchy)
                {
                    Object.Destroy(item.obj);
                }
                sceneLinkedObjects.RemoveAt(i);

                // 从对象池队列中也移除
                string cleanName = item.obj.name.Replace("(Clone)", "");
                if (instance.objectPool.ContainsKey(cleanName))
                {
                    var queue = instance.objectPool[cleanName];
                    while (queue.Contains(item.obj))
                    {
                        queue.Dequeue();
                    }
                }
            }
        }
    }

    // 单例访问器
    public static ObjectPool Instance
    {
        get
        {
            if (instance.pool == null)
            {
                instance.pool = new GameObject("ObjectPool");
                Object.DontDestroyOnLoad(instance.pool);
            }
            return instance;
        }
    }

    // 获取对象（接口不变）
    public GameObject GetObject(GameObject prefab)
    {
        string cleanName = prefab.name.Replace("(Clone)", "");

        if (!objectPool.ContainsKey(cleanName))
        {
            objectPool[cleanName] = new Queue<GameObject>();
        }

        GameObject obj;
        if (objectPool[cleanName].Count == 0)
        {
            obj = Object.Instantiate(prefab);
            obj.name = cleanName; // 统一名称
        }
        else
        {
            obj = objectPool[cleanName].Dequeue();
        }

        obj.SetActive(true);
        obj.transform.SetParent(null);

        // 记录非持久化对象的场景关联
        if (!IsPersistentObject(cleanName))
        {
            sceneLinkedObjects.Add(new SceneLinkedObject
            {
                obj = obj,
                sceneName = SceneManager.GetActiveScene().name
            });
        }

        return obj;
    }

    // 归还对象（接口不变）
    public void PushObject(GameObject obj)
    {
        if (obj == null) return;

        string cleanName = obj.name.Replace("(Clone)", "");

        if (!objectPool.ContainsKey(cleanName))
        {
            objectPool[cleanName] = new Queue<GameObject>();
        }

        obj.SetActive(false);
        StoreObject(obj, cleanName);

        // 移除场景关联记录
        sceneLinkedObjects.RemoveAll(x => x.obj == obj);
    }

    // 存储对象到子池
    private void StoreObject(GameObject obj, string poolName)
    {
        Transform childPool = pool.transform.Find(poolName + "Pool");
        if (childPool == null)
        {
            childPool = new GameObject(poolName + "Pool").transform;
            childPool.SetParent(pool.transform);
        }
        obj.transform.SetParent(childPool);
        objectPool[poolName].Enqueue(obj);
    }

    // 判断是否持久化对象
    private static bool IsPersistentObject(string prefabName)
    {
        // 在此配置需要跨场景保留的对象名称规则
        return 
               prefabName=="Bullet" ||
               prefabName.Contains("Slash");
    }

    // 预暖对象池（接口不变）
    public void PrewarmPool(GameObject prefab, int count)
    {
        string cleanName = prefab.name.Replace("(Clone)", "");

        if (!objectPool.ContainsKey(cleanName))
        {
            objectPool[cleanName] = new Queue<GameObject>();
        }

        // 确保子池存在
        if (pool.transform.Find(cleanName + "Pool") == null)
        {
            GameObject childPool = new GameObject(cleanName + "Pool");
            childPool.transform.SetParent(pool.transform);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Object.Instantiate(prefab);
            PushObject(obj);
        }
    }

    // 检查对象池是否存在（接口不变）
    public bool HasPool(string prefabName)
    {
        return objectPool.ContainsKey(prefabName) && objectPool[prefabName].Count > 0;
    }
}