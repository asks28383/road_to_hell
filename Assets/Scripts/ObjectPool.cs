using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    // 单例实例
    private static ObjectPool instance;

    // 对象池字典：key=预制体名称，value=可重用对象队列
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();

    // 用于组织场景中所有池对象的父物体
    private GameObject pool;

    // 单例访问器
    public static ObjectPool Instance
    {
        get
        {
            // 延迟初始化单例实例
            if (instance == null)
            {
                instance = new ObjectPool();
            }
            return instance;
        }
    }

    /// <summary>
    /// 从对象池获取对象（如果没有则创建新对象）
    /// </summary>
    /// <param name="prefab">需要的对象预制体</param>
    /// <returns>已激活的游戏对象</returns>
    public GameObject GetObject(GameObject prefab)
    {
        GameObject _object;

        // 检查是否存在对应对象池且池中有可用对象
        if (!objectPool.ContainsKey(prefab.name) || objectPool[prefab.name].Count == 0)
        {
            // 创建新对象
            _object = GameObject.Instantiate(prefab);

            // 将新对象先放回池中进行初始化
            PushObject(_object);

            // 创建总父物体（如果不存在）
            if (pool == null)
                pool = new GameObject("ObjectPool");

            // 查找/创建对应类型的子池
            GameObject childPool = GameObject.Find(prefab.name + "Pool");
            if (!childPool)
            {
                childPool = new GameObject(prefab.name + "Pool");
                childPool.transform.SetParent(pool.transform); // 设置层级关系
            }

            // 设置新对象的父级
            _object.transform.SetParent(childPool.transform);
        }

        // 从队列取出对象并激活
        _object = objectPool[prefab.name].Dequeue();
        _object.SetActive(true);
        return _object;
    }

    /// <summary>
    /// 将对象回收到对象池
    /// </summary>
    /// <param name="prefab">要回收的游戏对象</param>
    public void PushObject(GameObject prefab)
    {
        // 清理克隆体名称后缀
        string _name = prefab.name.Replace("(Clone)", string.Empty);

        // 创建新队列（如果不存在）
        if (!objectPool.ContainsKey(_name))
            objectPool.Add(_name, new Queue<GameObject>());

        // 将对象加入队列并禁用
        objectPool[_name].Enqueue(prefab);
        prefab.SetActive(false);
    }
}
