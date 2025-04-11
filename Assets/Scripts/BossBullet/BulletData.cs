using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NewBullet")]
public class BulletData : ScriptableObject
{
    public Vector3 P_Offset = Vector3.zero;     //位置的偏移量
    public Vector3 R_Offset = Vector3.zero;     //初始旋转的偏移量
    public int Count = 1;                  //一次生成的子弹的数量
    public float LifeTime = 4f;         //子弹的生命周期
    public float CdTime = 0.1f;         //子弹生成间隔时间
    public float Speed = 10f;           //子弹移动速度
    public float Angle = 0f;           //相邻子弹间的旋转角度
    public float Distance = 0f;      //相邻子弹间的距离
    public float CenterDis = 0f;     //与发射点的距离
    public float SelfRotation = 0f;     //每帧自转角度
    public float AddRotation = 0f;     //每帧自传角度增量

    public GameObject prefab;           //子弹预设体

    public float TempShootTime;

    public void ResetTempData()
    {
        TempShootTime = 0;
    }
}
