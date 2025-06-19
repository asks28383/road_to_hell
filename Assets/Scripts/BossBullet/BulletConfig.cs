using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletConfig : MonoBehaviour
{
    public BulletData[] bulletDatas;
    public Transform TargetTransform;

    private void Start()
    {
        // ��ʼ�������ӵ�����
        for (int i = 0; i < bulletDatas.Length; i++)
        {
            bulletDatas[i].ResetTempData(transform);

            // Ԥů�����
            ObjectPool.Instance.PrewarmPool(bulletDatas[i].prefab, 10);
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
                        // ʹ���µĶ���ػ�ȡ�ӵ�
                        GameObject bulletObj = ObjectPool.Instance.GetObject(data.prefab);
                        bulletObj.transform.position = transform.position + data.P_Offset;

                        // �����ӵ���������
                        if (data.LifeTime > 0)
                        {
                            StartCoroutine(ReturnBulletAfterTime(bulletObj, data.LifeTime));
                        }

                        BulletMove bullet = bulletObj.GetComponent<BulletMove>();
                        bullet.BulletSpeed = data.Speed;
                        bulletObj.transform.rotation = data.TempRotation * Quaternion.Euler(data.R_Offset);

                        if (data.Count % 2 == 1)
                        {
                            bulletObj.transform.rotation = bulletObj.transform.rotation * Quaternion.Euler(0, 0, -data.Angle * num);
                            bulletObj.transform.position = bulletObj.transform.position + bulletObj.transform.right * num * data.Distance;
                        }
                        else
                        {
                            bulletObj.transform.rotation = bulletObj.transform.rotation * Quaternion.Euler(0, 0, -(data.Angle / 2 + data.Angle * (num - 1)));
                            bulletObj.transform.position = bulletObj.transform.position + bulletObj.transform.right * ((num - 1) * data.Distance + data.Distance / 2);
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
                        // ʹ���µĶ���ػ�ȡ�ӵ�
                        GameObject bulletObj = ObjectPool.Instance.GetObject(data.prefab);
                        bulletObj.transform.position = transform.position + data.P_Offset;

                        // �����ӵ���������
                        if (data.LifeTime > 0)
                        {
                            StartCoroutine(ReturnBulletAfterTime(bulletObj, data.LifeTime));
                        }

                        BulletMove bullet = bulletObj.GetComponent<BulletMove>();
                        bullet.BulletSpeed = data.Speed;
                        bulletObj.transform.rotation = baseRotation * Quaternion.Euler(data.R_Offset);

                        if (data.Count % 2 == 1)
                        {
                            bulletObj.transform.rotation = bulletObj.transform.rotation * Quaternion.Euler(0, 0, -data.Angle * num);
                            bulletObj.transform.position = bulletObj.transform.position + bulletObj.transform.right * num * data.Distance;
                        }
                        else
                        {
                            bulletObj.transform.rotation = bulletObj.transform.rotation * Quaternion.Euler(0, 0, -(data.Angle / 2 + data.Angle * (num - 1)));
                            bulletObj.transform.position = bulletObj.transform.position + bulletObj.transform.right * ((num - 1) * data.Distance + data.Distance / 2);
                        }
                        num--;
                    }
                }
            }
        }
    }

    // Э�̣���һ��ʱ���黹�ӵ��������
    private IEnumerator ReturnBulletAfterTime(GameObject bulletObj, float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);

        // ����ӵ��Ƿ���Ȼ���ڣ��������������ڽ���ǰ�ѱ������߼����գ�
        if (bulletObj != null && bulletObj.activeSelf)
        {
            ObjectPool.Instance.PushObject(bulletObj);
        }
    }
}