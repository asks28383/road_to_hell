using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPool
{
    // ����ʵ��
    private static ObjectPool instance;
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    private GameObject pool;

    // ������������
    private class SceneLinkedObject
    {
        public GameObject obj;
        public string sceneName;
    }
    private static List<SceneLinkedObject> sceneLinkedObjects = new List<SceneLinkedObject>();

    // ��̬���캯����ʼ��
    static ObjectPool()
    {
        instance = new ObjectPool();
        instance.pool = new GameObject("ObjectPool");
        Object.DontDestroyOnLoad(instance.pool);

        SceneManager.sceneLoaded += OnSceneChanged;
    }

    // �����л��ص�
    private static void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        CleanupSceneObjects(scene.name);
    }

    // ����ǵ�ǰ�����Ķ���
    private static void CleanupSceneObjects(string currentSceneName)
    {
        for (int i = sceneLinkedObjects.Count - 1; i >= 0; i--)
        {
            var item = sceneLinkedObjects[i];

            // ������Ч����
            if (item.obj == null)
            {
                sceneLinkedObjects.RemoveAt(i);
                continue;
            }

            // ���ٷǵ�ǰ�����ķǳ־û�����
            if (item.sceneName != currentSceneName && !IsPersistentObject(item.obj.name))
            {
                if (item.obj.activeInHierarchy)
                {
                    Object.Destroy(item.obj);
                }
                sceneLinkedObjects.RemoveAt(i);

                // �Ӷ���ض�����Ҳ�Ƴ�
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

    // ����������
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

    // ��ȡ���󣨽ӿڲ��䣩
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
            obj.name = cleanName; // ͳһ����
        }
        else
        {
            obj = objectPool[cleanName].Dequeue();
        }

        obj.SetActive(true);
        obj.transform.SetParent(null);

        // ��¼�ǳ־û�����ĳ�������
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

    // �黹���󣨽ӿڲ��䣩
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

        // �Ƴ�����������¼
        sceneLinkedObjects.RemoveAll(x => x.obj == obj);
    }

    // �洢�����ӳ�
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

    // �ж��Ƿ�־û�����
    private static bool IsPersistentObject(string prefabName)
    {
        // �ڴ�������Ҫ�糡�������Ķ������ƹ���
        return 
               prefabName=="Bullet" ||
               prefabName.Contains("Slash");
    }

    // Ԥů����أ��ӿڲ��䣩
    public void PrewarmPool(GameObject prefab, int count)
    {
        string cleanName = prefab.name.Replace("(Clone)", "");

        if (!objectPool.ContainsKey(cleanName))
        {
            objectPool[cleanName] = new Queue<GameObject>();
        }

        // ȷ���ӳش���
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

    // ��������Ƿ���ڣ��ӿڲ��䣩
    public bool HasPool(string prefabName)
    {
        return objectPool.ContainsKey(prefabName) && objectPool[prefabName].Count > 0;
    }
}