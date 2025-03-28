using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    // ����ʵ��
    private static ObjectPool instance;

    // ������ֵ䣺key=Ԥ�������ƣ�value=�����ö������
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();

    // ������֯���������гض���ĸ�����
    private GameObject pool;

    // ����������
    public static ObjectPool Instance
    {
        get
        {
            // �ӳٳ�ʼ������ʵ��
            if (instance == null)
            {
                instance = new ObjectPool();
            }
            return instance;
        }
    }

    /// <summary>
    /// �Ӷ���ػ�ȡ�������û���򴴽��¶���
    /// </summary>
    /// <param name="prefab">��Ҫ�Ķ���Ԥ����</param>
    /// <returns>�Ѽ������Ϸ����</returns>
    public GameObject GetObject(GameObject prefab)
    {
        GameObject _object;

        // ����Ƿ���ڶ�Ӧ������ҳ����п��ö���
        if (!objectPool.ContainsKey(prefab.name) || objectPool[prefab.name].Count == 0)
        {
            // �����¶���
            _object = GameObject.Instantiate(prefab);

            // ���¶����ȷŻس��н��г�ʼ��
            PushObject(_object);

            // �����ܸ����壨��������ڣ�
            if (pool == null)
                pool = new GameObject("ObjectPool");

            // ����/������Ӧ���͵��ӳ�
            GameObject childPool = GameObject.Find(prefab.name + "Pool");
            if (!childPool)
            {
                childPool = new GameObject(prefab.name + "Pool");
                childPool.transform.SetParent(pool.transform); // ���ò㼶��ϵ
            }

            // �����¶���ĸ���
            _object.transform.SetParent(childPool.transform);
        }

        // �Ӷ���ȡ�����󲢼���
        _object = objectPool[prefab.name].Dequeue();
        _object.SetActive(true);
        return _object;
    }

    /// <summary>
    /// ��������յ������
    /// </summary>
    /// <param name="prefab">Ҫ���յ���Ϸ����</param>
    public void PushObject(GameObject prefab)
    {
        // �����¡�����ƺ�׺
        string _name = prefab.name.Replace("(Clone)", string.Empty);

        // �����¶��У���������ڣ�
        if (!objectPool.ContainsKey(_name))
            objectPool.Add(_name, new Queue<GameObject>());

        // �����������в�����
        objectPool[_name].Enqueue(prefab);
        prefab.SetActive(false);
    }
}
