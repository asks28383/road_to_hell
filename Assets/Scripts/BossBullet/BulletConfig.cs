using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletConfig : MonoBehaviour
{
    public BulletData[] bulletDatas;
    BaseGameObjectPool[] Pools;
    public Transform TargetTransform;

    private void Start()
    {
        Pools = new BaseGameObjectPool[bulletDatas.Length];
        for (int i = 0; i < bulletDatas.Length; i++)
        {
            Pools[i] = GameObjectPoolManager.Instance.CreatGameObjectPool<BaseGameObjectPool>(bulletDatas[i].prefab.name);
            Pools[i].prefab = bulletDatas[i].prefab;
            bulletDatas[i].ResetTempData(transform);
        }
    }

    private void FixedUpdate()
    {
        Shoot();
    }

    void Shoot()
    {
        for (int i = 0; i < bulletDatas.Length; i++)
        {
            BulletData data = bulletDatas[i];
            if (data.directionMode == BulletData.BulletDirectionMode.FixedRotation)
            {
                if (Time.time - data.TempShootTime >= data.CdTime)
                {
                    data.TempShootTime = Time.time;
                    int num = data.Count / 2;
                    Quaternion q = Quaternion.Euler(0, 0, data.TempSelfRotation);
                    data.TempRotation = data.TempRotation * q;
                    data.TempSelfRotation += data.AddRotation;
                    data.TempSelfRotation = data.TempSelfRotation % 360;
                    for (int j = 0; j < data.Count; j++)
                    {
                        GameObject pre = Pools[i].Get(transform.position + data.P_Offset, data.LifeTime);
                        BulletMove bullet = pre.GetComponent<BulletMove>();
                        bullet.BulletSpeed = data.Speed;
                        pre.transform.rotation = data.TempRotation * Quaternion.Euler(data.R_Offset);
                        if (data.Count % 2 == 1)
                        {
                            pre.transform.rotation = pre.transform.rotation * Quaternion.Euler(0, 0, -data.Angle * num);
                            pre.transform.position = pre.transform.position + pre.transform.right * num * data.Distance;
                        }
                        else
                        {
                            pre.transform.rotation = pre.transform.rotation * Quaternion.Euler(0, 0, -(data.Angle / 2 + data.Angle * (num - 1)));
                            pre.transform.position = pre.transform.position + pre.transform.right * ((num - 1) * data.Distance + data.Distance / 2);
                        }
                        num--;
                    }


                }
            }
            else if (data.directionMode == BulletData.BulletDirectionMode.TargetPosition)
            {
                if (Time.time - data.TempShootTime >= data.CdTime)
                {
                    data.TempShootTime = Time.time;
                    int num = data.Count / 2;

                    if (TargetTransform == null)
                    {
                        Debug.LogWarning("TargetTransform δ���ã��޷���Ŀ�귢�䣡");
                        return;
                    }

                    Vector2 dir = TargetTransform.position - (transform.position + data.P_Offset);
                    float angleToTarget = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                    Quaternion baseRotation = Quaternion.Euler(0, 0, angleToTarget);

                    for (int j = 0; j < data.Count; j++)
                    {
                        GameObject pre = Pools[i].Get(transform.position + data.P_Offset, data.LifeTime);
                        BulletMove bullet = pre.GetComponent<BulletMove>();
                        bullet.BulletSpeed = data.Speed;

                        pre.transform.rotation = baseRotation * Quaternion.Euler(data.R_Offset);

                        if (data.Count % 2 == 1)
                        {
                            pre.transform.rotation = pre.transform.rotation * Quaternion.Euler(0, 0, -data.Angle * num);
                            pre.transform.position = pre.transform.position + pre.transform.right * num * data.Distance;
                        }
                        else
                        {
                            pre.transform.rotation = pre.transform.rotation * Quaternion.Euler(0, 0, -(data.Angle / 2 + data.Angle * (num - 1)));
                            pre.transform.position = pre.transform.position + pre.transform.right * ((num - 1) * data.Distance + data.Distance / 2);
                        }
                        num--;
                    }
                }
            }

        }
    }
}
