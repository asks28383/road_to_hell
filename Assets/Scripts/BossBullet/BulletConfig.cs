using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletConfig : MonoBehaviour
{
    public BulletData[] bulletDatas;
    ObjectPool[] Pools;
    private void Start()
    {
        Pools = new ObjectPool[bulletDatas.Length];
        for (int i = 0; i < bulletDatas.Length; i++)
        {
            GameObject bullet = ObjectPool.Instance.GetObject(bulletDatas[i].prefab);
        }
    }

    void Shoot()
    {
        for (int i = 0; i < bulletDatas.Length; i++)
        {
            BulletData data = bulletDatas[i];
            if (Time.time-data.TempShootTime>=data.CdTime)
            {
                data.TempShootTime = Time.time;
            }
        }
    }
}
